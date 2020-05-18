using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using ControleSysteemParkeergarage.Models;
using System.Collections.Generic;
using System.Threading;

namespace ControleSysteemParkeergarage
{
    public partial class frmControle : Form
    {
        public frmControle()
        {
            InitializeComponent();
        }

        private async void frmControle_Load(object sender, EventArgs e)
        {
            
        }

        private static T DownloadEnDeserializeJsonData<T>(string url) where T : new()
        {
            using (var webClient = new WebClient())
            {
                var jsonData = string.Empty;
                try
                {
                    jsonData = webClient.DownloadString(url);
                }
                catch (Exception) { }

                return string.IsNullOrEmpty(jsonData) ? new T() : JsonConvert.DeserializeObject<T>(jsonData);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<JSONBericht> bericht;
            try
            {
                bericht = DownloadEnDeserializeJsonData<List<JSONBericht>>("https://giannikoch.com/test.json");
                foreach (JSONBericht jsonBericht in bericht)
                {
                    UpdateParkeerplaatsen(jsonBericht);
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void UpdateParkeerplaatsen(JSONBericht bericht)
        {
            foreach (Parkeerplaatsen parkeerplaats in bericht.ParkeerPlaatsen)
            {
                foreach (Control ctr in Controls.Find("Parkeerplaats" + parkeerplaats.id, true))
                {
                    ctr.Visible = parkeerplaats.B;
                }
                foreach(Control ctr in Controls.Find("cbParkeerplaats" + parkeerplaats.id, true))
                {
                    ctr.BackColor = parkeerplaats.H ? System.Drawing.Color.FromArgb(128, 128, 255) : DefaultBackColor;
                }
            }
        }
    }
}
