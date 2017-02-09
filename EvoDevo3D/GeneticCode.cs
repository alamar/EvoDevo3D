using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;
using Microsoft.CSharp;
using EvoDevo4.Support;


namespace EvoDevo4
{
    public partial class GeneticCode : Form
    {
        public bool screenshotAwaiting = false;
        public bool rendering = false;
        private ToolStrip renderToolStrip;
        private ToolStripButton tsbPlay;
        private ToolStripButton tsbPause;
        private ToolStripButton tsbStep;
        private ToolStripButton tsbSnapshot;
        private ToolStripButton tsbVideo;
        private ToolStripButton tsbClear;
        private System.Windows.Forms.Timer tmFPSChecker;
        private System.Windows.Forms.Timer tmWorldHeartbeat;
        private ToolStripCheckBox[] chbVisible;
        private ToolStripLabel lblProcess;
        private ToolStripLabel lblCells;
        private ToolStripLabel lblVisible;

        private Session runningSession;

        private string fileName = "";

        public GeneticCode()
        {
            InitializeComponent();
            InitializeToolStrip();
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

            tmFPSChecker.Start();

            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderWindow_FormClosing);
        }

        public bool runs() {
            return runningSession != null;
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

        private void InitializeToolStrip()
        {
            this.components = new System.ComponentModel.Container();
            this.renderToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbSnapshot = new System.Windows.Forms.ToolStripButton();
            this.tsbVideo = new System.Windows.Forms.ToolStripButton();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStep = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tmFPSChecker = new System.Windows.Forms.Timer(this.components);
            this.tmWorldHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.chbVisible = new ToolStripCheckBox[10];
            for (int i = 0; i < chbVisible.Length; i++)
            {
                this.chbVisible[i] = new ToolStripCheckBox();
            }
            this.lblProcess = new System.Windows.Forms.ToolStripLabel();
            this.lblCells = new System.Windows.Forms.ToolStripLabel();
            this.lblVisible = new System.Windows.Forms.ToolStripLabel();
            this.renderToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // renderToolStrip
            // 
            this.renderToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                    this.tsbSnapshot,
                    this.tsbVideo,
                    this.tsbPlay,
                    this.tsbPause,
                    this.tsbStep,
                    this.tsbClear,
                    this.lblProcess,
                    this.lblCells});
            this.renderToolStrip.Items.AddRange(chbVisible.Reverse().ToArray());
            this.renderToolStrip.Items.Add(this.lblVisible);
            this.renderToolStrip.Location = new System.Drawing.Point(0, 0);
            this.renderToolStrip.Name = "renderToolStrip";
            this.renderToolStrip.Size = new System.Drawing.Size(792, 25);
            this.renderToolStrip.TabIndex = 0;
            // 
            // tsbSnapshot
            // 
            this.tsbSnapshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSnapshot.Image = global::EvoDevo4.Properties.Resources.snapshot;
            this.tsbSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSnapshot.Name = "tsbSnapshot";
            this.tsbSnapshot.Size = new System.Drawing.Size(23, 22);
            this.tsbSnapshot.Text = "Snapshot";
            this.tsbSnapshot.Click += new System.EventHandler(this.tsbSnapshot_Click);
            // 
            // tsbVideo
            // 
            this.tsbVideo.CheckOnClick = true;
            this.tsbVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbVideo.Image = global::EvoDevo4.Properties.Resources.video;
            this.tsbVideo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbVideo.Name = "tsbVideo";
            this.tsbVideo.Size = new System.Drawing.Size(23, 22);
            this.tsbVideo.Text = "tsbVideo";
            this.tsbVideo.Click += new System.EventHandler(this.tsbVideo_Click);
            // 
            // tsbPlay
            // 
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Image = global::EvoDevo4.Properties.Resources.control_play;
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.Size = new System.Drawing.Size(23, 22);
            this.tsbPlay.Text = "Play";
            this.tsbPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.tsbPlay.Click += new System.EventHandler(this.tsbPlay_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPause.Enabled = false;
            this.tsbPause.Image = global::EvoDevo4.Properties.Resources.pause;
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(23, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // tsbStep
            // 
            this.tsbStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStep.Image = global::EvoDevo4.Properties.Resources.step;
            this.tsbStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStep.Name = "tsbStep";
            this.tsbStep.Size = new System.Drawing.Size(23, 22);
            this.tsbStep.Text = "Step Forward";
            this.tsbStep.Click += new System.EventHandler(this.tsbStep_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClear.Image = global::EvoDevo4.Properties.Resources.clear;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(23, 22);
            this.tsbClear.Text = "Clear";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // tmFPSChecker
            // 
            this.tmFPSChecker.Enabled = true;
            this.tmFPSChecker.Interval = 40;
            this.tmFPSChecker.Tick += new System.EventHandler(this.tmFPSChecker_Tick);
            // 
            // tmWorldHeartbeat
            // 
            this.tmWorldHeartbeat.Enabled = true;
            this.tmWorldHeartbeat.Interval = 50;
            this.tmWorldHeartbeat.Tick += new System.EventHandler(this.tmWorldHeartbeat_Tick);
            // 
            // chbVisible
            // 
            for (int i = 0; i < chbVisible.Length; i++)
            {
                this.chbVisible[i].CheckBox.Appearance = System.Windows.Forms.Appearance.Button;
                this.chbVisible[i].AutoSize = true;
                this.chbVisible[i].CheckBox.Checked = true;
                this.chbVisible[i].CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
                this.chbVisible[i].Name = "chbVisible" + i.ToString();
                this.chbVisible[i].Size = new System.Drawing.Size(23, 23);
                this.chbVisible[i].Text = i.ToString();
                this.chbVisible[i].Alignment = ToolStripItemAlignment.Right;
            }
            // 
            // lblProcess
            // 
            this.lblProcess.Name = "lblProcess";
            this.lblVisible.AutoSize = true;
            this.lblProcess.Text = "Process: NONE";
            // 
            // lblCells
            // 
            this.lblCells.Name = "lblCells";
            this.lblVisible.AutoSize = true;
            this.lblCells.Text = "Cells: 1";
            // 
            // lblVisible
            // 
            this.lblVisible.Name = "lblVisible";
            this.lblVisible.AutoSize = true;
            this.lblVisible.Alignment = ToolStripItemAlignment.Right;
            this.lblVisible.Text = "Toggle Cell Type Visibility:";
            // 
            // RenderWindow
            // 
            this.ClientSize = new System.Drawing.Size(792, 742);
            this.Controls.Add(this.renderToolStrip);
            this.renderToolStrip.ResumeLayout(false);
            this.renderToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
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
            if (Cell.Recompile())
            {
                runningSession = null;
            }
        }

        private void tsbPlay_Click(object sender, EventArgs e)
        {
            if (!runs()) {
                Simulation simulation = new Simulation();
                // XXX bad
                Thread heartbeatThread = new Thread(simulation.ActionsManager);
                heartbeatThread.IsBackground = true;
                heartbeatThread.Start();
                Thread evoAreaThread = new Thread(() => {
                    EvoArea evoArea = new EvoArea();
                    runningSession = new Session(this, simulation, evoArea);
                    evoArea.Session = runningSession;
                    runningSession.resume();
                    evoArea.Run();
                });
                evoAreaThread.IsBackground = true;
                evoAreaThread.Start();
            }

            tsbPause.Enabled = true;
            tsbPlay.Enabled = false;
        }

        private void tsbPause_Click(object sender, EventArgs e)
        {
            bool running = runningSession.toggle();
            tsbPause.Enabled = running;
            tsbPlay.Enabled = !running;
        }

        private void tsbStep_Click(object sender, EventArgs e)
        {
            if (runningSession != null)
            {
                runningSession.Simulation.AwaitingQueue.Enqueue('s');
                runningSession.resume();
                runningSession.Simulation.newActionAllowed = true;
                tsbPause.Enabled = false;
                tsbPlay.Enabled = true;
            }
        }

        private void tsbSnapshot_Click(object sender, EventArgs e)
        {
            screenshotAwaiting = true;
        }

        private void tsbVideo_Click(object sender, EventArgs e)
        {
            rendering = !rendering;
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset the world to initial state. Are you sure?", "EvoDevo IV", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                runningSession = null;
            }
        }

        public bool[] visibility()
        {
            bool[] visibility = new bool[chbVisible.Length];

            for (int i = 0; i < chbVisible.Length; i++)
            {
                visibility[i] = chbVisible[i].CheckBox.Checked;
            }
            return visibility;
        }

        private void tmWorldHeartbeat_Tick(object sender, EventArgs e)
        {
            if (runs())
            {
                // XXX WTF
                runningSession.Simulation.newActionAllowed = true;
            }
        }

        private void tmFPSChecker_Tick(object sender, EventArgs e)
        {
            if (runs())
            {
                lblProcess.Text = "Process: " + runningSession.Simulation.state;
                lblCells.Text = "Cells: " + runningSession.Simulation.Cells.Count + " Age: " + runningSession.Simulation.Cells[0].age;
                tsbPlay.Enabled = runningSession.Simulation.paused;
                tsbPause.Enabled = !runningSession.Simulation.paused;
            }
            else
            {
                tsbPlay.Enabled = true;
                tsbPause.Enabled = false;
            }
        }

        private void RenderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
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
