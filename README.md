# 📊 InfoTools CRM (SlamDunkApp)

## 📝 Présentation du projet
InfoTools est une application de bureau (Client Lourd) développée en **C# (WPF)** sous le framework **.NET 8**. 
Il s'agit d'un logiciel de gestion de la relation client (CRM) conçu pour une PME. Le projet s'inscrit dans une démarche de maintenance évolutive, avec un fort accent sur l'optimisation des données et la cybersécurité.

## ✨ Fonctionnalités Principales
* **Tableau de Bord Interactif :** Visualisation des statistiques de l'entreprise en temps réel via des graphiques (LiveCharts).
* **Gestion Commerciale (CRUD) :** Administration unifiée des clients, des prospects et du catalogue de produits IT.
* **Agenda Intelligent :** Système de prise de rendez-vous avec prévention automatique des chevauchements d'horaires.
* **Sécurité & Traçabilité :** Enregistrement complet des actions utilisateurs (historique et audit) avec sérialisation des données au format JSON.
* **Reporting :** Génération automatique de bilans et de statistiques au format PDF.

## 🛠️ Technologies Utilisées
* **Langage :** C#
* **Interface Graphique :** WPF (Windows Presentation Foundation)
* **Framework :** .NET 8.0
* **Base de données :** MySQL (Architecture Client-Serveur)
* **Tests Unitaires :** MSTest
* **Bibliothèques tierces :** * `LiveCharts` (Graphiques dynamiques)
  * `iTextSharp` (Génération d'exports PDF)
  * `System.Text.Json` (Sérialisation pour l'audit)

## ⚙️ Installation et Prérequis

1. **Prérequis matériels et logiciels :**
   * [Visual Studio 2022](https://visualstudio.microsoft.com/fr/)
   * SDK .NET 8.0
   * Un serveur MySQL (local ou distant)

2. **Cloner le dépôt :**
   ```bash
   git clone [https://github.com/TonNomDUtilisateur/SlamDunkApp.git](https://github.com/TonNomDUtilisateur/SlamDunkApp.git)
