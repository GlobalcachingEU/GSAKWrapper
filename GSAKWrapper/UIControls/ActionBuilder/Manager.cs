using GSAKWrapper.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class Manager
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        public ObservableCollection<ActionFlow> ActionFlows { get; set; }

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            ActionFlows = new ObservableCollection<ActionFlow>();
            loadFlows();
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance==null)
                {
                    lock(_lockObject)
                    {
                        if(_uniqueInstance==null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        private AsyncDelegateCommand _executeFlowCommand;
        public AsyncDelegateCommand ExecuteFlowCommand
        {
            get
            {
                if (_executeFlowCommand==null)
                {
                    _executeFlowCommand = new AsyncDelegateCommand(param => RunActionFow(param as ActionFlow));
                }
                return _executeFlowCommand;
            }
        }

        public async Task RunActionFow(ActionFlow af)
        {
            await Task.Run(() =>
                {
                    try
                    {
                        var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
                        if (System.IO.File.Exists(fn))
                        {
                            using (var db = new Database.DBConSqlite(fn))
                            {
                                runFlow(af, db);
                            }
                        }
                    }
                    catch
                    {
                    }
                });
        }

        private void runFlow(ActionFlow flow, Database.DBCon db)
        {
            try
            {
                int id = 0;
                foreach (ActionImplementation ai in flow.Actions)
                {
                    id++;
                    ai.PrepareRun(db, string.Format("gskwrp{0}", id));
                }

                //find start and run
                ActionStart startAction = (from a in flow.Actions where a is ActionStart select a).FirstOrDefault() as ActionStart;

                startAction.Run(null);

                foreach (ActionImplementation ai in flow.Actions)
                {
                    ai.FinalizeRun();
                }
            }
            catch(Exception e)
            {
            }
        }


        public void Save()
        {
            try
            {
                XmlDocument doc = createFlowXml(ActionFlows.ToList());
                StringBuilder sb = new StringBuilder();
                System.IO.TextWriter tr = new System.IO.StringWriter(sb);
                XmlTextWriter wr = new XmlTextWriter(tr);
                wr.Formatting = Formatting.None;
                doc.Save(wr);
                wr.Close();
                Settings.Settings.Default.ActionBuilderFlowsXml = sb.ToString();
            }
            catch
            {
            }
        }

        private void loadFlows()
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.Settings.Default.ActionBuilderFlowsXml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(Settings.Settings.Default.ActionBuilderFlowsXml);
                    getActionFlowsFromXml(doc);
                }
            }
            catch
            {
            }
        }

        private void getActionFlowsFromXml(XmlDocument doc)
        {
            List<ActionImplementation> allActionImpl = new List<ActionImplementation>();

            XmlNodeList nl = doc.SelectNodes("/flows/flow");
            foreach (XmlNode n in nl)
            {
                ActionFlow af = new ActionFlow();
                af.Name = n.Attributes["name"].Value;
                af.ID = n.Attributes["id"].Value;
                af.Actions = new List<ActionImplementation>();

                XmlNodeList al = n.SelectNodes("action");
                foreach (XmlNode a in al)
                {
                    Type t = Type.GetType(a.Attributes["type"].Value);
                    ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
                    ActionImplementation obj = (ActionImplementation)constructor.Invoke(null);
                    obj.ID = a.Attributes["id"].Value;
                    obj.Location = new System.Windows.Point((double)int.Parse(a.Attributes["x"].Value), (double)int.Parse(a.Attributes["y"].Value));

                    af.Actions.Add(obj);
                    allActionImpl.Add(obj);

                    XmlNodeList vl = a.SelectNodes("values/value");
                    foreach (XmlNode v in vl)
                    {
                        obj.Values.Add(v.InnerText);
                    }
                }

                ActionFlows.Add(af);
            }

            //all actions are created, now we can match the ID's for the connections.
            nl = doc.SelectNodes("/flows/flow/action");
            foreach (XmlNode n in nl)
            {
                ActionImplementation ai = (from ac in allActionImpl where ac.ID == n.Attributes["id"].Value select ac).FirstOrDefault();
                XmlNodeList conl;
                ActionImplementation.Operator op = ai.AllowOperators;
                if ((op & ActionImplementation.Operator.Equal) != 0)
                {
                    conl = n.SelectNodes("Equal/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.Equal);
                    }
                }
                if ((op & ActionImplementation.Operator.Larger) != 0)
                {
                    conl = n.SelectNodes("Larger/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.Larger);
                    }
                }
                if ((op & ActionImplementation.Operator.LargerOrEqual) != 0)
                {
                    conl = n.SelectNodes("LargerOrEqual/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.LargerOrEqual);
                    }
                }
                if ((op & ActionImplementation.Operator.Less) != 0)
                {
                    conl = n.SelectNodes("Less/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.Less);
                    }
                }
                if ((op & ActionImplementation.Operator.LessOrEqual) != 0)
                {
                    conl = n.SelectNodes("LessOrEqual/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.LessOrEqual);
                    }
                }
                if ((op & ActionImplementation.Operator.NotEqual) != 0)
                {
                    conl = n.SelectNodes("NotEqual/ID");
                    foreach (XmlNode con in conl)
                    {
                        ai.ConnectToOutput((from ac in allActionImpl where ac.ID == con.InnerText select ac).FirstOrDefault(), ActionImplementation.Operator.NotEqual);
                    }
                }
            }
        }


        private XmlDocument createFlowXml(List<ActionFlow> flows)
        {
            XmlDocument doc = new XmlDocument();
            //actionBuilderEditor1.CommitData();
            try
            {
                XmlElement root = doc.CreateElement("flows");
                doc.AppendChild(root);
                foreach (ActionFlow af in flows)
                {
                    XmlElement q = doc.CreateElement("flow");
                    XmlAttribute attr = doc.CreateAttribute("name");
                    XmlText txt = doc.CreateTextNode(af.Name);
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    attr = doc.CreateAttribute("id");
                    txt = doc.CreateTextNode(af.ID);
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    root.AppendChild(q);
                    foreach (ActionImplementation ai in af.Actions)
                    {
                        XmlElement r = doc.CreateElement("action");
                        q.AppendChild(r);

                        attr = doc.CreateAttribute("type");
                        txt = doc.CreateTextNode(ai.GetType().ToString());
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);

                        attr = doc.CreateAttribute("id");
                        txt = doc.CreateTextNode(ai.ID);
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);

                        attr = doc.CreateAttribute("x");
                        txt = doc.CreateTextNode(((int)ai.Location.X).ToString());
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);

                        attr = doc.CreateAttribute("y");
                        txt = doc.CreateTextNode(((int)ai.Location.Y).ToString());
                        attr.AppendChild(txt);
                        r.Attributes.Append(attr);

                        XmlElement v = doc.CreateElement("values");
                        r.AppendChild(v);
                        foreach (string s in ai.Values)
                        {
                            XmlElement val = doc.CreateElement("value");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(s);
                            val.AppendChild(txt);
                        }

                        List<ActionImplementation> actImpl = ai.GetOutputConnections(ActionImplementation.Operator.Equal);
                        v = doc.CreateElement("Equal");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                        actImpl = ai.GetOutputConnections(ActionImplementation.Operator.Larger);
                        v = doc.CreateElement("Larger");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                        actImpl = ai.GetOutputConnections(ActionImplementation.Operator.LargerOrEqual);
                        v = doc.CreateElement("LargerOrEqual");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                        actImpl = ai.GetOutputConnections(ActionImplementation.Operator.Less);
                        v = doc.CreateElement("Less");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                        actImpl = ai.GetOutputConnections(ActionImplementation.Operator.LessOrEqual);
                        v = doc.CreateElement("LessOrEqual");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                        actImpl = ai.GetOutputConnections(ActionImplementation.Operator.NotEqual);
                        v = doc.CreateElement("NotEqual");
                        r.AppendChild(v);
                        foreach (ActionImplementation act in actImpl)
                        {
                            XmlElement val = doc.CreateElement("ID");
                            v.AppendChild(val);

                            txt = doc.CreateTextNode(act.ID);
                            val.AppendChild(txt);
                        }

                    }
                }
            }
            catch
            {
            }
            return doc;
        }

    }
}
