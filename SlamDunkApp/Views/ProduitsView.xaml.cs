using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SlamDunkApp
{
    public partial class ProduitsView : UserControl
    {
        private Bdd bdd = new Bdd();
        private ObservableCollection<Produit> produits = new ObservableCollection<Produit>();

        private Produit? produitSelectionne;
        private Produit? produitEnCours;
        private bool isNewProduit = false;
        private bool isEditing = false;

        public ProduitsView()
        {
            InitializeComponent();
            ChargerProduits();
            VerrouillerChamps();
        }

        private void ChargerProduits()
        {
            produits = new ObservableCollection<Produit>(bdd.GetProduits());
            DataGridProduits.ItemsSource = produits;
        }

        // ========================= VERROUILLAGE DES CHAMPS =========================
        private void VerrouillerChamps()
        {
            txtNom.IsReadOnly = true;
            txtCategorie.IsReadOnly = true;
            txtPrix.IsReadOnly = true;
            txtStock.IsReadOnly = true;
        }

        private void DeverrouillerChamps()
        {
            txtNom.IsReadOnly = false;
            txtCategorie.IsReadOnly = false;
            txtPrix.IsReadOnly = false;
            txtStock.IsReadOnly = false;
        }

        private void ViderChamps()
        {
            txtNom.Text = "";
            txtCategorie.Text = "";
            txtPrix.Text = "";
            txtStock.Text = "";
        }

        // ========================= AJOUT =========================
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            isNewProduit = true;
            isEditing = false;

            produitEnCours = new Produit();
            GridDetails.DataContext = produitEnCours;

            DataGridProduits.SelectedItem = null;

            ViderChamps();
            DeverrouillerChamps();

            MessageBox.Show("Vous pouvez maintenant remplir les champs.\nCliquez sur 'Sauvegarder' pour valider l'ajout.",
                "Mode Ajout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ========================= SAUVEGARDER =========================
        private void BtnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewProduit && !isEditing)
            {
                MessageBox.Show("Veuillez d'abord cliquer sur 'Ajouter' ou 'Modifier' avant de sauvegarder.",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (produitEnCours == null) return;

            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Le nom du produit est requis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategorie.Text))
            {
                MessageBox.Show("La catégorie du produit est requise.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 🟢 LECTURE SÉCURISÉE : On remplace la virgule par un point et on lit au format international
            string prixTexte = txtPrix.Text.Replace(",", ".");
            if (!decimal.TryParse(prixTexte, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal prix) || prix < 0)
            {
                MessageBox.Show("Le prix doit être un nombre valide et positif.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Le stock doit être un nombre entier valide et positif.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            produitEnCours.Nom = txtNom.Text;
            produitEnCours.Categorie = txtCategorie.Text;
            produitEnCours.Prix = prix;
            produitEnCours.Stock = stock;

            try
            {
                if (isNewProduit)
                {
                    bdd.AjouterProduit(produitEnCours);
                    ChargerProduits();

                    MessageBox.Show("✅ Produit ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    isNewProduit = false;
                    produitSelectionne = null;
                    produitEnCours = null;
                    GridDetails.DataContext = null;
                    ViderChamps();
                    VerrouillerChamps();
                }
                else if (isEditing && produitSelectionne != null)
                {
                    produitSelectionne.Nom = produitEnCours.Nom;
                    produitSelectionne.Categorie = produitEnCours.Categorie;
                    produitSelectionne.Prix = produitEnCours.Prix;
                    produitSelectionne.Stock = produitEnCours.Stock;

                    bdd.ModifierProduit(produitSelectionne);
                    DataGridProduits.Items.Refresh();

                    MessageBox.Show("✅ Produit modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    isEditing = false;
                    produitEnCours = null;
                    GridDetails.DataContext = produitSelectionne;
                    VerrouillerChamps();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========================= MODIFIER =========================
        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (produitSelectionne == null)
            {
                MessageBox.Show("⚠️ Veuillez d'abord sélectionner un produit dans la liste.",
                    "Aucun produit sélectionné", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isEditing = true;
            isNewProduit = false;

            produitEnCours = new Produit
            {
                Id = produitSelectionne.Id,
                Nom = produitSelectionne.Nom,
                Categorie = produitSelectionne.Categorie,
                Prix = produitSelectionne.Prix,
                Stock = produitSelectionne.Stock
            };

            GridDetails.DataContext = produitEnCours;

            txtNom.Text = produitEnCours.Nom;
            txtCategorie.Text = produitEnCours.Categorie;

            // 🟢 AFFICHAGE PROPRE : Le "0.##" retire les zéros inutiles (ex: 12.00 devient 12)
            txtPrix.Text = produitEnCours.Prix.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

            txtStock.Text = produitEnCours.Stock.ToString();

            DeverrouillerChamps();

            MessageBox.Show("✏️ Vous pouvez maintenant modifier les champs.\nCliquez sur 'Sauvegarder' pour valider les modifications.",
                "Mode Modification", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ========================= SELECTION DANS LA LISTE =========================
        private void DataGridProduits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isNewProduit || isEditing)
            {
                MessageBox.Show("⚠️ Veuillez d'abord sauvegarder ou annuler vos modifications en cours.",
                    "Action en cours", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DataGridProduits.SelectedItem is Produit p)
            {
                produitSelectionne = p;
                isNewProduit = false;
                isEditing = false;
                GridDetails.DataContext = produitSelectionne;

                txtNom.Text = p.Nom;
                txtCategorie.Text = p.Categorie;

                // 🟢 AFFICHAGE PROPRE
                txtPrix.Text = p.Prix.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

                txtStock.Text = p.Stock.ToString();

                VerrouillerChamps();
            }
        }

        // ========================= SUPPRIMER =========================
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (produitSelectionne == null)
            {
                MessageBox.Show("⚠️ Veuillez sélectionner un produit à supprimer.",
                    "Aucun produit sélectionné", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"❓ Êtes-vous sûr de vouloir supprimer définitivement :\n\n{produitSelectionne.Nom} ?",
                "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bdd.SupprimerProduit(produitSelectionne.Id);
                    produits.Remove(produitSelectionne);

                    produitSelectionne = null;
                    produitEnCours = null;
                    isNewProduit = false;
                    isEditing = false;

                    GridDetails.DataContext = null;
                    ViderChamps();
                    VerrouillerChamps();

                    MessageBox.Show("✅ Produit supprimé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ========================= ANNULER =========================
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (isNewProduit || isEditing)
            {
                var result = MessageBox.Show("❓ Voulez-vous vraiment annuler les modifications en cours ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    isNewProduit = false;
                    isEditing = false;
                    produitEnCours = null;

                    if (produitSelectionne != null)
                    {
                        GridDetails.DataContext = produitSelectionne;
                        txtNom.Text = produitSelectionne.Nom;
                        txtCategorie.Text = produitSelectionne.Categorie;

                        // 🟢 AFFICHAGE PROPRE
                        txtPrix.Text = produitSelectionne.Prix.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

                        txtStock.Text = produitSelectionne.Stock.ToString();
                    }
                    else
                    {
                        GridDetails.DataContext = null;
                        ViderChamps();
                    }

                    VerrouillerChamps();
                    MessageBox.Show("❌ Modifications annulées.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // ========================= FILTRES ET RECHERCHE =========================
        private void AppliquerFiltres()
        {
            try
            {
                if (TxtRecherche == null || CmbCategorie == null || DataGridProduits == null) return;

                var produitsFiltres = bdd.GetProduits().AsEnumerable();

                string recherche = TxtRecherche.Text.Trim();
                if (!string.IsNullOrEmpty(recherche))
                {
                    produitsFiltres = produitsFiltres.Where(p =>
                        (p.Nom != null && p.Nom.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (p.Categorie != null && p.Categorie.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0)
                    );
                }

                if (CmbCategorie.SelectedIndex > 0)
                {
                    var selectedItem = (ComboBoxItem)CmbCategorie.SelectedItem;
                    string cat = selectedItem.Content.ToString();
                    produitsFiltres = produitsFiltres.Where(p => p.Categorie != null && p.Categorie.Equals(cat, StringComparison.OrdinalIgnoreCase));
                }

                produits = new ObservableCollection<Produit>(produitsFiltres.ToList());
                DataGridProduits.ItemsSource = produits;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e) => AppliquerFiltres();

        private void CmbCategorie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded) AppliquerFiltres();
        }

        private void BtnReinitialiser_Click(object sender, RoutedEventArgs e)
        {
            TxtRecherche.Text = "";
            CmbCategorie.SelectedIndex = 0;
            AppliquerFiltres();
            ViderChamps();
            VerrouillerChamps();
        }
    }
}