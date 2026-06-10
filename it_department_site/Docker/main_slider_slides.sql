-- Run once in MySQL Workbench (database: website)
USE website;

CREATE TABLE IF NOT EXISTS main_slider_slides (
  slot TINYINT NOT NULL PRIMARY KEY COMMENT '1,2,3',
  image_url VARCHAR(500) NULL,
  alt_text VARCHAR(200) DEFAULT '',
  updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

INSERT INTO main_slider_slides (slot, image_url, alt_text)
VALUES
  (1, '/images/banner_photo_1.JPG', 'Слайд 1'),
  (2, '/images/banner_photo_1.JPG', 'Слайд 2'),
  (3, '/images/banner_photo_1.JPG', 'Слайд 3')
ON DUPLICATE KEY UPDATE
  alt_text = VALUES(alt_text);

-- If table already exists with NOT NULL image_url:
-- ALTER TABLE main_slider_slides MODIFY image_url VARCHAR(500) NULL;
