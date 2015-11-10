using GSAKWrapper.UIControls.FormulaSolver.FormulaInterpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

namespace GSAKWrapper.UIControls.FormulaSolver
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private FormulaInterpreter.FormulaInterpreter _interpreter;

        public Control()
        {
            using (var strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("GSAKWrapper.UIControls.FormulaSolver.Grammar.SingleLineFormula.egt"))
            using (System.IO.BinaryReader br = new System.IO.BinaryReader(strm))
            {
                _interpreter = new FormulaInterpreter.FormulaInterpreter(br);
            }

            InitializeComponent();
            DataContext = this;

            UpdateView();
        }

        private void UpdateView()
        {
            tbSolutions.Text = "";
            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode))
            {
                tbFormula.Text = Settings.Settings.Default.GetFormula(Settings.Settings.Default.ActiveGeocacheCode) ?? "";
            }
            else
            {
                tbFormula.Text = Settings.Settings.Default.GetFormula("NOTE") ?? "";
            }
        }

        private void saveData()
        {
            if (!string.IsNullOrEmpty(Settings.Settings.Default.ActiveGeocacheCode))
            {
                Settings.Settings.Default.SetFormula(Settings.Settings.Default.ActiveGeocacheCode, tbFormula.Text);
            }
            else
            {
                Settings.Settings.Default.SetFormula("NOTE", tbFormula.Text);
            }
        }

        public override string ToString()
        {
            return Localization.TranslationManager.Instance.Translate("FormulaSolver") as string;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveData();

            String formula = tbFormula.Text;
            tbSolutions.Text = "";
            if (formula.Length > 0)
            {
                string[] lines = formula.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                StringBuilder sb = new StringBuilder();
                ExecutionContext ctx = new ExecutionContext();

                foreach (string line in lines)
                {
                    if (line.Length == 0)
                    {
                        sb.AppendLine("");
                    }
                    else
                    {
                        string res = Convert.ToString(_interpreter.Exec(line, ctx), new System.Globalization.CultureInfo(""));
                        sb.AppendLine(res);
                    }
                }

                if (ctx.HasMissingVariables())
                {
                    string text = string.Format(StrRes.GetString(StrRes.STR_MISSING_VARIABLES), String.Join(", ", ctx.GetMissingVariableNames()));
                    //if (MessageBox.Show(text, "Formula Solver", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        StringBuilder sbInput = new StringBuilder();
                        foreach (string name in ctx.GetMissingVariableNames())
                        {
                            sbInput.AppendLine(name + "=");
                        }
                        foreach (string line in lines)
                        {
                            sbInput.AppendLine(line);
                        }

                        tbFormula.Text = sbInput.ToString();
                    }
                    sb.Clear();
                }
                tbSolutions.Text = sb.ToString();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            InsertFormulaWindow dlg = new InsertFormulaWindow();
            if (dlg.ShowDialog()==true)
            {
                UpdateFormulaText(dlg.SelectedFunction, -1);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            WaypointSelectorWindow dlg = new WaypointSelectorWindow();
            if (dlg.ShowDialog()==true)
            {
                UpdateFormulaText(string.Format("WP(\"{0}\")", dlg.SelectedWaypoint), 0);
            }
        }

        private void UpdateFormulaText(string txt, int moveCursorTo)
        {
            tbFormula.Focus();
            var selectionIndex = tbFormula.SelectionStart;
            tbFormula.Text = tbFormula.Text.Insert(selectionIndex, txt);
            tbFormula.SelectionStart = selectionIndex + Math.Max(0, txt.Length + moveCursorTo);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //not implemented yet
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            UserHelpWindow dlg = new UserHelpWindow();
            dlg.ShowDialog();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            saveData();
        }


    }
}
