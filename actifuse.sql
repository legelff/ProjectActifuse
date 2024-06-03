-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 03, 2024 at 07:15 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `actifuse`
--

-- --------------------------------------------------------

--
-- Table structure for table `activitys`
--

CREATE TABLE `activitys` (
  `ActivityId` int(11) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Type` varchar(255) DEFAULT NULL,
  `Price` decimal(5,2) DEFAULT NULL,
  `Accessibility` decimal(5,2) DEFAULT NULL,
  `Participants` int(11) DEFAULT NULL,
  `Link` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `activitys`
--

INSERT INTO `activitys` (`ActivityId`, `Name`, `Type`, `Price`, `Accessibility`, `Participants`, `Link`) VALUES
(1162360, 'Paint the first thing you see', 'recreational', 0.25, 0.20, 1, ''),
(1288934, 'Pull a harmless prank on one of your friends', 'social', 0.10, 0.20, 1, ''),
(1432113, 'Hold a yard sale', 'social', 0.00, 0.10, 1, ''),
(1488053, 'Find a charity and donate to it', 'charity', 0.40, 0.10, 1, ''),
(1505028, 'Go swimming with a friend', 'social', 0.10, 0.10, 2, ''),
(1638604, 'Have a football scrimmage with some friends', 'social', 0.00, 0.20, 8, ''),
(1770521, 'Write a note of appreciation to someone', 'social', 0.00, 0.00, 1, ''),
(1799120, 'Cook something together with someone', 'cooking', 0.30, 0.80, 2, ''),
(1878070, 'Volunteer at your local food pantry', 'charity', 0.00, 0.10, 1, ''),
(2790297, 'Learn how to whistle with your fingers', 'education', 0.00, 0.00, 1, ''),
(3151646, 'Organize your music collection', 'busywork', 0.00, 0.00, 1, ''),
(3456114, 'Buy a new house decoration', 'recreational', 0.40, 0.30, 1, ''),
(3818400, 'Make homemade ice cream', 'cooking', 0.20, 0.20, 1, ''),
(3950821, 'Learn Kotlin', 'education', 0.00, 0.80, 1, 'https://kotlinlang.org/'),
(4101229, 'Write a thank you letter to an influential person in your life', 'social', 0.00, 0.10, 1, ''),
(4387026, 'Go to the gym', 'recreational', 0.20, 0.10, 1, ''),
(4704256, 'Learn to greet someone in a new language', 'education', 0.10, 0.20, 1, ''),
(4894697, 'Pick up litter around your favorite park', 'charity', 0.00, 0.05, 1, ''),
(5319204, 'Patronize a local independent restaurant', 'recreational', 0.20, 0.10, 1, ''),
(5590133, 'Catch up with a friend over a lunch date', 'social', 0.20, 0.15, 2, ''),
(5920481, 'Make a to-do list for your week', 'busywork', 0.00, 0.05, 1, ''),
(6081071, 'Text a friend you haven\'t talked to in a long time', 'social', 0.05, 0.20, 2, ''),
(6204657, 'Surprise your significant other with something considerate', 'social', 0.00, 0.00, 1, ''),
(6378359, 'Organize your movie collection', 'busywork', 0.00, 0.00, 1, ''),
(6509779, 'Donate blood at a local blood center', 'charity', 0.00, 0.35, 1, 'https://www.redcross.org/give-blood'),
(6613330, 'Pot some plants and put them around your house', 'relaxation', 0.40, 0.30, 1, ''),
(6925988, 'Fix something that\'s broken in your house', 'diy', 0.10, 0.30, 1, ''),
(7091374, 'Make a simple musical instrument', 'music', 0.40, 0.25, 1, ''),
(7096020, 'Practice coding in your favorite lanaguage', 'recreational', 0.00, 0.10, 1, ''),
(7806284, 'Fill out a basketball bracket', 'recreational', 0.00, 0.10, 1, ''),
(8081693, 'Watch a classic movie', 'recreational', 0.10, 0.10, 1, ''),
(8321894, 'Do something nice for someone you care about', 'social', 0.00, 0.10, 1, ''),
(8394738, 'Learn origami', 'education', 0.20, 0.30, 1, ''),
(8557562, 'Have a paper airplane contest with some friends', 'social', 0.02, 0.05, 4, ''),
(8631548, 'Create a compost pile', 'diy', 0.00, 0.15, 1, ''),
(9081214, 'Back up important computer files', 'busywork', 0.20, 0.20, 1, ''),
(9149470, 'Compliment someone', 'social', 0.00, 0.00, 2, ''),
(9660022, 'Learn and play a new card game', 'recreational', 0.00, 0.00, 1, 'https://www.pagat.com'),
(9908721, 'Sit in the dark and listen to your favorite music with no distractions', 'relaxation', 0.00, 1.00, 1, '');

-- --------------------------------------------------------

--
-- Table structure for table `admins`
--

CREATE TABLE `admins` (
  `AdminId` int(11) NOT NULL,
  `AdminUsername` varchar(255) NOT NULL,
  `AdminPassword` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `historys`
--

CREATE TABLE `historys` (
  `HistoryId` int(11) NOT NULL,
  `UserId` int(11) DEFAULT NULL,
  `ActivityId` int(11) DEFAULT NULL,
  `IsFavorite` tinyint(1) DEFAULT NULL,
  `IsCompleted` tinyint(1) DEFAULT NULL,
  `GeneratedDateTime` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `historys`
--

INSERT INTO `historys` (`HistoryId`, `UserId`, `ActivityId`, `IsFavorite`, `IsCompleted`, `GeneratedDateTime`) VALUES
(3, 3, 4704256, 1, 0, '2024-05-23 18:44:28'),
(4, 3, 7091374, 0, 0, '2024-05-23 18:44:28'),
(5, 3, 1162360, 0, 0, '2024-05-23 18:44:28'),
(6, 3, 8631548, 0, 1, '2024-05-23 18:44:28'),
(7, 3, 3950821, 1, 0, '2024-05-23 18:44:28'),
(9, 3, 6081071, 0, 0, '2024-05-23 18:44:28'),
(11, 3, 9149470, 1, 0, '2024-05-23 18:44:28'),
(12, 3, 1505028, 0, 1, '2024-05-23 18:44:28'),
(13, 3, 1799120, 0, 0, '2024-05-23 18:44:28'),
(14, 3, 4387026, 1, 0, '2024-05-23 18:44:28'),
(15, 3, 8081693, 0, 0, '2024-05-23 18:44:28'),
(16, 3, 1488053, 0, 0, '2024-05-23 18:44:28'),
(17, 3, 3456114, 0, 0, '2024-05-23 18:44:28'),
(18, 3, 9081214, 0, 0, '2024-05-23 18:44:28'),
(19, 3, 1638604, 0, 1, '2024-05-23 18:44:28'),
(21, 3, 1288934, 0, 0, '2024-05-23 18:44:28'),
(22, 1, 1432113, 0, 0, '2024-05-23 18:44:28'),
(23, 1, 2790297, 0, 0, '2024-05-23 18:44:28'),
(24, 2, 5319204, 1, 0, '2024-05-28 17:55:55'),
(25, 2, 1638604, 0, 0, '2024-05-28 17:56:00'),
(26, 2, 5590133, 0, 0, '2024-05-28 17:56:03'),
(27, 2, 9908721, 0, 1, '2024-05-28 17:56:05'),
(28, 2, 8321894, 0, 0, '2024-05-28 17:56:07'),
(29, 2, 9081214, 1, 0, '2024-05-28 17:56:09'),
(30, 2, 9149470, 0, 0, '2024-05-28 17:56:11'),
(31, 2, 5920481, 0, 0, '2024-05-28 17:56:13'),
(32, 2, 4894697, 0, 1, '2024-05-28 17:56:16'),
(33, 2, 4387026, 0, 0, '2024-05-28 17:56:25'),
(34, 2, 7091374, 0, 0, '2024-05-28 18:09:26'),
(35, 13, 1878070, 0, 1, '2024-05-30 22:00:27'),
(37, 13, 1162360, 1, 0, '2024-05-30 22:00:35');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UserId` int(11) NOT NULL,
  `Username` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL DEFAULT 'Actifuser',
  `ProfilePicturePath` varchar(255) DEFAULT NULL,
  `VerificationCode` int(11) DEFAULT NULL,
  `VerificationCodeExpiry` datetime DEFAULT NULL,
  `IsAdmin` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UserId`, `Username`, `Password`, `Email`, `Name`, `ProfilePicturePath`, `VerificationCode`, `VerificationCodeExpiry`, `IsAdmin`) VALUES
(1, 'testUser1', '45ae8dac8eb87defe1c5e9e351af26e47d75d2eca334722b5fd0f3d0bad4060a', 'testEmail1@gmail.com', 'test1', 'Images\\cc44673c-900c-45ce-8c14-5086038f8bf2.jpg', 952752, '2024-05-20 00:38:41', 0),
(2, 'testUser2', '193b910ff8865403bade74352661568a1cb9544e5466fc5e0ab2c249262a567b', 'testEmail2@gmail.com', 'test2', 'Images\\797ad66b-c83e-454a-9dc8-e79ef6104e95.jpg', NULL, NULL, 0),
(3, 'L145', '7e8de8c25fb0e53a98dd1f766c701dfb0d7ce66f56bb6e8777f4589bdb60a0b7', 'legend1409ps4@gmail.com', 'Aryan', 'Images\\41f0823a-f74d-431f-a2c4-ef2acbb9a58d.jpg', 399190, '2024-05-21 20:32:16', 0),
(9, 'Aryan', '222a71572d19d8dbbe7d3053d485d1e74550f881c1e16162982f5f4cde00b526', 'r0984834@student.thomasmore.be', 'Aryan', NULL, NULL, NULL, 1),
(13, 'testUser5', '23438a81537ceb91d24ae22d2191a8b28c8deba75c95ed48ab57e5376f7674de', 'legend1409alt@gmail.com', 'testChange5', 'Images\\7b89b247-b2bc-42af-b9c1-f602db665cc0.jpg', 175598, '2024-05-30 22:07:41', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `activitys`
--
ALTER TABLE `activitys`
  ADD PRIMARY KEY (`ActivityId`);

--
-- Indexes for table `admins`
--
ALTER TABLE `admins`
  ADD PRIMARY KEY (`AdminId`),
  ADD UNIQUE KEY `AdminUsername` (`AdminUsername`);

--
-- Indexes for table `historys`
--
ALTER TABLE `historys`
  ADD PRIMARY KEY (`HistoryId`),
  ADD KEY `UserId` (`UserId`),
  ADD KEY `ActivityId` (`ActivityId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UserId`),
  ADD UNIQUE KEY `Username` (`Username`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `admins`
--
ALTER TABLE `admins`
  MODIFY `AdminId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `historys`
--
ALTER TABLE `historys`
  MODIFY `HistoryId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=38;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UserId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `historys`
--
ALTER TABLE `historys`
  ADD CONSTRAINT `historys_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`UserId`),
  ADD CONSTRAINT `historys_ibfk_2` FOREIGN KEY (`ActivityId`) REFERENCES `activitys` (`ActivityId`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
