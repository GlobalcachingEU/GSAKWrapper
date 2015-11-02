using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Threading;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Windows.Resources;

namespace GSAKWrapper.Localization
{
    public class TranslationManager
    {
        private static TranslationManager _translationManager;
        private Hashtable _overrideTranslation;

        public event EventHandler LanguageChanged;

        private class LanguageItem
        {
            public string Original { get; set; }
            public string Translation { get; set; }

            public LanguageItem(string org, string trans)
            {
                Original = org;
                Translation = trans;
            }
        }

        private void loadOverrides()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Settings.Default.SelectedCulture);

            _overrideTranslation.Clear();
            string xmlFileContents = null;
            try
            {
                if (CurrentLanguage.TwoLetterISOLanguageName.ToLower().Length == 2 &&
                    CurrentLanguage.TwoLetterISOLanguageName.ToLower()!="iv")
                {
                    StreamResourceInfo sri = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/Resources/Language.{0}.xml", CurrentLanguage.TwoLetterISOLanguageName.ToLower())));
                    if (sri != null)
                    {
                        using (StreamReader textStreamReader = new StreamReader(sri.Stream))
                        {
                            xmlFileContents = textStreamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                //not avialable
            }
            if (xmlFileContents != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlFileContents);
                XmlElement root = doc.DocumentElement;
                XmlNodeList strngs = root.SelectNodes("string");
                if (strngs != null)
                {
                    foreach (XmlNode sn in strngs)
                    {
                        if (!string.IsNullOrEmpty(sn.Attributes["value"].InnerText))
                        {
                            _overrideTranslation[sn.Attributes["name"].InnerText.ToLower()] = sn.Attributes["value"].InnerText;
                        }
                    }
                }
            }

        }

        private TranslationManager() 
        {
            _overrideTranslation = new Hashtable();
            loadOverrides();
        }

        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if( value != Thread.CurrentThread.CurrentUICulture)
                {
                    Settings.Settings.Default.SelectedCulture = value.ToString();
                    Thread.CurrentThread.CurrentUICulture = value;
                    loadOverrides();
                    OnLanguageChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
               if( TranslationProvider != null)
               {
                   return TranslationProvider.Languages;
               }
               return Enumerable.Empty<CultureInfo>();
            }
        }

        public static TranslationManager Instance
        {
            get
            {
                if (_translationManager == null)
                    _translationManager = new TranslationManager();
                return _translationManager;
            }
        }

        public ITranslationProvider TranslationProvider { get; set; }

        public void ReloadUserTranslation()
        {
            loadOverrides();
            OnLanguageChanged();
        }

        private void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }

        public object Translate(string key)
        {
            if( TranslationProvider!= null)
            {
                object translatedValue =TranslationProvider.Translate(key);
                if (translatedValue != null)
                {
                    //see if there is an override in XML file
                    if (translatedValue is string)
                    {
                        var patchedValue = _overrideTranslation[(translatedValue as string).ToLower()] as string;
                        if (!string.IsNullOrEmpty(patchedValue))
                        {
                            return patchedValue;
                        }
                    }
                    return translatedValue;
                }
                else //not in resource file, but maybe in override xml file
                {
                    string patchedValue = _overrideTranslation[(key as string).ToLower()] as string;
                    if (!string.IsNullOrEmpty(patchedValue))
                    {
                        return patchedValue;
                    }
                }
            }
#if DEBUG
            return string.Format("!{0}!", key);
#else
            return string.Format(key);
#endif
        }

#if DEBUG
        public void CreateOrUpdateXmlFiles()
        {
            var resourceList = new List<LanguageItem>();
            var sc = Localization.TranslationManager.Instance.TranslationProvider.ResourceManager.GetString("Cancel", CultureInfo.InvariantCulture);
            System.Resources.ResourceSet rset = Localization.TranslationManager.Instance.TranslationProvider.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, false, true);
            if (rset != null)
            {
                foreach (DictionaryEntry entry in rset)
                {
                    string s = entry.Value as string;
                    if (s != null)
                    {
                        s = s.ToLower();
                        resourceList.Add(new LanguageItem(s,""));
                    }
                }
            }
            //for each possible language, read file patch and save
            foreach (var l in TranslationProvider.Languages)
            {
                var emptyList = (from a in resourceList select new LanguageItem(a.Original, "")).OrderBy(x => x.Original).Distinct().ToList();
                if (l.TwoLetterISOLanguageName.ToLower().Length == 2 &&
                    l.TwoLetterISOLanguageName.ToLower() != "iv")
                {
                    XmlDocument doc;
                    XmlElement root;
                    StreamResourceInfo sri = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/Resources/Language.{0}.xml", l.TwoLetterISOLanguageName.ToLower())));
                    if (sri != null)
                    {
                        using (StreamReader textStreamReader = new StreamReader(sri.Stream))
                        {
                            var xmlFileContents = textStreamReader.ReadToEnd();
                            doc = new XmlDocument();
                            doc.LoadXml(xmlFileContents);
                            root = doc.DocumentElement;
                            XmlNodeList strngs = root.SelectNodes("string");
                            if (strngs != null)
                            {
                                foreach (XmlNode sn in strngs)
                                {
                                    if (!string.IsNullOrEmpty(sn.Attributes["value"].InnerText))
                                    {
                                        var k = emptyList.FirstOrDefault(x => x.Original == sn.Attributes["name"].InnerText.ToLower());
                                        if (k != null)
                                        {
                                            k.Translation = sn.Attributes["value"].InnerText;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //write xml file
                    string fn = Path.Combine(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "GSAKWrapper"), string.Format("Language.{0}.xml", l.TwoLetterISOLanguageName.ToLower()));
                    doc = new XmlDocument();
                    root = doc.CreateElement("resources");
                    foreach (LanguageItem di in emptyList)
                    {
                        XmlElement lngElement = doc.CreateElement("string");
                        lngElement.SetAttribute("name", di.Original);
                        lngElement.SetAttribute("value", di.Translation);
                        root.AppendChild(lngElement);
                    }
                    doc.AppendChild(root);
                    using (System.IO.TextWriter sw = new System.IO.StreamWriter(fn, false, Encoding.UTF8)) //Set encoding
                    {
                        doc.Save(sw);
                    }
                }
            }
        }
#endif
    }
}
