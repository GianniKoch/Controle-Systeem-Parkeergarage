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

        public frmControle()
        {
            InitializeComponent();
            
        }

        private void frmControle_Load(object sender, EventArgs e)
        {
            _business = new Business()
            {
                MySqlSettings = new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text }
            };
            Thread th = getConnectionThread();
            th.Start();
            startEventTimerThread();
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
                        var a = _business.log(Persistance.logType.ConnectionSucces, "test", DateTime.Now);
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
                    UpdateParkeerplaatsen(bericht);
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
            _business = new Business()
            {
                MySqlSettings = new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text }
            };
        }
    }
}
