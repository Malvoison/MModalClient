using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace MModalClientHost
{
    public class JavaScriptHelper
    {
        private WebBrowser _webBrowser = null;

        public JavaScriptHelper(WebBrowser wb)
        {
            _webBrowser = wb;
        }

        public void  InvokePageJavaScriptFunction(string func, string[] parameters)
        {
            _webBrowser.Document.InvokeScript(func, parameters);
        }
    }
}
