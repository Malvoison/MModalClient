using CefSharp.Example;
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

using ClientHostCef.AppClasses;

namespace ClientHostCef.Views
{
    public partial class BrowserView : UserControl
    {        
        public BrowserView()
        {
            InitializeComponent();

            browser.RequestHandler = new RequestHandler();

            this.Loaded += BrowserView_Loaded;
        }

        void BrowserView_Loaded(object sender, RoutedEventArgs e)
        {            
        }
    }
}
