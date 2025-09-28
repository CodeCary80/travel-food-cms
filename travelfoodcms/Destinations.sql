-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost:8889
-- Generation Time: Sep 28, 2025 at 03:07 AM
-- Server version: 8.0.35
-- PHP Version: 8.3.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `TravelFoodDB`
--

-- --------------------------------------------------------

--
-- Table structure for table `Destinations`
--

CREATE TABLE `Destinations` (
  `DestinationId` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Location` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ImageUrl` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Date` datetime(6) NOT NULL,
  `CreatorUserId` int DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Destinations`
--

INSERT INTO `Destinations` (`DestinationId`, `Name`, `Location`, `Description`, `ImageUrl`, `Date`, `CreatorUserId`) VALUES
(1, 'Paris', 'France', 'The City of Light featuring iconic Eiffel Tower, world-class museums, and exquisite culinary experiences', 'paris_skyline.jpg', '2023-01-15 10:30:00.000000', 1),
(2, 'Tokyo', 'Japan', 'Ultra-modern metropolis with traditional temples, vibrant street food, and cutting-edge technology', 'tokyo_view.jpg', '2023-02-18 14:45:00.000000', 1),
(3, 'New York 1', 'Canada 1', 'The Big Apple offering diverse neighborhoods, world-famous attractions, and international cuisine', '/images/ea84d772-373e-4277-bb5f-063bc4f1f5ac_laravel_terminal.png', '2023-03-10 09:15:00.000000', 2),
(4, 'Barcelona', 'Spain', 'Mediterranean city known for Gaudí architecture, beautiful beaches, and vibrant food scene', 'barcelona_view.jpg', '2023-04-05 11:20:00.000000', 3),
(5, 'Sydney', 'Australia', 'Harbor city featuring the Opera House, stunning beaches, and multicultural dining experiences', 'sydney_harbor.jpg', '2023-05-12 16:30:00.000000', 2),
(6, 'Paris', 'France', 'The City of Light featuring iconic Eiffel Tower, world-class museums, and exquisite culinary experiences', 'paris_skyline.jpg', '2023-01-15 10:30:00.000000', 1),
(7, 'Tokyo', 'Japan', 'Ultra-modern metropolis with traditional temples, vibrant street food, and cutting-edge technology', 'tokyo_view.jpg', '2023-02-18 14:45:00.000000', 1),
(8, 'New York', 'USA', 'The Big Apple offering diverse neighborhoods, world-famous attractions, and international cuisine', 'nyc_skyline.jpg', '2023-03-10 09:15:00.000000', 2),
(9, 'Barcelona', 'Spain', 'Mediterranean city known for Gaudí architecture, beautiful beaches, and vibrant food scene', 'barcelona_view.jpg', '2023-04-05 11:20:00.000000', 3),
(10, 'Sydney', 'Australia', 'Harbor city featuring the Opera House, stunning beaches, and multicultural dining experiences', 'sydney_harbor.jpg', '2023-05-12 16:30:00.000000', 2),
(15, 'Paris1', 'France1', 'h111', '/images/ed8b63a5-1df9-4f5c-81b7-917071a08687_laravel_screen.png', '2025-04-03 09:26:24.646512', NULL),
(16, 'Le Petit Bistro10', 'France10', 'I don\'t even know where it is', '/images/2bb5c9a7-447b-4bd0-8f53-96da4da4eb64_laravel_screen.png', '2025-04-04 19:23:26.140388', NULL);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Destinations`
--
ALTER TABLE `Destinations`
  ADD PRIMARY KEY (`DestinationId`),
  ADD KEY `IX_Destinations_CreatorUserId` (`CreatorUserId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `Destinations`
--
ALTER TABLE `Destinations`
  MODIFY `DestinationId` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `Destinations`
--
ALTER TABLE `Destinations`
  ADD CONSTRAINT `FK_Destinations_Users_CreatorUserId` FOREIGN KEY (`CreatorUserId`) REFERENCES `Users` (`UserId`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
