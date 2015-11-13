using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using GSAKWrapper.Excel;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionExportExcel : ActionExport
    {
        public class ExportExcelSettings
        {
            public string FileName { get; set; }
            public List<Excel.Sheet> Sheets { get; set; }

            public ExportExcelSettings()
            {
                FileName = "";
                Sheets = new List<Excel.Sheet>();
            }
        }

        public const string STR_NAME = "ExportExcel";
        public ActionExportExcel()
            : base(STR_NAME)
        {
        }

        private ExportExcelSettings GetSettingsFromValue(string v)
        {
            var result = new ExportExcelSettings();
            if (!string.IsNullOrEmpty(v))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(v);

                    XmlNodeList nl = doc.SelectNodes("/settings/setting");
                    foreach (XmlNode n in nl)
                    {
                        switch (n.Attributes["name"].Value)
                        {
                            case "FileName":
                                result.FileName = n.Attributes["value"].Value;
                                break;
                        }
                    }
                    nl = doc.SelectNodes("/settings/sheet");
                    foreach (XmlNode n in nl)
                    {
                        var sheet = new Excel.Sheet();
                        sheet.Name = n.Attributes["name"].Value;
                        var parts = n.Attributes["items"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var p in parts)
                        {
                            var pi = (from a in Excel.PropertyItem.PropertyItems where a.GetType().ToString() == p select a).FirstOrDefault();
                            if (pi != null)
                            {
                                sheet.SelectedItems.Add(pi);
                            }
                        }
                        result.Sheets.Add(sheet);
                    }
                }
                catch
                {
                    result = null;
                }
            }
            return result;
        }

        private string GetSettingsFromValue(ExportExcelSettings s)
        {
            var result = "";
            if (s != null)
            {
                if (s != null)
                {
                    var doc = new XmlDocument();
                    var root = doc.CreateElement("settings");
                    doc.AppendChild(root);

                    var q = doc.CreateElement("setting");
                    var attr = doc.CreateAttribute("name");
                    var txt = doc.CreateTextNode("FileName");
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    attr = doc.CreateAttribute("value");
                    txt = doc.CreateTextNode(s.FileName ?? "");
                    attr.AppendChild(txt);
                    q.Attributes.Append(attr);
                    root.AppendChild(q);

                    foreach (var sheet in s.Sheets)
                    {
                        q = doc.CreateElement("sheet");
                        attr = doc.CreateAttribute("name");
                        txt = doc.CreateTextNode(sheet.Name);
                        attr.AppendChild(txt);
                        q.Attributes.Append(attr);
                        attr = doc.CreateAttribute("items");
                        txt = doc.CreateTextNode(string.Join(",",(from a in sheet.SelectedItems select a.GetType().ToString()).ToArray()));
                        attr.AppendChild(txt);
                        q.Attributes.Append(attr);
                        root.AppendChild(q);
                    }

                    var sb = new StringBuilder();
                    var tr = new System.IO.StringWriter(sb);
                    var wr = new XmlTextWriter(tr);
                    wr.Formatting = Formatting.None;
                    doc.Save(wr);
                    wr.Close();

                    result = sb.ToString();
                }
            }
            return result;
        }

        protected override string SetSettings(string currentSettings)
        {
            var s = GetSettingsFromValue(currentSettings);
            var dlg = new Dialogs.WindowExportExcelSettings(s);
            dlg.ShowDialog();
            return GetSettingsFromValue(s);
        }

        protected override object PrepareSettings(string settings)
        {
            return GetSettingsFromValue(settings as string);
        }

        protected override void PerformExport(object settings)
        {
            var excelSetting = settings as ExportExcelSettings;
            if (excelSetting != null && !string.IsNullOrEmpty(excelSetting.FileName))
            {
                using (FileStream stream = new FileStream(excelSetting.FileName, FileMode.Create, FileAccess.Write))
                {
                    IWorkbook wb = new XSSFWorkbook();
                    using (var db = new NPoco.Database(this.DatabaseConnection.Connection, NPoco.DatabaseType.SQLite))
                    {
                        List<string> geocaches = db.Fetch<string>(string.Format("select Caches.Code from Caches inner join {0} on Caches.Code={0}.gccode", ActionInputTableName));

                        var nextUpdate = DateTime.Now.AddSeconds(2);
                        using (Utils.ProgressBlock progress = new Utils.ProgressBlock("ExportExcel", "CreatingFile", geocaches.Count, 0, true))
                        {
                            foreach (var sh in excelSetting.Sheets)
                            {
                                var sheet = wb.CreateSheet(sh.Name);
                                var r = sheet.GetOrCreateRow(0);
                                for (int c = 0; c < sh.SelectedItems.Count; c++)
                                {
                                    var cell = r.GetOrCreateCol(c);
                                    cell.SetCellValue(sh.SelectedItems[c].Text);
                                }
                            }
                            for (int index = 0; index < geocaches.Count; index++)
                            {
                                var gc = db.Fetch<DataTypes.GSAKCaches, DataTypes.GSAKCacheMemo, DataTypes.GSAKCorrected, Excel.GeocacheData>((a, b, c) => { return new Excel.GeocacheData() { Caches = a, CacheMemo = b, Corrected = c }; }, string.Format("select Caches.*, CacheMemo.*, Corrected.* from Caches inner join {0} on Caches.Code={0}.gccode left join CacheMemo on Caches.Code=CacheMemo.Code left join Corrected on Caches.Code=Corrected.kCode where Caches.Code=@0", ActionInputTableName), geocaches[index]).FirstOrDefault();
                                foreach (var sh in excelSetting.Sheets)
                                {
                                    var sheet = wb.GetSheet(sh.Name);
                                    var r = sheet.GetOrCreateRow(index+1);
                                    for (int c = 0; c < sh.SelectedItems.Count; c++)
                                    {
                                        var cell = r.GetOrCreateCol(c);
                                        var obj = sh.SelectedItems[c].GetValue(gc);
                                        if (obj == null)
                                        {
                                        }
                                        else
                                        {
                                            var t = obj.GetType();
                                            if (t== typeof(bool) || t==typeof(bool?))
                                            {
                                                cell.SetCellValue((bool)obj);
                                            }
                                            else if (t == typeof(int) || t == typeof(int?))
                                            {
                                                cell.SetCellValue((int)obj);
                                            }
                                            else if (t == typeof(string))
                                            {
                                                cell.SetCellValue((string)obj);
                                            }
                                            else if (t == typeof(DateTime) || t == typeof(DateTime?))
                                            {
                                                cell.SetCellValue((DateTime)obj);
                                            }
                                            else if (t == typeof(double) || t == typeof(double?))
                                            {
                                                cell.SetCellValue((double)obj);
                                            }
                                            else if (t == typeof(float) || t == typeof(float?))
                                            {
                                                cell.SetCellValue(Convert.ToDouble(obj));
                                            }
                                            else
                                            {
                                                cell.SetCellValue(obj.ToString());
                                            }
                                        }
                                    }
                                }

                                if (DateTime.Now >= nextUpdate)
                                {
                                    if (!progress.Update("CreatingFile", geocaches.Count, index + 1))
                                    {
                                        break;
                                    }
                                    nextUpdate = DateTime.Now.AddSeconds(1);
                                }
                            }
                        }
                    }
                    wb.Write(stream);
                }
            }
        }
    }
}
