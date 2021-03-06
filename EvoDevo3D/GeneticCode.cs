﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.CSharp;
using Troschuetz.Random;
using Troschuetz.Random.Generators;

namespace EvoDevo3D
{
    public partial class GeneticCode : Form
    {
        private EvoForm evoForm = null;

        private string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "code.gp");

        public GeneticCode()
        {
            InitializeComponent();
            this.components = new System.ComponentModel.Container();
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

            btnReroll_Click(this, null);

            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderWindow_FormClosing);
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

        void btnAction_Click(object sender, EventArgs e)
        {
            string invocation = "";
            invocation += ((string)(((Button)sender).Tag));
            //rtCode.Selection.Text = invocation;
            rtCode.Focus();
        }

        public void btnCompile_Click(object sender, EventArgs e)
        {
            Cell.GeneticCode = rtCode.Text;
            Cell.Recompile(str => MessageBox.Show(str));
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (evoForm != null && !evoForm.IsDisposed)
            {
                MessageBox.Show("Simulation already running!");
                return;
            }

            Cell.GeneticCode = rtCode.Text;
            Type compiledCell = Cell.Recompile(str => MessageBox.Show(str));
            if (compiledCell != null)
            {
                Cell.Program = new FileInfo(fileName);

                int seed = 0;
                Int32.TryParse(txtSeed.Text, out seed);
                Cell.Random = new TRandom(new XorShift128Generator(seed));

                evoForm = new EvoForm();
                evoForm.Simulation = new Simulation(compiledCell);
                evoForm.Show();
            }
        }

        private void RenderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
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
        
        public void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Genetic Programs (*.gp)|*.gp";
            saveFileDialog1.DefaultExt = "gp";
            
            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK && saveFileDialog1.FileName.Length > 3)
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

        private void btnReroll_Click(object sender, EventArgs e)
        {
            this.txtSeed.Text = new Random().Next(100000).ToString();
        }

        private void GeneticCode_Load(object sender, EventArgs e)
        {

        }

        private void GeneticCode_Resize(object sender, EventArgs e)
        {
            flpCodeHelpers.Height = this.Height - 40;
            rtCode.Height = this.Height - 40;
            rtCode.Width = this.Width - 183;

            btnCompile.Left = Math.Max(630, this.Width - btnCompile.Width - 10);
        }

    }
}
