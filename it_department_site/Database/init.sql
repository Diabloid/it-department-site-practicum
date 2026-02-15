CREATE TABLE partners (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) DEFAULT NULL,
    logo_file_name VARCHAR(255) NOT NULL,
    sort_order INT DEFAULT 0
);

INSERT INTO partners (name, logo_file_name, sort_order) VALUES 
('SoftServe', '/images/partners/softserve.png', 1),
('Yukon', '/images/partners/yukon.png', 2),
('Valtech', '/images/partners/valtech.png', 3);