using System;
using System.Diagnostics;
using System.Windows.Forms;
using MMSysTrayClient.Properties;
using System.Drawing;

using Microsoft.Win32;
using System.Configuration;
using System.IO;

using System.Threading;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace MMSysTrayClient
{
    class ContextMenus
    {
        bool IsAboutLoaded { get; set; }

        public ContextMenuStrip Create()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;



            //  Test Items
            //item = new ToolStripMenuItem();
            //item.Text = "Start Chrome";
            //item.Image = Resources.Google_Chrome_icon;
            //item.Click += StartChrome_Click;
            //menu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Reset Recorder...";
            item.Image = Resources.Google_Chrome_icon;
            item.Click += CloseChrome_Click;
            menu.Items.Add(item);

            //sep = new ToolStripSeparator();
            //menu.Items.Add(sep);

            //item = new ToolStripMenuItem();
            //item.Text = "Set WAVE File...";
            //item.Click += SetWave_Click;
            //menu.Items.Add(item);

            //item = new ToolStripMenuItem();
            //item.Text = "Test Pended Dictation...";
            //item.Click += TestPended_Click;
            //menu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "Test Error Log";
            item.Click += TestErrorLog_Click;
            menu.Items.Add(item);

            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            //  About
            item = new ToolStripMenuItem();
            item.Text = "About";
            item.Click += About_Click;
            item.Image = Resources.About;
            menu.Items.Add(item);

            //  Separator
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            //  Exit
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += Exit_Click;
            item.Image = Resources.Exit;
            menu.Items.Add(item);

            return menu;
        }

        void TestErrorLog_Click(object sender, EventArgs e)
        {
            HostErrorHandler.GetInstance().LogHostError(typeof(System.Net.HttpListenerException).ToString(), "Host Exception", Environment.StackTrace);
        }

        void CloseChrome_Click(object sender, EventArgs e)
        {
            Program._formSmHost.Navigate("about:blank");
            Program._formSmHost.Hide();            
        }

        async void TestPended_Click(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                string ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
                client.BaseAddress = new Uri(ApiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                dynamic reqObject = new ExpandoObject();
                reqObject.Credentials = new ExpandoObject();
                reqObject.Credentials.Platform = 0;  // SuccessEHS
                reqObject.Credentials.PartnerCredentials = new ExpandoObject() as dynamic;
                reqObject.Credentials.PartnerCredentials.PartnerLoginId = "PARTNER_MMODAL";
                reqObject.Credentials.PartnerCredentials.PartnerPassword = "API_PWD";
                reqObject.Credentials.ClientCredentials = new ExpandoObject() as dynamic;
                reqObject.Credentials.ClientCredentials.ClientId = ConfigurationManager.AppSettings["ClientId"];
                reqObject.Credentials.ClientCredentials.LoginId = ConfigurationManager.AppSettings["LoginId"];
                reqObject.Credentials.ClientCredentials.Password = ConfigurationManager.AppSettings["Password"];
                reqObject.RequestObject = null;

                int nJobId = Convert.ToInt32(ConfigurationManager.AppSettings["TestJobId"]);
                string requestUri = String.Format("api/DictationJobs/GetPendingJobContext/{0}", nJobId);
                HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, (ExpandoObject)reqObject);
                if (response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("******Fetch pending job succeeded*****");
                    dynamic dynJob = await response.Content.ReadAsAsync<dynamic>();

                    reqObject.RequestObject = dynJob;

                    MModalRtcHost _rtc = MModalRtcHost.GetInstance();
                    _rtc.SetRequestContextTest(reqObject);
                    _rtc.LaunchSPATest();

                    //requestUri = String.Format("api/DictationJobs/GetPendingJobWave/{0}", nJobId);
                    //response = await client.PostAsJsonAsync(requestUri, (ExpandoObject)reqObject);
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    byte[] arrWave = await response.Content.ReadAsAsync<byte[]>();
                    //}

                    //byte[] arrWave = dynJob.WaveData;

                    //m_arrWaveBytes = dynJob.WaveFileBytes;
                    //m_sVoiceFile = System.IO.Path.GetTempFileName();
                    //File.Delete(m_sVoiceFile);
                    //m_sVoiceFile += ".WAV";

                    //using (FileStream fs = new FileStream(m_sVoiceFile, FileMode.Create))
                    //{
                    //    fs.Write(m_arrWaveBytes, 0, m_arrWaveBytes.Length);
                    //    fs.Close();
                    //}
                }
                else
                {
                    Trace.WriteLine("******Fetch pending job failed ******");
                }
            }            
        }

        void StartChrome_Click(object sender, EventArgs e)
        {
            ////  Let's find Chrome
            //RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe");
            //string sChromePath = key.GetValue("") as string;
            //if (sChromePath == null)
            //    MessageBox.Show("Chrome is NOT installed", "Oy Veh!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //key.Close();

            //Process.Start(sChromePath, String.Format("--new-window --app={0}?group={1}", 
            //    ConfigurationManager.AppSettings["appUrl"], MModalRtcHost.GetInstance().RtcGroup));            

            string sUri = String.Format("{0}?group={1}", ConfigurationManager.AppSettings["appUrl"], MModalRtcHost.GetInstance().RtcGroup);

            Program._formSmHost.Opacity = 100.0;
            Program._formSmHost.Show();
            Program._formSmHost.Navigate(sUri);
        }

        void SetWave_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "WAV Files (*.wav)|*.wav";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    byte[] arrFile = new byte[fs.Length];
                    fs.Read(arrFile, 0, (int)fs.Length);

                    MModalRtcHost _rtc = MModalRtcHost.GetInstance();
                    //_rtc.SendFileData(arrFile);
                }
            }
        }

        void About_Click(object sender, EventArgs e)
        {
            if (!IsAboutLoaded)
            {
                IsAboutLoaded = true;
                new AboutBox().ShowDialog();
                IsAboutLoaded = false;
            }
        }

        void Exit_Click(object sender, EventArgs e)
        {
            MModalRtcHost rtc = MModalRtcHost.GetInstance();
            rtc.DisconnectFromServer();

            Program._formSmHost.Close();

            Application.Exit();
        }
    }
}
