using GSAKWrapper.Localization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace GSAKWrapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            GSAKWrapper.Settings.Settings.ApplicationRunning = true;
            TranslationManager.Instance.TranslationProvider = new ResxTranslationProvider("GSAKWrapper.Properties.Resources", Assembly.GetExecutingAssembly()); 
            WpfSingleInstance.Make("GSAKWrapper", this);
        }
    }
}
