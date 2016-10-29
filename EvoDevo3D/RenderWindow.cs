using System;
using SD = System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EvoDevo4.Support;


namespace EvoDevo4
{
    public class RenderWindow : Form
    {
        private System.ComponentModel.IContainer components;
        private ArrayList celllist = new ArrayList();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private int WIDTH = 256;
        private int HEIGHT = 256;
        public bool screenshotAwaiting = false;
        public bool rendering = false;
        private int frameNo = 0;
        private string screenshotFile = @"screenshot.bmp";
        private float turnAxis1=0;
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
        private int cellSelectionIndex;
        private ToolStripLabel lblProcess;
        private ToolStripLabel lblCells;
        private ToolStripLabel lblVisible;


        private Thread heartbeatThread;
        private EvoArea evoArea;
        private Thread evoAreaThread;

        /// <summary>
        /// Creates new Render window instance;
        /// </summary>
        public RenderWindow()
        {

            InitializeComponent();

            GeneticCode gc = new GeneticCode();
            gc.Show();

            evoArea = new EvoArea();
            evoAreaThread = new Thread(evoArea.Run);
            evoAreaThread.Start();

            tmFPSChecker.Start();
            heartbeatThread = new Thread(World.Instance.ActionsManager);
            heartbeatThread.Start();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            gc.BringToFront();
        }

        /*void timer_Tick(object sender, EventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
        }*/

        /*protected override void OnMouseClick(MouseEventArgs e)
        {
            World.Instance.ReTarget(mouseRay, mousePos);
            base.OnMouseClick(e);
        }
            
        public int frames;
        public DateTime initTime = DateTime.Now;

        void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            deviceBlock = true;
        }*/

        private bool nowPainting = false;
        /*protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Thread drawthread = new Thread(Draw);
            if (!nowPainting)
            {
                nowPainting = true;
                drawthread.Start();
            }
            base.OnPaint(e);
        }*/

        static Random random = new Random();
        
        private void InitializeComponent()
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
            // toolStrip1
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
            this.tsbStep.Text = "Step Froward";
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
                //this.chb0Visible.Location = new System.Drawing.Point(0, 28);
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
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenderWindow";
            this.Text = "EvoDevo 4";
            //this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderWindow_FormClosing);
            this.renderToolStrip.ResumeLayout(false);
            this.renderToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /*private void RenderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }*/

        private void tsbPlay_Click(object sender, EventArgs e)
        {
            World.Instance.paused = !World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
            tsbPlay.Enabled = World.Instance.paused;
        }

        private void tsbPause_Click(object sender, EventArgs e)
        {
            World.Instance.paused = !World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
            tsbPlay.Enabled = World.Instance.paused;
        }

        private void tsbStep_Click(object sender, EventArgs e)
        {
            World.Instance.AwaitingQueue.Enqueue('s');
            World.Instance.paused = false;
            World.Instance.newActionAllowed = true;
            tsbPause.Enabled = false;
            tsbPlay.Enabled = true;
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
                World.Instance.Reset();
            }
        }

        private void tmWorldHeartbeat_Tick(object sender, EventArgs e)
        {
            
            World.Instance.newActionAllowed = true;
        }

        private void tmFPSChecker_Tick(object sender, EventArgs e)
        {
            lblProcess.Text = "Process: " + World.Instance.state;
            lblCells.Text = "Cells: " + World.Instance.Cells.Count;
            tsbPlay.Enabled = World.Instance.paused;
            tsbPause.Enabled = !World.Instance.paused;
        }

        private void RenderWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }


    }
}
