using System.Windows;
using System.Windows.Input;

namespace SlamDunkApp
{
    public partial class MainWindow : Window
    {
        private bool estConnecte = false;
        private Utilisateur? utilisateurConnecte;

        public MainWindow()
        {
            InitializeComponent();
            AfficherPageConnexion();
        }

        private void AfficherPageConnexion()
        {
            // Cacher le menu au démarrage ou à la déconnexion
            BtnAccueil.Visibility = Visibility.Collapsed;
            BtnProduits.Visibility = Visibility.Collapsed;
            BtnClients.Visibility = Visibility.Collapsed;
            BtnRendezVous.Visibility = Visibility.Collapsed;
            BtnFactures.Visibility = Visibility.Collapsed; // 🟢 Bien caché
            BtnConnexion.Visibility = Visibility.Collapsed;
            ProfilUserBorder.Visibility = Visibility.Collapsed;

            // Charger la vue de login
            var loginView = new LoginView();
            loginView.LoginAttempted += LoginView_LoginAttempted;
            MainContent.Content = loginView;
        }

        // Cette méthode est appelée quand on clique sur "Se connecter" dans LoginView
        private void LoginView_LoginAttempted(object sender, Utilisateur utilisateur)
        {
            // 🟢 J'ai fusionné tes deux blocs "if" pour que ce soit plus propre
            if (utilisateur != null)
            {
                // Connexion RÉUSSIE
                this.estConnecte = true;
                this.utilisateurConnecte = utilisateur;

                // Mettre à jour l'affichage du profil
                UpdateUserDisplay(utilisateur);

                // Afficher le menu
                BtnAccueil.Visibility = Visibility.Visible;
                BtnProduits.Visibility = Visibility.Visible;
                BtnClients.Visibility = Visibility.Visible;
                BtnRendezVous.Visibility = Visibility.Visible;
                BtnFactures.Visibility = Visibility.Visible; // 🟢 Bien affiché
                BtnConnexion.Visibility = Visibility.Visible;
                ProfilUserBorder.Visibility = Visibility.Visible;

                BtnConnexion.Content = "Déconnexion";

                // Redirection vers le Dashboard
                MainContent.Content = new DashboardView();
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- GESTION DES BOUTONS DU MENU ---

        private void BtnAccueil_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte) MainContent.Content = new DashboardView();
        }

        private void BtnProduits_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte) MainContent.Content = new ProduitsView();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte) MainContent.Content = new ClientsView();
        }

        private void BtnRendezVous_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte) MainContent.Content = new RendezVousView();
        }

        // 🟢 NOUVEAU BOUTON FACTURES
        private void BtnFactures_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte) MainContent.Content = new FacturesView();
        }

        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            if (estConnecte)
            {
                if (MessageBox.Show("Se déconnecter ?", "Déconnexion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    estConnecte = false;
                    utilisateurConnecte = null;

                    // Nettoyage visuel du profil
                    txtUserName.Text = "ID: -";
                    txtUserInitial.Text = "?";

                    // Retour à l'écran de connexion (ça cachera tous les boutons automatiquement)
                    AfficherPageConnexion();
                }
            }
        }

        public void UpdateUserDisplay(Utilisateur u)
        {
            if (u != null)
            {
                txtUserName.Text = u.Prenom;
                txtUserRole.Text = "Manager";

                if (!string.IsNullOrEmpty(u.Prenom))
                {
                    txtUserInitial.Text = u.Prenom.Substring(0, 1).ToUpper();
                }
            }
        }

        private void BarreTitre_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    if (this.WindowState == WindowState.Maximized)
                        this.WindowState = WindowState.Normal;
                    else
                        this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.DragMove();
                }
            }
        }
    }
}