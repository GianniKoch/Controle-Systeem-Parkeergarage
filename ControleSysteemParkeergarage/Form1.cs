/*
 *  GIP - Gianni Koch - Jens Penneman - Miniatuur Parkeergarage
 *  Dit programma maakt een verbinding met en geeft een overzicht van onze parkeergarage.
 *  Het project kan u ook via github forken / downloaden : https://github.com/GianniKoch/Controle-Systeem-Parkeergarage
 */

//Bibliotheken (zie klassen in programma)
using System;
using System.Windows.Forms;
using Newtonsoft.Json; //JSON.
using System.Net.Http; // HTTP Request.
using ControleSysteemParkeergarage.Models; //JSON Models.
using System.Collections.Generic; //List<>
using System.Threading; //Threads

namespace ControleSysteemParkeergarage
{
    public partial class frmControle : Form
    {
        //Globale variabelen.
        private string getRequest = "";
        private Business _business;
        private JSONBericht _vorigBericht;

        public frmControle()
        {
            InitializeComponent();
            
        }

        private void frmControle_Load(object sender, EventArgs e)
        {
            //Klasse 3 lagen met connectie gegevens voor de database logging.
            _business = new Business(new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text});

            //Verbinding checken.
            label43.Text = _business.ConnectionOpen ? "Verbonden" : "Niet verbonden";

            //Thread die de connectie over IP regelt.
            Thread th = getConnectionThread();
            th.Start(); // Thread starten.

            //Thread die het gedrag van de checkboxes nakijkt.
            startEventTimerThread();
        }

        private void startEventTimerThread()
        {
            //Thread aanmaken met de lambda expresie (() => //code...)
            new Thread(() =>
            {
                //Begin variabele met vorige toestanden van alle knoppen op false.
                bool[] blnArray = new bool[30];
                for (int teller = 0; teller < 30; teller++)
                {
                    blnArray[teller] = false;
                }

                //While true lus die het gedrage van alle knoppen opvangt en verwerkt.
                while (true)
                {
                    for (int teller = 0; teller < 30; teller++)
                    {
                        //Knop in een variabele steken. ( Kans op meerdere resultaten, vandaar de array)
                        Control[] controls = Controls.Find("cbParkeerplaats" + (teller + 1), true);
                        foreach (Control ctl in controls)
                        {
                            //Controleren of "control" een checkbox is en eerst checken met een if-statement om een exception te voorkomen.
                            if (ctl is CheckBox)
                            {
                                CheckBox cb = (CheckBox)ctl;
                                if (cb.Checked != blnArray[teller]) //Als het gedrag verandert:
                                {
                                    //Getrequest updaten.
                                    cbClickedEvent(teller + 1, cb.Checked);
                                }
                                //Huidige waarde bewaren voor de volgende lus. (flank detectie)
                                blnArray[teller] = cb.Checked;
                            }
                        }
                    }
                }
                //Thread starten.
            }).Start();
        }

        private void cbClickedEvent(int intParkeerplaats, bool blnValue)
        {
            //Getrequest updaten met ',' als splitter. (bv.: 1:true,2:false,3:true,)
            getRequest += intParkeerplaats + ":" + blnValue + ",";
        }

        private Thread getConnectionThread()
        {
            //Terug sturen thread, 'async' als keyword gebruiken om de functie asynchroon te laten lopen met de thread d.m.v. "await" keyword. (De connectie kan niet direct worden terug gestuurd.)
            return new Thread(async () =>
            {
                while (true)
                {
                    // 1/5 thread wachten.
                    Thread.Sleep(200);

                    //Client init.
                    HttpClient client = new HttpClient();
                    try
                    {
                        //Variabele om GetRequest te verwerken.
                        string get = "";
                        string tempReq = getRequest;
                        if (getRequest != "")// .Length -1 controleren. (buiten de index waarden als index 0 is => -1 => Error)
                        {
                            get = "(" + tempReq.Remove(tempReq.Length - 1) + ")"; //Haakjes worden in het arduino programma gebruikt om de waarden makkelijker uit de url te halen.
                        }

                        //URL met de variabele IP met als toevoeging /..getRequest..
                        String strUrl = $"http://{txtIP.Text}/{get}";

                        //BeginInvoke om cross-Threading toe te laten.
                        BeginInvoke((MethodInvoker)(() => textBox2.Text = strUrl));

                        //Antwoord van de client asynchroon ontvangen. (Thread wacht op het antwoord.)
                        HttpResponseMessage response = await client.GetAsync(strUrl);

                        //Variabele resetten.
                        getRequest = "";
                        get = "";

                        //Result inlezen.
                        string strResult = await response.Content.ReadAsStringAsync();

                        //Request verder afhandelen om lange functies te voorkomen.
                        handleResult(strResult);
                    }
                    catch (Exception) { }
                }
            });
        }

        private void handleResult(string strResult)
        {
            //JSONBericht init.
            List<JSONBericht> berichten;
            try
            {
                //JSONData inlezen en omzetten in een klasse structuur via de models. (Opnieuw kans op meerdere berichten, vandaar de list)
                berichten = getjsonData<List<JSONBericht>>(strResult);
                foreach(JSONBericht bericht in berichten)
                {
                    //Verder afhandelen bericht.
                    UpdateParkeerplaatsen(bericht);
                    CheckLogging(bericht);

                    //Nodig voor de logging.
                    _vorigBericht = bericht;
                }
            }
            catch (Exception) { }

            //JSON bericht in textbox weergeven voor debugging.
            BeginInvoke((MethodInvoker)(() => textBox1.Text = strResult));
        }

        private T getjsonData<T>(string jsonData) where T : new()
        {
            //Return de JSON klasse structuur met DeserializeObject<T>
            return string.IsNullOrEmpty(jsonData) ? new T() : JsonConvert.DeserializeObject<T>(jsonData);
        }

        private void UpdateParkeerplaatsen(JSONBericht bericht)
        {
            //Elke parkeerplaats nakijken.
            foreach (Parkeerplaatsen parkeerplaats in bericht.ParkeerPlaatsen)
            {
                //Bezet of niet bezet controleren.
                foreach (Control ctr in Controls.Find("Parkeerplaats" + parkeerplaats.id, true))
                {
                    BeginInvoke((MethodInvoker)(() => ctr.Visible = parkeerplaats.B));
                }
                //Plek voor invaliden controleren met de checkbox blauw of default te zetten.
                foreach(Control ctr in Controls.Find("cbParkeerplaats" + parkeerplaats.id, true))
                {
                    BeginInvoke((MethodInvoker)(() => ctr.BackColor = parkeerplaats.H ? System.Drawing.Color.FromArgb(128, 128, 255) : DefaultBackColor));
                }
            }
        }

        private void btnHerlaadConnectieGegevens_Click(object sender, EventArgs e)
        {
            //Nieuwe business object maken met geüpdatete gegevens.
            _business = new Business(new string[] { txtServer.Text, txtUser.Text, txtPassword.Text, txtDatabase.Text });

            //Verbinding checken.
            label43.Text = _business.ConnectionOpen ? "Verbonden" : "Niet verbonden";
        }

        private void CheckLogging(JSONBericht huidigBericht)
        {
            //Controleren via "vorig bericht" of er een ingaande auto is.
            if (huidigBericht.AantalAutosInParkeerplaats > _vorigBericht.AantalAutosInParkeerplaats)
                _business.log(Persistance.logType.AutoIngaand, "Ingaande auto. Totaal: " + 
                    huidigBericht.AantalAutosInParkeerplaats, DateTime.Now);

            //Controleren of er een uitgaande auto is.
            if (huidigBericht.AantalAutosInParkeerplaats < _vorigBericht.AantalAutosInParkeerplaats)
                _business.log(Persistance.logType.AutoUitgaand, "Uitgaande auto. Totaal: " + 
                    huidigBericht.AantalAutosInParkeerplaats, DateTime.Now);

            //Controleren of de Parkeergarage vol is.
            if (huidigBericht.AantalAutosInParkeerplaats >= 24)
                _business.log(Persistance.logType.ParkeergarageVol, "Parkeergarage is vol.", DateTime.Now); 
            
            //Controleren of de parkeergarage leeg is.
            if (huidigBericht.AantalAutosInParkeerplaats <= 0)
                _business.log(Persistance.logType.ParkeergarageVol, "Parkeergarage is leeg.", DateTime.Now);

            //Controleren of de parkeerplaats verandert van gedrag.
            foreach (var plaats in huidigBericht.ParkeerPlaatsen)
            {
                if (!plaats.B.Equals(Parkeerplaatsen.getParkeerplaatsFromId(_vorigBericht.ParkeerPlaatsen, plaats.id).B))
                    _business.log(plaats.B ? Persistance.logType.ParkeerplaatsBezet : Persistance.logType.ParkeerplaatsVrij,
                        $"Parkeerplaats {plaats.id} is " + (plaats.B ? "bezet." : "vrij."), DateTime.Now);
            }
        }
    }
}
