using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using Microsoft.Win32;
using System.IO;

using CefSharp.Example;
using ClientHostCef.MVVM;
using CefSharp.Wpf;
using CefSharp;

using ClientHostCef.AppClasses;

namespace ClientHostCef.ViewModels
{
    public class BrowserViewModel : INotifyPropertyChanged
    {
        private ObjectForScripting _ofs = null;
        private MModalRtcHost _rtc = null;

        private string _address;
        public String Address
        {
            get { return _address; }
            set { PropertyChanged.ChangeAndNotify(ref _address, value, () => Address);}
        }

        private string _outputMessage;
        public string OutputMessage
        {
            get { return _outputMessage; }
            set { PropertyChanged.ChangeAndNotify(ref _outputMessage, value, () => OutputMessage); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { PropertyChanged.ChangeAndNotify(ref _statusMessage, value, () => StatusMessage); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { PropertyChanged.ChangeAndNotify(ref _title, value, () => Title); }
        }

        private IWpfWebBrowser _webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return _webBrowser; }
            set 
            { 
                PropertyChanged.ChangeAndNotify(ref _webBrowser, value, () => WebBrowser);
                //_webBrowser.RegisterJsObject("dotNetCallbackObj", _ofs);
                _ofs.HostWindow = _webBrowser;
                _webBrowser.ExecuteScriptAsync("doConsoleLog('Hello from CefSharp!!!!'");
            }
        }

        public ICommand ShowDevToolsCommand { get; set; }

        public ICommand InitializePhilipsCommand { get; set; }

        public ICommand DoFileBytesCommand { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public BrowserViewModel(string address)
        {
            Address = address;

            ShowDevToolsCommand = new DelegateCommand(ShowDevTools);
            InitializePhilipsCommand = new DelegateCommand(InitializePhilips);
            DoFileBytesCommand = new DelegateCommand(DoFileBytes);

            var version = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            OutputMessage = version;

            _ofs = new ObjectForScripting();
            _ofs.MainWindow = Application.Current.MainWindow;
            _ofs.AppUrl = Application.Current.Properties["AppUrl"].ToString();
            _ofs.ApiUrl = Application.Current.Properties["ApiUrl"].ToString();
            _ofs.RtcUrl = Application.Current.Properties["RtcUrl"].ToString();

            _rtc = MModalRtcHost.GetInstance();

            _ofs.RtcClient = _rtc.RtcGroup;
            _ofs.RtcGroup = _rtc.RtcGroup;

            //WebBrowser.RegisterJsObject("dotNetCallbackObj", _ofs);
        }

        private void ShowDevTools()
        {
            _webBrowser.ShowDevTools();
        }

        private void InitializePhilips()
        {
            _ofs.InitializeSpeechMike();
        }

        private void DoFileBytes()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files (*.*)|*.*";

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                string filename = ofd.FileName;
                byte[] arrFile;
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    arrFile = new byte[fs.Length];
                    fs.Read(arrFile, 0, (int)fs.Length);
                }

                string fileBase64 = Convert.ToBase64String(arrFile, Base64FormattingOptions.None);
                _webBrowser.ExecuteScriptAsync(String.Format("setFileBlob('{0}');", fileBase64));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {                    
                case "WebBrowser":
                    if (WebBrowser != null)
                    {
                        WebBrowser.ConsoleMessage += OnWebBrowserConsoleMessage;
                        WebBrowser.StatusMessage += OnWebBrowserStatusMessage;
                        WebBrowser.LoadError += OnWebBrowserLoadError;

                        WebBrowser.FrameLoadEnd +=
                            delegate { Application.Current.Dispatcher.BeginInvoke((Action)(() => _webBrowser.Focus())); };                        
                    }

                    break;
            }
        }

        private void OnWebBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            OutputMessage = e.Message;
        }

        private void OnWebBrowserStatusMessage(object sender, StatusMessageEventArgs e)
        {
            StatusMessage = e.Value;
        }

        private void OnWebBrowserLoadError(object sender, LoadErrorEventArgs args)
        {
            // Don't display an error for downloaded files where the user aborted the download.
            if (args.ErrorCode == CefErrorCode.Aborted)
                return;

            var errorMessage = "<html><body><h2>Failed to load URL " + args.FailedUrl +
                  " with error " + args.ErrorText + " (" + args.ErrorCode +
                  ").</h2></body></html>";

            _webBrowser.LoadHtml(errorMessage, args.FailedUrl);
        }
    }
}
