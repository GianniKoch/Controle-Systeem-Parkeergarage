using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Http;
using ControleSysteemParkeergarage.Models;
using System.Collections.Generic;
using System.Threading;

namespace ControleSysteemParkeergarage
{
    public partial class frmControle : Form
    {
        private string getRequest = "";
        private Business _business;
        private JSONBericht _vorigBericht;

        public frmControle()
        {
            InitializeComponent();
            
        }

        private void frmControle_Load(object sender, EventArgs e)
        {
            _business = new Business(new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text});
            Thread th = getConnectionThread();
            th.Start();
            startEventTimerThread();
            MessageBox.Show(_business.ConnectionOpen.ToString());
        }

        private void startEventTimerThread()
        {
            new Thread(() =>
            {
                bool[] blnArray = new bool[30];
                for (int teller = 0; teller < 30; teller++)
                {
                    blnArray[teller] = false;
                }
                while (true)
                {
                    for (int teller = 0; teller < 30; teller++)
                    {
                        Control[] controls = Controls.Find("cbParkeerplaats" + (teller + 1), true);
                        foreach (Control ctl in controls)
                        {
                            if (ctl is CheckBox)
                            {
                                CheckBox cb = (CheckBox)ctl;
                                if (cb.Checked != blnArray[teller])
                                {
                                    cbClickedEvent(teller + 1, cb.Checked);
                                }
                                blnArray[teller] = cb.Checked;
                            }
                        }
                    }
                }
            }).Start();
        }

        private void cbClickedEvent(int intParkeerplaats, bool blnValue)
        {
            getRequest += intParkeerplaats + ":" + blnValue + ",";
        }

        private Thread getConnectionThread()
        {
            return new Thread(async () =>
            {
                while (true)
                {
                    Thread.Sleep(200);
                    HttpClient client = new HttpClient();
                    try
                    {
                        string get = "";
                        string tempReq = getRequest;
                        if (getRequest != "")
                        {
                            get = "(" + tempReq.Remove(tempReq.Length - 1) + ")";
                        }
                        String strUrl = "http://192.168.0.177/" + get;
                        BeginInvoke((MethodInvoker)(() => textBox2.Text = strUrl));
                        HttpResponseMessage response = await client.GetAsync(strUrl);
                        getRequest = "";
                        get = "";
                        string strResult = await response.Content.ReadAsStringAsync();
                        handleResult(strResult);
                    }
                    catch (Exception) { }
                }
            });
        }

        private void handleResult(string strResult)
        {
            List<JSONBericht> berichten;
            try
            {
                berichten = getjsonData<List<JSONBericht>>(strResult);
                foreach(JSONBericht bericht in berichten)
                {
                    UpdateParkeerplaatsen(bericht);
                    CheckLogging(bericht);
                    _vorigBericht = bericht;
                }
            }
            catch (Exception) { }

            BeginInvoke((MethodInvoker)(() => textBox1.Text = strResult));
        }

        private T getjsonData<T>(string jsonData) where T : new()
        {
            return string.IsNullOrEmpty(jsonData) ? new T() : JsonConvert.DeserializeObject<T>(jsonData);
        }

        private void UpdateParkeerplaatsen(JSONBericht bericht)
        {
            foreach (Parkeerplaatsen parkeerplaats in bericht.ParkeerPlaatsen)
            {
                foreach (Control ctr in Controls.Find("Parkeerplaats" + parkeerplaats.id, true))
                {
                    BeginInvoke((MethodInvoker)(() => ctr.Visible = parkeerplaats.B));
                }
                foreach(Control ctr in Controls.Find("cbParkeerplaats" + parkeerplaats.id, true))
                {
                    BeginInvoke((MethodInvoker)(() => ctr.BackColor = parkeerplaats.H ? System.Drawing.Color.FromArgb(128, 128, 255) : DefaultBackColor));
                }
            }
        }

        private void btnHerlaadConnectieGegevens_Click(object sender, EventArgs e)
        {
            _business = new Business(new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text });
        }

        private void CheckLogging(JSONBericht huidigBericht)
        {
            if (huidigBericht.AantalAutosInParkeerplaats > _vorigBericht.AantalAutosInParkeerplaats)
                _business.log(Persistance.logType.AutoIngaand, "Ingaande auto. Totaal: " + huidigBericht.AantalAutosInParkeerplaats, DateTime.Now);
            if (huidigBericht.AantalAutosInParkeerplaats < _vorigBericht.AantalAutosInParkeerplaats)
                _business.log(Persistance.logType.AutoUitgaand, "Uitgaande auto. Totaal: " + huidigBericht.AantalAutosInParkeerplaats, DateTime.Now);
            if (huidigBericht.AantalAutosInParkeerplaats >= 24)
                _business.log(Persistance.logType.ParkeergarageVol, "Parkeergarage is vol.", DateTime.Now); 
            if (huidigBericht.AantalAutosInParkeerplaats <= 0)
                _business.log(Persistance.logType.ParkeergarageVol, "Parkeergarage is leeg.", DateTime.Now);
            foreach (var plaats in huidigBericht.ParkeerPlaatsen)
            {
                if (!plaats.B.Equals(Parkeerplaatsen.getParkeerplaatsFromId(_vorigBericht.ParkeerPlaatsen, plaats.id).B))
                    _business.log(plaats.B ? Persistance.logType.ParkeerplaatsBezet : Persistance.logType.ParkeerplaatsVrij, $"Parkeerplaats {plaats.id} is " + (plaats.B ? "bezet." : "vrij."), DateTime.Now);
            }
        }
    }
}
