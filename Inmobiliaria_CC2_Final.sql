CREATE DATABASE  IF NOT EXISTS `inmobiliaria_cc2` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `inmobiliaria_cc2`;
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
  `Foto` varchar(255) DEFAULT NULL,
  `PorcentajeReserva` decimal(5,2) NOT NULL DEFAULT '0.00',
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
INSERT INTO `inmueble` VALUES (1,'Falucho 542',4,1,NULL,2000.00,1,1,NULL,0.00),(2,'Belgrano 658',6,2,NULL,2500.00,1,1,NULL,20.00),(3,'Junin 2316',2,2,NULL,3000.00,2,1,NULL,0.00),(4,'Valle de Piedra 5627',8,1,NULL,6000.00,2,1,NULL,0.00),(5,'Pedernera 1000',3,3,NULL,1500.00,3,1,NULL,0.00),(6,'Bolivar 900',10,1,NULL,8000.00,3,1,NULL,0.00);
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
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilino`
--

LOCK TABLES `inquilino` WRITE;
/*!40000 ALTER TABLE `inquilino` DISABLE KEYS */;
INSERT INTO `inquilino` VALUES (1,'32325','Maria','Del Valle','mdv@gmail.com','56487',NULL,1,'2026-09-03 16:38:36'),(2,'23258','Fabian ','Castro','fc@gmail.com','45678',NULL,1,'2026-09-03 16:39:10'),(3,'75478','Jose','Paez','jp@gmail.com','231598',NULL,1,'2026-09-03 16:39:54'),(4,'8546','Josefina','Sosa','js@gmail.com','562479',NULL,1,'2026-09-03 16:40:33'),(5,'56245','Favio','Garcia','fg@gmail.com','23214552',NULL,0,'2026-09-16 22:55:02'),(6,'665469','Ana','Costas','ac@gmail.com','256478',NULL,1,'2026-09-16 23:18:40');
/*!40000 ALTER TABLE `inquilino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pago`
--

DROP TABLE IF EXISTS `pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pago` (
  `IdPago` int NOT NULL AUTO_INCREMENT,
  `IdReserva` int NOT NULL,
  `Concepto` varchar(100) NOT NULL,
  `Monto` decimal(10,2) NOT NULL,
  `FechaPago` datetime NOT NULL,
  `MedioPago` varchar(50) NOT NULL,
  `Observacion` varchar(255) DEFAULT NULL,
  `IdUsuarioCreacion` int DEFAULT NULL,
  `IdUsuarioAnulacion` int DEFAULT NULL,
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`IdPago`),
  KEY `IX_Pago_IdReserva` (`IdReserva`),
  KEY `IX_Pago_IdUsuarioCreacion` (`IdUsuarioCreacion`),
  KEY `IX_Pago_IdUsuarioAnulacion` (`IdUsuarioAnulacion`),
  CONSTRAINT `FK_Pago_Reserva` FOREIGN KEY (`IdReserva`) REFERENCES `reserva` (`IdReserva`),
  CONSTRAINT `FK_Pago_UsuarioAnulacion` FOREIGN KEY (`IdUsuarioAnulacion`) REFERENCES `usuario` (`id_usuario`),
  CONSTRAINT `FK_Pago_UsuarioCreacion` FOREIGN KEY (`IdUsuarioCreacion`) REFERENCES `usuario` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pago`
--

LOCK TABLES `pago` WRITE;
/*!40000 ALTER TABLE `pago` DISABLE KEYS */;
INSERT INTO `pago` VALUES (1,9,'Pago parcial de seña',1000.00,'2026-09-17 14:38:14','Efectivo','Prueba integral - pago parcial',1,NULL,1),(2,9,'Completar seña',500.00,'2026-09-17 14:39:30','Efectivo','Prueba integral - completar seña',1,1,0);
/*!40000 ALTER TABLE `pago` ENABLE KEYS */;
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
  `IdReservaOrigen` int DEFAULT NULL,
  `IdInquilino` int NOT NULL,
  `IdInmueble` int NOT NULL,
  `MontoDia` decimal(10,2) NOT NULL,
  `PorcentajeReserva` decimal(5,2) NOT NULL DEFAULT '0.00',
  `FechaDesde` date NOT NULL,
  `FechaHasta` date NOT NULL,
  `FechaTerminacionAnticipada` date DEFAULT NULL,
  `Multa` decimal(10,2) NOT NULL DEFAULT '0.00',
  `Estado` tinyint(1) NOT NULL DEFAULT '1',
  `IdUsuarioCreacion` int DEFAULT NULL,
  `IdUsuarioTerminacion` int DEFAULT NULL,
  PRIMARY KEY (`IdReserva`),
  KEY `FK_Reserva_Inquilino` (`IdInquilino`),
  KEY `FK_Reserva_Inmueble` (`IdInmueble`),
  KEY `FK_Reserva_ReservaOrigen` (`IdReservaOrigen`),
  CONSTRAINT `FK_Reserva_Inmueble` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`),
  CONSTRAINT `FK_Reserva_Inquilino` FOREIGN KEY (`IdInquilino`) REFERENCES `inquilino` (`IdInquilino`),
  CONSTRAINT `FK_Reserva_ReservaOrigen` FOREIGN KEY (`IdReservaOrigen`) REFERENCES `reserva` (`IdReserva`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reserva`
--

LOCK TABLES `reserva` WRITE;
/*!40000 ALTER TABLE `reserva` DISABLE KEYS */;
INSERT INTO `reserva` VALUES (1,NULL,1,1,2000.00,0.00,'2026-09-04','2026-09-12',NULL,0.00,1,NULL,NULL),(2,NULL,2,1,2000.00,0.00,'2026-09-13','2026-09-17','2026-09-15',1000.00,0,NULL,NULL),(3,NULL,4,6,8000.00,0.00,'2026-09-04','2026-09-08',NULL,0.00,1,NULL,NULL),(4,NULL,3,4,6000.00,0.00,'2026-09-11','2026-09-18',NULL,0.00,1,NULL,NULL),(5,NULL,1,2,2500.00,0.00,'2026-09-10','2026-09-17',NULL,0.00,1,NULL,NULL),(6,NULL,3,5,1500.00,0.00,'2026-09-18','2026-09-21','2026-09-19',1500.00,0,NULL,NULL),(7,NULL,6,6,8000.00,0.00,'2026-10-14','2026-10-22',NULL,0.00,1,NULL,NULL),(8,NULL,1,2,2500.00,0.00,'2026-10-27','2026-10-30',NULL,0.00,1,1,NULL),(9,NULL,1,2,2500.00,20.00,'2026-11-01','2026-11-04',NULL,0.00,1,1,NULL);
/*!40000 ALTER TABLE `reserva` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `solicitudreserva`
--

DROP TABLE IF EXISTS `solicitudreserva`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `solicitudreserva` (
  `IdSolicitud` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) NOT NULL,
  `Apellido` varchar(50) NOT NULL,
  `Dni` varchar(20) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Telefono` varchar(30) NOT NULL,
  `IdInmueble` int NOT NULL,
  `FechaDesde` date NOT NULL,
  `FechaHasta` date NOT NULL,
  `FechaSolicitud` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Estado` varchar(20) NOT NULL DEFAULT 'Pendiente',
  PRIMARY KEY (`IdSolicitud`),
  KEY `FK_SolicitudReserva_Inmueble` (`IdInmueble`),
  CONSTRAINT `FK_SolicitudReserva_Inmueble` FOREIGN KEY (`IdInmueble`) REFERENCES `inmueble` (`IdInmueble`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `solicitudreserva`
--

LOCK TABLES `solicitudreserva` WRITE;
/*!40000 ALTER TABLE `solicitudreserva` DISABLE KEYS */;
INSERT INTO `solicitudreserva` VALUES (1,'Juan','Cruz','999','j@gmail.com','6523',1,'2026-09-30','2026-10-07','2026-09-16 18:53:39','Pendiente'),(2,'Ana','Costas','665469','ac@gmail.com','256478',6,'2026-10-14','2026-10-22','2026-09-16 22:47:08','Confirmada');
/*!40000 ALTER TABLE `solicitudreserva` ENABLE KEYS */;
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

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `email` varchar(150) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `rol` varchar(30) NOT NULL,
  `estado` tinyint(1) NOT NULL DEFAULT '1',
  `avatar` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Administrador','admin@inmobiliaria.com','AQAAAAIAAYagAAAAEDfhEkjHJawp7VU2ON6iY5pCNkx7Hv9VxRn98+zM9khUGjkjRT1fjGASzleFBX+NXQ==','Administrador',1,NULL),(2,'Guillermo','empleado@inmobiliaria.com','AQAAAAIAAYagAAAAEAb+xzUBeKsRjRc6GTlQ4VSxFP/ncNB25qVBGwMEc4uqUFLSqnsLQ+z9MVkmsdKjAg==','Empleado',1,'/uploads/usuarios/usuario_2_1f7d6a84-561f-4dab-b013-7073b0d0f2a6.jpeg');
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-17 22:00:36
