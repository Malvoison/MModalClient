using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CefSharp.Wpf;

namespace ClientHostCef.AppClasses
{
    public class JavaScriptHelper
    {
        private IWpfWebBrowser _webBrowser;

        public JavaScriptHelper(IWpfWebBrowser browser)
        {
            _webBrowser = browser;
        }

        public void InvokePageJavaScriptFunction(string func, string param1)
        {
            string script = func + String.Format("('{0}')", param1);
            _webBrowser.ExecuteScriptAsync(script);
        }

        public void InvokePageJavaScriptFunction(string func, string param1, string param2)
        {
            string script = func + String.Format("('{0}', '{1}')", param1, param2);
            _webBrowser.ExecuteScriptAsync(script);
        }

        public void InvokePageJavaScriptFunction(string func, string param1, string param2, string param3)
        {
            string script = func + String.Format("('{0}', '{1}', '{2}')", param1, param2, param3);
            _webBrowser.ExecuteScriptAsync(script);
        }        
    }
}
