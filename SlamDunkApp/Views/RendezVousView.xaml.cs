using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using SlamDunkApp;

namespace SlamDunkApp
{
    public partial class RendezVousView : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        private DateTime lundiEnCours;

        // Une seule déclaration propre ici pour gérer la modification
        private int? _idRdvEnCoursDeModification = null;

        public RendezVousView()
        {
            InitializeComponent();
            InitializeBaseProperties();
            lundiEnCours = GetLundiDeLaSemaine(new DateTime(2025, 1, 6));
            ChargerLesElementsVisuels();
        }

        public RendezVousView(DateTime dateDuRdv)
        {
            InitializeComponent();
            InitializeBaseProperties();
            lundiEnCours = GetLundiDeLaSemaine(dateDuRdv);
            ChargerLesElementsVisuels();
        }

        private void InitializeBaseProperties()
        {
            SeriesCollection = new SeriesCollection();
            Labels = new string[] { };

            InitGraphique();
            ChargerLesClients();
            ChargerUtilisateurs();

            DataContext = this;
        }

        private void ChargerLesElementsVisuels()
        {
            ChargerAgendaSemaine();
        }

        // --- NAVIGATION SEMAINE ---
        private void BtnSemainePrecedente_Click(object sender, RoutedEventArgs e)
        {
            lundiEnCours = lundiEnCours.AddDays(-7);
            ChargerAgendaSemaine();
        }

        private void BtnSemaineSuivante_Click(object sender, RoutedEventArgs e)
        {
            lundiEnCours = lundiEnCours.AddDays(7);
            ChargerAgendaSemaine();
        }

        private DateTime GetLundiDeLaSemaine(DateTime date)
        {
            int delta = date.DayOfWeek == DayOfWeek.Sunday ? -6 : -(int)date.DayOfWeek + 1;
            return date.AddDays(delta).Date;
        }

        // --- LECTURE BDD ---
        private void ChargerAgendaSemaine()
        {
            try
            {
                DateTime dimanche = lundiEnCours.AddDays(6);
                TxtSemaineEnCours.Text = $"Semaine du {lundiEnCours:dd/MM} au {dimanche:dd/MM/yyyy}";

                var elementsSupprimer = new List<UIElement>();
                foreach (UIElement child in GridAgenda.Children)
                {
                    if (child is Border) elementsSupprimer.Add(child);
                }
                foreach (var el in elementsSupprimer) GridAgenda.Children.Remove(el);

                Bdd bdd = new Bdd();
                List<RendezVous> lesRdv = bdd.GetRendezVousParDate(lundiEnCours, dimanche.AddDays(1).AddSeconds(-1));

                foreach (var rdv in lesRdv)
                {
                    int jourIndex = (int)rdv.DateHeure.DayOfWeek;
                    if (jourIndex == 0 || jourIndex > 5) continue;

                    int heure = rdv.DateHeure.Hour;
                    string texteAffiche = $"{rdv.Titre}\n{rdv.NomClient}\nAssigné à : {rdv.NomUtilisateur}";

                    AjouterRdvVisuel(texteAffiche, jourIndex, heure, 2, rdv.Id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Agenda : " + ex.Message);
            }
        }

        // --- FORMULAIRE ET CLICS ---

        // 1. Quand tu cliques sur un bloc jaune dans l'agenda
        private void ChargerDetailsRdvPourModification(int idRdv)
        {
            Bdd bdd = new Bdd();
            RendezVous rdv = bdd.GetRendezVousById(idRdv);

            if (rdv != null)
            {
                _idRdvEnCoursDeModification = rdv.Id;

                // Sélectionner le Client
                foreach (Client c in CboClient.Items)
                {
                    if (c.IdClient == rdv.IdClient) { CboClient.SelectedItem = c; break; }
                }

                // Sélectionner l'Utilisateur
                foreach (Utilisateur u in CboUtilisateur.Items)
                {
                    if (u.Id == rdv.IdUtilisateur) { CboUtilisateur.SelectedItem = u; break; }
                }

                int indexJour = (int)rdv.DateHeure.DayOfWeek - 1;
                if (indexJour == -1) indexJour = 6;
                CboJour.SelectedIndex = indexJour;

                int indexHeure = rdv.DateHeure.Hour - 8;
                CboHeure.SelectedIndex = indexHeure;

                // On active le bouton Modifier
                if (BtnModifier != null) BtnModifier.IsEnabled = true;
            }
        }

        // 2. Le bouton AJOUTER (Il ne fait QUE ajouter)
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (CboClient.SelectedItem == null || CboJour.SelectedIndex == -1 || CboHeure.SelectedIndex == -1 || CboUtilisateur.SelectedItem == null)
            {
                MessageBox.Show("Veuillez remplir tous les champs.");
                return;
            }

            Client c = (Client)CboClient.SelectedItem;
            int idUtilisateur = Convert.ToInt32(CboUtilisateur.SelectedValue);
            DateTime dateRdv = lundiEnCours.AddDays(CboJour.SelectedIndex).AddHours(CboHeure.SelectedIndex + 8);
            string titre = "RDV Client: " + c.Nom;

            Bdd bdd = new Bdd();
            if (!bdd.EstCreneauLibre(dateRdv))
            {
                MessageBox.Show("Ce créneau est indisponible.");
                return;
            }

            bdd.AjouterRdvBDD(c.IdClient, dateRdv, titre, idUtilisateur);

            ChargerAgendaSemaine();
            RefreshGraphique();
        }

        // 3. Le NOUVEAU bouton MODIFIER (Il ne fait QUE modifier)
        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_idRdvEnCoursDeModification == null || CboClient.SelectedItem == null || CboJour.SelectedIndex == -1 || CboHeure.SelectedIndex == -1 || CboUtilisateur.SelectedItem == null)
                return;

            Client c = (Client)CboClient.SelectedItem;
            int idUtilisateur = Convert.ToInt32(CboUtilisateur.SelectedValue);
            DateTime dateRdv = lundiEnCours.AddDays(CboJour.SelectedIndex).AddHours(CboHeure.SelectedIndex + 8);
            string titre = "RDV Client: " + c.Nom;

            Bdd bdd = new Bdd();
            bdd.ModifierRdvBDD(_idRdvEnCoursDeModification.Value, c.IdClient, dateRdv, titre, idUtilisateur);

            // Réinitialisation
            _idRdvEnCoursDeModification = null;
            if (BtnModifier != null) BtnModifier.IsEnabled = false; // On regrise le bouton
            CboClient.SelectedIndex = -1;
            CboJour.SelectedIndex = -1;
            CboHeure.SelectedIndex = -1;
            CboUtilisateur.SelectedIndex = -1;

            ChargerAgendaSemaine();
        }

        // --- DESSIN RDV ---
        public void AjouterRdvVisuel(string texte, int jourIndex, int heureDebut, int duree, int idRdv)
        {
            int ligne = heureDebut - 7;
            if (ligne < 1 || ligne > 10) return;

            Border rdvBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF7C13C")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED2F36")),
                BorderThickness = new Thickness(4, 0, 0, 0),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(2),
                Padding = new Thickness(5),
                Tag = idRdv,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            rdvBorder.MouseLeftButtonDown += (s, e) =>
            {
                ChargerDetailsRdvPourModification(idRdv);
            };

            ContextMenu menu = new ContextMenu();
            MenuItem deleteItem = new MenuItem { Header = "Supprimer ce rendez-vous" };
            deleteItem.Click += (s, e) =>
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer ce rendez-vous ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Bdd bdd = new Bdd();
                    bdd.SupprimerRdv(idRdv);
                    ChargerAgendaSemaine();
                    RefreshGraphique();
                }
            };
            menu.Items.Add(deleteItem);
            rdvBorder.ContextMenu = menu;

            TextBlock txt = new TextBlock
            {
                Text = texte,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 10
            };
            rdvBorder.Child = txt;

            try
            {
                Grid.SetColumn(rdvBorder, jourIndex);
                Grid.SetRow(rdvBorder, ligne);
                Grid.SetRowSpan(rdvBorder, duree);
                GridAgenda.Children.Add(rdvBorder);
            }
            catch { }
        }

        // --- UTILITAIRES ---
        private void InitGraphique()
        {
            RefreshGraphique();
        }

        private void RefreshGraphique()
        {
            try
            {
                Bdd bdd = new Bdd();
                SeriesCollection.Clear();
                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "RDV 2025",
                    Values = new ChartValues<int>(bdd.GetStatistiquesAnnee()),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFED2F36")),
                    DataLabels = true
                });
                Labels = new[] { "Jan", "Fév", "Mar", "Avr", "Mai", "Juin" };
            }
            catch { }
        }

        private void ChargerLesClients()
        {
            try { CboClient.ItemsSource = new Bdd().GetClients(); } catch { }
        }

        private void ChargerUtilisateurs()
        {
            try
            {
                CboUtilisateur.ItemsSource = new Bdd().GetUtilisateurs();
            }
            catch { }
        }

        private void CboUtilisateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void CboJour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}