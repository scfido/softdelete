CREATE DATABASE `test`;

USE `test`;

CREATE TABLE `Users` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(64) NOT NULL,
  `DeleteTime` datetime(6) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL,
  PRIMARY KEY (`UserId`)
) ;
