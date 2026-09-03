-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: inmobiliaria_cc2
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `inmueble`
--

DROP TABLE IF EXISTS `inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmueble` (
  `IdInmueble` int NOT NULL AUTO_INCREMENT,
  `Direccion` varchar(255) NOT NULL,
  `Cupo` int NOT NULL,
  `IdTipo` int NOT NULL,
  `Coordenadas` varchar(100) DEFAULT NULL,
  `PrecioDia` decimal(10,2) NOT NULL,
  `IdPropietario` int NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdInmueble`),
  KEY `FK_Inmueble_Tipo` (`IdTipo`),
  KEY `FK_Inmueble_Propietario` (`IdPropietario`),
  CONSTRAINT `FK_Inmueble_Propietario` FOREIGN KEY (`IdPropietario`) REFERENCES `propietario` (`IdPropietario`),
  CONSTRAINT `FK_Inmueble_Tipo` FOREIGN KEY (`IdTipo`) REFERENCES `tipoinmueble` (`IdTipo`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmueble`
--

LOCK TABLES `inmueble` WRITE;
/*!40000 ALTER TABLE `inmueble` DISABLE KEYS */;
INSERT INTO `inmueble` VALUES (1,'Falucho 542',4,1,NULL,2000.00,1,1),(2,'Belgrano 658',6,2,NULL,2500.00,1,1),(3,'Junin 2316',2,2,NULL,3000.00,2,1),(4,'Valle de Piedra 5627',8,1,NULL,6000.00,2,1),(5,'Pedernera 1000',3,3,NULL,1500.00,3,1),(6,'Bolivar 900',10,1,NULL,8000.00,3,1);
/*!40000 ALTER TABLE `inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilino`
--

DROP TABLE IF EXISTS `inquilino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilino` (
  `IdInquilino` int NOT NULL AUTO_INCREMENT,
  `Dni` varchar(20) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(30) NOT NULL,
  `DireccionOrigen` varchar(150) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdInquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilino`
--

LOCK TABLES `inquilino` WRITE;
/*!40000 ALTER TABLE `inquilino` DISABLE KEYS */;
INSERT INTO `inquilino` VALUES (1,'32325','Maria','Del Valle','mdv@gmail.com','56487',NULL,1,'2026-09-03 16:38:36'),(2,'23258','Fabian ','Castro','fc@gmail.com','45678',NULL,1,'2026-09-03 16:39:10'),(3,'75478','Jose','Paez','jp@gmail.com','231598',NULL,1,'2026-09-03 16:39:54'),(4,'8546','Josefina','Sosa','js@gmail.com','562479',NULL,1,'2026-09-03 16:40:33');
/*!40000 ALTER TABLE `inquilino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietario`
--

DROP TABLE IF EXISTS `propietario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietario` (
  `IdPropietario` int NOT NULL AUTO_INCREMENT,
  `Dni` varchar(20) NOT NULL,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(30) NOT NULL,
  `Direccion` varchar(150) DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdPropietario`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietario`
--

LOCK TABLES `propietario` WRITE;
/*!40000 ALTER TABLE `propietario` DISABLE KEYS */;
INSERT INTO `propietario` VALUES (1,'88888','Guillermo','Concha','g@gmail.com','9999',NULL,1,'2026-09-03 16:28:36'),(2,'99999','Natalia','Camargo','n@gmail.com','44444',NULL,1,'2026-09-03 16:29:06'),(3,'555','Pablo','Perez','pp@gmail.com','77878',NULL,1,'2026-09-03 16:29:36'),(4,'2223','Maria','Lopez','ml@gmail.com','65624',NULL,1,'2026-09-03 16:30:08');
/*!40000 ALTER TABLE `propietario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reserva`
--

DROP TABLE IF EXISTS `reserva`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reserva` (
  `IdReserva` int NOT NULL AUTO_INCREMENT,
  `IdInquilino` int NOT NULL,
  `IdInmueble` int NOT NULL,
  `MontoDia` decimal(10,2) NOT NULL,
  `FechaDesde` date NOT NULL,
  `FechaHasta` date NOT NULL,
  `FechaTerminacionAnticipada` date DEFAULT NULL,
  `Multa` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdReserva`),
  KEY `FK_Reserva_Inquilino` (`IdInquilino`),
  KEY `FK_Reserva_Inmueble` (`IdInmueble`),
  CONSTRAINT `FK_Reserva_Inmueble` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`),
  CONSTRAINT `FK_Reserva_Inquilino` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilino` (`IdInquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reserva`
--

LOCK TABLES `reserva` WRITE;
/*!40000 ALTER TABLE `reserva` DISABLE KEYS */;
INSERT INTO `reserva` VALUES (1,1,1,2000.00,'2026-09-04','2026-09-12',NULL,0.00,1),(2,2,1,2000.00,'2026-09-13','2026-09-17',NULL,0.00,1),(3,4,6,8000.00,'2026-09-04','2026-09-08',NULL,0.00,1),(4,3,4,6000.00,'2026-09-11','2026-09-18',NULL,0.00,1),(5,1,2,2500.00,'2026-09-10','2026-09-17',NULL,0.00,1),(6,3,5,1500.00,'2026-09-18','2026-09-21','2026-09-19',1500.00,0);
/*!40000 ALTER TABLE `reserva` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tipoinmueble`
--

DROP TABLE IF EXISTS `tipoinmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipoinmueble` (
  `IdTipo` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdTipo`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tipoinmueble`
--

LOCK TABLES `tipoinmueble` WRITE;
/*!40000 ALTER TABLE `tipoinmueble` DISABLE KEYS */;
INSERT INTO `tipoinmueble` VALUES (1,'Casa',1),(2,'Departamento',1),(3,'Cabaña',1);
/*!40000 ALTER TABLE `tipoinmueble` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-03 18:20:37
