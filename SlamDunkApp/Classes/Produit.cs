using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlamDunkApp
{
    public class Produit
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Stock { get; set; }
    }
}

