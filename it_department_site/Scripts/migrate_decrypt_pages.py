"""
One-off migration script for IT-98.

Context: PageService used to store page.Content as AES-encrypted text
(EncryptionService.Encrypt/Decrypt) in the `content` column. After removing
encryption from PageService, any already-encrypted rows became unreadable
(non-JSON) and had to be decrypted back to plain JSON.

This script was used to:
  1. Decrypt any `pages.content` values that were still AES-encrypted
     (identified as rows where content does NOT start with '{').
  2. Fix a double-backslash-escaping issue that occurred from an earlier
     manual `mysql -e "UPDATE ..."` attempt (MySQL swallowed backslashes
     in \\uXXXX sequences).

Usage:
    DB_HOST=localhost DB_USER=site_admin DB_PASSWORD=*** DB_NAME=website \
    ENCRYPTION_KEY=**** ENCRYPTION_IV=**** python3 migrate_decrypt_pages.py

Run this ONCE per environment (local/staging/prod) after pulling IT-98,
BEFORE relying on Page content being readable. Safe to re-run: it only
touches rows whose content is not valid JSON yet.
"""

import os
import re
import json
import base64
import pymysql
from Crypto.Cipher import AES  # pip install pycryptodome

DB_HOST = os.environ.get("DB_HOST", "localhost")
DB_USER = os.environ["DB_USER"]
DB_PASSWORD = os.environ["DB_PASSWORD"]
DB_NAME = os.environ.get("DB_NAME", "website")

ENCRYPTION_KEY = os.environ.get("ENCRYPTION_KEY")  # 16-byte AES key, if still known
ENCRYPTION_IV = os.environ.get("ENCRYPTION_IV")    # 16-byte AES IV


def decrypt_aes(cipher_b64: str) -> str:
    if not ENCRYPTION_KEY or not ENCRYPTION_IV:
        raise RuntimeError(
            "ENCRYPTION_KEY/ENCRYPTION_IV env vars are required to decrypt "
            "legacy AES-encrypted rows."
        )
    key = ENCRYPTION_KEY.encode("utf-8")
    iv = ENCRYPTION_IV.encode("utf-8")
    raw = base64.b64decode(cipher_b64)
    cipher = AES.new(key, AES.MODE_CBC, iv)
    decrypted = cipher.decrypt(raw)
    pad_len = decrypted[-1]
    return decrypted[:-pad_len].decode("utf-8")


def fix_escaping(text: str) -> str:
    """Collapse any number of backslashes before a \\uXXXX sequence to one."""
    return re.sub(r"\\+u([0-9A-Fa-f]{4})", r"\\u\1", text)


def main():
    conn = pymysql.connect(
        host=DB_HOST, user=DB_USER, password=DB_PASSWORD,
        database=DB_NAME, charset="utf8mb4",
    )
    cur = conn.cursor()

    cur.execute("SELECT id, content FROM pages WHERE content NOT LIKE '{%'")
    rows = cur.fetchall()

    if not rows:
        print("No corrupted/encrypted rows found. Nothing to do.")
        return

    for page_id, content in rows:
        try:
            # Try treating it as double/triple-escaped JSON first
            fixed = fix_escaping(content)
            json.loads(fixed)
        except (json.JSONDecodeError, TypeError):
            # Fall back to real AES decryption
            fixed = decrypt_aes(content)
            json.loads(fixed)  # validate

        cur.execute("UPDATE pages SET content=%s WHERE id=%s", (fixed, page_id))
        print(f"Fixed page id={page_id}")

    conn.commit()
    print(f"\nDone. Migrated {len(rows)} row(s).")
    conn.close()


if __name__ == "__main__":
    main()
