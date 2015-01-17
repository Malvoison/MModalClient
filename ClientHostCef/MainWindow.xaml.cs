using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CefSharp.Example;
using ClientHostCef.ViewModels;
using System.ComponentModel;
using ClientHostCef.AppClasses;

namespace ClientHostCef
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string DefaultUrl = "http://foxnews.com";  //  fair and balanced default URL
        
        private BrowserViewModel _viewModel;
        public BrowserViewModel ViewModel
        {
            get { return _viewModel; }
            set { PropertyChanged.ChangeAndNotify(ref _viewModel, value, () => ViewModel); }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string sUrl = Application.Current.Properties["AppUrl"].ToString();
            ViewModel = new BrowserViewModel(sUrl != null ? sUrl : DefaultUrl);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
