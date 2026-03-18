using System;

namespace SlamDunkApp
{
    public class LigneFacture
    {
        public int IdLigne { get; set; }
        public int IdFacture { get; set; }
        public int IdProduit { get; set; }

        // Pour l'affichage dans l'interface
        public string NomProduit { get; set; } = string.Empty;

        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }

        // 🟢 LE POINT FORT POUR TES TESTS UNITAIRES :
        // Le C# calcule automatiquement le sous-total de cette ligne
        public decimal SousTotal => Quantite * PrixUnitaire;
    }
}