using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Windows;
using CefSharp.Wpf;
using SMAUDIOLib;
using System.IO;
using System.Diagnostics;

namespace ClientHostCef.AppClasses
{
    public class ObjectForScripting
    {
        #region Private Fields

        private SmAudioClass _smAudio = null;
        private string m_sVoiceFile = null;
        private byte[] m_arrWaveBytes = null;

        private System.Threading.Timer stateTimer = null;

        #endregion

        #region Public Properties

        public Window MainWindow { get; set; }
        public IWpfWebBrowser HostWindow { get; set; }
        public string AppUrl { get; set; }
        public string ApiUrl { get; set; }
        public string RtcUrl { get; set; }
        public string RtcClient { get; set; }
        public string RtcGroup { get; set; }

        public bool IsInitialized { get; set; }


        #endregion

        public ObjectForScripting()
        {

        }

        #region Configuration Methods

        public string getAppUrl()
        {
            return AppUrl;
        }

        public string getApiUrl()
        {
            return ApiUrl;
        }

        public string getRtcUrl()
        {
            return RtcUrl;
        }

        public string getRtcClient()
        {
            return RtcClient;
        }

        public string getRtcGroup()
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

                //stateTimer = new System.Threading.Timer(new TimerCallback(TimerCallback), null, 0, 5000);

                InvokePageJavaScriptFunction("setVolume", "0");
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

                InvokePageJavaScriptFunction("setVolume", "0");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #endregion

        #region SpeechMike Commands

        public void doInstructions()
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

        public void doRecord()
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

        public void doPlay()
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

        public void doStop()
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

        public void doRewind()
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

        public void doFastForward()
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

        public void doBeginning()
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

        public void doEnd()
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

        #region SpeechMagic Events

        void _smAudio_SpokenInstruction()
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: SpokenInstruction");
        }

        void _smAudio__DSmAudioEvents_Event_DownloadLength(int NewLength)
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: DownloadLength");
        }

        void _smAudio__DSmAudioEvents_Event_State(SmXAudioState NewState)
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: State");
        }

        void _smAudio_Volume(int NewLevel)
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: NewLevel");
            InvokePageJavaScriptFunction("setVolume", NewLevel.ToString());
        }

        void _smAudio__DSmAudioEvents_Event_SoundPosition(int NewPosition)
        {
            int nSeconds = (NewPosition / 1000);
            InvokePageJavaScriptFunction("addEventTrace", "Event: SoundPosition");
            InvokePageJavaScriptFunction("setSoundPosition", nSeconds.ToString());
        }

        void _smAudio__DSmAudioEvents_Event_SoundLength(int NewLength)
        {
            int nLength = (NewLength / 1000);
            InvokePageJavaScriptFunction("addEventTrace", "Event: SoundLength");
            InvokePageJavaScriptFunction("setSoundLength", nLength.ToString());
        }

        void _smAudio__DSmAudioEvents_Event_RecordMode(SmXRecordMode NewRecordMode)
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: RecordMode");
        }

        void _smAudio_Error(SmXAudioError Error)
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: Error");
        }

        void _smAudio_EOS()
        {
            InvokePageJavaScriptFunction("addEventTrace", "Event: EOS");
        }

        void _smAudio_ControlDevice(SmXAudioControlDeviceEvent ControlDeviceEvent, SmXAudioControlDevice ControlSource)
        {
            Trace.WriteLine("Event: ControlDevice");
        }

        #endregion

        #region Calling JavaScript

        private void ThreadSafeInvokePageJsFunction(string fn, string parameter)
        {
            Action<string, string> pageInvoker = new Action<string, string>(InvokePageJavaScriptFunction);            
            MainWindow.Dispatcher.BeginInvoke(pageInvoker, parameter);
        }

        public void InvokePageJavaScriptFunction(string func, string param1)
        {
            string script = func + String.Format("('{0}')", param1);
            HostWindow.ExecuteScriptAsync(script);
        }

        public void InvokePageJavaScriptFunction(string func, string param1, string param2)
        {
            string script = func + String.Format("('{0}', '{1}')", param1, param2);
            HostWindow.ExecuteScriptAsync(script);
        }

        public void InvokePageJavaScriptFunction(string func, string param1, string param2, string param3)
        {
            string script = func + String.Format("('{0}', '{1}', '{2}')", param1, param2, param3);
            HostWindow.ExecuteScriptAsync(script);
        }        

        #endregion

        #region Utility Functions

        public void outputDebugString(string message)
        {
            Trace.WriteLine(message);
        }

        public void setTitle(string newTitle)
        {
            MainWindow.Title = newTitle;
        }

        public void setSize(int width, int height)
        {
            //System.Drawing.Size clientSize = new System.Drawing.Size();
            //clientSize.Width = width;
            //clientSize.Height = height;

            //MainWindow.ClientSize = clientSize;

            //Screen screen = Screen.FromControl(MainWindow);

            //Rectangle workingArea = screen.WorkingArea;
            //MainWindow.Location = new Point()
            //{
            //    X = Math.Max(workingArea.X, workingArea.X + (workingArea.Width - MainWindow.Width) / 2),
            //    Y = Math.Max(workingArea.Y, workingArea.Y + (workingArea.Height - MainWindow.Height) / 2)
            //};
        }

        #endregion

        #region Timer-related Stuff

        private void TimerCallback(object stateInfo)
        {
            try
            {
                //ThreadSafeInvokePageJsFunction("doConsoleLog", "It's Howdy Doody Time");

                bool bCanExecute = false;

                bCanExecute = (m_sVoiceFile != null && m_sVoiceFile != "");

                ThreadSafeInvokePageJsFunction("setInstructionsCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setRecordCanExecute",  bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setPlayCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setStopCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setRewindCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setFastForwardCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setBeginningCanExecute", bCanExecute ? "true" : "false");
                ThreadSafeInvokePageJsFunction("setEndCanExecute", bCanExecute ? "true" : "false");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
