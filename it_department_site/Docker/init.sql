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

UNLOCK TABLES;

--
-- Table structure for table `menu_items`
--

DROP TABLE IF EXISTS `menu_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `menu_items` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `parent_id` int DEFAULT NULL,
  `page_id` int DEFAULT NULL,
  `sort_order` int DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `parent_id` (`parent_id`),
  KEY `page_id` (`page_id`),
  CONSTRAINT `menu_items_ibfk_1` FOREIGN KEY (`parent_id`) REFERENCES `menu_items` (`id`) ON DELETE SET NULL,
  CONSTRAINT `menu_items_ibfk_2` FOREIGN KEY (`page_id`) REFERENCES `pages` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `menu_items`
--

LOCK TABLES `menu_items` WRITE;
/*!40000 ALTER TABLE `menu_items` DISABLE KEYS */;
INSERT INTO `menu_items` VALUES (1,'Home',NULL,1,1),(4,'Blog',NULL,NULL,2),(13,'About Us',NULL,2,3),(14,'Blog Post 1',4,36,99),(16,'Нова сторінка!!!',4,38,0),(17,'новий пункт',NULL,34,12);
/*!40000 ALTER TABLE `menu_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `page_views`
--

DROP TABLE IF EXISTS `page_views`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `page_views` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PageId` int NOT NULL,
  `UserId` int NOT NULL,
  `ViewedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `PageId` (`PageId`,`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `page_views`
--

LOCK TABLES `page_views` WRITE;
/*!40000 ALTER TABLE `page_views` DISABLE KEYS */;
INSERT INTO `page_views` VALUES (1,2,1,'2025-06-16 22:09:50'),(2,1,1,'2025-06-16 22:09:53'),(3,36,1,'2025-06-16 22:10:19'),(4,2,3,'2025-06-16 22:19:16'),(5,1,3,'2025-06-16 22:19:42'),(6,34,1,'2025-06-16 22:36:09'),(7,35,1,'2025-06-16 22:37:54'),(8,1,4,'2025-06-16 23:20:22'),(9,2,4,'2025-06-16 23:20:25'),(10,34,4,'2025-06-16 23:32:36'),(11,2,5,'2025-06-20 03:56:05'),(12,36,5,'2025-06-20 03:56:23'),(13,1,6,'2025-06-20 04:07:22'),(14,2,6,'2025-06-20 04:07:45'),(15,38,1,'2025-06-20 04:09:29'),(16,34,6,'2025-06-20 04:11:12'),(17,38,6,'2025-06-20 04:11:42');
/*!40000 ALTER TABLE `page_views` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pages`
--

DROP TABLE IF EXISTS `pages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  `slug` varchar(255) NOT NULL,
  `status` enum('Draft','Published') NOT NULL DEFAULT 'Draft',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `layout_type` varchar(50) NOT NULL DEFAULT 'no-sidebar',
  `content` text,
  `allow_comments` tinyint(1) DEFAULT '1',
  `Views` int DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `slug` (`slug`)
) ENGINE=InnoDB AUTO_INCREMENT=39 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pages`
--

LOCK TABLES `pages` WRITE;
/*!40000 ALTER TABLE `pages` DISABLE KEYS */;
INSERT INTO `pages` VALUES (1,'Home Page','home-page','Published','2025-03-13 20:54:36','2025-06-20 04:07:22','no-sidebar','{\"main\": \"<p><strong>This is the content for the Home Page.</strong></p>\"}',1,4),(2,'About Us','about-us','Published','2025-03-13 20:54:36','2025-06-20 04:07:45','no-sidebar','{\"main\": \"This page contains information about our company.\"}',1,5),(3,'Contact Us','contact-us','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page is under construction. Contact form will be available soon.\"}',1,0),(5,'Privacy Policy','privacy-policy','Published','2025-03-13 20:54:36','2025-06-11 12:44:49','no-sidebar','Bxze6i/YUpVwUQ02TAli0NUqfVK7t1itB5Axaj23jIIFh4YrswG6oY4d1eenXa6UOIl50jKsMCAXwC8TTly5AA==',1,0),(6,'Terms of Service','terms-of-service','Draft','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"This page contains the terms of service for our website.\"}',1,0),(7,'FAQ','faq','Published','2025-03-13 20:54:36','2025-05-22 19:48:40','no-sidebar','{\"main\": \"Frequently Asked Questions about our products and services.\"}',1,0),(27,'hello!!!','hello','Draft','2025-05-23 22:36:42','2025-05-23 22:36:42','right-sidebar','{\"left\": \"\", \"main\": \"<p>hello!!!</p>\", \"right\": \"<p>hello!!!</p>\"}',1,0),(28,'ывывыв','storink','Draft','2025-05-26 12:37:53','2025-05-26 12:37:53','left-sidebar','{\"left\": \"<p>ывывывывывыв</p>\", \"main\": \"<p>ывывывыв</p>\", \"right\": \"\"}',1,0),(32,'encrypted','encrypted','Draft','2025-06-10 18:02:43','2025-06-10 18:13:45','no-sidebar','Bxze6i/YUpVwUQ02TAli0C5nw0wnyqPTHWLPrbk4Iei/Zr68pYsQ+JJXKsoPjAEnlGdzbTvQfqHb5XJrxDLJHAnYECe5w/osE3XJR5zzmAI=',1,0),(33,'<script>alert(\"XSS\");</script>','encrypted-sidebar','Draft','2025-06-10 18:14:08','2025-06-10 18:18:04','right-sidebar','Bxze6i/YUpVwUQ02TAli0FpD3eO7F3lX5KMoK/suMBiYR0K2wrt1kj4gpZBxRePTkg5IRirJ+hgo2Pi3XCKjJ0qTpvO1+jj/yVWAXtizGJQwbioAxrbUP1PLVM6YNnRdJzTzLZHYh/NDNBZYlkWEaqQOhAVpUAZFGhtl6uEVh5ZgSS9BQ2jC1r5Q8/0zFc+B',1,0),(34,'picture!!!','picture','Published','2025-06-10 19:42:26','2025-06-20 04:11:12','no-sidebar','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',1,3),(35,'nov','nov','Published','2025-06-11 12:36:02','2025-06-16 22:37:54','right-sidebar','Bxze6i/YUpVwUQ02TAli0C+irqnth2qec4i1PBqo3PkIboMsnzABgCKYzPfUtIe36sSi6QPyRGPi63BoqhL+4XC44W7wMhdhhTFhWBerO0EPxBm7BelcWsnngvdCjvAmTj+QUrt21BG9yMq6z4kF5RedeKuXp3oZoh7Q5vELi91cIAINWGUSslZgnKIkqKXyp5fv3jFUzPYddPCcKVBjuWtLbjcFIX56vz4DT9tywqamCRPcgTlnHu0+cg/ldQYLn8ZXwyFNtXQ4b/n4uIZEwuaxaHxBOMSBHTDHHmadQyugtPngmEO7QOjf5BVzMkHFdz6J9qqxhL/FPi3GPXzj8G5FaA6yxVEFkhUqAFYSoSFUvp9rVtNYhipIJAvjPNUxybwfH9lI3WlfAGDx2MvXB9RHM7XjZgvRW9W4d2+TTPh4A0lxWaGyMKuUAgjzOkAwjDgoF8hI6OY7TVe51Yi5uPu7ovb8XIAOMTwZ4mQXv2eKOd46irtBMm7RRKzSwBi26rvii2ATzsZMd4iTZ9OUS1wizshrJVAjYiRqwPKPuDLpHX7YYC63TnR0Ld/Hoh3dkSNZ8AzTj88AvqCFvxcnlqL8Rnxbz/q2NqC56sUSpZ8zIHCZUnQnxjzXQLU2wvLG+R4dPRLMGGUH7KvPh4X3bXDCG5FO9YAD77TqlkQMaBx5nqb8x6aW+DjrH0w71fzwO1N48bNaL9GaC27ucjpz8MWWkDMYMJ4MrmulSGSDcYXerz//QccWur2B2voCNhZHQDJQImWqYR+WR+JZjmnphNog2cOjCHhYX+FoeUtU39B7nxy2k5ELjtBcjhxJfY2R6EbDu/F2XAwcFaAvCfzly2slpOEYrxKDn+IRiYYWiStuA0l8pp+qFCfM/gzabQ4E',0,1),(36,'storink','storink2','Published','2025-06-12 11:21:11','2025-06-20 03:56:23','no-sidebar','HprPgUZx0G6JOaoUmqZOwQI3qgf1JE1ZZetwfrT60YDoCiGG5oU2Elg2fxaB7xFNbKAxp3u0EAz1oB8Qof1LEbBhH42XkQMk0Z4TAaNv3eLfXN7tTM7BqaiGzIwkgaJCqms3G+1PcBxb52mn4cHR0j+0QeYyh+5DETrj8keOT4wfeVQNyodWcNSSCnorNZMam/Lhlq+eAd5Y67ngyWb1xz0LdYGe5E8obGnA/jDhGOE=',1,2),(38,'Нова сторінка!!!','new-page','Published','2025-06-20 04:09:03','2025-06-20 04:11:42','no-sidebar','Bxze6i/YUpVwUQ02TAli0Hcc0Ngv0b5O0soM7ipmD4sB8VK3UFuh6S1gwh1gnwOLeEX00XOvjr7apAkyYaO6LXrfIoNJS/t2O+li5DItNtG850Lju1Inmtl3xl1Q5VzZDnSeXKTcIl0DRSRyTy4CHlvBGuB0X7Bvk9kxeS1XDw0Y5TNM+PNrZ9Urf55AHRfgOVfjYAWocru4ci0oB4oRFtPfHfYCrCZ6MxXQkZiAPOx3DccoilK9s1qApphsKh7p',0,2);
/*!40000 ALTER TABLE `pages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `partners`
--

DROP TABLE IF EXISTS `partners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `partners` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) DEFAULT NULL,
  `logo_file_name` varchar(255) NOT NULL,
  `sort_order` int DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `partners`
--

LOCK TABLES `partners` WRITE;
/*!40000 ALTER TABLE `partners` DISABLE KEYS */;
INSERT INTO `partners` VALUES (6,'SoftServe','/images/partners/158dfc60-8098-4b55-bb6e-bff312b4bef7.png',1),(7,'valtech','/images/partners/b0b9aa9a-2e00-4c73-9756-094b75e149ed.png',0),(8,'yukon','/images/partners/fb19c67f-c425-4398-976d-0b52bf443e63.png',2);
/*!40000 ALTER TABLE `partners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `site_settings`
--

DROP TABLE IF EXISTS `site_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `site_settings` (
  `Id` int NOT NULL,
  `HeaderHtml` text,
  `FooterHtml` text,
  `HeaderCss` text,
  `FooterCss` text,
  `CustomCss` text,
  `SiteTitle` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `SiteLogoUrl` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `site_settings`
--

LOCK TABLES `site_settings` WRITE;
/*!40000 ALTER TABLE `site_settings` DISABLE KEYS */;
INSERT INTO `site_settings` VALUES (1,'<header>     <nav class=\"navbar navbar-expand-sm navbar-toggleable-sm bg-primary border-bottom box-shadow mb-3\" data-bs-theme=\"dark\">         <div class=\"container\">             <a class=\"navbar-brand\" href=\"/Pages/Index\">@SiteTitle</a>             <button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#navbarContent\" aria-controls=\"navbarContent\"                     aria-expanded=\"false\" aria-label=\"Toggle navigation\">                 <span class=\"navbar-toggler-icon\"></span>             </button>             <div class=\"collapse navbar-collapse\" id=\"navbarContent\">                 <ul class=\"navbar-nav ms-auto\">      <li class=\"nav-item\" data-dynamic=\"search\">[пошук]</li>                     <li class=\"nav-item\" data-dynamic=\"menu\">[меню]</li>                     <li class=\"nav-item\" data-dynamic=\"login-button\">[вхід/адмін]</li>     </ul>             </div>         </div>     </nav> </header>','<p><span style=\"font-size:18px;\">© 2025 - супер - </span><a href=\"/about-us\"><span style=\"font-size:18px;\">посилання</span></a></p>','','','','Супер крутий сайт',NULL);
/*!40000 ALTER TABLE `site_settings` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','admin@example.com','$2a$11$Fcmgk/lA.JA1k8A8BPtjVu7bKcy/l/33gcQDbI3xH39T/qFtvm43e','2025-02-13 19:26:22');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

ALTER TABLE partners ADD COLUMN link VARCHAR(255) DEFAULT NULL;

-- Dump completed on 2026-02-18 10:54:48
INSERT INTO `partners` 
VALUES 
(10, 'Чернівецький національний університет імені Юрія Федьковича', '/images/partners/133bd514-6658-48de-998d-b72bf0fad8c0.jpg', 1, 'https://www.chnu.edu.ua/'),
(11, 'Yukon Software', '/images/partners/d42be15e-beaa-4a91-8b1c-5a138a69d19b.png', 2, 'https://yukon.software/ua/'),
(12, 'Elogic Commerce', '/images/partners/581dd57a-d469-40c1-a861-405a56dd17ff.png', 3, 'https://elogic.co/'),
(13, 'SoftServe', '/images/partners/36af068e-18f8-43ac-9a1d-a44d3341b042.png', 4, 'https://www.softserveinc.com/uk-ua'),
(14, 'Chernivtsi IT Cluster', '/images/partners/3bdbc070-8131-461a-88bc-b9958bb395c7.png', 5, 'https://www.it-cluster.cv.ua/'),
(15, 'Datawiz.io', '/images/partners/4d7223f6-56bd-42f9-940b-ba1dbc96a4bf.png', 6, 'https://datawiz.io/uk/');

CREATE TABLE posts (
  id int NOT NULL AUTO_INCREMENT,
  author_id int DEFAULT NULL,
  page_id int DEFAULT NULL,
  slug varchar(150) NOT NULL,
  title varchar(200) NOT NULL,
  excerpt text NOT NULL,
  thumbnail text,
  created_at datetime DEFAULT CURRENT_TIMESTAMP,
  updated_at datetime DEFAULT CURRENT_TIMESTAMP,
  content text,
  PRIMARY KEY (id),
  KEY posts_user_id_fk (author_id),
  CONSTRAINT posts_user_id_fk FOREIGN KEY (author_id) REFERENCES users (id)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;