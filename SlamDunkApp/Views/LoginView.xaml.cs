using System;
using System.Windows;
using System.Windows.Controls;

namespace SlamDunkApp
{
    public partial class LoginView : UserControl
    {
        // Événement pour informer la MainWindow qu'une connexion a été faite
        // Ajoute le ? après EventHandler<Utilisateur>
        public event EventHandler<Utilisateur>? LoginAttempted;

        // CORRECTION 1 : On utilise la VRAIE Bdd (plus de FakeBdd)
        private Bdd bdd = new Bdd();

        public LoginView()
        {
            InitializeComponent();

            // Associer l'événement TextChanged pour le TextBox visible
            TxtMotDePasseVisible.TextChanged += TxtMotDePasseVisible_TextChanged;
        }

        private void BtnConnexion_Click(object sender, RoutedEventArgs e)
        {
            string identifiant = TxtIdentifiant.Text;
            string motDePasse;

            // Vérifier si le mot de passe est affiché ou non
            if (TxtMotDePasse.Visibility == Visibility.Visible)
                motDePasse = TxtMotDePasse.Password;
            else
                motDePasse = TxtMotDePasseVisible.Text;

            // CORRECTION 2 : On utilise la méthode 'VerifierConnexion' qu'on a créée dans Bdd.cs
            // Elle va vérifier dans MySQL si l'utilisateur existe
            Utilisateur? user = bdd.VerifierConnexion(identifiant, motDePasse);

            if (user != null)
            {
                TxtErreur.Visibility = Visibility.Collapsed;

                // Déclenche l'événement pour la MainWindow avec le vrai utilisateur chargé
                LoginAttempted?.Invoke(this, user);
            }
            else
            {
                TxtErreur.Text = "Identifiant ou mot de passe incorrect.";
                TxtErreur.Visibility = Visibility.Visible;
            }
        }

        // --- LE RESTE DU CODE NE CHANGE PAS (C'est purement visuel) ---

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            if (TxtMotDePasse.Visibility == Visibility.Visible)
            {
                // Montrer le mot de passe
                TxtMotDePasseVisible.Text = TxtMotDePasse.Password;
                TxtMotDePasse.Visibility = Visibility.Collapsed;
                TxtMotDePasseVisible.Visibility = Visibility.Visible;
            }
            else
            {
                // Masquer le mot de passe
                TxtMotDePasse.Password = TxtMotDePasseVisible.Text;
                TxtMotDePasse.Visibility = Visibility.Visible;
                TxtMotDePasseVisible.Visibility = Visibility.Collapsed;
            }
        }

        // Synchroniser TextBox visible avec PasswordBox
        private void TxtMotDePasseVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtMotDePasseVisible.Visibility == Visibility.Visible)
            {
                TxtMotDePasse.Password = TxtMotDePasseVisible.Text;
            }
        }
    }
}