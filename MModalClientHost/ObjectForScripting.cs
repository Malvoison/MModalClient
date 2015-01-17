using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using SMAUDIOLib;
using System.IO;

namespace MModalClientHost
{
    [ComVisible(true)]
    public class ObjectForScripting
    {
        #region Private Fields

        private SmAudioClass _smAudio = null;
        private bool bIsInitialized = false;
        private string m_sVoiceFile = null;
        private byte[] m_arrWaveBytes = null;

        private System.Threading.Timer stateTimer = null;

        #endregion

        #region Public Properties
        public Form MainWindow { get; set; }
        public WebBrowser HostWindow { get; set; }
        public string AppUrl { get; set; }
        public string ApiUrl { get; set; }
        public string RtcUrl { get; set; }
        public string RtcClient { get; set; }
        public string RtcGroup { get; set; }

        public bool IsInitialized
        {
            get { return bIsInitialized; }
            set { bIsInitialized = value; }
        }

        #endregion

        public ObjectForScripting()
        {            
        }

        #region Configuration Methods

        public string GetAppUrl()
        {
            return AppUrl;
        }

        public string GetApiUrl()
        {
            return ApiUrl;
        }

        public string GetRtcUrl()
        {
            return RtcUrl;
        }

        public string GetRtcClient()
        {
            return RtcClient;
        }

        public string GetRtcGroup()
        {
            return RtcGroup;
        }

        #endregion

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
                }
                else
                {
                    _smAudio.OpenFile(m_sVoiceFile);
                }

                m_sVoiceFile = _smAudio.FileName;

                //  Set the file format
                _smAudio.AudioFormat = SmXAudioFormat.smxPCM16Bit8kHzMono;

                //  Set the Wave Volumes  (temp values for now - read configuration later)                
                _smAudio.GlobalWaveVolume = 100;

                IsInitialized = true;

                stateTimer = new System.Threading.Timer(new TimerCallback(TimerCallback), null, 0, 5000);

                InvokePageJsFunction("setVolume", new string[] { "0" });
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

                IsInitialized = false;

                InvokePageJsFunction("setVolume", new string[] { "0" });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
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

        #endregion

        #region Application Commands

        public void DoAttach()
        {

        }

        public void DoSubmit()
        {

        }

        public void DoPend()
        {

        }

        public void AudioWizard()
        {

        }

        public void SwitchTemplates()
        {

        }

        #endregion

        #region SpeechMagic Events

        void _smAudio_SpokenInstruction()
        {            
            InvokePageJsFunction("addEventTrace", new string[] { "Event: SpokenInstruction" });
        }

        void _smAudio__DSmAudioEvents_Event_DownloadLength(int NewLength)
        {         
            InvokePageJsFunction("addEventTrace", new string[] { "Event: DownloadLength" });
        }

        void _smAudio__DSmAudioEvents_Event_State(SmXAudioState NewState)
        {            
            InvokePageJsFunction("addEventTrace", new string[] { "Event: State" });
        }

        void _smAudio_Volume(int NewLevel)
        {            
            InvokePageJsFunction("addEventTrace", new string[] { "Event: NewLevel" });
            InvokePageJsFunction("setVolume", new string[] { NewLevel.ToString() });
        }

        void _smAudio__DSmAudioEvents_Event_SoundPosition(int NewPosition)
        {
            int nSeconds = (NewPosition / 1000);
            InvokePageJsFunction("addEventTrace", new string[] { "Event: SoundPosition" });
            InvokePageJsFunction("setSoundPosition", new string[] { nSeconds.ToString() });
        }

        void _smAudio__DSmAudioEvents_Event_SoundLength(int NewLength)
        {
            int nLength = (NewLength / 1000);
            InvokePageJsFunction("addEventTrace", new string[] { "Event: SoundLength" });
            InvokePageJsFunction("setSoundLength", new string[] { nLength.ToString() });
        }

        void _smAudio__DSmAudioEvents_Event_RecordMode(SmXRecordMode NewRecordMode)
        {
            InvokePageJsFunction("addEventTrace", new string[] { "Event: RecordMode" });
        }

        void _smAudio_Error(SmXAudioError Error)
        {            
            InvokePageJsFunction("addEventTrace", new string[] { "Event: Error" });
        }

        void _smAudio_EOS()
        {         
            InvokePageJsFunction("addEventTrace", new string[] { "Event: EOS" });
        }

        void _smAudio_ControlDevice(SmXAudioControlDeviceEvent ControlDeviceEvent, SmXAudioControlDevice ControlSource)
        {
            Trace.WriteLine("Event: ControlDevice");
        }

        #endregion

        #region Utility Methods

        private void ThreadSafeInvokePageJsFunction(string fn, string[] parameters)
        {
            Func<string, string[], object> pageInvoker = new Func<string,string[],object>(InvokePageJsFunction);
            HostWindow.BeginInvoke(pageInvoker, new object[] { fn, parameters });
        }

        private object InvokePageJsFunction(string fn, string[] parameters)
        {
            return HostWindow.Document.InvokeScript(fn, parameters);
        }

        public void OutputDebugString(string message)
        {
            Trace.WriteLine(message);
        }

        public void SetTitle(string newTitle)
        {
            MainWindow.Text = newTitle;
        }

        public void SetSize(int width, int height)
        {
            System.Drawing.Size clientSize = new System.Drawing.Size();
            clientSize.Width = width;
            clientSize.Height = height;

            MainWindow.ClientSize = clientSize;

            Screen screen = Screen.FromControl(MainWindow);

            Rectangle workingArea = screen.WorkingArea;
            MainWindow.Location = new Point()
            {
                X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - MainWindow.Width) / 2),
                Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - MainWindow.Height) / 2)
            };

        }

        #endregion

        #region Timer-related Stuff
        
        private void TimerCallback(object stateInfo)
        {
            try
            {
                bool bCanExecute = false;

                bCanExecute = (m_sVoiceFile != null && m_sVoiceFile != "");

                ThreadSafeInvokePageJsFunction("setInstructionsCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setRecordCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setPlayCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setStopCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setRewindCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setFastForwardCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setBeginningCanExecute", new string[] { bCanExecute ? "true" : "false" });
                ThreadSafeInvokePageJsFunction("setEndCanExecute", new string[] { bCanExecute ? "true" : "false" });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Test Methods

        public void TestMe(string message, string title)
        {
            MessageBox.Show(message, title);
            //InvokePageJsFunction("hostAlert", new string[] { "TestMe" });
        }

        public void SetFileBlob(byte[] arrBlob)
        {
            string strBase64 = Convert.ToBase64String(arrBlob, Base64FormattingOptions.None);
            HostWindow.Document.InvokeScript("setFileBlob", new string[] { strBase64 });
        }

        #endregion
    }
}
