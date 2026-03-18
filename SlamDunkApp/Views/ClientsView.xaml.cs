using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SlamDunkApp
{
    public partial class ClientsView : UserControl
    {
        private Bdd bdd = new Bdd();
        private ObservableCollection<Client> clients = new ObservableCollection<Client>();

        private Client? clientSelectionne;
        private Client? clientEnCours;
        private bool isNewClient = false;
        private bool isEditing = false;

        public ClientsView()
        {
            InitializeComponent();
            ChargerClients();
            VerrouillerChamps(); // Verrouiller au démarrage
        }

        private void ChargerClients()
        {
            var listeSql = bdd.GetClients();
            clients = new ObservableCollection<Client>(listeSql);
            DtgClients.ItemsSource = clients;
        }

        // ========================= VERROUILLAGE DES CHAMPS =========================
        private void VerrouillerChamps()
        {
            txtNom.IsReadOnly = true;
            txtPrenom.IsReadOnly = true;
            txtEntreprise.IsReadOnly = true;
            txtEmail.IsReadOnly = true;
            txtTelephone.IsReadOnly = true;
            txtAdresse.IsReadOnly = true;
        }

        private void DeverrouillerChamps()
        {
            txtNom.IsReadOnly = false;
            txtPrenom.IsReadOnly = false;
            txtEntreprise.IsReadOnly = false;
            txtEmail.IsReadOnly = false;
            txtTelephone.IsReadOnly = false;
            txtAdresse.IsReadOnly = false;
        }

        private void ViderChamps()
        {
            txtNom.Text = "";
            txtPrenom.Text = "";
            txtEntreprise.Text = "";
            txtEmail.Text = "";
            txtTelephone.Text = "";
            txtAdresse.Text = "";
        }

        // ========================= AJOUT =========================
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            isNewClient = true;
            isEditing = false;

            clientEnCours = new Client();
            GridDetails.DataContext = clientEnCours;

            DtgClients.SelectedItem = null;

            ViderChamps();
            DeverrouillerChamps();

            MessageBox.Show("Vous pouvez maintenant remplir les champs.\nCliquez sur 'Sauvegarder' pour valider l'ajout.",
                "Mode Ajout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ========================= SAUVEGARDER =========================
        private void BtnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewClient && !isEditing)
            {
                MessageBox.Show("Veuillez d'abord cliquer sur 'Ajouter' ou 'Modifier' avant de sauvegarder.",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (clientEnCours == null) return;

            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Le nom du client est requis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("L'email du client est requis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            clientEnCours.Nom = txtNom.Text;
            clientEnCours.Prenom = txtPrenom.Text;
            clientEnCours.Entreprise = txtEntreprise.Text;
            clientEnCours.Email = txtEmail.Text;
            clientEnCours.Telephone = txtTelephone.Text;
            clientEnCours.Adresse = txtAdresse.Text;

            try
            {
                if (isNewClient)
                {
                    bdd.AjouterClient(clientEnCours);
                    ChargerClients();
                    MessageBox.Show("✅ Client ajouté avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (isEditing && clientSelectionne != null)
                {
                    clientSelectionne.Nom = clientEnCours.Nom;
                    clientSelectionne.Prenom = clientEnCours.Prenom;
                    clientSelectionne.Entreprise = clientEnCours.Entreprise;
                    clientSelectionne.Email = clientEnCours.Email;
                    clientSelectionne.Telephone = clientEnCours.Telephone;
                    clientSelectionne.Adresse = clientEnCours.Adresse;

                    bdd.ModifierClient(clientSelectionne);
                    DtgClients.Items.Refresh();
                    MessageBox.Show("✅ Client modifié avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                isNewClient = false;
                isEditing = false;
                clientEnCours = null;
                ViderChamps();
                VerrouillerChamps();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========================= MODIFIER =========================
        // ========================= MODIFIER =========================
        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            // 1. Vérification : un client doit être sélectionné
            if (clientSelectionne == null)
            {
                MessageBox.Show("⚠️ Veuillez d'abord sélectionner un client dans la liste.",
                    "Aucun client sélectionné", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. On active les drapeaux d'état
            isEditing = true;
            isNewClient = false;

            // 3. On crée le clone pour travailler en sécurité (sans écraser l'original tout de suite)
            clientEnCours = new Client
            {
                IdClient = clientSelectionne.IdClient,
                Nom = clientSelectionne.Nom,
                Prenom = clientSelectionne.Prenom,
                Entreprise = clientSelectionne.Entreprise,
                Email = clientSelectionne.Email,
                Telephone = clientSelectionne.Telephone,
                Adresse = clientSelectionne.Adresse
            };

            // 4. On lie l'interface au clone et on déverrouille
            GridDetails.DataContext = clientEnCours;
            DeverrouillerChamps();

            // 🟢 LA VOILÀ : La petite popup de confirmation
            MessageBox.Show("✏️ Vous pouvez maintenant modifier les informations du client.\nCliquez sur 'Sauvegarder' pour valider les modifications.",
                "Mode Modification", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ========================= SELECTION DANS LA LISTE =========================
        private void DtgClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isNewClient || isEditing) return;

            if (DtgClients.SelectedItem is Client c)
            {
                clientSelectionne = c;
                GridDetails.DataContext = clientSelectionne;
                txtNom.Text = c.Nom;
                txtPrenom.Text = c.Prenom;
                txtEntreprise.Text = c.Entreprise;
                txtEmail.Text = c.Email;
                txtTelephone.Text = c.Telephone;
                txtAdresse.Text = c.Adresse;
                VerrouillerChamps();
            }
        }

        // ========================= SUPPRIMER =========================
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (clientSelectionne == null) return;

            var result = MessageBox.Show($"❓ Supprimer définitivement {clientSelectionne.Nom} {clientSelectionne.Prenom} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bdd.SupprimerClient(clientSelectionne.IdClient);
                clients.Remove(clientSelectionne);
                ViderChamps();
            }
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            // On ne fait rien si on n'est pas en train d'ajouter ou de modifier
            if (isNewClient || isEditing)
            {
                // 1. Demander confirmation pour ne pas perdre les données saisies par erreur
                var result = MessageBox.Show(
                    "❓ Voulez-vous vraiment annuler les modifications en cours ?",
                    "Confirmation d'annulation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // 2. On remet les indicateurs à zéro
                    isNewClient = false;
                    isEditing = false;
                    clientEnCours = null;

                    // 3. On restaure l'affichage
                    if (clientSelectionne != null)
                    {
                        // Si on modifiait un client existant, on réaffiche ses vraies infos
                        GridDetails.DataContext = clientSelectionne;

                        txtNom.Text = clientSelectionne.Nom;
                        txtPrenom.Text = clientSelectionne.Prenom;
                        txtEntreprise.Text = clientSelectionne.Entreprise;
                        txtEmail.Text = clientSelectionne.Email;
                        txtTelephone.Text = clientSelectionne.Telephone;
                        txtAdresse.Text = clientSelectionne.Adresse;
                    }
                    else
                    {
                        // Si on ajoutait un nouveau client, on vide tout simplement
                        GridDetails.DataContext = null;
                        ViderChamps();
                    }

                    // 4. On reverrouille les champs (IsReadOnly = true)
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
                if (TxtRechercheClient == null || CmbTypeClient == null || DtgClients == null) return;

                // 1. Récupérer la liste de base
                var tousLesClients = bdd.GetClients().AsEnumerable();

                // 2. Filtre Recherche (Nom, Prénom ou Entreprise)
                string recherche = TxtRechercheClient.Text.Trim();
                if (!string.IsNullOrEmpty(recherche))
                {
                    tousLesClients = tousLesClients.Where(c =>
                        (c.Nom != null && c.Nom.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.Prenom != null && c.Prenom.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (c.Entreprise != null && c.Entreprise.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0)
                    );
                }

                // 3. Filtre Type (ComboBox)
                if (CmbTypeClient.SelectedIndex == 1) // Particuliers
                {
                    tousLesClients = tousLesClients.Where(c => string.IsNullOrEmpty(c.Entreprise));
                }
                else if (CmbTypeClient.SelectedIndex == 2) // Entreprises
                {
                    tousLesClients = tousLesClients.Where(c => !string.IsNullOrEmpty(c.Entreprise));
                }

                // 4. Mise à jour de l'UI
                clients = new ObservableCollection<Client>(tousLesClients.ToList());
                DtgClients.ItemsSource = clients;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur filtrage : " + ex.Message);
            }
        }

        // Événement quand on tape du texte
        private void TxtRechercheClient_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppliquerFiltres();
        }

        // Événement quand on change la sélection du ComboBox
        private void CmbTypeClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded) AppliquerFiltres();
        }


        private void BtnReinitialiser_Click(object sender, RoutedEventArgs e)
        {
            TxtRechercheClient.Text = "";
            CmbTypeClient.SelectedIndex = 0;
            AppliquerFiltres();
            ViderChamps();
            VerrouillerChamps();
        }
    }
}