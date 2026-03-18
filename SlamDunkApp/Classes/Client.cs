using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamDunkApp
{
    public class Client
    {
        public int IdClient { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string? Entreprise { get; set; } // Peut être null dans la BDD
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;

        // Propriété magique pour l'affichage dans la ComboBox
        public string NomComplet
        {
            get
            {
                // Affiche "DUPONT Jean (Dupont SA)" ou juste "DUPONT Jean"
                string ste = string.IsNullOrEmpty(Entreprise) ? "" : $" ({Entreprise})";
                return $"{Nom.ToUpper()} {Prenom}{ste}";
            }
        }
    }
}