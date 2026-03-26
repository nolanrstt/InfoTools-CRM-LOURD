-- phpMyAdmin SQL Dump
-- version 4.5.4.1
-- http://www.phpmyadmin.net
--
-- Client :  localhost
-- Généré le :  Mer 18 Mars 2026 à 15:16
-- Version du serveur :  5.7.11
-- Version de PHP :  7.0.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données :  `infotools`
--

-- --------------------------------------------------------

--
-- Structure de la table `audit_logs`
--

CREATE TABLE `audit_logs` (
  `id` int(11) NOT NULL,
  `action_date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `user_login` varchar(255) NOT NULL,
  `action_type` enum('INSERT','UPDATE','DELETE') NOT NULL,
  `table_name` varchar(100) NOT NULL,
  `record_id` varchar(255) NOT NULL,
  `old_value` longtext,
  `new_value` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Contenu de la table `audit_logs`
--

INSERT INTO `audit_logs` (`id`, `action_date`, `user_login`, `action_type`, `table_name`, `record_id`, `old_value`, `new_value`) VALUES
(33, '2026-03-18 16:02:59', 'mpereira', 'INSERT', 'Client', '34', 'null', '{"IdClient":34,"Nom":"TEST","Prenom":"TEST","Entreprise":"TEST","Email":"TEST","Telephone":"TEST","Adresse":"TEST","Statut":"Client","NomComplet":"\\uD83D\\uDC64 [CLIENT] TEST TEST (TEST)"}'),
(34, '2026-03-18 16:03:44', 'mpereira', 'UPDATE', 'Client', '34', '{"IdClient":34,"Nom":"TEST","Prenom":"TEST","Entreprise":"TEST","Email":"TEST","Telephone":"TEST","Adresse":"TEST","Statut":"Client","NomComplet":"\\uD83D\\uDC64 [CLIENT] TEST TEST (TEST)"}', '{"IdClient":34,"Nom":"TEST","Prenom":"nabil","Entreprise":"TEST","Email":"TEST","Telephone":"TEST","Adresse":"TEST","Statut":"Prospect","NomComplet":"\\uD83C\\uDFAF [PROSPECT] TEST nabil (TEST)"}'),
(35, '2026-03-18 16:04:29', 'mpereira', 'DELETE', 'Client', '34', '{"IdClient":34,"Nom":"TEST","Prenom":"nabil","Entreprise":"TEST","Email":"TEST","Telephone":"TEST","Adresse":"TEST","Statut":"Prospect","NomComplet":"\\uD83C\\uDFAF [PROSPECT] TEST nabil (TEST)"}', 'null'),
(36, '2026-03-18 16:11:59', 'mpereira', 'UPDATE', 'Produit', '12', '{"Id":12,"Nom":"Windows Server 2022 Standard","Categorie":"Logiciel","Prix":950.00,"Stock":50}', '{"Id":12,"Nom":"Windows Server 2022 Standard","Categorie":"Logiciel","Prix":95,"Stock":50}'),
(37, '2026-03-18 16:12:11', 'mpereira', 'INSERT', 'Produit', '15', 'null', '{"Id":15,"Nom":"TEST","Categorie":"TEST","Prix":13,"Stock":13}'),
(38, '2026-03-18 16:12:36', 'mpereira', 'DELETE', 'Produit', '15', '{"Id":15,"Nom":"TEST","Categorie":"TEST","Prix":13.00,"Stock":13}', 'null');

-- --------------------------------------------------------

--
-- Structure de la table `client`
--

CREATE TABLE `client` (
  `IdClient` int(11) NOT NULL,
  `Nom` varchar(100) NOT NULL,
  `Prenom` varchar(100) NOT NULL,
  `Entreprise` varchar(150) DEFAULT NULL,
  `Email` varchar(150) NOT NULL,
  `Telephone` varchar(30) NOT NULL,
  `Adresse` varchar(255) NOT NULL,
  `DateCreation` datetime DEFAULT CURRENT_TIMESTAMP,
  `Statut` varchar(20) DEFAULT 'Client'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Contenu de la table `client`
--

INSERT INTO `client` (`IdClient`, `Nom`, `Prenom`, `Entreprise`, `Email`, `Telephone`, `Adresse`, `DateCreation`, `Statut`) VALUES
(1, 'Duponts', 'Antoine', 'TechCorp', 'a.dupont@tech.fr', '0610000001', 'Paris', '2025-06-20 00:00:00', 'Prospect'),
(2, 'Lefevre', 'Sophie', 'InnovDesign', 's.lefevre@design.com', '0610000002', 'Lyon', '2025-01-03 00:00:00', 'Client'),
(3, 'Martin', 'Lucas', 'TanaLand', 'lucas.m@mail.fr', '0610000003', 'Marseille', '2025-04-24 00:00:00', 'Client'),
(6, 'Thomas', 'Émilie', NULL, 'e.thomas@free.fr', '0610000006', 'Lille', '2025-03-12 00:00:00', 'Client'),
(7, 'Robert', 'Hugo', 'TransLog', 'h.robert@log.eu', '0610000007', 'Toulouse', '2025-01-16 00:00:00', 'Client'),
(8, 'Richard', 'Chloé', 'FinancePlus', 'c.richard@finance.org', '0610000008', 'Nice', '2025-01-16 00:00:00', 'Client'),
(9, 'Petit', 'Jules', NULL, 'jules.p@web.fr', '0610000009', 'Strasbourg', '2025-02-01 00:00:00', 'Client'),
(10, 'Durand', 'Manon', 'MediaStart', 'm.durand@media.co', '0610000010', 'Rennes', '2025-04-22 00:00:00', 'Client'),
(11, 'Leroy', 'Gabriel', 'GreenEnergy', 'g.leroy@energy.net', '0610000011', 'Grenoble', '2025-01-15 00:00:00', 'Client'),
(12, 'Moreau', 'Léa', NULL, 'lea.m@yahoo.com', '0610000012', 'Montpellier', '2025-02-05 00:00:00', 'Client'),
(13, 'Simon', 'Arthur', 'CyberSec', 'a.simon@cyber.fr', '0610000013', 'Clermont-Ferrand', '2025-05-16 00:00:00', 'Client'),
(14, 'Laurent', 'Eva', 'ImmoPro', 'e.laurent@immo.fr', '0610000014', 'Dijon', '2025-05-02 00:00:00', 'Client'),
(15, 'Michel', 'Louis', '', 'louis.m@outlook.fr', '0610 50 50 60 10', 'Rouen', '2025-02-22 00:00:00', 'Client'),
(16, 'Garcia', 'Nina', 'FashionTech', 'n.garcia@ft.com', '0610000016', 'Brest', '2025-02-14 00:00:00', 'Client'),
(17, 'David', 'Maxime', 'IndusFab', 'm.david@indus.net', '0610000017', 'Le Havre', '2025-03-08 00:00:00', 'Client'),
(18, 'Bertrand', 'Zoé', NULL, 'zoe.b@mail.com', '0610000018', 'Reims', '2025-02-20 00:00:00', 'Client'),
(19, 'Roux', 'Clément', 'EcoBuild', 'c.roux@eco.fr', '0610000019', 'Tours', '2025-02-23 00:00:00', 'Client'),
(20, 'Vincent', 'Jeanne', 'AgriSol', 'j.vincent@agri.eu', '0610000020', 'Limoges', '2025-04-27 00:00:00', 'Client'),
(21, 'Fournier', 'Paul', 'OptiMaint', 'p.fournier@maint.fr', '0610000021', 'Aix-en-Provence', '2025-05-02 00:00:00', 'Prospect'),
(22, 'Meyer', 'Alice', NULL, 'alice.m@gmail.com', '0610000022', 'Annecy', '2025-04-16 00:00:00', 'Client'),
(23, 'Faure', 'Maxence', 'SpeedDelivery', 'm.faure@speed.com', '0610000023', 'Angers', '2025-01-16 00:00:00', 'Prospect'),
(24, 'Lambert', 'Rose', 'SoftDev', 'r.lambert@soft.net', '0610000024', 'Perpignan', '2025-03-04 00:00:00', 'Client'),
(25, 'Rousseau', 'Victor', NULL, 'victor.r@web.com', '0610000025', 'Orléans', '2025-04-24 00:00:00', 'Client'),
(26, 'Blanchard', 'Oceane', 'Architech', 'i.blanchard@arch.fr', '0610000026', 'Metz', '2025-03-24 00:00:00', 'Prospect'),
(27, 'Giraud', 'Samy', 'HydroClean', 's.giraud@hydro.com', '0610000027', 'Besançon', '2025-03-13 00:00:00', 'Client'),
(28, 'Guillot', 'Sarah', NULL, 'sarah.g@mail.fr', '0610000028', 'Caen', '2025-04-19 00:00:00', 'Client'),
(29, 'Da Silva', 'Tiago', 'GlobalExport', 't.silva@global.net', '0610000029', 'Valence', '2025-06-11 00:00:00', 'Client'),
(30, 'Brunet', 'Margaux', 'ComNet', 'm.brunet@com.fr', '0610000030', 'La Rochelle', '2025-06-18 00:00:00', 'Client'),
(31, 'Roisot', 'Nolan', 'Tanaland', 'nolanroisot71@gmail.com', '0762178972', 'Paris', '2025-06-03 13:36:20', 'Client'),
(32, 'Rimane', 'Rayan', 'Rimane Auto ', 'rrimane71@gmail.com', '06 26 25 24 25', 'Bat 4 La Chaney Macon', '2026-02-25 13:48:11', 'Client');

-- --------------------------------------------------------

--
-- Structure de la table `produit`
--

CREATE TABLE `produit` (
  `Id` int(11) NOT NULL,
  `Nom` varchar(150) NOT NULL,
  `Categorie` varchar(100) NOT NULL,
  `Prix` decimal(10,2) NOT NULL,
  `Stock` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Contenu de la table `produit`
--

INSERT INTO `produit` (`Id`, `Nom`, `Categorie`, `Prix`, `Stock`) VALUES
(1, 'Serveur Dell PowerEdge R740', 'Infrastructure', '12.00', 8),
(2, 'Baie de Stockage SAN HP 10To', 'Infrastructure', '42.00', 3),
(3, 'Onduleur APC Smart-UPS 1500VA', 'Infrastructure', '480.00', 25),
(4, 'Baie de Brassage 42U', 'Infrastructure', '60.00', 10),
(5, 'Switch Cisco Catalyst 9300', 'Réseau', '120.00', 15),
(6, 'Routeur MikroTik CCR1036', 'Réseau', '95.00', 9),
(7, 'Point d\'Accès Ubiquiti UniFi U6-Pro', 'Réseau', '180.00', 45),
(8, 'Firewall Fortinet FortiGate 60F', 'Réseau', '85.00', 12),
(9, 'Licence Microsoft 365 Business (Annuelle)', 'Logiciel', '150.00', 500),
(10, 'Antivirus ESET Endpoint Security', 'Logiciel', '85.00', 999),
(11, 'ERP Microsoft Dynamics 365', 'Logiciel', '45.00', 5),
(12, 'Windows Server 2022 Standard', 'Logiciel', '95.00', 50),
(13, 'VMware vSphere Standard', 'Logiciel', '120.00', 20),
(14, 'Adobe Creative Cloud (Equipe)', 'Logiciel', '89.00', 40);

-- --------------------------------------------------------

--
-- Structure de la table `rendezvous`
--

CREATE TABLE `rendezvous` (
  `IdRdv` int(11) NOT NULL,
  `Titre` varchar(150) NOT NULL,
  `Description` text NOT NULL,
  `Date_Heure` datetime NOT NULL,
  `Lieu` varchar(200) NOT NULL,
  `IdProspect` int(11) DEFAULT NULL,
  `IdClient` int(11) DEFAULT NULL,
  `IdUtilisateur` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Contenu de la table `rendezvous`
--

INSERT INTO `rendezvous` (`IdRdv`, `Titre`, `Description`, `Date_Heure`, `Lieu`, `IdProspect`, `IdClient`, `IdUtilisateur`) VALUES
(1, 'Audit IndusFab', 'Contrôle', '2025-02-03 08:00:00', 'Client', NULL, 17, 1),
(2, 'Démo HydroClean', 'Logiciel', '2025-02-04 10:00:00', 'Bureau', NULL, 27, 2),
(3, 'Vente FashionTech', 'E-commerce', '2025-02-05 14:00:00', 'Bureau', NULL, 16, 3),
(4, 'Point Architech', 'Plan', '2025-02-13 09:00:00', 'Teams', NULL, 26, 4),
(5, 'RDV Client: BERTRAND Zoé', 'Sécurité', '2025-02-14 11:00:00', 'Client', NULL, 18, 1),
(6, 'Suivi Client Thomas', 'Licences', '2025-02-20 15:00:00', 'Teams', NULL, 6, 2),
(7, 'Installation MediaStart', 'Matériel', '2025-02-25 16:00:00', 'Client', NULL, 10, 3),
(8, 'Visite Client Dupont', 'Besoin annuel', '2025-03-03 08:00:00', 'Client', NULL, 1, 4),
(9, 'Bilan mensuel ComNet', 'Statistiques', '2025-03-04 14:00:00', 'Bureau', NULL, 30, 1),
(10, 'Prospection Sarah Guillot', 'Contact', '2025-03-06 10:00:00', 'Café', NULL, 28, 2),
(11, 'Projet InnovDesign', 'Phase 2', '2025-03-12 09:00:00', 'Visio', NULL, 2, 3),
(12, 'Vente Martin Lucas', 'Devis perso', '2025-03-18 11:00:00', 'Bureau', NULL, 3, 4),
(13, 'Audit Bertrand Zoé', 'Audit domicile', '2025-03-20 15:00:00', 'Client', NULL, 18, 1),
(14, 'RDV Paul Fournier', 'Suivi', '2025-03-26 13:00:00', 'Teams', NULL, 21, 2),
(15, 'RDV TransLog', 'Support', '2025-03-28 09:00:00', 'Distant', NULL, 7, 3),
(16, 'Contrat ImmoPro', 'Signature', '2025-04-01 10:00:00', 'Bureau', NULL, 14, 4),
(17, 'Formation FashionTech', 'Equipe', '2025-04-03 14:00:00', 'Client', NULL, 16, 1),
(18, 'Prospection SpeedDelivery', 'Logistique', '2025-04-08 16:00:00', 'Extérieur', NULL, 23, 2),
(19, 'RDV Rousseau Victor', 'Matériel', '2025-04-16 11:00:00', 'Bureau', NULL, 25, 3),
(20, 'Support Louis Michel', 'Intervention', '2025-04-18 15:00:00', 'Client', NULL, 15, 4),
(21, 'Vente Meyer Alice', 'PC Portable', '2025-04-22 13:00:00', 'Bureau', NULL, 22, 1),
(22, 'Démo SoftDev', 'Logiciel gestion', '2025-04-29 10:00:00', 'Visio', NULL, 24, 2),
(23, 'Audit AgriSol', 'Infrastructure', '2025-05-05 14:00:00', 'Client', NULL, 20, 3),
(24, 'RDV GlobalExport', 'Négociation', '2025-05-07 11:00:00', 'Bureau', NULL, 29, 4),
(25, 'Bilan Quadri ComNet', 'Objectifs Q2', '2025-05-13 09:00:00', 'Teams', NULL, 30, 1),
(26, 'Installation CyberSec', 'Firewall', '2025-05-15 15:00:00', 'Client', NULL, 13, 2),
(27, 'RDV Vincent Jeanne', 'Suivi', '2025-05-21 13:00:00', 'Client', NULL, 20, 3),
(28, 'Prospection Valence', 'Secteur', '2025-05-23 16:00:00', 'Extérieur', NULL, 29, 4),
(29, 'Formation TechCorp', 'Sécurité', '2025-05-28 12:00:00', 'Client', NULL, 1, 1),
(30, 'Maintenance TransLog', 'Routine', '2025-06-03 08:00:00', 'Distant', NULL, 7, 2),
(31, 'RDV Louis Michel', 'Upgrade', '2025-06-05 10:00:00', 'Bureau', NULL, 15, 3),
(32, 'Devis EcoBuild', 'Chantier BTP', '2025-06-11 14:00:00', 'Client', NULL, 19, 4),
(33, 'Vente GreenEnergy', 'Solaire', '2025-06-17 11:00:00', 'Visio', NULL, 11, 1),
(34, 'Déjeuner Dupont', 'Partenariat', '2025-06-25 12:00:00', 'Resto', NULL, 1, 2),
(35, 'RDV Client: BERTRAND Zoé', '', '2025-01-06 09:00:00', 'Bureau', NULL, 18, 2),
(36, 'RDV Client: DURAND Manon (MediaStart)', '', '2025-01-06 13:00:00', 'Bureau', NULL, 10, 4),
(37, 'RDV Client: David', '', '2025-01-09 08:00:00', 'Bureau', NULL, 17, 1),
(38, 'RDV Client: FOURNIER Paul (OptiMaint)', '', '2025-01-10 15:00:00', 'Bureau', NULL, 21, 1),
(39, 'RDV Client: DAVID Maxime (IndusFab)', '', '2025-01-07 11:00:00', 'Bureau', NULL, 17, 3),
(44, 'RDV Client: BERTRAND Zoé', '', '2025-01-07 15:00:00', 'Bureau', NULL, 18, 2),
(45, 'RDV Client: Blanchard', '', '2025-01-06 11:00:00', 'Bureau', NULL, 26, 2),
(46, 'RDV Client: David', '', '2025-01-09 08:00:00', 'Bureau', NULL, 17, 1);

-- --------------------------------------------------------

--
-- Structure de la table `utilisateur`
--

CREATE TABLE `utilisateur` (
  `Id` int(11) NOT NULL,
  `Nom` varchar(50) NOT NULL,
  `Prenom` varchar(50) NOT NULL,
  `Identifiant` varchar(50) NOT NULL,
  `MotDePasse` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Contenu de la table `utilisateur`
--

INSERT INTO `utilisateur` (`Id`, `Nom`, `Prenom`, `Identifiant`, `MotDePasse`) VALUES
(1, 'ROISOT', 'Nolan', 'nolan', 'root'),
(2, 'PEREIRA', 'Maxence', 'maxence', 'root'),
(3, 'RIMANE', 'Rayan', 'Rayan', 'root'),
(4, 'RIOUAL', 'Océane', 'oceane', 'root');

--
-- Index pour les tables exportées
--

--
-- Index pour la table `audit_logs`
--
ALTER TABLE `audit_logs`
  ADD PRIMARY KEY (`id`);

--
-- Index pour la table `client`
--
ALTER TABLE `client`
  ADD PRIMARY KEY (`IdClient`);

--
-- Index pour la table `produit`
--
ALTER TABLE `produit`
  ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `rendezvous`
--
ALTER TABLE `rendezvous`
  ADD PRIMARY KEY (`IdRdv`),
  ADD KEY `FK_Rdv_Prospect` (`IdProspect`),
  ADD KEY `FK_Rdv_Utilisateur` (`IdUtilisateur`);

--
-- Index pour la table `utilisateur`
--
ALTER TABLE `utilisateur`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT pour les tables exportées
--

--
-- AUTO_INCREMENT pour la table `audit_logs`
--
ALTER TABLE `audit_logs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=39;
--
-- AUTO_INCREMENT pour la table `client`
--
ALTER TABLE `client`
  MODIFY `IdClient` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=35;
--
-- AUTO_INCREMENT pour la table `produit`
--
ALTER TABLE `produit`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;
--
-- AUTO_INCREMENT pour la table `rendezvous`
--
ALTER TABLE `rendezvous`
  MODIFY `IdRdv` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=47;
--
-- AUTO_INCREMENT pour la table `utilisateur`
--
ALTER TABLE `utilisateur`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
--
-- Contraintes pour les tables exportées
--

--
-- Contraintes pour la table `rendezvous`
--
ALTER TABLE `rendezvous`
  ADD CONSTRAINT `FK_Rdv_Prospect` FOREIGN KEY (`IdProspect`) REFERENCES `prospect` (`IdProspect`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `FK_Rdv_Utilisateur` FOREIGN KEY (`IdUtilisateur`) REFERENCES `utilisateur` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
