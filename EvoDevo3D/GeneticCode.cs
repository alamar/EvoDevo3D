using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;


namespace EvoDevo4
{
    public partial class GeneticCode : Form
    {

        private string fileName = "";
        public GeneticCode()
        {
            
            InitializeComponent();
            Label lblAction = new Label();
            lblAction.Name = "action";
            lblAction.Text = "Actions";
            lblAction.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            lblAction.Width = 140;
            lblAction.TextAlign = ContentAlignment.MiddleCenter;
            lblAction.FlatStyle = FlatStyle.Popup;
            lblAction.Padding = Padding.Empty;
            lblAction.Margin = Padding.Empty;
            flpCodeHelpers.Controls.Add(lblAction);
            //rtCode.AutoComplete.List.Clear();
            //rtCode.AutoComplete.ListSeparator = ' ';
            //rtCode.AutoComplete.ListString = "";
            foreach (string action in Cell.MemberMethods.Keys)
            {
                Button btnAction = new Button();
                btnAction.Name = action;
                btnAction.Text = action;
                btnAction.Tag = Cell.MemberMethods[action] + "\n";
                btnAction.Width = 140;
                btnAction.FlatStyle = FlatStyle.Popup;
                btnAction.Padding = Padding.Empty;
                btnAction.Margin = Padding.Empty;
                btnAction.Font = new Font(FontFamily.GenericSansSerif, 7);
                btnAction.Padding = Padding.Empty;
                btnAction.Margin = Padding.Empty;
                btnAction.Click += new EventHandler(btnAction_Click);
                btnAction.FlatStyle = FlatStyle.Flat;
                btnAction.Height = 22;
                flpCodeHelpers.Controls.Add(btnAction);
                //rtCode.AutoComplete.ListString+=(action.Trim()+";");
                //rtCode.AutoComplete.List.Add(action.Trim());
            }
            Label lblProperties = new Label();
            lblProperties.Name = "aproperties";
            lblProperties.Text = "Properties";
            lblProperties.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            lblProperties.Width = 140;
            lblProperties.TextAlign = ContentAlignment.MiddleCenter;
            lblProperties.FlatStyle = FlatStyle.Popup;
            lblProperties.Padding = Padding.Empty;
            lblProperties.Margin = Padding.Empty;
            flpCodeHelpers.Controls.Add(lblProperties);
            foreach (string action in Cell.MemberProperties.Keys)
            {
                Button btnAction = new Button();
                btnAction.Name = action;
                btnAction.Text = action;
                btnAction.Tag = Cell.MemberProperties[action];
                btnAction.Width = 140;
                btnAction.FlatStyle = FlatStyle.Popup;
                btnAction.Padding = Padding.Empty;
                btnAction.Margin = Padding.Empty;
                btnAction.Font = new Font(FontFamily.GenericSansSerif, 7);
                btnAction.Padding = Padding.Empty;
                btnAction.Margin = Padding.Empty;
                btnAction.Click += new EventHandler(btnAction_Click);
                btnAction.FlatStyle = FlatStyle.Flat;
                btnAction.Height = 22;
                flpCodeHelpers.Controls.Add(btnAction);
                //rtCode.AutoComplete.ListString+=(action.Trim()+";");
                //rtCode.AutoComplete.List.Add(action.Trim());
            }
            
        }

        void btnAction_Click(object sender, EventArgs e)
        {
            string invocation = "";
            invocation += ((string)(((Button)sender).Tag));
            //rtCode.Selection.Text = invocation;
            rtCode.Focus();
        }

        public void btnCompile_Click(object sender, EventArgs e)
        {
            World.Instance.paused = true;
            Cell.GeneticCode = rtCode.Text;
            if (Cell.Recompile())
            {
                World.Instance.Reset();
            }
        }
        public static CompilerResults CompileScript(string Source, string Reference, CodeDomProvider Provider)
        {
            CompilerParameters parms = new CompilerParameters();
            CompilerResults results;
            // Configure parameters
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = true;
            parms.IncludeDebugInformation = false;
            if (Reference != null && Reference.Length != 0)
                parms.ReferencedAssemblies.Add(Reference);
            parms.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parms.ReferencedAssemblies.Add("System.dll");
            parms.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            // Compile
            results = Provider.CompileAssemblyFromSource(parms, Source);
            return results;
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Genetic Programs (*.gp)|*.gp";
            saveFileDialog1.DefaultExt = "gp";
            
            DialogResult dr = saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName.Length > 3)
            {
                using (FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    AddText(fs, rtCode.Text);
                }
                fileName = saveFileDialog1.FileName;
            }
        }
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public void btnLoad_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Filter = "Genetic Programs (*.gp)|*.gp";
            if (openFileDialog1.ShowDialog()!=DialogResult.OK) return;
            if (openFileDialog1.FileName.Length>3)
            {
                fileName = openFileDialog1.FileName;
                rtCode.Text = "";
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                {
                    rtCode.Text=sr.ReadToEnd();                    
                }
                this.Text = "Genetic Code - " + openFileDialog1.FileName;

                btnCompile.PerformClick();
            }
            
        }

        private void GeneticCode_Load(object sender, EventArgs e)
        {

        }

        private void GeneticCode_Resize(object sender, EventArgs e)
        {
            flpCodeHelpers.Height = this.Height - 70;
            rtCode.Height = this.Height - 70;
            rtCode.Width = this.Width - 183;

            btnCompile.Left = this.Width - btnCompile.Width - 10;
        }

        private void btnSaveNew_Click(object sender, EventArgs e)
        {
            if (fileName != "")
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    AddText(fs, rtCode.Text);
                }
            }
            else
            {
                btnSave_Click(sender, e);
            }
        }

       
    }
}
