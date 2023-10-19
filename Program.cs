using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Gestion_de_Vente_des_Cartes_d_Abonnement
{
    public static class Program
    {
        public static SqlConnection con = new SqlConnection("server=DESKTOP-7SCK97D;database=GestionVenteCarteAbonnement;integrated security=true;");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
