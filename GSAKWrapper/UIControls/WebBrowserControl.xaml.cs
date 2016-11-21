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

        public class JSCallback
        {
            public string Name { get; set; }
            public object Instance { get; set; }
        }

        private string _requestedDocumentText = null;
        private List<JSCallback> _registerCallbacks;

        public WebBrowserControl()
        {
            _registerCallbacks = new List<JSCallback>();
            DataContext = this;
            InitializeComponent();
            browser.IsBrowserInitializedChanged += browser_IsBrowserInitializedChanged;
            this.IsVisibleChanged += WebBrowser_IsVisibleChanged;
        }

        void browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (browser != null && !browser.IsBrowserInitialized)
            {
                foreach (var cb in _registerCallbacks)
                {
                    RegisterJSCallback(cb);
                }
                _registerCallbacks.Clear();
            }
            if (browser != null && browser.IsBrowserInitialized)
            {
                if (!string.IsNullOrEmpty(_requestedDocumentText))
                {
                    DocumentText = _requestedDocumentText;
                    _requestedDocumentText = null;
                }
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
                    if (!browser.IsBrowserInitialized)
                    {
                        foreach (var cb in _registerCallbacks)
                        {
                            RegisterJSCallback(cb);
                        }
                        _registerCallbacks.Clear();
                    }
                    if (browser.IsBrowserInitialized)
                    {
                        if (!string.IsNullOrEmpty(_requestedDocumentText))
                        {
                            DocumentText = _requestedDocumentText;
                            _requestedDocumentText = null;
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
                if (browser != null && browser.IsBrowserInitialized)
                {
                    //ChromiumBrowser.LoadHtml(value, @"c:\temp.html");
                    browser.LoadHtml(value, @"http://www.google.com/");
                }
                else
                {
                    _requestedDocumentText = value;
                }
            }
        }


        public void RegisterJSCallback(JSCallback cb)
        {
            if (browser != null && !browser.IsBrowserInitialized)
            {
                browser.RegisterJsObject(cb.Name, cb.Instance);
            }
            else
            {
                _registerCallbacks.Add(cb);
            }
        }
    }
}
