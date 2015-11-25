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
using CefSharp;
using System.ComponentModel;
using CefSharp.Wpf;
using System.Runtime.CompilerServices;

namespace GSAKWrapper.UIControls
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WebBrowserControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _requestedDocumentText = null;

        public WebBrowserControl()
        {
            DataContext = this;
            InitializeComponent();
            browser.IsBrowserInitializedChanged += browser_IsBrowserInitializedChanged;
            this.IsVisibleChanged += WebBrowser_IsVisibleChanged;
        }

        void browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (WPFWebBrowser != null && WPFWebBrowser.IsBrowserInitialized && !string.IsNullOrEmpty(_requestedDocumentText))
            {
                DocumentText = _requestedDocumentText;
            }            
        }

        void WebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_webBrowser != null)
            {
                _webBrowser.Dispose();
                _webBrowser = null;
            }
        }
 
        private IWpfWebBrowser _webBrowser;
        public IWpfWebBrowser WPFWebBrowser
        {
            get { return _webBrowser; }
            set 
            { 
                SetProperty(ref _webBrowser, value);
                if (_webBrowser != null)
                {
                    if (_webBrowser.IsBrowserInitialized)
                    {
                        if (!string.IsNullOrEmpty(_requestedDocumentText))
                        {
                            DocumentText = _requestedDocumentText;
                        }
                    }
                }
            }
        }


        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public string DocumentText
        {
            get { return null; }
            set
            {
                if (WPFWebBrowser != null && WPFWebBrowser.IsBrowserInitialized)
                {
                    //ChromiumBrowser.LoadHtml(value, @"c:\temp.html");
                    WPFWebBrowser.LoadHtml(value, @"http://www.google.com/");
                }
                else
                {
                    _requestedDocumentText = value;
                }
            }
        }

    }
}
