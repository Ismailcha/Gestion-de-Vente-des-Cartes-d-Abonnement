using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using SautinSoft.Document;
using System.IO;
using System.Text.RegularExpressions;

namespace Gestion_de_Vente_des_Cartes_d_Abonnement
{
    public partial class Form1 : Form
    {
        void chargerClient()
        {
            SqlCommand cmd = new SqlCommand("select codeCarte,Nom,Prenom,numCIN,TypeCarte,dateExpiration from client inner join carte on client.codeClient = carte.codeClient",Program.con);
            DataTable dt = new DataTable();
            Program.con.Open();
            dt.Load(cmd.ExecuteReader());
            Program.con.Close();
            dgvClients.DataSource = dt;
            txtRechercheCodeCarte.Text = txtRechercheCIN.Text = txtRechercheNom.Text = "";
        }
        public static void FindAndReplace(string nom, string prenom, string numCarte, string trajet, string date, string type)
        {
            string loadPath = @"..\..\images\carte d'abonnement.docx";
            if(type == "reduction")
            {
                loadPath = @"..\..\images\carte de reduction.docx";
            }

            // Load a document into DocumentCore.
            DocumentCore dc = DocumentCore.Load(loadPath);

            Regex regNom = new Regex(@"nomdb");
            Regex regPrenom = new Regex(@"predb");
            Regex regTrajet = new Regex(@"Trajet");
            Regex regNumCarte = new Regex(@"nc");
            Regex regDateExp = new Regex(@"Date d'expiration");

            //Find the word and Replace it
            //Reverse() makes sure that action replace not affects to Find().
            foreach (ContentRange item in dc.Content.Find(regNom).Reverse())
            {
                item.Replace(nom);
            }
            foreach (ContentRange item in dc.Content.Find(regPrenom).Reverse())
            {
                item.Replace(prenom);
            }
            foreach (ContentRange item in dc.Content.Find(regTrajet).Reverse())
            {
                item.Replace(trajet);
            }
            foreach (ContentRange item in dc.Content.Find(regNumCarte).Reverse())
            {
                item.Replace(numCarte);
            }
            foreach (ContentRange item in dc.Content.Find(regDateExp).Reverse())
            {
                item.Replace(date);
            }

            // Save our document into PDF format.
            string savePath = @"..\..\test.pdf";
            dc.Save(savePath, new PdfSaveOptions());
        }
        string TypeCarte;
        List<Panel> Panels = new List<Panel>();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPrecedent_Click(object sender, EventArgs e)
        {
            Panels[1].BringToFront();
        }

        private void btnConfirmerPaiement_Click(object sender, EventArgs e)
        {
            string nomClt = txtNomClient.Text;
            string prenomClt = txtPrenomClient.Text;
            string trajetClt ="";
            if (rbAbonnement.Checked)
            {
                trajetClt = cmbGareDepart.GetItemText(cmbGareDepart.SelectedItem) + "-" + cmbGareArrivee.GetItemText(cmbGareArrivee.SelectedItem);
            }
            string numCarteClt = "";
            string dateExpiration = "";
            string typeClt = TypeCarte;
            if (rbCarteBancaire.Checked)
            {
                if(txtNomCarte.Text != "" && txtPrenomCarte.Text != "" 
                   && txtCCV.Text != "" && txtNumCarteBancaire.Text != ""
                   && cmbMoisExp.SelectedIndex != -1 && cmbAnneeExp.SelectedIndex != -1
                   && cmbTypeCarte.SelectedIndex != -1)
                {
                    if(TypeCarte != "reduction")
                    {
                        // ------ Sql commande passe (Code Ajout Client)
                        SqlCommand cmdClient = new SqlCommand("sp_add_Client", Program.con);
                        SqlCommand cmdCard = new SqlCommand("sp_add_card", Program.con);

                        cmdClient.CommandType = CommandType.StoredProcedure;
                        cmdCard.CommandType = CommandType.StoredProcedure;

                        cmdClient.Parameters.AddWithValue("@nom", txtNomClient.Text);
                        cmdClient.Parameters.AddWithValue("@prenom", txtPrenomClient.Text);
                        cmdClient.Parameters.AddWithValue("@dateNaiss", DateNaissClient.Value);
                        cmdClient.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);

                        cmdCard.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);
                        cmdCard.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmdCard.Parameters.AddWithValue("@typeCarte", TypeCarte);
                        cmdCard.Parameters.AddWithValue("@gareDepart", cmbGareDepart.GetItemText(cmbGareDepart.SelectedItem));
                        cmdCard.Parameters.AddWithValue("@gareArrivee", cmbGareArrivee.GetItemText(cmbGareArrivee.SelectedItem));
                        Program.con.Open();
                        cmdClient.ExecuteNonQuery();
                        cmdCard.ExecuteNonQuery();
                        Program.con.Close();

                        MessageBox.Show("Client Ajoute avec Succes!", "ajout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dateExpiration = DateTime.Today.AddMonths(1).ToString();
                        FindAndReplace(nomClt, prenomClt, numCarteClt, trajetClt, dateExpiration, typeClt);
                        // vider les zones de texte
                        txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
                        cmbMoisExp.Text = "--Month--";
                        cmbAnneeExp.Text = "--Year--";
                        rbCarteBancaire.Checked = true;
                        txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
                        DateTime today = DateTime.Today;
                        DateNaissClient.Value = today;
                        cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
                        rbAbonnement.Checked = true;
                        Panels[1].BringToFront();
                    }
                    else
                    {
                        // ------ Sql commande passe (Code Ajout Client)
                        SqlCommand cmdClient = new SqlCommand("sp_add_Client", Program.con);
                        SqlCommand cmdCard = new SqlCommand("sp_add_card", Program.con);

                        cmdClient.CommandType = CommandType.StoredProcedure;
                        cmdCard.CommandType = CommandType.StoredProcedure;

                        cmdClient.Parameters.AddWithValue("@nom", txtNomClient.Text);
                        cmdClient.Parameters.AddWithValue("@prenom", txtPrenomClient.Text);
                        cmdClient.Parameters.AddWithValue("@dateNaiss", DateNaissClient.Value);
                        cmdClient.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);

                        cmdCard.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);
                        cmdCard.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmdCard.Parameters.AddWithValue("@typeCarte", TypeCarte);

                        Program.con.Open();
                        cmdClient.ExecuteNonQuery();
                        cmdCard.ExecuteNonQuery();
                        Program.con.Close();

                        MessageBox.Show("Client Ajoute avec Succes!", "ajout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dateExpiration = DateTime.Today.AddMonths(1).ToString();
                        FindAndReplace(nomClt, prenomClt, numCarteClt, trajetClt, dateExpiration, typeClt);
                        //Vider les zones de texte
                        txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
                        cmbMoisExp.Text = "--Month--";
                        cmbAnneeExp.Text = "--Year--";
                        rbCarteBancaire.Checked = true;
                        txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
                        DateTime today = DateTime.Today;
                        DateNaissClient.Value = today;
                        cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
                        rbAbonnement.Checked = true;
                        Panels[1].BringToFront();
                    }
                }
                else
                {
                    MessageBox.Show("Un ou Plusieurs Champs sont Vides!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if(TypeCarte == "reduction")
                {
                    // ------ Sql commande passe (Code Ajout Client)
                    SqlCommand cmdClient = new SqlCommand("sp_add_Client", Program.con);
                    SqlCommand cmdCard = new SqlCommand("sp_add_card", Program.con);

                    cmdClient.CommandType = CommandType.StoredProcedure;
                    cmdCard.CommandType = CommandType.StoredProcedure;

                    cmdClient.Parameters.AddWithValue("@nom", txtNomClient.Text);
                    cmdClient.Parameters.AddWithValue("@prenom", txtPrenomClient.Text);
                    cmdClient.Parameters.AddWithValue("@dateNaiss", DateNaissClient.Value);
                    cmdClient.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);

                    cmdCard.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);
                    cmdCard.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmdCard.Parameters.AddWithValue("@typeCarte", TypeCarte);

                    Program.con.Open();
                    cmdClient.ExecuteNonQuery();
                    cmdCard.ExecuteNonQuery();
                    Program.con.Close();

                    MessageBox.Show("Client Ajoute avec Succes!", "ajout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dateExpiration = DateTime.Today.AddMonths(1).ToString();
                    FindAndReplace(nomClt, prenomClt, numCarteClt, trajetClt, dateExpiration, typeClt);
                    //vider les zones de texte
                    txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
                    cmbMoisExp.Text = "--Month--";
                    cmbAnneeExp.Text = "--Year--";
                    rbCarteBancaire.Checked = true;
                    txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
                    DateTime today = DateTime.Today;
                    DateNaissClient.Value = today;
                    cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
                    rbAbonnement.Checked = true;
                    Panels[1].BringToFront();
                }
                else
                {
                    // ------ Sql commande passe (Code Ajout Client)
                    SqlCommand cmdClient = new SqlCommand("sp_add_Client", Program.con);
                    SqlCommand cmdCard = new SqlCommand("sp_add_card", Program.con);

                    cmdClient.CommandType = CommandType.StoredProcedure;
                    cmdCard.CommandType = CommandType.StoredProcedure;

                    cmdClient.Parameters.AddWithValue("@nom", txtNomClient.Text);
                    cmdClient.Parameters.AddWithValue("@prenom", txtPrenomClient.Text);
                    cmdClient.Parameters.AddWithValue("@dateNaiss", DateNaissClient.Value);
                    cmdClient.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);

                    cmdCard.Parameters.AddWithValue("@numCIN", txtCIN_NumE.Text);
                    cmdCard.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmdCard.Parameters.AddWithValue("@typeCarte", TypeCarte);
                    cmdCard.Parameters.AddWithValue("@gareDepart", cmbGareDepart.GetItemText(cmbGareDepart.SelectedItem));
                    cmdCard.Parameters.AddWithValue("@gareArrivee", cmbGareArrivee.GetItemText(cmbGareArrivee.SelectedItem));
                    Program.con.Open();
                    cmdClient.ExecuteNonQuery();
                    cmdCard.ExecuteNonQuery();
                    Program.con.Close();

                    MessageBox.Show("Client Ajoute avec Succes!", "ajout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dateExpiration = DateTime.Today.AddMonths(1).ToString();
                    FindAndReplace(nomClt, prenomClt, numCarteClt, trajetClt, dateExpiration, typeClt);
                    //vider les zones de texte
                    txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
                    cmbMoisExp.Text = "--Month--";
                    cmbAnneeExp.Text = "--Year--";
                    rbCarteBancaire.Checked = true;
                    txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
                    DateTime today = DateTime.Today;
                    DateNaissClient.Value = today;
                    cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
                    rbAbonnement.Checked = true;
                    Panels[1].BringToFront();
                }
            }
        }

        private void btnAnnulerPaiement_Click(object sender, EventArgs e)
        {
            txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
            cmbMoisExp.Text = "--Month--";
            cmbAnneeExp.Text = "--Year--";
            rbCarteBancaire.Checked = true;
        }

        private void btnContinuer_Click(object sender, EventArgs e)
        {
            if (rbAbonnement.Checked)
            {
                if (txtNomClient.Text != "" && txtPrenomClient.Text != ""
                && txtCIN_NumE.Text != "" && cmbGareDepart.Text != "--Select--"
                && cmbGareArrivee.Text != "--Select--"
                && cmbGareArrivee.SelectedIndex != cmbGareDepart.SelectedIndex
                && DateNaissClient.Value != DateTime.Today)
                {
                    TypeCarte = "abonnement";
                    groupBox2.Enabled = true;
                    prix.Text = Math.Abs(cmbGareArrivee.SelectedIndex - cmbGareDepart.SelectedIndex) * 180 * 0.6 + ".00";
                    if ((DateTime.Today - DateNaissClient.Value).TotalDays < 10221)
                    {
                        prix.Text = Math.Abs(cmbGareArrivee.SelectedIndex - cmbGareDepart.SelectedIndex) * 180 * 0.3 + ".00";
                    }
                    Panels[2].BringToFront();
                }
                else
                {
                    MessageBox.Show("Veuillez verifier les donnees saisis", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (txtNomClient.Text != "" && txtPrenomClient.Text != ""
                && txtCIN_NumE.Text != ""
                && DateNaissClient.Value != DateTime.Today)
                {
                    Panels[2].BringToFront();
                }
                else
                {
                    MessageBox.Show("Veuillez verifier les donnees saisis", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnAnnulerClient_Click(object sender, EventArgs e)
        {
            txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
            DateTime today = DateTime.Today;
            DateNaissClient.Value = today;
            cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
            rbAbonnement.Checked = true;
        }

        private void btnConnecter_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text != "" && txtMotPasse.Text != "")
            {
                SqlCommand cmd = new SqlCommand("sp_verify_user",Program.con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@motPasse", txtMotPasse.Text);
                var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                Program.con.Open();
                cmd.ExecuteNonQuery();
                int result = (int)returnParameter.Value;
                Program.con.Close();
                if (result== 1)
                {
                    SqlCommand com = new SqlCommand("sp_get_user",Program.con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@email",txtEmail.Text);
                    Program.con.Open();
                    var employe = com.ExecuteScalar().ToString();
                    Program.con.Close();
                    Panels[4].BringToFront();
                    lblUserClient.Text = lblUserPaiement.Text = lblUserMenu.Text = lblUserRerchercher.Text = employe;
                    incorrect.Visible = false;
                }
                else
                {
                    incorrect.Visible = true;
                    txtEmail.Text = txtMotPasse.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Un ou Plusieurs Champs sont Vides!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnnulerEmploye_Click(object sender, EventArgs e)
        {
            txtEmail.Text = txtMotPasse.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Panels.Add(PanelEmploye);
            Panels.Add(PanelClient);
            Panels.Add(PanelPaiement);
            Panels.Add(PanelAccountCreation);
            Panels.Add(PanelMenu);
            Panels.Add(PanelRecherche);
            Panels[0].BringToFront();
            rbAbonnement.Checked = true;
            rbCarteBancaire.Checked = true;
            DateNaissClient.MaxDate = DateTime.Today;
            DateNaissClient.MinDate = new DateTime(1900, 01, 01);
            DateNaissClient.Value = DateTime.Today;
        }

        private void rbEspece_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEspece.Checked)
            {
                groupBox4.Enabled = false;
            }
            else
            {
                groupBox4.Enabled = true;
            }
        }

        private void rbReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (rbReduction.Checked)
            {
                TypeCarte = "reduction";
                groupBox2.Enabled = false;
                prix.Text = "399.99";
            }
            else
            {
                TypeCarte = "abonnement";
                groupBox2.Enabled = true;
                prix.Text = Math.Abs(cmbGareArrivee.SelectedIndex - cmbGareDepart.SelectedIndex) * 180 * 0.6 + ".00";
                if ((DateTime.Today - DateNaissClient.Value).TotalDays < 10221)
                {
                    prix.Text = Math.Abs(cmbGareArrivee.SelectedIndex - cmbGareDepart.SelectedIndex) * 180 * 0.3 + ".00";
                }
            }
        }

        private void lnkCreateAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Panels[3].BringToFront();
        }

        private void btnQuitterInscription_Click(object sender, EventArgs e)
        {
            incorrect.Visible = false;
            txtNouveauPrenom.Text = txtNouveauMotPasse.Text = txtNouveauNom.Text = txtConfirmerNouveauMail.Text = txtNouveauMail.Text = txtConfimerNouveauMotPasse.Text = "";
            Panels[0].BringToFront();
        }

        private void btnAnnulerNouveauEmploye_Click(object sender, EventArgs e)
        {
            txtNouveauPrenom.Text = txtNouveauMotPasse.Text = txtNouveauNom.Text = txtConfirmerNouveauMail.Text = txtNouveauMail.Text = txtConfimerNouveauMotPasse.Text = "";
        }

        private void btnAddEmploye_Click(object sender, EventArgs e)
        {
            if(txtNouveauPrenom.Text != "" && txtNouveauMotPasse.Text != "" 
                && txtNouveauNom.Text != "" && txtConfirmerNouveauMail.Text != ""
                && txtNouveauMail.Text != "" && txtConfimerNouveauMotPasse.Text != ""
                && txtNouveauMotPasse.Text == txtConfimerNouveauMotPasse.Text
                && txtNouveauMail.Text == txtConfirmerNouveauMail.Text)
            {
                SqlCommand command = new SqlCommand("sp_verify_email", Program.con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@email", txtNouveauMail.Text);
                int var;
                var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;
                Program.con.Open();
                command.ExecuteNonQuery();
                var = (int)returnParameter.Value;
                Program.con.Close();
                if(var == 1)
                {
                    SqlCommand cmd = new SqlCommand("sp_add_employe",Program.con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nom",txtNouveauNom.Text);
                    cmd.Parameters.AddWithValue("@prenom", txtNouveauPrenom.Text);
                    cmd.Parameters.AddWithValue("@email", txtNouveauMail.Text);
                    cmd.Parameters.AddWithValue("@motPasse", txtNouveauMotPasse.Text);
                    Program.con.Open();
                    cmd.ExecuteNonQuery();
                    Program.con.Close();
                    MessageBox.Show("Employe enregistre avec succes","Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    Panels[0].BringToFront();
                    txtNouveauPrenom.Text = txtNouveauMotPasse.Text = txtNouveauNom.Text = txtConfirmerNouveauMail.Text = txtNouveauMail.Text = txtConfimerNouveauMotPasse.Text = "";
                }
                else
                {
                    MessageBox.Show("Email Deja Existant!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNouveauPrenom.Text = txtNouveauMotPasse.Text = txtNouveauNom.Text = txtConfirmerNouveauMail.Text = txtNouveauMail.Text = txtConfimerNouveauMotPasse.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Veuillez Verifier les donnees saisies!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnAjoutClientMenu_Click(object sender, EventArgs e)
        {
            Panels[1].BringToFront();
        }

        private void btnQuitterRechercher_Click(object sender, EventArgs e)
        {
            txtRechercheCIN.Text = txtRechercheCodeCarte.Text = txtRechercheNom.Text = "";
            Panels[4].BringToFront();
        }

        private void btnAjouterRechercher_Click(object sender, EventArgs e)
        {
            Panels[1].BringToFront();
        }

        private void btnXML_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("select codeCarte,Nom,Prenom,numCIN,TypeCarte,dateExpiration from client inner join carte on client.codeClient = carte.codeClient", Program.con);
            DataTable dt = new DataTable("clients");
            Program.con.Open();
            dt.Load(cmd.ExecuteReader());
            Program.con.Close();
            dt.WriteXml(@"C:\Users\yassi\Desktop\Gestion de Vente des Cartes d'Abonnement\Clients.xml");
        }

        private void btnSupprimerRechercher_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Voulez vous vraiment supprimer ce client?","Warning",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
            if(result == DialogResult.Yes)
            {
                SqlCommand cmd = new SqlCommand("sp_supprimer_Client", Program.con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codeCarte", dgvClients.SelectedCells[0].Value);
                cmd.Parameters.AddWithValue("@numCIN", dgvClients.SelectedCells[3].Value);
                Program.con.Open();
                cmd.ExecuteNonQuery();
                Program.con.Close();
                MessageBox.Show("Client Supprimé avec succees!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                chargerClient();
            }
        }

        private void btnAfficherTout_Click(object sender, EventArgs e)
        {
            chargerClient();
        }

        private void btnRechercherCode_Click(object sender, EventArgs e)
        {
            if(txtRechercheCodeCarte.Text != "")
            {
                SqlCommand cmd = new SqlCommand("sp_rechercher_code", Program.con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codeCarte", int.Parse(txtRechercheCodeCarte.Text));
                DataTable dt = new DataTable();
                Program.con.Open();
                dt.Load(cmd.ExecuteReader());
                Program.con.Close();
                dgvClients.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Veuillez inserer le code de la carte a rechercher!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRechercherCIN_Click(object sender, EventArgs e)
        {
            if (txtRechercheCIN.Text != "")
            {
                SqlCommand cmd = new SqlCommand("sp_rechercher_CIN", Program.con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numCIN", txtRechercheCIN.Text);
                DataTable dt = new DataTable();
                Program.con.Open();
                dt.Load(cmd.ExecuteReader());
                Program.con.Close();
                dgvClients.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Veuillez inserer le CIN a rechercher!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRechercherNom_Click(object sender, EventArgs e)
        {
            if (txtRechercheNom.Text != "")
            {
                SqlCommand cmd = new SqlCommand("sp_rechercher_Nom", Program.con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nom", txtRechercheNom.Text);
                DataTable dt = new DataTable();
                Program.con.Open();
                dt.Load(cmd.ExecuteReader());
                Program.con.Close();
                dgvClients.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Veuillez inserer le nom a rechercher!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRechercherMenu_Click(object sender, EventArgs e)
        {
            chargerClient();
            Panels[5].BringToFront();
        }

        private void btnQuitterClient_Click(object sender, EventArgs e)
        {
            txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
            cmbMoisExp.Text = "--Month--";
            cmbAnneeExp.Text = "--Year--";
            rbCarteBancaire.Checked = true;
            txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
            DateTime today = DateTime.Today;
            DateNaissClient.Value = today;
            cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
            rbAbonnement.Checked = true;
            Panels[4].BringToFront();
        }

        private void btnQuitterPaiement_Click(object sender, EventArgs e)
        {
            txtCCV.Text = txtNumCarteBancaire.Text = txtNomCarte.Text = txtPrenomCarte.Text = cmbTypeCarte.Text = "";
            cmbMoisExp.Text = "--Month--";
            cmbAnneeExp.Text = "--Year--";
            rbCarteBancaire.Checked = true;
            txtNomClient.Text = txtPrenomClient.Text = txtCIN_NumE.Text = "";
            DateTime today = DateTime.Today;
            DateNaissClient.Value = today;
            cmbGareArrivee.Text = cmbGareDepart.Text = "--Select--";
            rbAbonnement.Checked = true;
            Panels[4].BringToFront();
        }
    }
}
