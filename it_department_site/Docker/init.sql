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
INSERT INTO `menu_items` VALUES (1,'Home',NULL,1,1),(4,'Blog',NULL,NULL,2),(13,'About Us',NULL,2,3),(14,'Blog Post 1',4,36,99),(16,'Нова сторінка!!!',4,38,0),(17,'новий пункт',NULL,34,12);
/*!40000 ALTER TABLE `menu_items` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `pages` WRITE;
/*!40000 ALTER TABLE `pages` DISABLE KEYS */;
INSERT INTO `pages` VALUES
(1,'Home Page','home-page','Published','2025-03-13 20:54:36','2025-06-20 04:07:22','no-sidebar','{\"main\": \"<p><strong>This is the content for the Home Page.</strong></p>\"}',1,4),
(2,'About Us','about-us','Published','2025-03-13 20:54:36','2025-06-20 04:07:45','no-sidebar','{\"main\": \"This page contains information about our company.\"}',1,5),
(3,'Contact Us','contact-us','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page is under construction. Contact form will be available soon.\"}',1,0),
(5,'Privacy Policy','privacy-policy','Published','2025-03-13 20:54:36','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cp\\u003E234\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',1,0),
(6,'Terms of Service','terms-of-service','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page contains the terms of service for our website.\"}',1,0),
(7,'FAQ','faq','Published','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"Frequently Asked Questions about our products and services.\"}',1,0),
(27,'hello!!!','hello','Draft','2025-05-23 22:36:42','2025-05-23 22:36:42','right-sidebar','{\"left\": \"\", \"main\": \"<p>hello!!!</p>\", \"right\": \"<p>hello!!!</p>\"}',1,0),
(28,'ывывыв','storink','Draft','2025-05-26 12:37:53','2025-05-26 12:37:53','left-sidebar','{\"left\": \"<p>ывывывывывыв</p>\", \"main\": \"<p>ывывывыв</p>\", \"right\": \"\"}',1,0),
(32,'encrypted','encrypted','Draft','2025-06-10 18:02:43','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cp\\u003Eencrypted1\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',1,0),
(33,'<script>alert(\"XSS\");</script>','encrypted-sidebar','Draft','2025-06-10 18:14:08','2026-08-05 11:44:37','right-sidebar','{\"main\":\"\\u003Cp\\u003Eencrypted-sidebar.alert(\\u0022qqq\\u0022)\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\\u003Cp\\u003Eencrypted-sidebar\\u003C/p\\u003E\"}',1,0),
(34,'picture!!!','picture','Published','2025-06-10 19:42:26','2026-08-05 11:44:37','no-sidebar','{\"main\":\"\\u003Cfigure class=\\u0022image image-style-align-left image_resized\\u0022 style=\\u0022width:40.5%;\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/6562b57b-7180-4494-8fc0-3980c6b51020.jpg\\u0022\\u003E\\u003C/figure\\u003E\\u003Cp\\u003ELorem ipsum dolor sit amet...\",\"left\":\"\",\"right\":\"\"}',1,3),
(35,'nov','nov','Published','2025-06-11 12:36:02','2026-08-05 11:44:38','right-sidebar','{\"main\":\"\\u003Cp\\u003Eergjiortjhhoiprtjgip4rthjip0ghj4pihj4igh4ijgpiefjvipdfhvipehrfpijpifhepfjepirgfjpiehjgfpiehipehvkpekpvhepkvhepkghperhgi\\u003C/p\\u003E\\u003Cfigure class=\\u0022image image_resized\\u0022 style=\\u0022width:53.06%;\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/d8a627f2-0d47-4507-bb43-b3bf1882d428.jpg\\u0022\\u003E\\u003C/figure\\u003E\",\"left\":\"\",\"right\":\"\\u003Cp\\u003Eednjvoerefipgjrighjeoigjhereoigehgiehvgvpidsdpkdvfjerpighiprvjiehjgip3rhjgpedhjdvpierhrvfpihepicnpiedcnpie3rhrpienievcip3jpiwev\\u003C/p\\u003E\\u003Cfigure class=\\u0022image\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/da7b40d6-b3ac-496e-bdc4-b66754a1ffa2.png\\u0022\\u003E\\u003C/figure\\u003E\"}',0,1),
(36,'storink','storink2','Published','2025-06-12 11:21:11','2026-08-05 11:44:38','no-sidebar','{\"main\":\"\\u003Cfigure class=\\u0022image\\u0022\\u003E\\u003Cimg src=\\u0022/uploads/f0593841-cb3d-442c-b3df-fc81467cfa21.gif\\u0022\\u003E\\u003C/figure\\u003E\",\"left\":\"\",\"right\":\"\"}',1,2),
(38,'Нова сторінка!!!','new-page','Published','2025-06-20 04:09:03','2026-08-05 11:44:38','no-sidebar','{\"main\":\"\\u003Cp\\u003E\\u0446\\u0435 \\u0441\\u0443\\u043F\\u0435\\u0440 \\u043A\\u0440\\u0443\\u0442\\u0430 \\u0441\\u0442\\u043E\\u0440\\u0456\\u043D\\u043A\\u0430!!!\\u003C/p\\u003E\",\"left\":\"\",\"right\":\"\"}',0,2);
UNLOCK TABLES;

LOCK TABLES `site_settings` WRITE;
/*!40000 ALTER TABLE `site_settings` DISABLE KEYS */;
INSERT INTO `site_settings` VALUES (1,'<header>     <nav class=\"navbar navbar-expand-sm navbar-toggleable-sm bg-primary border-bottom box-shadow mb-3\" data-bs-theme=\"dark\">         <div class=\"container\">             <a class=\"navbar-brand\" href=\"/Pages/Index\">@SiteTitle</a>             <button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#navbarContent\" aria-controls=\"navbarContent\"                     aria-expanded=\"false\" aria-label=\"Toggle navigation\">                 <span class=\"navbar-toggler-icon\"></span>             </button>             <div class=\"collapse navbar-collapse\" id=\"navbarContent\">                 <ul class=\"navbar-nav ms-auto\">      <li class=\"nav-item\" data-dynamic=\"search\">[пошук]</li>                     <li class=\"nav-item\" data-dynamic=\"menu\">[меню]</li>                     <li class=\"nav-item\" data-dynamic=\"login-button\">[вхід/адмін]</li>     </ul>             </div>         </div>     </nav> </header>','<p><span style=\"font-size:18px;\">© 2025 - супер - </span><a href=\"/about-us\"><span style=\"font-size:18px;\">посилання</span></a></p>','','','','Супер крутий сайт',NULL);
/*!40000 ALTER TABLE `site_settings` ENABLE KEYS */;
UNLOCK TABLES;

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','admin@example.com','$2a$11$Fcmgk/lA.JA1k8A8BPtjVu7bKcy/l/33gcQDbI3xH39T/qFtvm43e','2025-02-13 19:26:22');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

INSERT INTO `partners` 
VALUES 
(6,'SoftServe','/images/partners/158dfc60-8098-4b55-bb6e-bff312b4bef7.png',1),
(7,'valtech','/images/partners/b0b9aa9a-2e00-4c73-9756-094b75e149ed.png',0),
(8,'yukon','/images/partners/fb19c67f-c425-4398-976d-0b52bf443e63.png',2);
(10, 'Чернівецький національний університет імені Юрія Федьковича', '/images/partners/133bd514-6658-48de-998d-b72bf0fad8c0.jpg', 1, 'https://www.chnu.edu.ua/'),
(11, 'Yukon Software', '/images/partners/d42be15e-beaa-4a91-8b1c-5a138a69d19b.png', 2, 'https://yukon.software/ua/'),
(12, 'Elogic Commerce', '/images/partners/581dd57a-d469-40c1-a861-405a56dd17ff.png', 3, 'https://elogic.co/'),
(13, 'SoftServe', '/images/partners/36af068e-18f8-43ac-9a1d-a44d3341b042.png', 4, 'https://www.softserveinc.com/uk-ua'),
(14, 'Chernivtsi IT Cluster', '/images/partners/3bdbc070-8131-461a-88bc-b9958bb395c7.png', 5, 'https://www.it-cluster.cv.ua/'),
(15, 'Datawiz.io', '/images/partners/4d7223f6-56bd-42f9-940b-ba1dbc96a4bf.png', 6, 'https://datawiz.io/uk/');

INSERT INTO main_slider_slides (slot, image_url, alt_text) VALUES
(1, '/images/banner_photo_1.JPG', 'Слайд 1'),
(2, '/images/banner_photo_1.JPG', 'Слайд 2'),
(3, '/images/banner_photo_1.JPG', 'Слайд 3');
