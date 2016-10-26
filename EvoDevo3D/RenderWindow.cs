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
        private ToolStrip toolStrip1;
        private ToolStripButton tsbPlay;
        private ToolStripButton tsbPause;
        private ToolStripButton tsbStep;
        private ToolStripButton tsbSnapshot;
        private ToolStripButton tsbVideo;
        private ToolStripButton tsbClear;
        private System.Windows.Forms.Timer tmFPSChecker;
        private System.Windows.Forms.Timer tmWorldHeartbeat;
        private CheckBox chb0Visible;
        private CheckBox chb1Visible;
        private CheckBox chb2Visible;
        private CheckBox chb3Visible;
        private CheckBox chb4Visible;
        private CheckBox chb5Visible;
        private CheckBox chb6Visible;
        private CheckBox chb7Visible;
        private CheckBox chb8Visible;
        private CheckBox chb9Visible;
        private int cellSelectionIndex;
        private ToolStripLabel lblProcess;
        private ToolStripLabel lblCells;


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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSnapshot = new System.Windows.Forms.ToolStripButton();
            this.tsbVideo = new System.Windows.Forms.ToolStripButton();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStep = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tmFPSChecker = new System.Windows.Forms.Timer(this.components);
            this.tmWorldHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.chb0Visible = new System.Windows.Forms.CheckBox();
            this.chb1Visible = new System.Windows.Forms.CheckBox();
            this.chb2Visible = new System.Windows.Forms.CheckBox();
            this.chb3Visible = new System.Windows.Forms.CheckBox();
            this.chb4Visible = new System.Windows.Forms.CheckBox();
            this.chb5Visible = new System.Windows.Forms.CheckBox();
            this.chb6Visible = new System.Windows.Forms.CheckBox();
            this.chb7Visible = new System.Windows.Forms.CheckBox();
            this.chb8Visible = new System.Windows.Forms.CheckBox();
            this.chb9Visible = new System.Windows.Forms.CheckBox();
            this.lblProcess = new System.Windows.Forms.ToolStripLabel();
            this.lblCells = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSnapshot,
            this.tsbVideo,
            this.tsbPlay,
            this.tsbPause,
            this.tsbStep,
            this.tsbClear,
            this.lblProcess,
            this.lblCells});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(792, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
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
            // chb0Visible
            // 
            this.chb0Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb0Visible.AutoSize = true;
            this.chb0Visible.Checked = true;
            this.chb0Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb0Visible.Location = new System.Drawing.Point(0, 28);
            this.chb0Visible.Name = "chb0Visible";
            this.chb0Visible.Size = new System.Drawing.Size(23, 23);
            this.chb0Visible.TabIndex = 1;
            this.chb0Visible.TabStop = false;
            this.chb0Visible.Text = "0";
            this.chb0Visible.UseVisualStyleBackColor = true;
            this.chb0Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb1Visible
            // 
            this.chb1Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb1Visible.AutoSize = true;
            this.chb1Visible.Checked = true;
            this.chb1Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb1Visible.Location = new System.Drawing.Point(0, 50);
            this.chb1Visible.Name = "chb1Visible";
            this.chb1Visible.Size = new System.Drawing.Size(23, 23);
            this.chb1Visible.TabIndex = 2;
            this.chb1Visible.TabStop = false;
            this.chb1Visible.Text = "1";
            this.chb1Visible.UseVisualStyleBackColor = true;
            this.chb1Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb2Visible
            // 
            this.chb2Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb2Visible.AutoSize = true;
            this.chb2Visible.Checked = true;
            this.chb2Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb2Visible.Location = new System.Drawing.Point(0, 72);
            this.chb2Visible.Name = "chb2Visible";
            this.chb2Visible.Size = new System.Drawing.Size(23, 23);
            this.chb2Visible.TabIndex = 3;
            this.chb2Visible.TabStop = false;
            this.chb2Visible.Text = "2";
            this.chb2Visible.UseVisualStyleBackColor = true;
            this.chb2Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb3Visible
            // 
            this.chb3Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb3Visible.AutoSize = true;
            this.chb3Visible.Checked = true;
            this.chb3Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb3Visible.Location = new System.Drawing.Point(0, 94);
            this.chb3Visible.Name = "chb3Visible";
            this.chb3Visible.Size = new System.Drawing.Size(23, 23);
            this.chb3Visible.TabIndex = 4;
            this.chb3Visible.TabStop = false;
            this.chb3Visible.Text = "3";
            this.chb3Visible.UseVisualStyleBackColor = true;
            this.chb3Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb4Visible
            // 
            this.chb4Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb4Visible.AutoSize = true;
            this.chb4Visible.Checked = true;
            this.chb4Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb4Visible.Location = new System.Drawing.Point(0, 116);
            this.chb4Visible.Name = "chb4Visible";
            this.chb4Visible.Size = new System.Drawing.Size(23, 23);
            this.chb4Visible.TabIndex = 5;
            this.chb4Visible.TabStop = false;
            this.chb4Visible.Text = "4";
            this.chb4Visible.UseVisualStyleBackColor = true;
            this.chb4Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb5Visible
            // 
            this.chb5Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb5Visible.AutoSize = true;
            this.chb5Visible.Checked = true;
            this.chb5Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb5Visible.Location = new System.Drawing.Point(0, 138);
            this.chb5Visible.Name = "chb5Visible";
            this.chb5Visible.Size = new System.Drawing.Size(23, 23);
            this.chb5Visible.TabIndex = 6;
            this.chb5Visible.TabStop = false;
            this.chb5Visible.Text = "5";
            this.chb5Visible.UseVisualStyleBackColor = true;
            this.chb5Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb6Visible
            // 
            this.chb6Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb6Visible.AutoSize = true;
            this.chb6Visible.Checked = true;
            this.chb6Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb6Visible.Location = new System.Drawing.Point(0, 160);
            this.chb6Visible.Name = "chb6Visible";
            this.chb6Visible.Size = new System.Drawing.Size(23, 23);
            this.chb6Visible.TabIndex = 7;
            this.chb6Visible.TabStop = false;
            this.chb6Visible.Text = "6";
            this.chb6Visible.UseVisualStyleBackColor = true;
            this.chb6Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb7Visible
            // 
            this.chb7Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb7Visible.AutoSize = true;
            this.chb7Visible.Checked = true;
            this.chb7Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb7Visible.Location = new System.Drawing.Point(0, 182);
            this.chb7Visible.Name = "chb7Visible";
            this.chb7Visible.Size = new System.Drawing.Size(23, 23);
            this.chb7Visible.TabIndex = 8;
            this.chb7Visible.TabStop = false;
            this.chb7Visible.Text = "7";
            this.chb7Visible.UseVisualStyleBackColor = true;
            this.chb7Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb8Visible
            // 
            this.chb8Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb8Visible.AutoSize = true;
            this.chb8Visible.Checked = true;
            this.chb8Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb8Visible.Location = new System.Drawing.Point(0, 204);
            this.chb8Visible.Name = "chb8Visible";
            this.chb8Visible.Size = new System.Drawing.Size(23, 23);
            this.chb8Visible.TabIndex = 9;
            this.chb8Visible.TabStop = false;
            this.chb8Visible.Text = "8";
            this.chb8Visible.UseVisualStyleBackColor = true;
            this.chb8Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // chb9Visible
            // 
            this.chb9Visible.Appearance = System.Windows.Forms.Appearance.Button;
            this.chb9Visible.AutoSize = true;
            this.chb9Visible.Checked = true;
            this.chb9Visible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb9Visible.Location = new System.Drawing.Point(0, 226);
            this.chb9Visible.Name = "chb9Visible";
            this.chb9Visible.Size = new System.Drawing.Size(23, 23);
            this.chb9Visible.TabIndex = 10;
            this.chb9Visible.TabStop = false;
            this.chb9Visible.Text = "9";
            this.chb9Visible.UseVisualStyleBackColor = true;
            this.chb9Visible.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chb9Visible_KeyDown);
            // 
            // lblProcess
            // 
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(86, 22);
            this.lblProcess.Text = "Process: NONE";
            // 
            // lblCells
            // 
            this.lblCells.Name = "lblCells";
            this.lblCells.Size = new System.Drawing.Size(44, 22);
            this.lblCells.Text = "Cells: 1";
            // 
            // RenderWindow
            // 
            this.ClientSize = new System.Drawing.Size(792, 742);
            this.Controls.Add(this.chb9Visible);
            this.Controls.Add(this.chb8Visible);
            this.Controls.Add(this.chb7Visible);
            this.Controls.Add(this.chb6Visible);
            this.Controls.Add(this.chb5Visible);
            this.Controls.Add(this.chb4Visible);
            this.Controls.Add(this.chb3Visible);
            this.Controls.Add(this.chb2Visible);
            this.Controls.Add(this.chb1Visible);
            this.Controls.Add(this.chb0Visible);
            this.Controls.Add(this.toolStrip1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenderWindow";
            this.Text = "EvoDevo 4";
            //this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseUp);
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RenderWindow_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenderWindow_FormClosing);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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

        private void chb9Visible_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
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
