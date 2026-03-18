using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Text.Json;
using System.Linq;

namespace SlamDunkApp
{
    public class Bdd
    {
        // =========================================================
        // 0. CONFIGURATION
        // =========================================================
        private string connectionString = "Server=127.0.0.1;Database=InfoTools;Uid=root;Pwd=root;";

        // =========================================================
        // MÉTHODE D'AUDIT (LOGS)
        // =========================================================
        public void AjouterLog(string actionType, string tableName, string recordId, object? ancienObjet, object? nouvelObjet)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string userLogin = Environment.UserName;

                    string jsonOld = ancienObjet != null ? JsonSerializer.Serialize(ancienObjet) : "null";
                    string jsonNew = nouvelObjet != null ? JsonSerializer.Serialize(nouvelObjet) : "null";

                    string query = @"INSERT INTO audit_logs (user_login, action_type, table_name, record_id, old_value, new_value) 
                                     VALUES (@user, @action, @table, @id, @old, @new)";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user", userLogin);
                    cmd.Parameters.AddWithValue("@action", actionType);
                    cmd.Parameters.AddWithValue("@table", tableName);
                    cmd.Parameters.AddWithValue("@id", recordId);
                    cmd.Parameters.AddWithValue("@old", jsonOld);
                    cmd.Parameters.AddWithValue("@new", jsonNew);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur Audit : " + ex.Message);
            }
        }

        // =========================================================
        // 1. GESTION UTILISATEUR (LOGIN)
        // =========================================================
        public Utilisateur? VerifierConnexion(string identifiant, string motDePasse)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Utilisateur WHERE Identifiant = @user AND MotDePasse = @pass";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@user", identifiant);
                    cmd.Parameters.AddWithValue("@pass", motDePasse);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Utilisateur
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom"),
                                Identifiant = reader.GetString("Identifiant"),
                                Password = reader.GetString("MotDePasse")
                            };
                        }

                        if (reader.Read())
                        {
                            return new Utilisateur
                            {
                                Id = reader.GetInt32("idUtilisateur"),
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom"),
                                Identifiant = reader.GetString("Identifiant")
                            };
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur Login : " + ex.Message); }
            }
            return null;
        }

        public bool VerifierUtilisateur(string identifiant, string motDePasse)
        {
            return VerifierConnexion(identifiant, motDePasse) != null;
        }

        public List<Utilisateur> GetUtilisateurs()
        {
            List<Utilisateur> liste = new List<Utilisateur>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, Nom, Prenom FROM Utilisateur ORDER BY Nom ASC";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Utilisateur
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur GetUtilisateurs : " + ex.Message); }
            }
            return liste;
        }

        // =========================================================
        // 2. CLIENTS (Lecture, Ajout, Modif, Suppr)
        // =========================================================
        public List<Client> GetClients()
        {
            List<Client> liste = new List<Client>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Client ORDER BY Nom ASC";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Client
                            {
                                IdClient = reader.GetInt32("IdClient"),
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom"),
                                Entreprise = reader.IsDBNull(reader.GetOrdinal("Entreprise")) ? null : reader.GetString("Entreprise"),
                                Email = reader.GetString("Email"),
                                Telephone = reader.GetString("Telephone"),
                                Adresse = reader.GetString("Adresse")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur GetClients : " + ex.Message); }
            }
            return liste;
        }

        public void AjouterClient(Client c)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Client (Nom, Prenom, Entreprise, Email, Telephone, Adresse) VALUES (@nom, @prenom, @ent, @email, @tel, @adr)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nom", c.Nom);
                    cmd.Parameters.AddWithValue("@prenom", c.Prenom);
                    cmd.Parameters.AddWithValue("@ent", c.Entreprise);
                    cmd.Parameters.AddWithValue("@email", c.Email);
                    cmd.Parameters.AddWithValue("@tel", c.Telephone);
                    cmd.Parameters.AddWithValue("@adr", c.Adresse);
                    cmd.ExecuteNonQuery();
                    c.IdClient = (int)cmd.LastInsertedId;

                    AjouterLog("INSERT", "Client", c.IdClient.ToString(), null, c);
                }
                catch (Exception ex) { MessageBox.Show("Erreur AjouterClient : " + ex.Message); }
            }
        }

        public void ModifierClient(Client c)
        {
            Client? ancien = GetClients().FirstOrDefault(x => x.IdClient == c.IdClient);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Client SET Nom=@nom, Prenom=@prenom, Entreprise=@ent, Email=@email, Telephone=@tel, Adresse=@adr WHERE IdClient=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", c.IdClient);
                    cmd.Parameters.AddWithValue("@nom", c.Nom);
                    cmd.Parameters.AddWithValue("@prenom", c.Prenom);
                    cmd.Parameters.AddWithValue("@ent", c.Entreprise);
                    cmd.Parameters.AddWithValue("@email", c.Email);
                    cmd.Parameters.AddWithValue("@tel", c.Telephone);
                    cmd.Parameters.AddWithValue("@adr", c.Adresse);
                    cmd.ExecuteNonQuery();

                    AjouterLog("UPDATE", "Client", c.IdClient.ToString(), ancien, c);
                }
                catch (Exception ex) { MessageBox.Show("Erreur ModifierClient : " + ex.Message); }
            }
        }

        public void SupprimerClient(int idClient)
        {
            Client? ancien = GetClients().FirstOrDefault(x => x.IdClient == idClient);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM Client WHERE IdClient = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", idClient);
                    cmd.ExecuteNonQuery();

                    AjouterLog("DELETE", "Client", idClient.ToString(), ancien, null);
                }
                catch (Exception ex) { MessageBox.Show("Erreur SupprimerClient : " + ex.Message); }
            }
        }

        // =========================================================
        // 3. PRODUITS (Lecture, Ajout, Modif, Suppr)
        // =========================================================
        public List<Produit> GetProduits()
        {
            List<Produit> liste = new List<Produit>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Produit ORDER BY Nom ASC";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Produit
                            {
                                Id = reader.GetInt32("Id"),
                                Nom = reader.GetString("Nom"),
                                Categorie = reader.GetString("Categorie"),

                                // ✅ CORRECTION DU BUG DES PRIX
                                Prix = Convert.ToDecimal(reader["Prix"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),

                                Stock = reader.GetInt32("Stock")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur GetProduits : " + ex.Message); }
            }
            return liste;
        }

        public void AjouterProduit(Produit p)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Produit (Nom, Categorie, Prix, Stock) VALUES (@nom, @cat, @prix, @stock)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nom", p.Nom);
                    cmd.Parameters.AddWithValue("@cat", p.Categorie);
                    cmd.Parameters.AddWithValue("@prix", p.Prix);
                    cmd.Parameters.AddWithValue("@stock", p.Stock);
                    cmd.ExecuteNonQuery();
                    p.Id = (int)cmd.LastInsertedId;

                    AjouterLog("INSERT", "Produit", p.Id.ToString(), null, p);
                }
                catch (Exception ex) { MessageBox.Show("Erreur AjouterProduit : " + ex.Message); }
            }
        }

        public void ModifierProduit(Produit p)
        {
            Produit? ancien = GetProduits().FirstOrDefault(x => x.Id == p.Id);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Produit SET Nom=@nom, Categorie=@cat, Prix=@prix, Stock=@stock WHERE Id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", p.Id);
                    cmd.Parameters.AddWithValue("@nom", p.Nom);
                    cmd.Parameters.AddWithValue("@cat", p.Categorie);
                    cmd.Parameters.AddWithValue("@prix", p.Prix);
                    cmd.Parameters.AddWithValue("@stock", p.Stock);
                    cmd.ExecuteNonQuery();

                    AjouterLog("UPDATE", "Produit", p.Id.ToString(), ancien, p);
                }
                catch (Exception ex) { MessageBox.Show("Erreur ModifierProduit : " + ex.Message); }
            }
        }

        public void SupprimerProduit(int idProduit)
        {
            Produit? ancien = GetProduits().FirstOrDefault(x => x.Id == idProduit);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM Produit WHERE Id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", idProduit);
                    cmd.ExecuteNonQuery();

                    AjouterLog("DELETE", "Produit", idProduit.ToString(), ancien, null);
                }
                catch (Exception ex) { MessageBox.Show("Erreur SupprimerProduit : " + ex.Message); }
            }
        }

        // =========================================================
        // 4. STATISTIQUES (Graphique)
        // =========================================================
        public int[] GetStatistiquesAnnee()
        {
            int[] resultats = new int[] { 0, 0, 0, 0, 0, 0 };

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MONTH(Date_Heure) as Mois, COUNT(*) as Total FROM RendezVous WHERE YEAR(Date_Heure) = 2025 GROUP BY Mois";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int mois = Convert.ToInt32(reader["Mois"]);
                            int total = Convert.ToInt32(reader["Total"]);
                            if (mois >= 1 && mois <= 6)
                            {
                                resultats[mois - 1] = total;
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur lecture Stats : " + ex.Message); }
            }
            return resultats;
        }

        // =========================================================
        // 5. GESTION AGENDA (Lecture, Verif, Ajout, Suppr)
        // =========================================================

        public List<RendezVous> GetRendezVousParDate(DateTime debut, DateTime fin)
        {
            List<RendezVous> liste = new List<RendezVous>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT 
                                      r.IdRdv, r.Titre, r.Date_Heure, r.Lieu, r.IdClient,
                                      c.Nom AS ClientNom, c.Prenom AS ClientPrenom,
                                      p.Nom AS ProspectNom, p.Prenom AS ProspectPrenom,
                                      u.Nom AS UserNom, u.Prenom AS UserPrenom 
                                     FROM RendezVous r 
                                     LEFT JOIN Client c ON r.IdClient = c.IdClient 
                                     LEFT JOIN Prospect p ON r.IdProspect = p.IdProspect
                                     LEFT JOIN Utilisateur u ON r.IdUtilisateur = u.Id
                                     WHERE r.Date_Heure BETWEEN @debut AND @fin";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@debut", debut);
                    cmd.Parameters.AddWithValue("@fin", fin);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nomComplet = "Inconnu";
                            if (!reader.IsDBNull(reader.GetOrdinal("ClientNom")))
                            {
                                nomComplet = reader.GetString("ClientNom") + " " + reader.GetString("ClientPrenom");
                            }
                            else if (!reader.IsDBNull(reader.GetOrdinal("ProspectNom")))
                            {
                                nomComplet = "PROSPECT: " + reader.GetString("ProspectNom") + " " + reader.GetString("ProspectPrenom");
                            }

                            string nomUser = "Non assigné";
                            if (!reader.IsDBNull(reader.GetOrdinal("UserNom")))
                            {
                                nomUser = reader.GetString("UserPrenom") + " " + reader.GetString("UserNom");
                            }

                            liste.Add(new RendezVous
                            {
                                Id = reader.GetInt32("IdRdv"),
                                Titre = reader.GetString("Titre"),
                                DateHeure = reader.GetDateTime("Date_Heure"),
                                Lieu = reader.GetString("Lieu"),
                                IdClient = reader.IsDBNull(reader.GetOrdinal("IdClient")) ? 0 : reader.GetInt32("IdClient"),
                                NomClient = nomComplet,
                                NomUtilisateur = nomUser
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur lecture Agenda : " + ex.Message); }
            }
            return liste;
        }

        public RendezVous GetRendezVousById(int idRdv)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM RendezVous WHERE IdRdv = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", idRdv);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RendezVous
                            {
                                Id = reader.GetInt32("IdRdv"),
                                Titre = reader.GetString("Titre"),
                                DateHeure = reader.GetDateTime("Date_Heure"),
                                IdClient = reader.IsDBNull(reader.GetOrdinal("IdClient")) ? 0 : reader.GetInt32("IdClient"),
                                IdUtilisateur = reader.IsDBNull(reader.GetOrdinal("IdUtilisateur")) ? 0 : reader.GetInt32("IdUtilisateur")
                            };
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur récupération RDV : " + ex.Message); }
            }
            return null;
        }

        public void ModifierRdvBDD(int idRdv, int idClient, DateTime dateHeure, string titre, int idUtilisateur)
        {
            // ✅ AUDIT : On récupère l'ancien RDV avant modif
            var ancienRdv = GetRendezVousById(idRdv);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE RendezVous 
                             SET IdClient = @idC, Date_Heure = @date, Titre = @titre, IdUtilisateur = @idU 
                             WHERE IdRdv = @idRdv";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@idRdv", idRdv);
                    cmd.Parameters.AddWithValue("@idC", idClient);
                    cmd.Parameters.AddWithValue("@date", dateHeure);
                    cmd.Parameters.AddWithValue("@titre", titre);
                    cmd.Parameters.AddWithValue("@idU", idUtilisateur);

                    cmd.ExecuteNonQuery();

                    // ✅ AUDIT
                    var nouveauRdv = new { IdRdv = idRdv, IdClient = idClient, Date = dateHeure, Titre = titre, IdUtilisateur = idUtilisateur };
                    AjouterLog("UPDATE", "RendezVous", idRdv.ToString(), ancienRdv, nouveauRdv);
                }
                catch (Exception ex) { MessageBox.Show("Erreur Modification RDV : " + ex.Message); }
            }
        }


        public bool EstCreneauLibre(DateTime nouvelleDate)
        {
            DateTime finNouveau = nouvelleDate.AddHours(2);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT COUNT(*) FROM RendezVous 
                                     WHERE Date_Heure < @finNouveau 
                                     AND ADDTIME(Date_Heure, '02:00:00') > @debutNouveau";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@debutNouveau", nouvelleDate);
                    cmd.Parameters.AddWithValue("@finNouveau", finNouveau);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
                catch { return false; }
            }
        }

        public void AjouterRdvBDD(int idClient, DateTime dateHeure, string titre, int idUtilisateur)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO RendezVous (IdClient, Date_Heure, Titre, Description, Lieu, IdUtilisateur) VALUES (@id, @date, @titre, '', 'Bureau', @idU)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@id", idClient);
                    cmd.Parameters.AddWithValue("@date", dateHeure);
                    cmd.Parameters.AddWithValue("@titre", titre);
                    cmd.Parameters.AddWithValue("@idU", idUtilisateur);

                    cmd.ExecuteNonQuery();

                    // ✅ AUDIT
                    var rdvLog = new { IdClient = idClient, Date = dateHeure, Titre = titre, IdUtilisateur = idUtilisateur };
                    AjouterLog("INSERT", "RendezVous", "N/A", null, rdvLog);
                }
                catch (Exception ex) { MessageBox.Show("Erreur Ajout RDV : " + ex.Message); }
            }
        }

        public void SupprimerRdv(int idRdv)
        {
            // ✅ AUDIT : On récupère l'ancien avant de supprimer
            var ancienRdv = GetRendezVousById(idRdv);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM RendezVous WHERE IdRdv = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", idRdv);
                    cmd.ExecuteNonQuery();

                    // ✅ AUDIT
                    AjouterLog("DELETE", "RendezVous", idRdv.ToString(), ancienRdv, null);
                }
                catch (Exception ex) { MessageBox.Show("Erreur Suppression RDV : " + ex.Message); }
            }
        }

        // =========================================================
        // 6. DASHBOARD (Compteurs et Listes)
        // =========================================================
        public int GetNombreClients() { return CountTable("Client"); }
        public int GetNombreProduits() { return CountTable("Produit"); }
        public int GetNombreProspects() { return CountTable("Prospect"); }
        public int GetNombreRendezVous() { return CountTable("RendezVous"); }

        private int CountTable(string tableName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }

        public List<Prospect> GetDerniersProspects()
        {
            List<Prospect> liste = new List<Prospect>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Nom, Prenom, Entreprise FROM Prospect ORDER BY IdProspect DESC LIMIT 10";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Prospect
                            {
                                Nom = reader.GetString("Nom"),
                                Prenom = reader.GetString("Prenom"),
                                Entreprise = reader.IsDBNull(reader.GetOrdinal("Entreprise")) ? "Particulier" : reader.GetString("Entreprise")
                            });
                        }
                    }
                }
                catch { }
            }
            return liste;
        }

        public List<RendezVous> GetProchainsRendezVous()
        {
            List<RendezVous> liste = new List<RendezVous>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT IdRdv, Date_Heure, Lieu, IdClient FROM RendezVous ORDER BY Date_Heure ASC LIMIT 20";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new RendezVous
                            {
                                Id = reader.GetInt32("IdRdv"),
                                DateHeure = reader.GetDateTime("Date_Heure"),
                                Lieu = reader.GetString("Lieu"),
                                IdClient = reader.IsDBNull(reader.GetOrdinal("IdClient")) ? 0 : reader.GetInt32("IdClient")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur RDV Dashboard : " + ex.Message); }
            }
            return liste;
        }

        public List<Produit> GetProduitsDashboard()
        {
            List<Produit> liste = new List<Produit>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Nom, Prix, Stock FROM Produit ORDER BY Stock ASC LIMIT 20";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Produit
                            {
                                Nom = reader.GetString("Nom"),

                                // ✅ CORRECTION DU BUG DES PRIX (Dashboard)
                                Prix = Convert.ToDecimal(reader["Prix"].ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture),

                                Stock = reader.GetInt32("Stock")
                            });
                        }
                    }
                }
                catch { }
            }
            return liste;
        }

        public int[] GetStatsClientsParMois()
        {
            int[] stats = new int[6];
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT MONTH(DateCreation) as Mois, COUNT(*) as Total 
                             FROM Client 
                             WHERE YEAR(DateCreation) = 2025 
                             GROUP BY Mois";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int mois = Convert.ToInt32(reader["Mois"]);
                            if (mois >= 1 && mois <= 6) stats[mois - 1] = Convert.ToInt32(reader["Total"]);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur Stats Clients : " + ex.Message); }
            }
            return stats;
        }


        // =========================================================
        // 7. GESTION DES FACTURES (Avec Transaction et maj des stocks)
        // =========================================================
        public void AjouterFacture(Facture f)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Démarrage de la TRANSACTION SQL
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insérer l'en-tête de la facture
                        string queryFacture = "INSERT INTO Facture (IdClient, DateFacture, TotalTTC) VALUES (@idClient, @date, @total)";
                        MySqlCommand cmdFacture = new MySqlCommand(queryFacture, connection, transaction);
                        cmdFacture.Parameters.AddWithValue("@idClient", f.IdClient);
                        cmdFacture.Parameters.AddWithValue("@date", f.DateFacture);
                        cmdFacture.Parameters.AddWithValue("@total", f.TotalTTC);
                        cmdFacture.ExecuteNonQuery();

                        // Récupérer le numéro de la facture générée
                        f.IdFacture = (int)cmdFacture.LastInsertedId;

                        // 2. Insérer chaque produit (les lignes) et baisser le stock
                        foreach (var ligne in f.Lignes)
                        {
                            // A. Créer la ligne de facture
                            string queryLigne = "INSERT INTO LigneFacture (IdFacture, IdProduit, Quantite, PrixUnitaire) VALUES (@idFac, @idProd, @qte, @prix)";
                            MySqlCommand cmdLigne = new MySqlCommand(queryLigne, connection, transaction);
                            cmdLigne.Parameters.AddWithValue("@idFac", f.IdFacture);
                            cmdLigne.Parameters.AddWithValue("@idProd", ligne.IdProduit);
                            cmdLigne.Parameters.AddWithValue("@qte", ligne.Quantite);
                            cmdLigne.Parameters.AddWithValue("@prix", ligne.PrixUnitaire);
                            cmdLigne.ExecuteNonQuery();

                            // B. Déduire la quantité du stock du produit
                            string queryStock = "UPDATE Produit SET Stock = Stock - @qte WHERE Id = @idProd";
                            MySqlCommand cmdStock = new MySqlCommand(queryStock, connection, transaction);
                            cmdStock.Parameters.AddWithValue("@qte", ligne.Quantite);
                            cmdStock.Parameters.AddWithValue("@idProd", ligne.IdProduit);
                            cmdStock.ExecuteNonQuery();
                        }

                        // 3. Si aucune erreur, on VALIDE tout définitivement en base de données
                        transaction.Commit();

                        // 4. ✅ AJOUT AUDIT : On trace la création de la facture
                        AjouterLog("INSERT", "Facture", f.IdFacture.ToString(), null, f);
                    }
                    catch (Exception ex)
                    {
                        // En cas de bug (ex: perte de connexion), on ANNULE tout pour protéger la base
                        transaction.Rollback();
                        MessageBox.Show("Erreur lors de la création de la facture : " + ex.Message, "Erreur Critique", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // 🟢 NOUVELLE MÉTHODE : Récupérer l'historique des factures d'un client
        public List<Facture> GetFacturesClient(int idClient)
        {
            List<Facture> liste = new List<Facture>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // On récupère les factures du plus récent au plus ancien
                    string query = "SELECT IdFacture, DateFacture, TotalTTC FROM Facture WHERE IdClient = @id ORDER BY DateFacture DESC";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", idClient);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new Facture
                            {
                                IdFacture = reader.GetInt32("IdFacture"),
                                DateFacture = reader.GetDateTime("DateFacture"),
                                TotalTTC = reader.GetDecimal("TotalTTC")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur Historique : " + ex.Message); }
            }
            return liste;
        }
        // 🟢 NOUVELLE MÉTHODE : Récupérer le détail (les produits) d'une ancienne facture
        public List<LigneFacture> GetLignesFacture(int idFacture)
        {
            List<LigneFacture> liste = new List<LigneFacture>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"SELECT lf.IdLigne, lf.Quantite, lf.PrixUnitaire, p.Nom as NomProduit 
                                     FROM LigneFacture lf
                                     INNER JOIN Produit p ON lf.IdProduit = p.Id
                                     WHERE lf.IdFacture = @idFac";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@idFac", idFacture);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            liste.Add(new LigneFacture
                            {
                                IdLigne = reader.GetInt32("IdLigne"),
                                NomProduit = reader.GetString("NomProduit"),
                                Quantite = reader.GetInt32("Quantite"),
                                PrixUnitaire = reader.GetDecimal("PrixUnitaire")
                            });
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erreur Détail Facture : " + ex.Message); }
            }
            return liste;
        }

    }


}