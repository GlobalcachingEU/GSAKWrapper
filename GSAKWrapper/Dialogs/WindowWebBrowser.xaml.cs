using GSAKWrapper.UIControls;
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
using System.Windows.Shapes;

namespace GSAKWrapper.Dialogs
{
    /// <summary>
    /// Interaction logic for WindowWebBrowser.xaml
    /// </summary>
    public partial class WindowWebBrowser : Window
    {
        private UIControls.WebBrowserControl _browser = null;

        public WindowWebBrowser()
        {
            InitializeComponent();
            _browser = new UIControls.WebBrowserControl();
            this.browser.Children.Add(_browser);

            ApplicationData.Instance.OpenWindows.Add(this);
            this.Closed += WindowWebBrowser_Closed;
        }

        void WindowWebBrowser_Closed(object sender, EventArgs e)
        {
            ApplicationData.Instance.OpenWindows.Remove(this);
            if (_browser != null)
            {
                _browser.Dispose();
                _browser = null;
            }
        }

        public WindowWebBrowser(string htmlContent)
            : this(htmlContent, null)
        {
        }

        public WindowWebBrowser(string htmlContent, WebBrowserControl.JSCallback jsCallback)
            : this()
        {
            if (jsCallback != null)
            {
                _browser.RegisterJSCallback(jsCallback);
            }
            if (htmlContent != null)
            {
                _browser.DocumentText = htmlContent;
            }
        }
    }
}
