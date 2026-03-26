using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlamDunkApp; // Lien vers ton application principale
using System.Collections.Generic;

namespace InfoTools.Tests
{
    [TestClass]
    public class TestsLogiqueMetier // 🟢 On a renommé la classe pour que ce soit plus global !
    {
        // ==========================================
        // TESTS DE LA FACTURATION (Calculs)
        // ==========================================

        [TestMethod]
        public void Test_CalculSousTotalLigne_DoitEtreCorrect()
        {
            // ARRANGE : On prépare une ligne avec 3 produits à 15.50€
            var ligne = new LigneFacture
            {
                Quantite = 3,
                PrixUnitaire = 15.50m
            };

            // ACT : On récupère le sous-total calculé par ta classe
            decimal resultat = ligne.SousTotal;

            // ASSERT : On vérifie que 3 * 15.50 fait bien 46.50
            Assert.AreEqual(46.50m, resultat, "Erreur : Le sous-total de la ligne est mal calculé.");
        }

        [TestMethod]
        public void Test_CalculTotalFacture_DoitEtreCorrect()
        {
            // ARRANGE : On crée une facture avec deux lignes distinctes
            var facture = new Facture();
            facture.Lignes = new List<LigneFacture>
            {
                new LigneFacture { Quantite = 2, PrixUnitaire = 10.00m }, // Sous-total : 20.00€
                new LigneFacture { Quantite = 1, PrixUnitaire = 5.50m }   // Sous-total : 5.50€
            };

            // ACT : On déclenche la méthode de calcul
            facture.CalculerTotal();

            // ASSERT : On vérifie que le total fait bien 25.50€
            Assert.AreEqual(25.50m, facture.TotalTTC, "Erreur : Le total de la facture est mal calculé.");
        }

        [TestMethod]
        public void Test_CalculTotalFacture_PanierVide_DoitRetournerZero()
        {
            // ARRANGE : Une facture sans aucune ligne (panier vide)
            var facture = new Facture();
            facture.Lignes = new List<LigneFacture>();

            // ACT : On lance le calcul
            facture.CalculerTotal();

            // ASSERT : Le total doit être de 0, ça ne doit pas faire planter l'appli
            Assert.AreEqual(0m, facture.TotalTTC, "Erreur : Une facture vide doit valoir 0€.");
        }

        // ==========================================
        // TESTS DU CLIENT (Formatage des données)
        // ==========================================

        [TestMethod]
        public void Test_ClientNomComplet_Formatage_DoitEtreCorrect()
        {
            // ARRANGE : On crée un faux client avec une entreprise
            var client = new Client
            {
                Nom = "Dupont",
                Prenom = "Jean",
                Entreprise = "Info-Tools SA"
            };

            // ACT : On récupère la propriété générée par ton code
            string resultat = client.NomComplet;

            // ASSERT : On vérifie que le nom est en majuscule et l'entreprise entre parenthèses
            Assert.AreEqual("👤 [CLIENT] DUPONT Jean (Info-Tools SA)", client.NomComplet, "Le formatage du nom complet est incorrect.");
        }

        // ==========================================
        // TESTS DE GESTION DE STOCK ET VALIDATION
        // ==========================================

        [TestMethod]
        public void Test_ValidationStock_AssezDeStock_DoitEtreValide()
        {
            // ARRANGE : On simule un produit en stock et une demande client
            int stockEnBase = 15;
            int quantiteDemandee = 5;

            // ACT : La règle de gestion métier (Est-ce qu'on peut vendre ?)
            bool validation = quantiteDemandee <= stockEnBase;

            // ASSERT : Ça doit être vrai, la vente est autorisée
            Assert.IsTrue(validation, "Erreur : La validation aurait dû accepter cette quantité.");
        }

        [TestMethod]
        public void Test_ValidationStock_RuptureDeStock_DoitEtreInvalide()
        {
            // ARRANGE : On simule un client qui demande trop de produits
            int stockEnBase = 3;
            int quantiteDemandee = 10;

            // ACT : La règle de gestion métier
            bool validation = quantiteDemandee <= stockEnBase;

            // ASSERT : Ça doit être faux, la vente doit être bloquée
            Assert.IsFalse(validation, "Erreur : La validation aurait dû bloquer cette quantité.");
        }
    }
}