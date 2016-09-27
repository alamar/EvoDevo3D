namespace EvoDevo4
{
    partial class WorldController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmFPSChecker = new System.Windows.Forms.Timer(this.components);
            this.tmWorldHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tmFPSChecker
            // 
            this.tmFPSChecker.Enabled = true;
            this.tmFPSChecker.Interval = 1000;
            this.tmFPSChecker.Tick += new System.EventHandler(this.tmFPSChecker_Tick);
            // 
            // tmWorldHeartbeat
            // 
            this.tmWorldHeartbeat.Enabled = true;
            this.tmWorldHeartbeat.Interval = 50;
            this.tmWorldHeartbeat.Tick += new System.EventHandler(this.tmWorldHeartbeat_Tick);
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = "WorldControllerHelp.xml";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(5, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "EvoDevo 3-D";
            // 
            // WorldController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(170, 35);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorldController";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WorldController";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WorldController_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmFPSChecker;
        private System.Windows.Forms.Timer tmWorldHeartbeat;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label label1;
    }
}