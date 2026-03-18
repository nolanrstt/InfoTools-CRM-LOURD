using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SlamDunkApp
{
    public partial class FacturesView : UserControl
    {
        private Bdd bdd = new Bdd();
        private Facture factureEnCours;
        private ObservableCollection<LigneFacture> panier;

        public FacturesView()
        {
            InitializeComponent();
            InitialiserNouvelleFacture();
            ChargerDonnees();
        }

        // =========================================================
        // 1. CHARGEMENT DES DONNÉES
        // =========================================================
        private void ChargerDonnees()
        {
            CmbClients.ItemsSource = bdd.GetClients();
            DtgProduitsDispo.ItemsSource = bdd.GetProduits();
        }

        private void InitialiserNouvelleFacture()
        {
            factureEnCours = new Facture();
            panier = new ObservableCollection<LigneFacture>();
            DtgPanier.ItemsSource = panier;
            TxtTotal.Text = "0,00 €";
        }

        // 🟢 NOUVEAU : QUAND ON SÉLECTIONNE UN CLIENT, ON AFFICHE SON HISTORIQUE
        private void CmbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbClients.SelectedItem is Client clientSelectionne)
            {
                // Appelle la nouvelle méthode de la BDD et remplit le tableau de droite
                DtgHistorique.ItemsSource = bdd.GetFacturesClient(clientSelectionne.IdClient);
            }
            else
            {
                // Si on désélectionne, on vide le tableau
                DtgHistorique.ItemsSource = null;
            }
        }

        // =========================================================
        // 2. GESTION DU PANIER
        // =========================================================
        private void BtnAjouterPanier_Click(object sender, RoutedEventArgs e)
        {
            if (DtgProduitsDispo.SelectedItem is Produit produitSelectionne)
            {
                if (int.TryParse(TxtQuantite.Text, out int qte) && qte > 0)
                {
                    if (qte > produitSelectionne.Stock)
                    {
                        MessageBox.Show($"Stock insuffisant ! Il ne reste que {produitSelectionne.Stock} unité(s) de {produitSelectionne.Nom}.",
                            "Erreur de stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var ligneExistante = panier.FirstOrDefault(l => l.IdProduit == produitSelectionne.Id);

                    if (ligneExistante != null)
                    {
                        if (ligneExistante.Quantite + qte > produitSelectionne.Stock)
                        {
                            MessageBox.Show("La quantité totale dans le panier dépasse le stock disponible !",
                                "Erreur de stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        ligneExistante.Quantite += qte;
                    }
                    else
                    {
                        panier.Add(new LigneFacture
                        {
                            IdProduit = produitSelectionne.Id,
                            NomProduit = produitSelectionne.Nom,
                            PrixUnitaire = produitSelectionne.Prix,
                            Quantite = qte
                        });
                    }

                    DtgPanier.Items.Refresh();
                    MettreAJourTotal();
                }
                else
                {
                    MessageBox.Show("Veuillez saisir une quantité valide (nombre entier).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un produit dans le catalogue à gauche.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MettreAJourTotal()
        {
            factureEnCours.Lignes = panier.ToList();
            factureEnCours.CalculerTotal();
            TxtTotal.Text = factureEnCours.TotalTTC.ToString("C");
        }

        private void BtnViderPanier_Click(object sender, RoutedEventArgs e)
        {
            if (panier.Count > 0)
            {
                var result = MessageBox.Show("Voulez-vous vraiment vider la facture en cours ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    panier.Clear();
                    MettreAJourTotal();
                }
            }
        }

        // =========================================================
        // 3. VALIDATION DE LA FACTURE EN BDD
        // =========================================================
        private void BtnValiderFacture_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClients.SelectedItem is not Client clientSelectionne)
            {
                MessageBox.Show("Veuillez sélectionner un client pour cette facture.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (panier.Count == 0)
            {
                MessageBox.Show("Le panier est vide. Ajoutez des produits avant de valider.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            factureEnCours.IdClient = clientSelectionne.IdClient;
            factureEnCours.DateFacture = DateTime.Now;
            factureEnCours.Lignes = panier.ToList();
            factureEnCours.CalculerTotal();

            try
            {
                bdd.AjouterFacture(factureEnCours);

                MessageBox.Show($"✅ Facture validée avec succès !\n\nClient : {clientSelectionne.NomComplet}\nTotal : {factureEnCours.TotalTTC.ToString("C")}",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                InitialiserNouvelleFacture();
                ChargerDonnees();

                // 🟢 NOUVEAU : On met à jour l'historique en direct pour voir la facture qu'on vient de créer !
                DtgHistorique.ItemsSource = bdd.GetFacturesClient(clientSelectionne.IdClient);

                TxtQuantite.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors de la validation : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =========================================================
        // 4. POP-UP DÉTAIL DE LA FACTURE (DOUBLE-CLIC)
        // =========================================================
        private void DtgHistorique_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Vérifier qu'on a bien cliqué sur une facture
            if (DtgHistorique.SelectedItem is Facture factureSelectionnee)
            {
                // 1. Récupérer les produits de cette facture via la BDD
                var lignes = bdd.GetLignesFacture(factureSelectionnee.IdFacture);

                // 2. Préparer le texte de la Pop-up
                string message = $"Détail de la facture n°{factureSelectionnee.IdFacture} du {factureSelectionnee.DateFacture:dd/MM/yyyy}\n";
                message += "---------------------------------------------------\n\n";

                // 3. Ajouter chaque produit au texte
                foreach (var ligne in lignes)
                {
                    message += $"- {ligne.Quantite}x {ligne.NomProduit} (à {ligne.PrixUnitaire:C} l'unité)\n";
                }

                message += "\n---------------------------------------------------\n";
                message += $"TOTAL TTC : {factureSelectionnee.TotalTTC:C}";

                // 4. Afficher la belle pop-up !
                MessageBox.Show(message, "Détails de la commande", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}