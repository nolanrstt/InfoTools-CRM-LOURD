using System;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SlamDunkApp
{
    public partial class DashboardView : UserControl
    {
        // Propriétés pour le graphique LiveCharts
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }

        public DashboardView()
        {
            InitializeComponent();

            // 1. On charge toutes les données
            ChargerStatistiques();
            ChargerTableaux();
            ChargerGraphique();

            // 2. On lie les données au XAML
            DataContext = this;
        }

        private void ChargerStatistiques()
        {
            try
            {
                Bdd bdd = new Bdd();
                // Mise à jour des textes des gros compteurs
                TxtNbClients.Text = bdd.GetNombreClients().ToString();
                TxtNbProduits.Text = bdd.GetNombreProduits().ToString();
                TxtNbProspects.Text = bdd.GetNombreProspects().ToString();
                TxtNbRendezVous.Text = bdd.GetNombreRendezVous().ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de chargement des statistiques : " + ex.Message);
            }
        }

        private void ChargerTableaux()
        {
            try
            {
                Bdd bdd = new Bdd();
                // Remplissage des 3 petits tableaux en bas
                DataGridProspects.ItemsSource = bdd.GetDerniersProspects();
                DataGridRdv.ItemsSource = bdd.GetProchainsRendezVous();
                DataGridProduitsDashboard.ItemsSource = bdd.GetProduitsDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de chargement des tableaux : " + ex.Message);
            }
        }

        private void ChargerGraphique()
        {
            try
            {
                Bdd bdd = new Bdd();

                // Création de la courbe pour l'évolution des clients
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Nouveaux Clients",
                        Values = new ChartValues<int>(bdd.GetStatsClientsParMois()),
                        PointGeometry = DefaultGeometries.Circle,
                        PointGeometrySize = 12,
                        Stroke = System.Windows.Media.Brushes.Red,
                        Fill = System.Windows.Media.Brushes.Transparent // Transparent pour avoir juste une ligne
                    }
                };

                // Labels pour l'axe X (de Janvier à Juin)
                Labels = new[] { "Jan", "Fév", "Mar", "Avr", "Mai", "Juin" };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de chargement du graphique : " + ex.Message);
            }
        }

        // =======================================================
        // MÉTHODE D'EXPORT PDF
        // =======================================================
        private void BtnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Fichier PDF (*.pdf)|*.pdf";
            sfd.FileName = "Bilan_CRM_" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    // Récupération des données
                    Bdd bdd = new Bdd();
                    int nbClients = bdd.GetNombreClients();
                    int nbProduits = bdd.GetNombreProduits();
                    int nbProspects = bdd.GetNombreProspects();
                    int nbRdv = bdd.GetNombreRendezVous();

                    // Couleurs personnalisées (Ton rouge d'interface)
                    BaseColor couleurRouge = new BaseColor(237, 47, 54);
                    BaseColor couleurGrisClair = new BaseColor(245, 245, 245);

                    Document doc = new Document(PageSize.A4, 50, 50, 50, 50);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));

                    doc.Open();

                    // ----------------------------------------------------
                    // 1. AJOUT DU LOGO
                    // ----------------------------------------------------
                    try
                    {
                        string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo-infotools.png");
                        if (File.Exists(logoPath))
                        {
                            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                            logo.Alignment = Element.ALIGN_CENTER;
                            logo.ScaleToFit(200f, 100f); // Taille max de l'image
                            logo.SpacingAfter = 20;
                            doc.Add(logo);
                        }
                    }
                    catch
                    {
                        // Si l'image n'est pas trouvée, on ne fait pas crasher l'app, on passe à la suite
                    }

                    // ----------------------------------------------------
                    // 2. EN-TÊTE DU DOCUMENT
                    // ----------------------------------------------------
                    Font titreFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, couleurRouge);
                    Paragraph titre = new Paragraph("RAPPORT D'ACTIVITÉ CRM", titreFont);
                    titre.Alignment = Element.ALIGN_CENTER;
                    titre.SpacingAfter = 5;
                    doc.Add(titre);

                    Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 11, BaseColor.DARK_GRAY);
                    Paragraph datePara = new Paragraph("Généré le : " + DateTime.Now.ToString("dd/MM/yyyy à HH:mm"), dateFont);
                    datePara.Alignment = Element.ALIGN_CENTER;
                    datePara.SpacingAfter = 20;
                    doc.Add(datePara);

                    // Ligne de séparation
                    iTextSharp.text.pdf.draw.LineSeparator ligne = new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -1);
                    doc.Add(new Chunk(ligne));
                    doc.Add(new Paragraph("\n")); // Espace

                    Font introFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                    Paragraph intro = new Paragraph("Voici le résumé officiel des données actuelles enregistrées dans la base de données InfoTools :", introFont);
                    intro.SpacingAfter = 30;
                    doc.Add(intro);

                    // ----------------------------------------------------
                    // 3. TABLEAU DES STATISTIQUES
                    // ----------------------------------------------------
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 80; // Le tableau prend 80% de la largeur de la page
                    table.SetWidths(new float[] { 2f, 1f }); // Colonne de gauche plus large que celle de droite

                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, BaseColor.BLACK);
                    Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, couleurRouge);

                    // Création d'une fonction interne pour générer les lignes du tableau rapidement
                    void AjouterLigneStats(string libelle, string valeur)
                    {
                        PdfPCell cell1 = new PdfPCell(new Phrase(libelle, cellFont));
                        cell1.Padding = 12;
                        cell1.BackgroundColor = couleurGrisClair;
                        cell1.BorderColor = BaseColor.LIGHT_GRAY;

                        PdfPCell cell2 = new PdfPCell(new Phrase(valeur, valueFont));
                        cell2.Padding = 12;
                        cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell2.BorderColor = BaseColor.LIGHT_GRAY;

                        table.AddCell(cell1);
                        table.AddCell(cell2);
                    }

                    // Ajout des données au tableau
                    AjouterLigneStats("👥 Total des Clients", nbClients.ToString());
                    AjouterLigneStats("📦 Produits en catalogue", nbProduits.ToString());
                    AjouterLigneStats("🎯 Prospects en attente", nbProspects.ToString());
                    AjouterLigneStats("📅 Rendez-vous enregistrés", nbRdv.ToString());

                    doc.Add(table);

                    // Fermeture du fichier
                    doc.Close();

                    MessageBox.Show("Le rapport PDF a été généré avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de la génération du PDF : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}