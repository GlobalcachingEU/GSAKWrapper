using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GSAKWrapper.FlowSequences
{
    public class Manager
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        public ObservableCollection<FlowSequence> FlowSequences { get; set; }

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            FlowSequences = new ObservableCollection<FlowSequence>();
            loadFlowSeqeunces();
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        private void loadFlowSeqeunces()
        {
            if (!string.IsNullOrEmpty(Settings.Settings.Default.FlowSequencesXml))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Settings.Settings.Default.FlowSequencesXml);

                    XmlNodeList nl = doc.SelectNodes("/sequences/sequence");
                    foreach (XmlNode n in nl)
                    {
                        var af = new FlowSequence();
                        af.Name = n.Attributes["name"].Value;
                        af.Database = n.Attributes["database"].Value;
                        af.FlowSequenceItems = new ObservableCollection<FlowSequenceItem>();

                        XmlNodeList al = n.SelectNodes("flow");
                        foreach (XmlNode a in al)
                        {
                            var obj = new FlowSequenceItem();
                            obj.FlowName = a.Attributes["name"].Value;
                            obj.Database = a.Attributes["database"].Value;
                            af.FlowSequenceItems.Add(obj);
                        }
                        FlowSequences.Add(af);
                    }

                }
                catch
                {
                }
            }
        }

        public void Save()
        {
            try
            {
                var doc = createSequencesXml(FlowSequences.ToList());
                var sb = new StringBuilder();
                var tr = new System.IO.StringWriter(sb);
                var wr = new XmlTextWriter(tr);
                wr.Formatting = Formatting.None;
                doc.Save(wr);
                wr.Close();
                Settings.Settings.Default.FlowSequencesXml = sb.ToString();
            }
            catch
            {
            }
        }

        private XmlDocument createSequencesXml(List<FlowSequence> sequence)
        {
            var doc = new XmlDocument();
            try
            {
                var root = doc.CreateElement("sequences");
                doc.AppendChild(root);
                foreach (var af in sequence)
                {
                    var q = doc.CreateElement("sequence");
                    var attr = doc.CreateAttribute("name");
                    var txt = doc.CreateTextNode(af.Name);
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    attr = doc.CreateAttribute("database");
                    txt = doc.CreateTextNode(af.Database ?? "");
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    root.AppendChild(q);
                    foreach (var fl in af.FlowSequenceItems)
                    {
                        XmlElement r = doc.CreateElement("flow");
                        q.AppendChild(r);

                        attr = doc.CreateAttribute("name");
                        txt = doc.CreateTextNode(fl.FlowName);
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);

                        attr = doc.CreateAttribute("database");
                        txt = doc.CreateTextNode(fl.Database ?? "");
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);
                    }
                }
            }
            catch
            {
            }
            return doc;
        }

        public async Task RunFowSequence(FlowSequence fs)
        {
            await Task.Run(() =>
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    string defaultFlowDatabasePath = null;
                    if (string.IsNullOrEmpty(fs.Database))
                    {
                        defaultFlowDatabasePath = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                    }
                    else
                    {
                        defaultFlowDatabasePath = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, fs.Database, "sqlite.db3");
                    }
                    if (System.IO.File.Exists(defaultFlowDatabasePath))
                    {
                        foreach (var sq in fs.FlowSequenceItems)
                        {
                            string fn = null;
                            if (string.IsNullOrEmpty(sq.Database))
                            {
                                fn = defaultFlowDatabasePath;
                            }
                            else
                            {
                                fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, sq.Database, "sqlite.db3");
                            }
                            var af = (from a in UIControls.ActionBuilder.Manager.Instance.ActionFlows where string.Compare(a.Name, sq.FlowName, true) == 0 select a).FirstOrDefault();
                            if (af != null)
                            {
                                using (var db = new Database.DBConSqlite(fn))
                                {
                                    UIControls.ActionBuilder.Manager.Instance.RunFlow(af, db);
                                }
                            }
                        }
                    }
                    sw.Stop();
                    ApplicationData.Instance.StatusText = string.Format(Localization.TranslationManager.Instance.Translate("FlowSequenceFinishedIn") as string, fs.Name, sw.Elapsed.TotalSeconds.ToString("0.0"));
                }
                catch (Exception e)
                {
                    sw.Stop();
                    ApplicationData.Instance.StatusText = string.Format("{0}: {1}", Localization.TranslationManager.Instance.Translate("Error"), e.Message);
                }
            });
        }

    }
}
