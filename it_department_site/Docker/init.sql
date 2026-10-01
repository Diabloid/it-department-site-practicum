-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: website
-- ------------------------------------------------------
-- Server version	8.0.39
use website;
/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

LOCK TABLES `menu_items` WRITE;
/*!40000 ALTER TABLE `menu_items` DISABLE KEYS */;
INSERT INTO menu_items (id, name, parent_id, page_id, sort_order, custom_url) VALUES
(18, 'Співробітники', 113, NULL, 1, 'Employees'),
(113, 'Про нас', NULL, NULL, 0, NULL),
(114, 'Історія спеціальності', 113, NULL, 0, NULL),
(115, 'Наукова діяльність', 113, NULL, 2, NULL),
(116, 'Стейкхолдери', 113, NULL, 3, NULL),
(117, 'Освітній процес', NULL, NULL, 1, NULL),
(118, 'Освітні програми', 117, NULL, 0, NULL),
(121, 'Вибіркові дисципліни', 117, NULL, 3, NULL),
(122, 'Освітня траєкторія', 117, NULL, 4, NULL),
(123, 'Нормативно-правове забезпечення', 117, NULL, 5, NULL),
(124, 'Обовязкові дисципліни', 117, NULL, 2, 'Disciplines'),
(125, 'Студентам', NULL, NULL, 2, NULL),
(127, 'Система електронного навчання', 125, NULL, 1, NULL),
(128, 'Анкетування', 125, NULL, 2, NULL),
(129, 'Абітурієнтам', NULL, NULL, 3, NULL),
(130, 'Вступ 2026', 129, NULL, 0, NULL),
(131, 'Чому саме ми ?', 129, NULL, 2, NULL),
(132, 'Інформація про спеціальність', 129, NULL, 2, 'proudofyou'),
(133, 'Чому обирають IT', 129, NULL, 3, NULL),
(134, 'Розклад', 125, 39, 0, NULL);

/*!40000 ALTER TABLE `menu_items` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `pages` WRITE;
/*!40000 ALTER TABLE `pages` DISABLE KEYS */;
INSERT INTO pages
(id, title, slug, status, created_at, updated_at, layout_type, content, views_count) VALUES
(1,'Home Page','home-page','Published','2025-03-13 20:54:36','2025-06-20 04:07:22','no-sidebar','{\"main\": \"<p><strong>This is the content for the Home Page.</strong></p>\"}',4),
(2,'About Us','about-us','Published','2025-03-13 20:54:36','2025-06-20 04:07:45','no-sidebar','{\"main\": \"This page contains information about our company.\"}',5),
(3,'Contact Us','contact-us','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page is under construction. Contact form will be available soon.\"}',0),
(5,'Privacy Policy','privacy-policy','Published','2025-03-13 20:54:36','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cp\\u003E234\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',0),
(6,'Terms of Service','terms-of-service','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page contains the terms of service for our website.\"}',0),
(7,'FAQ','faq','Published','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"Frequently Asked Questions about our products and services.\"}',0),
(27,'hello!!!','hello','Draft','2025-05-23 22:36:42','2025-05-23 22:36:42','right-sidebar','{\"left\": \"\", \"main\": \"<p>hello!!!</p>\", \"right\": \"<p>hello!!!</p>\"}',0),
(28,'ывывыв','storink','Draft','2025-05-26 12:37:53','2025-05-26 12:37:53','left-sidebar','{\"left\": \"<p>ывывывывывыв</p>\", \"main\": \"<p>ывывывыв</p>\", \"right\": \"\"}',0),
(32,'encrypted','encrypted','Draft','2025-06-10 18:02:43','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cp\\u003Eencrypted1\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',0),
(33,'<script>alert(\"XSS\");</script>','encrypted-sidebar','Draft','2025-06-10 18:14:08','2026-08-05 11:44:37','right-sidebar','{\"main\":\"\\u003Cp\\u003Eencrypted-sidebar.alert(\\u0022qqq\\u0022)\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\\u003Cp\\u003Eencrypted-sidebar\\u003C/p\\u003E\"}',0),
(34,'picture!!!','picture','Published','2025-06-10 19:42:26','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cfigure class=\\u0022image image-style-align-left image_resized\\u0022 style=\\u0022width:40.5%;\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/6562b57b-7180-4494-8fc0-3980c6b51020.jpg\\u0022\\u003E\\u003C/figure\\u003E\\u003Cp\\u003ELorem ipsum dolor sit amet...\",\"left\":\"\",\"right\":\"\"}',3),
(35,'nov','nov','Published','2025-06-11 12:36:02','2026-08-05 11:44:38','right-sidebar','{\"main\":\"\\u003Cp\\u003Eergjiortjhhoiprtjgip4rthjip0ghj4pihj4igh4ijgpiefjvipdfhvipehrfpijpifhepfjepirgfjpiehjgfpiehipehvkpekpvhepkvhepkghperhgi\\u003C/p\\u003E\\u003Cfigure class=\\u0022image image_resized\\u0022 style=\\u0022width:53.06%;\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/d8a627f2-0d47-4507-bb43-b3bf1882d428.jpg\\u0022\\u003E\\u003C/figure\\u003E\",\"left\":\"\",\"right\":\"\\u003Cp\\u003Eednjvoerefipgjrighjeoigjhereoigehgiehvgvpidsdpkdvfjerpighiprvjiehjgip3rhjgpedhjdvpierhrvfpihepicnpiedcnpie3rhrpienievcip3jpiwev\\u003C/p\\u003E\\u003Cfigure class=\\u0022image\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/da7b40d6-b3ac-496e-bdc4-b66754a1ffa2.png\\u0022\\u003E\\u003C/figure\\u003E\"}',1),
(36,'storink','storink2','Published','2025-06-12 11:21:11','2026-08-05 11:44:38','no-sidebar','{\"main\":\"\\u003Cfigure class=\\u0022image\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/f0593841-cb3d-442c-b3df-fc81467cfa21.gif\\u0022\\u003E\\u003C/figure\\u003E\",\"left\":\"\",\"right\":\"\"}',1),
(38,'Нова сторінка!!!','new-page','Published','2025-06-20 04:09:03','2026-08-05 11:44:38','no-sidebar','{\"main\":\"\\u003Cp\\u003E\\u0446\\u0435 \\u0441\\u0443\\u043F\\u0435\\u0440 \\u043A\\u0440\\u0443\\u0442\\u0430 \\u0441\\u0442\\u043E\\u0440\\u0456\\u043D\\u043A\\u0430!!!\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',0);
UNLOCK TABLES;

LOCK TABLES `site_settings` WRITE;
/*!40000 ALTER TABLE `site_settings` DISABLE KEYS */;
INSERT INTO `site_settings` VALUES (1,'<header>     <nav class=\"navbar navbar-expand-sm navbar-toggleable-sm bg-primary border-bottom box-shadow mb-3\" data-bs-theme=\"dark\">         <div class=\"container\">             <a class=\"navbar-brand\" href=\"/Pages/Index\">@SiteTitle</a>             <button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#navbarContent\" aria-controls=\"navbarContent\"                     aria-expanded=\"false\" aria-label=\"Toggle navigation\">                 <span class=\"navbar-toggler-icon\"></span>             </button>             <div class=\"collapse navbar-collapse\" id=\"navbarContent\">                 <ul class=\"navbar-nav ms-auto\">      <li class=\"nav-item\" data-dynamic=\"search\">[пошук]</li>                     <li class=\"nav-item\" data-dynamic=\"menu\">[меню]</li>                     <li class=\"nav-item\" data-dynamic=\"login-button\">[вхід/адмін]</li>     </ul>             </div>         </div>     </nav> </header>','<p><span style=\"font-size:18px;\">© 2025 - супер - </span><a href=\"/about-us\"><span style=\"font-size:18px;\">посилання</span></a></p>','','','','Супер крутий сайт',NULL, NULL);
/*!40000 ALTER TABLE `site_settings` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users`
VALUES (1,'admin','admin@example.com','$2a$11$Fcmgk/lA.JA1k8A8BPtjVu7bKcy/l/33gcQDbI3xH39T/qFtvm43e','2025-02-13 19:26:22', NULL, '["Admin"]');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

INSERT INTO `partners` 
VALUES 
(1, 'Чернівецький національний університет імені Юрія Федьковича', '/uploads/partners/133bd514-6658-48de-998d-b72bf0fad8c0.jpg', 1, 'https://www.chnu.edu.ua/'),
(2, 'Yukon Software', '/uploads/partners/d42be15e-beaa-4a91-8b1c-5a138a69d19b.png', 2, 'https://yukon.software/ua/'),
(3, 'Elogic Commerce', '/uploads/partners/581dd57a-d469-40c1-a861-405a56dd17ff.png', 3, 'https://elogic.co/'),
(4, 'SoftServe', '/uploads/partners/36af068e-18f8-43ac-9a1d-a44d3341b042.png', 4, 'https://www.softserveinc.com/uk-ua'),
(5, 'Chernivtsi IT Cluster', '/uploads/partners/3bdbc070-8131-461a-88bc-b9958bb395c7.png', 5, 'https://www.it-cluster.cv.ua/'),
(6, 'Datawiz.io', '/uploads/partners/4d7223f6-56bd-42f9-940b-ba1dbc96a4bf.png', 6, 'https://datawiz.io/uk/');

INSERT INTO main_slider_slides (slot, image_url, alt_text) VALUES
(1, '/images/default/slider-image.jpg', 'Слайд 1'),
(2, '/images/default/slider-image.jpg', 'Слайд 2'),
(3, '/images/default/slider-image.jpg', 'Слайд 3');

INSERT INTO disciplines (name, description, sort_order, is_required) VALUES
('Soft Skills для інженерів програмного забезпечення','', 0, true),
('Алгоритми та структури даних','', 0, true),
('Архітектура комп''ютера','', 0, true),
('Бази даних','', 0, true),
('Вебдизайн','', 0, true),
('Вебпрограмування','', 0, true),
('Інформатика (Вступ до спеціальності)','', 0, true),
('Інформатика (Основи програмування)','', 0, true),
('Комп''ютерна дискретна математика','', 0, true),
('Компютерна графіка','', 0, true),
('Людино-машинний інтерфейс','', 0, true),
('Мова програмування Java','', 0, true),
('Моделювання та проєктування програмного забезпечення','', 0, true),
('Об''єктно-орієнтоване програмування','', 0, true),
('Операційні системи','', 0, true),
('Організація комп''ютерних мереж','', 0, true),
('Основи кібербезпеки','', 0, true),
('Основи командної розробки програмного забезпечення','', 0, true),
('Основи програмної інженерії','', 0, true),
('Основи програмування','', 0, true),
('Практикум з програмної інженерії','', 0, true),
('Прикладне програмування та штучний інтелект мовою Python','', 0, true),
('Програмування мобільних додатків','', 0, true),
('Технології (Вебдизайн)','', 0, true),
('Технології (Операційні системи)','', 0, true),
('Технології створення додатків для платформи .NET','', 0, true),
('Управління ІТ проєктами','', 0, true),
('Якість та тестування програмного забезпечення','', 0, true);

INSERT INTO categories (name, title) VALUES
('our-life', 'Наше житття'),
('events', 'Події');

INSERT INTO employees (id, lastname, firstname, patronymic_name, email, google_drive_folder_url, specialization, bio, photo_url) VALUES
(1, 'Атвіновський','Олексій','Ігорович','oleksii.atvinovskyi@polytech.cv.ua', 'https://drive.google.com/drive/folders/1H-2NKUMYjiqDmLExP6UC39M55WEgftv7','','',''),
(2, 'Басовська','Антоніна','Іванівна','toniabasovska@polytech.cv.ua', 'https://drive.google.com/drive/folders/1a5bcjGjVFUhuh-q3h3qW4CQtXI75WD5t','','',''),
(3, 'Воронов','Максим','Олегович','mvoronov@polytech.cv.ua', 'https://drive.google.com/drive/folders/1-jGieVmcft7iGoRKKyZ8iVsBag510En5','','',''),
(4, 'Воропаєва', 'Світлана', 'Львівна', 'voropaeva@chnu.edu.ua','https://drive.google.com/drive/folders/1wtQO68Nm30scTkGTN7N_vge2moHSXM1O','','',''),
(5, 'Гуменна','Тетяна','Миколаївна','gumenna.t.m@polytech.cv.ua', 'https://drive.google.com/drive/folders/1gXaKebFswjvh_lyIuQGm5CbqigpXbV5S','','',''),
(6, 'Кравченко','Лариса','Богданівна','lkravchenko@polytech.cv.ua', 'https://drive.google.com/drive/folders/1E7oO8kqDqHX0suM058FyvWY4L5NHdRIg','','',''),
(7, 'Кравчук','Оксана','Миколаївна','oksana.kravchuk@polytech.cv.ua', 'https://drive.google.com/drive/folders/1CwEE1v9gssYoyvsnu6WmUosV4nF6siIN','','',''),
(8, 'Серьогіна','Ірина','Вікторівна','seryogina.i.v@polytech.cv.ua', 'https://drive.google.com/drive/folders/1BqmoM2rmgCudSQtNVSIrG25Ygny7fQgT','','',''),
(9, 'Скуртул', 'Сергій','Сергійович', 'Serhii.Skurtul@polytech.cv.ua', 'https://drive.google.com/drive/folders/1nksiaUgZ2TJeV58Ja7OVeHbf_fzaSXOR','','',''),
(10, 'Смерека','Анна','Георгіївна','smereka.a.h@polytech.cv.ua', 'https://drive.google.com/drive/folders/1-DPYpxLY3hi2yKJtVhXasmGaOvoWMNAd','','',''),
(11, 'Шпак','Андрій','Олексійович','shpak.andrii@polytech.cv.ua', 'https://drive.google.com/drive/folders/1N9RvV2_YcLg6D0D24k3cgcV4s1LwRJhZ','','','');

INSERT INTO organization_info (id, address, email, phone_numbers) VALUES(1, '58029, Чернівці, вул. Павла Каспрука, 2', 'ipz@polytech.cv.ua', '(0372) 56-70-47,+38 (095) 700-64-51');

INSERT INTO organization_info_social_links (id, title, url, icon, `order`, organization_info_id) VALUES(1, 'Instagram', 'https://www.instagram.com/ckprogr', 'instagram-icon.svg', 0, 1);
INSERT INTO organization_info_social_links (id, title, url, icon, `order`, organization_info_id) VALUES(2, 'YouTube', 'https://www.youtube.com/channel/UC2wIoLJ20hBEZhvrqtOSRBw', 'youtube-icon.svg', 1, 1);
INSERT INTO organization_info_social_links (id, title, url, icon, `order`, organization_info_id) VALUES(3, 'Email', 'mailto:ipz@polytech.cv.ua', 'email-icon.svg', 2, 1);


INSERT INTO posts (slug, title, excerpt, content, thumbnail, status, created_at, updated_at, published_at, author_id, views_count) VALUES
('welcome-to-our-site', 'Ласкаво просимо на сайт кафедри!', 'Коротко про те, чим ми займаємось', '<p>Це перший тестовий пост для перевірки роботи стрічки новин.</p>', NULL, 'Published', '2026-08-01 10:00:00', '2026-08-01 10:00:00', '2026-08-01 10:00:00', 1, 12),
('open-day-announcement', 'День відкритих дверей', 'Запрошуємо абітурієнтів', '<p>Деталі про день відкритих дверей будуть додані пізніше.</p>', NULL, 'Published', '2026-08-15 09:30:00', '2026-08-15 09:30:00', '2026-08-15 09:30:00', 1, 5),
('draft-post-example', 'Чернетка для тестування', NULL, '<p>Цей пост ще не опубліковано — для перевірки статусу Draft.</p>', NULL, 'Draft', '2026-09-01 14:00:00', '2026-09-01 14:00:00', NULL, 1, 0);

INSERT INTO post_categories (category_id, post_id) VALUES
(1, 1),
(2, 2);
