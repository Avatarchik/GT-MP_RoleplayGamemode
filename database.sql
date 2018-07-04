-- phpMyAdmin SQL Dump
-- version 4.7.8
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Erstellungszeit: 04. Jul 2018 um 11:06
-- Server-Version: 5.7.22-0ubuntu0.16.04.1
-- PHP-Version: 7.1.14

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `gta_test`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Accounts`
--

CREATE TABLE `Accounts` (
  `Id` int(11) NOT NULL,
  `SocialClubName` longtext,
  `Password` longtext,
  `CreatedAt` datetime NOT NULL,
  `LastActivity` datetime NOT NULL,
  `IsLocked` tinyint(1) NOT NULL,
  `AdminLevel` int(11) NOT NULL,
  `Comment` longtext,
  `Ip` longtext,
  `HardwareID` longtext,
  `MaxCharacters` int(11) NOT NULL,
  `IsLoggedIn` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `BankAccounts`
--

CREATE TABLE `BankAccounts` (
  `Id` int(11) NOT NULL,
  `AccountNumber` int(11) NOT NULL,
  `Money` double NOT NULL,
  `PinCode` int(11) NOT NULL,
  `HistoryString` longtext,
  `OwnerUser` int(11) NOT NULL,
  `OwnerGroup` int(11) NOT NULL,
  `IsPrivate` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Characters`
--

CREATE TABLE `Characters` (
  `Id` int(11) NOT NULL,
  `SocialClubName` longtext,
  `CreatedAt` datetime NOT NULL,
  `LastActivity` datetime NOT NULL,
  `Locked` tinyint(1) NOT NULL,
  `Hunger` int(11) NOT NULL,
  `Thirst` int(11) NOT NULL,
  `Health` int(11) NOT NULL,
  `Armor` int(11) NOT NULL,
  `FirstName` longtext,
  `LastName` longtext,
  `Cash` double NOT NULL,
  `CharacterStyleString` longtext,
  `Position` longtext,
  `Rotation` double NOT NULL,
  `ClothingString` longtext,
  `BankAccountAccessString` longtext,
  `SalaryTime` int(11) NOT NULL,
  `TotalPlayTime` int(11) NOT NULL,
  `KeyRingString` longtext,
  `InventoryString` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Garages`
--

CREATE TABLE `Garages` (
  `Id` int(11) NOT NULL,
  `Position` longtext,
  `OwnerUser` int(11) NOT NULL,
  `OwnerGroup` int(11) NOT NULL,
  `OwnerProperty` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `NeedKey` tinyint(1) NOT NULL,
  `MaxTotalVehicle` int(11) NOT NULL,
  `MaxBigVehicle` int(11) NOT NULL,
  `MaxMediumVehicle` int(11) NOT NULL,
  `MaxSmallVehicle` int(11) NOT NULL,
  `ParkOutSpotsString` longtext,
  `ParkInSpotsString` longtext,
  `Enabled` tinyint(1) NOT NULL,
  `PedPosition` longtext,
  `PedRotation` float NOT NULL,
  `Name` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `ItemInformations`
--

CREATE TABLE `ItemInformations` (
  `Id` int(11) NOT NULL,
  `Type` int(11) NOT NULL,
  `Name` longtext,
  `NamePlural` longtext,
  `Description` longtext,
  `Weight` int(11) NOT NULL,
  `BuyPrice` double NOT NULL,
  `SellPrice` double NOT NULL,
  `UsageTrigger` longtext,
  `DisposeTrigger` longtext,
  `Usable` tinyint(1) NOT NULL,
  `Disposable` tinyint(1) NOT NULL,
  `Buyable` tinyint(1) NOT NULL,
  `Sellable` tinyint(1) NOT NULL,
  `Giftable` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `OwnedVehicles`
--

CREATE TABLE `OwnedVehicles` (
  `Id` int(11) NOT NULL,
  `OwnerUser` int(11) NOT NULL,
  `OwnerGroup` int(11) NOT NULL,
  `ModelName` longtext,
  `ModelHash` int(11) NOT NULL,
  `ColorPrimary` int(11) NOT NULL,
  `ColorSecondary` int(11) NOT NULL,
  `NumberPlate` longtext,
  `Health` float NOT NULL,
  `Fuel` double NOT NULL,
  `IsDeath` tinyint(1) NOT NULL,
  `DirtLevel` float NOT NULL,
  `DamageString` longtext,
  `TuningString` longtext,
  `Position` longtext,
  `Rotation` longtext,
  `LastGarageId` int(11) NOT NULL,
  `CanParkOutEverywhere` tinyint(1) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `LastUsage` datetime NOT NULL,
  `InGarage` tinyint(1) NOT NULL,
  `EngineHealth` float NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `__MigrationHistory`
--

CREATE TABLE `__MigrationHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ContextKey` varchar(300) NOT NULL,
  `Model` longblob NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `Accounts`
--
ALTER TABLE `Accounts`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `BankAccounts`
--
ALTER TABLE `BankAccounts`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `Characters`
--
ALTER TABLE `Characters`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `Garages`
--
ALTER TABLE `Garages`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `ItemInformations`
--
ALTER TABLE `ItemInformations`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `OwnedVehicles`
--
ALTER TABLE `OwnedVehicles`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `__MigrationHistory`
--
ALTER TABLE `__MigrationHistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `Accounts`
--
ALTER TABLE `Accounts`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT für Tabelle `BankAccounts`
--
ALTER TABLE `BankAccounts`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT für Tabelle `Characters`
--
ALTER TABLE `Characters`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT für Tabelle `Garages`
--
ALTER TABLE `Garages`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT für Tabelle `ItemInformations`
--
ALTER TABLE `ItemInformations`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `OwnedVehicles`
--
ALTER TABLE `OwnedVehicles`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
