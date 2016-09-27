
namespace EvoDevo4
{
    partial class GeneticCode 
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
            this.flpCodeHelpers = new System.Windows.Forms.FlowLayoutPanel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.rtCode = new ScintillaNet.Scintilla();
            this.btnSaveNew = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.rtCode)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCodeHelpers
            // 
            this.flpCodeHelpers.AutoScroll = true;
            this.flpCodeHelpers.Location = new System.Drawing.Point(1, 34);
            this.flpCodeHelpers.Name = "flpCodeHelpers";
            this.flpCodeHelpers.Size = new System.Drawing.Size(156, 466);
            this.flpCodeHelpers.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(163, 2);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(45, 26);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(265, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(62, 26);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save As";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRun
            // 
            this.btnRun.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnRun.Location = new System.Drawing.Point(687, 2);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(60, 26);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Compile";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // rtCode
            // 
            this.rtCode.ConfigurationManager.Language = "cs";
            this.rtCode.Location = new System.Drawing.Point(163, 34);
            this.rtCode.Name = "rtCode";
            this.rtCode.Size = new System.Drawing.Size(584, 466);
            this.rtCode.Styles.BraceBad.FontName = "Verdana";
            this.rtCode.Styles.BraceLight.FontName = "Verdana";
            this.rtCode.Styles.ControlChar.FontName = "Verdana";
            this.rtCode.Styles.Default.FontName = "Verdana";
            this.rtCode.Styles.IndentGuide.FontName = "Verdana";
            this.rtCode.Styles.LastPredefined.FontName = "Verdana";
            this.rtCode.Styles.LineNumber.FontName = "Verdana";
            this.rtCode.Styles.Max.FontName = "Verdana";
            this.rtCode.TabIndex = 6;
            // 
            // btnSaveNew
            // 
            this.btnSaveNew.Location = new System.Drawing.Point(214, 2);
            this.btnSaveNew.Name = "btnSaveNew";
            this.btnSaveNew.Size = new System.Drawing.Size(45, 26);
            this.btnSaveNew.TabIndex = 7;
            this.btnSaveNew.Text = "Save";
            this.btnSaveNew.UseVisualStyleBackColor = true;
            this.btnSaveNew.Click += new System.EventHandler(this.btnSaveNew_Click);
            // 
            // GeneticCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 509);
            this.ControlBox = false;
            this.Controls.Add(this.btnSaveNew);
            this.Controls.Add(this.rtCode);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.flpCodeHelpers);
            this.Controls.Add(this.btnRun);
            this.Name = "GeneticCode";
            this.Text = "GeneticCode";
            this.Load += new System.EventHandler(this.GeneticCode_Load);
            this.Resize += new System.EventHandler(this.GeneticCode_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.rtCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.FlowLayoutPanel flpCodeHelpers;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private ScintillaNet.Scintilla rtCode;
        private System.Windows.Forms.Button btnSaveNew;
    }
}