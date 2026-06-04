public class PageService 
{
    private readonly EncryptionService _encryption;

    public PageService( EncryptionService encryption)
    {
        _encryption = encryption;
    }


    // --- Private Methods (Encryption) ---
    // Вони лишаються синхронними, бо це робота з пам'яттю/CPU

    private void EncryptContent(Page page)
    {
        var json = JsonSerializer.Serialize(page.Content);
        // Прибрав зайві логи в консоль
        var encrypted = _encryption.Encrypt(json);
        page.ContentJson = encrypted;
    }

    private void DecryptContent(Page page)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(page.ContentJson))
            {
                page.Content = new ContentSections();
                return;
            }

            var decrypted = _encryption.Decrypt(page.ContentJson);

            if (string.IsNullOrWhiteSpace(decrypted))
            {
                page.Content = new ContentSections();
                return;
            }

            page.Content = JsonSerializer.Deserialize<ContentSections>(decrypted) ?? new ContentSections();
        }
        catch (FormatException)
        {
            try
            {
                page.Content = JsonSerializer.Deserialize<ContentSections>(page.ContentJson) ?? new ContentSections();
            }
            catch
            {
                page.Content = new ContentSections();
            }
        }
        catch (JsonException)
        {
            page.Content = new ContentSections();
        }
    }
}