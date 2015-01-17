using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SMAUDIOLib;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Dynamic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MMSysTrayClient
{
    public partial class FormSmHost : Form
    {
        #region Private Fields

        private SmAudioClass _smAudio = null;
        private bool bIsInitialized = false;
        private string m_sVoiceFile = null;
        private byte[] m_arrWaveBytes = null;

        //private ChromiumWebBrowser _chrome;
        private WebBrowser _chrome;

        private System.Threading.Timer stateTimer = null;

        private MModalRtcHost _rtcHost;

        private ExpandoObject _requestContext = null;

/*
 Client Recorder
mmodal-auth-app: Client
mmodal-auth-key: 22BA3601-969A-4FD5-BA74-3BB6ACEDC01B
 */
        private const string MMODAL_AUTH_APP_HEADER = "mmodal-auth-app";
        private const string MMODAL_AUTH_APP_VALUE = "Client";
        private const string MMODAL_AUTH_KEY_HEADER = "mmodal-auth-key";
        private const string MMODAL_AUTH_KEY_VALUE = "22BA3601-969A-4FD5-BA74-3BB6ACEDC01B";
        
        #endregion

        public string ApiUrl { get; set; }

        public bool IsInitialized
        {
            get { return bIsInitialized;  }
            set { bIsInitialized = value; }
        }

        public FormSmHost()
        {
            InitializeComponent();

            //var settings = new CefSharp.CefSettings
            //{
            //    PackLoadingDisabled = false
            //};

            //if (CefSharp.Cef.Initialize(settings))
            //{
            //    _chrome = new ChromiumWebBrowser("about:blank");
            //    _chrome.Dock = DockStyle.Fill;
            //    this.Controls.Add(_chrome);
            //}

            _chrome = new WebBrowser();
            _chrome.Dock = DockStyle.Fill;
            this.Controls.Add(_chrome);
        }

        private void FormSmHost_Load(object sender, EventArgs e)
        {
            try
            {
                Trace.WriteLine("*****FORMSMHOST_LOAD*****");
                
                _rtcHost = MModalRtcHost.GetInstance();

                //_chrome.Load(ApiUrl);

                //InitializeSpeechMike();

                //stateTimer = new System.Threading.Timer(new TimerCallback(TimerCallback), null, 0, 5000);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #region SpeechMagic Initialization/Deinitialization

        internal void InitializeSpeechMike()
        {
            try
            {
                _smAudio = new SmAudioClass();
                _smAudio.Initialize();

                //  Hook Events
                _smAudio.ControlDevice += _smAudio_ControlDevice;
                _smAudio.EOS += _smAudio_EOS;
                _smAudio.Error += _smAudio_Error;
                _smAudio._DSmAudioEvents_Event_RecordMode += _smAudio__DSmAudioEvents_Event_RecordMode;
                _smAudio._DSmAudioEvents_Event_SoundLength += _smAudio__DSmAudioEvents_Event_SoundLength;
                _smAudio._DSmAudioEvents_Event_SoundPosition += _smAudio__DSmAudioEvents_Event_SoundPosition;
                _smAudio._DSmAudioEvents_Event_State += _smAudio__DSmAudioEvents_Event_State;
                _smAudio.Volume += _smAudio_Volume;
                _smAudio._DSmAudioEvents_Event_DownloadLength += _smAudio__DSmAudioEvents_Event_DownloadLength;
                _smAudio.SpokenInstruction += _smAudio_SpokenInstruction;

                _smAudio.ActivateControlDevice();

                if (m_sVoiceFile == null || m_sVoiceFile == "" || m_arrWaveBytes == null)
                {
                    _smAudio.CloseFile();

                    _smAudio.NewFileType = SmXFileType.smxaudDictation;
                    _smAudio.NewFile();

                    m_sVoiceFile = System.IO.Path.GetTempFileName();
                    File.Delete(m_sVoiceFile);
                    m_sVoiceFile += ".WAV";
                    _smAudio.SaveFileAs(m_sVoiceFile);

                    //  Set the file format
                    _smAudio.AudioFormat = SmXAudioFormat.smxPCM16Bit8kHzMono;
                }
                else
                {
                    _smAudio.OpenFile(m_sVoiceFile);
                }

                m_sVoiceFile = _smAudio.FileName;

                //  Set the Wave Volumes  (temp values for now - read configuration later)                
                _smAudio.GlobalWaveVolume = 100;

                //IsInitialized = true;

                stateTimer = new System.Threading.Timer(new TimerCallback(TimerCallback), null, 0, 5000);                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        internal void DeinitializeSpeechMike()
        {
            try
            {
                stateTimer.Dispose();
                stateTimer = null;

                _smAudio.DeactivateControlDevice();

                _smAudio.ControlDevice -= _smAudio_ControlDevice;
                _smAudio.EOS -= _smAudio_EOS;
                _smAudio.Error -= _smAudio_Error;
                _smAudio._DSmAudioEvents_Event_RecordMode -= _smAudio__DSmAudioEvents_Event_RecordMode;
                _smAudio._DSmAudioEvents_Event_SoundLength -= _smAudio__DSmAudioEvents_Event_SoundLength;
                _smAudio._DSmAudioEvents_Event_SoundPosition -= _smAudio__DSmAudioEvents_Event_SoundPosition;
                _smAudio._DSmAudioEvents_Event_State -= _smAudio__DSmAudioEvents_Event_State;
                _smAudio.Volume -= _smAudio_Volume;
                _smAudio._DSmAudioEvents_Event_DownloadLength -= _smAudio__DSmAudioEvents_Event_DownloadLength;
                _smAudio.SpokenInstruction -= _smAudio_SpokenInstruction;

                _smAudio.Deinitialize();
                _smAudio = null;

                File.Delete(m_sVoiceFile);
                m_sVoiceFile = null;
                m_arrWaveBytes = null;

                //IsInitialized = false;                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #endregion

        #region SpeechMagic Events

        void _smAudio_SpokenInstruction()
        {
            
        }

        void _smAudio__DSmAudioEvents_Event_DownloadLength(int NewLength)
        {
            
        }

        void _smAudio__DSmAudioEvents_Event_State(SmXAudioState NewState)
        {
            
        }

        void _smAudio_Volume(int NewLevel)
        {
            _rtcHost.SetVolume(NewLevel);
        }

        private int m_nSoundLength = 0;
        private int m_nFilePosition = 0;

        void _smAudio__DSmAudioEvents_Event_SoundPosition(int NewPosition)
        {
            m_nFilePosition = NewPosition;

            int nSeconds = (NewPosition / 1000);

            _rtcHost.SetSoundPosition(NewPosition);

            string timeText = String.Format("{0:D2}:{1:D2}/{2:D2}:{3:D2}",
                    new object[] {((NewPosition / 1000) / 60), ((NewPosition / 1000) % 60),
					((m_nSoundLength / 1000) / 60), ((m_nSoundLength / 1000) % 60)});
            _rtcHost.SetRecordingTimeDisplay(timeText);
        }

        void _smAudio__DSmAudioEvents_Event_SoundLength(int NewLength)
        {
            m_nSoundLength = NewLength;

            int nLength = (NewLength / 1000);

            _rtcHost.SetSoundLength(NewLength);
        }

        void _smAudio__DSmAudioEvents_Event_RecordMode(SmXRecordMode NewRecordMode)
        {
            
        }

        void _smAudio_Error(SmXAudioError Error)
        {

        }

        void _smAudio_EOS()
        {

        }

        void _smAudio_ControlDevice(SmXAudioControlDeviceEvent ControlDeviceEvent, SmXAudioControlDevice ControlSource)
        {
            Trace.WriteLine("Event: ControlDevice");
        }

        #endregion

        #region SpeechMike Commands

        public void DoInstructions()
        {
            try
            {
                _smAudio.RecordSpokenInstruction();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoRecord()
        {
            try
            {
                _smAudio.Stop();
                _smAudio.Record();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoPlay()
        {
            try
            {
                _smAudio.Stop();
                _smAudio.Play();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoStop()
        {
            try
            {
                _smAudio.Stop();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoRewind()
        {
            try
            {
                _smAudio.FastRewind();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoFastForward()
        {
            try
            {
                _smAudio.FastForward();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoBeginning()
        {
            try
            {
                _smAudio.SoundPosition = 0;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void DoEnd()
        {
            try
            {
                _smAudio.SoundPosition = _smAudio.SoundLength;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void SetGlobalWaveVolume(int volume)
        {
            _smAudio.GlobalWaveVolume = volume;
        }

        #endregion

        #region Application Commands

        public void Navigate(string address)
        {
            //_chrome.Load(address);
            _chrome.Navigate(address);
        }

        public void ShowApp(string uri)
        {
            Trace.WriteLine("*****ShowApp*****");            
            this.Opacity = 100.0;
            this.ShowInTaskbar = true;
            this.SetTopLevel(true);
            this.Visible = false;            
            this.Show(Program._desktopParent);            
            this.Focus();
            Trace.WriteLine("***** End ShowApp*****");
            Navigate(uri);    
        }

        public void HideApp()
        {

            Navigate("about:blank");
            this.ShowInTaskbar = false;
            this.Parent = null;
            this.SetTopLevel(false);
            this.Hide();
        }

        private HttpRequestMessage GetRequestWithAuthHeaders(HttpMethod method, string uri, string accept, string contentType, string strContent = null, byte[] arrContent = null)
        {
            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri);
                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                requestMessage.Headers.Add(MMODAL_AUTH_APP_HEADER, MMODAL_AUTH_APP_VALUE);
                requestMessage.Headers.Add(MMODAL_AUTH_KEY_HEADER, MMODAL_AUTH_KEY_VALUE);

                if (strContent != null)
                {
                    StringContent msgContent = new StringContent(strContent, Encoding.UTF8, contentType);
                    requestMessage.Content = msgContent;
                }
                else
                {
                    if (arrContent != null)
                    {
                        ByteArrayContent msgContent = new ByteArrayContent(arrContent);
                        requestMessage.Content = msgContent;
                    }
                }

                return requestMessage;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        public async void SetApiRequestContext(ExpandoObject reqcontext)
        {
            Trace.WriteLine("===>SetApiRequestContext<===");

            _requestContext = reqcontext;

            //  if job is pended, download it and write it out to temp file
            //  and set m_sVoiceFile
            dynamic reqObj = _requestContext;
            if (reqObj.RequestObject != null)
            {
                if (reqObj.RequestObject.FftJobStatus == 1)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ApiUrl);
                        //client.DefaultRequestHeaders.Accept.Clear();
                        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        dynamic reqObject = _requestContext;
                        string requestUri = String.Format("api/DictationJobs/GetPendingJobWave/{0}", reqObj.RequestObject.DictationJobId);

                        var converter = new ExpandoObjectConverter();
                        string sRequestContext = JsonConvert.SerializeObject(_requestContext, new JsonConverter[] {converter});
                        HttpRequestMessage requestMessage = GetRequestWithAuthHeaders(HttpMethod.Post, requestUri, 
                            "application/octet-stream", "application/json", sRequestContext);

                        //HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, _requestContext);
                        HttpResponseMessage response = await client.SendAsync(requestMessage);
                        if (response.IsSuccessStatusCode)
                        {
                            Trace.WriteLine("******Fetch pending job succeeded*****");
                            
                            m_arrWaveBytes = await response.Content.ReadAsAsync<byte[]>();
                            m_sVoiceFile = System.IO.Path.GetTempFileName();
                            File.Delete(m_sVoiceFile);
                            m_sVoiceFile += ".WAV";

                            using (FileStream fs = new FileStream(m_sVoiceFile, FileMode.Create))
                            {
                                fs.Write(m_arrWaveBytes, 0, m_arrWaveBytes.Length);
                                fs.Close();
                            }
                        }
                        else
                        {
                            Trace.WriteLine("******Fetch pending job failed ******");
                        }
                    }
                }
            }

            InitializeSpeechMike();

            Trace.WriteLine("===> End SetApiRequestContext<===");
        }

        #region Private Helper Methods

        private void SetRequestObject(int fftJobStatus)
        {
            if (_smAudio.State != SmXAudioState.smxStop)
            {
                _smAudio.Stop();
            }

            _smAudio.SaveFile();
            _smAudio.CloseFile();

            using (FileStream fs = new FileStream(m_sVoiceFile, FileMode.Open))
            {
                m_arrWaveBytes = new byte[fs.Length];
                fs.Read(m_arrWaveBytes, 0, (int)fs.Length);
            }

            dynamic reqObj = _requestContext;
            reqObj.RequestObject.WaveData = m_arrWaveBytes;
            reqObj.RequestObject.FFtJobStatus = fftJobStatus;

            DeinitializeSpeechMike();
        }

        #endregion

        public async void DoSave(ExpandoObject job_state)
        {
            Trace.WriteLine("===>DoSave");

            dynamic jobState = job_state;

            if (_smAudio == null)
                return;

            //  ExportFailed (i.e., ready for export) 
            // exportfailed does not seem to mean ready for export
            SetRequestObject(12);  
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                dynamic reqObject = _requestContext;

                if (jobState.stat == true)
                    reqObject.RequestObject.Priority = 1;

                string requestUri = String.Format("api/DictationJobs/UpdateDictationJob/{0}?review_required={1}",
                    jobState.dictationJobId, jobState.reviewRequired);

                var converter = new ExpandoObjectConverter();
                string sRequestContext = JsonConvert.SerializeObject(_requestContext, new JsonConverter[] { converter });
                HttpRequestMessage requestMessage = GetRequestWithAuthHeaders(HttpMethod.Post, requestUri,
                    "application/json", "application/json", sRequestContext);

                //HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, _requestContext);
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("******DoSave succeeded*****");
                }
                else
                {
                    Trace.WriteLine("******DoSave failed ******");
                }
            }

            HideApp();
        }

        public async void DoPend(ExpandoObject job_state)
        {
            Trace.WriteLine("===>DoPend");

            dynamic jobState = job_state;

            if (_smAudio == null)
                return;

            SetRequestObject(1);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiUrl);
                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                dynamic reqObject = _requestContext;
                if (jobState.stat == true)
                    reqObject.RequestObject.Priority = 1;

                string requestUri = String.Format("api/DictationJobs/UpdateDictationJob/{0}?review_required={1}", 
                    jobState.dictationJobId, jobState.reviewRequired);

                var converter = new ExpandoObjectConverter();
                string sRequestContext = JsonConvert.SerializeObject(_requestContext, new JsonConverter[] { converter });
                HttpRequestMessage requestMessage = GetRequestWithAuthHeaders(HttpMethod.Post, requestUri,
                    "application/json", "application/json", sRequestContext);

                //HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, _requestContext);
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("******DoPend succeeded*****");
                }
                else
                {
                    Trace.WriteLine("******DoPend failed ******");
                }
            }

            HideApp();
        }

        public async void DoCancel(int job_id)
        {
            try
            {
                Trace.WriteLine("===>DoCancel");

                if (_smAudio == null)
                    return;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApiUrl);
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    dynamic reqObject = _requestContext;
                    string requestUri = String.Format("api/DictationJobs/DeleteDictationJob/{0}", job_id);

                    var converter = new ExpandoObjectConverter();
                    string sRequestContext = JsonConvert.SerializeObject(_requestContext, new JsonConverter[] { converter });
                    HttpRequestMessage requestMessage = GetRequestWithAuthHeaders(HttpMethod.Post, requestUri,
                        "application/json", "application/json", sRequestContext);

                    //HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, _requestContext);
                    HttpResponseMessage response = await client.SendAsync(requestMessage);
                    if (response.IsSuccessStatusCode)
                    {
                        Trace.WriteLine("******DoCancel succeeded*****");
                    }
                    else
                    {
                        Trace.WriteLine("******DoCancel failed ******");
                    }
                }

                HideApp();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void AudioWizard()
        {

        }

        public void SwitchTemplates()
        {

        }

        #endregion

        #region Etc
        private void TimerCallback(object stateInfo)
        {
            //  perform useful work here
            bool bCanExecute = false;

            bCanExecute = (m_sVoiceFile != null && m_sVoiceFile != "");

            if (_rtcHost.IsSpaConnected)
            {
                _rtcHost.SetInstructionsCanExecute(bCanExecute);
                _rtcHost.SetRecordCanExecute(bCanExecute);
                _rtcHost.SetPlayCanExecuute(bCanExecute);
                _rtcHost.SetStopCanExecute(bCanExecute);
                _rtcHost.SetRewindCanExecute(bCanExecute);
                _rtcHost.SetFastForwardCanExecute(bCanExecute);
                _rtcHost.SetBeginningCanExecute(bCanExecute);
                _rtcHost.SetEndCanExecute(bCanExecute);
            }
            //_rtcHost.SetCurrentDateTime();
        }

        #endregion

        private void FormSmHost_Shown(object sender, EventArgs e)
        {
            Trace.WriteLine("----------->>>FormSmHost_Shown<<<<<------------------");
        }
    }
}
