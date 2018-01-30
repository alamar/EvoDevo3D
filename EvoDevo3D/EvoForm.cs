using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using EvoDevo3D.Support;

namespace EvoDevo3D
{
    public class EvoForm : Form
    {
        //Controls.

        private ToolStrip renderToolStrip;
        private ToolStripButton tsbPause;
        private ToolStripButton tsbStep;
        private ToolStripButton tsbSnapshot;
        //private ToolStripButton tsbVideo;
        private ToolStripButton tsbClear;

        private ToolStripCheckBox[] chbVisible;
        private ToolStripLabel lblVisible;

        public EvoArea evoArea = new EvoArea();
        public Simulation Simulation
        {
            set
            {
                evoArea.Simulation = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeToolStrip();

            evoArea.Name = "evoArea";
            evoArea.Location = new System.Drawing.Point(0, renderToolStrip.Height);
            this.Controls.Add(evoArea);
            OnResize(null);
            KeyDown += evoArea.Keyboard_KeyDown;

            System.Windows.Forms.Timer refresher = new System.Windows.Forms.Timer();
            refresher.Interval = 250;
            refresher.Tick += new System.EventHandler(Refresh);
            refresher.Start();
        }

        private void InitializeToolStrip()
        {
            //this.components = new System.ComponentModel.Container();
            this.renderToolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbSnapshot = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStep = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.chbVisible = new ToolStripCheckBox[10];
            for (int i = 0; i < chbVisible.Length; i++)
            {
                this.chbVisible[i] = new ToolStripCheckBox();
            }
            this.lblVisible = new System.Windows.Forms.ToolStripLabel();
            this.renderToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // renderToolStrip
            // 
            this.renderToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbSnapshot,
                this.tsbPause,
                this.tsbStep,
                this.tsbClear});
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
            this.tsbSnapshot.Image = global::EvoDevo3D.Properties.Resources.snapshot;
            this.tsbSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSnapshot.Name = "tsbSnapshot";
            this.tsbSnapshot.Size = new System.Drawing.Size(23, 22);
            this.tsbSnapshot.Text = "Snapshot";
            this.tsbSnapshot.Click += new System.EventHandler(this.tsbSnapshot_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPause.Image = global::EvoDevo3D.Properties.Resources.pause;
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(23, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // tsbStep
            // 
            this.tsbStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStep.Image = global::EvoDevo3D.Properties.Resources.step;
            this.tsbStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStep.Name = "tsbStep";
            this.tsbStep.Size = new System.Drawing.Size(23, 22);
            this.tsbStep.Text = "Step Forward";
            this.tsbStep.Click += new System.EventHandler(this.tsbStep_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClear.Image = global::EvoDevo3D.Properties.Resources.clear;
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(23, 22);
            this.tsbClear.Text = "Clear";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
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
                this.chbVisible[i].CheckBox.CheckedChanged +=
                    this.chbVisible_CheckedChanged(i);
            }
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
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.renderToolStrip);
            this.renderToolStrip.ResumeLayout(false);
            this.renderToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void tsbPause_Click(object sender, EventArgs e)
        {
            evoArea.TogglePause();
            this.tsbPause.Image = global::EvoDevo3D.Properties.Resources.pause;
        }

        private void tsbStep_Click(object sender, EventArgs e)
        {
            evoArea.Step();
        }

        private void tsbSnapshot_Click(object sender, EventArgs e)
        {
            evoArea.Screenshot();
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will end the simulation. Are you sure?", "EvoDevo IV",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Dispose();
            }
        }

        public System.EventHandler chbVisible_CheckedChanged(int i)
        {
            return new System.EventHandler((sender, e) =>
            {
                evoArea.SetVisibility(i, ((CheckBox)sender).Checked);
            });
        }

        public void Resize()
        {
            evoArea.Width = ClientSize.Width;
            evoArea.Height = Math.Max(ClientSize.Height - renderToolStrip.Height, 1);
        }
       
        private void Refresh(object sender, EventArgs e)
        {
            evoArea.Invalidate();
        }

        public void Paused()
        {
            this.tsbPause.Image = global::EvoDevo3D.Properties.Resources.control_play;
            this.tsbPause.Text = "Resume";
        }
    }
}

