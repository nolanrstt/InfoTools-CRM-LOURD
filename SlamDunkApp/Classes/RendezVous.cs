using System;

namespace SlamDunkApp
{
    public class RendezVous
    {
        public int Id { get; set; } // L'ID que le code C# utilise
        public string Titre { get; set; } = string.Empty;
        public string NomClient { get; set; } = string.Empty;
        public DateTime DateHeure { get; set; }
        public string Lieu { get; set; } = string.Empty;
        public string NomUtilisateur { get; set; }
        public int IdClient { get; set; }
        public int IdUtilisateur { get; set; }
    }
}