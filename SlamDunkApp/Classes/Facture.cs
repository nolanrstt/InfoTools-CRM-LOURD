using System;
using System.Collections.Generic;
using System.Linq;

namespace SlamDunkApp
{
    public class Facture
    {
        public int IdFacture { get; set; }
        public int IdClient { get; set; }

        // Pour l'affichage
        public string NomClient { get; set; } = string.Empty;

        public DateTime DateFacture { get; set; } = DateTime.Now;
        public decimal TotalTTC { get; set; }

        // Une facture contient une liste de plusieurs lignes (le panier)
        public List<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();

        // 🟢 METHODE MÉTIER (Ce qu'on va tester pour tes 60% de couverture !)
        public void CalculerTotal()
        {
            // Additionne tous les "SousTotal" de chaque ligne
            TotalTTC = Lignes.Sum(ligne => ligne.SousTotal);
        }
    }
}