using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamDunkApp
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Identifiant { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NomComplet => $"{Prenom} {Nom}";
    }
}
