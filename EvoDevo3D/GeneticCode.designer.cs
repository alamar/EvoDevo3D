using System.Drawing;

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
            this.btnCompile = new System.Windows.Forms.Button();
            this.rtCode = new System.Windows.Forms.TextBox();
            this.btnSaveNew = new System.Windows.Forms.Button();
            //((System.ComponentModel.ISupportInitialize)(this.rtCode)).BeginInit();
            this.SuspendLayout();
            // 
            // flpCodeHelpers
            // 
            this.flpCodeHelpers.AutoScroll = true;
            this.flpCodeHelpers.Location = new System.Drawing.Point(1, 34);
            this.flpCodeHelpers.Name = "flpCodeHelpers";
            this.flpCodeHelpers.Size = new System.Drawing.Size(160, 460);
            this.flpCodeHelpers.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(165, 30);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(50, 26);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(265, 30);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 26);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save As";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Location = new System.Drawing.Point(690, 30);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(60, 26);
            this.btnCompile.TabIndex = 1;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // rtCode
            // 
            this.rtCode.Multiline = true;
            //this.rtCode.ConfigurationManager.Language = "cs";
            this.rtCode.Location = new System.Drawing.Point(165, 60);
            this.rtCode.Name = "rtCode";
            this.rtCode.Size = new System.Drawing.Size(580, 460);
            this.rtCode.Font = new Font(FontFamily.GenericMonospace, rtCode.Font.Size);
            //this.rtCode.Styles.BraceBad.FontName = "Verdana";
            //this.rtCode.Styles.BraceLight.FontName = "Verdana";
            //this.rtCode.Styles.ControlChar.FontName = "Verdana";
            //this.rtCode.Styles.Default.FontName = "Verdana";
            //this.rtCode.Styles.IndentGuide.FontName = "Verdana";
            //this.rtCode.Styles.LastPredefined.FontName = "Verdana";
            //this.rtCode.Styles.LineNumber.FontName = "Verdana";
            //this.rtCode.Styles.Max.FontName = "Verdana";
            //this.rtCode.TabIndex = 6;
            // 
            // btnSaveNew
            // 
            this.btnSaveNew.Location = new System.Drawing.Point(215, 30);
            this.btnSaveNew.Name = "btnSaveNew";
            this.btnSaveNew.Size = new System.Drawing.Size(50, 26);
            this.btnSaveNew.TabIndex = 7;
            this.btnSaveNew.Text = "Save";
            this.btnSaveNew.UseVisualStyleBackColor = true;
            this.btnSaveNew.Click += new System.EventHandler(this.btnSaveNew_Click);
            // 
            // GeneticCode
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 500);
            this.Controls.Add(this.btnSaveNew);
            this.Controls.Add(this.rtCode);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.flpCodeHelpers);
            this.Controls.Add(this.btnCompile);
            this.Name = "GeneticCode";
            this.Text = "GeneticCode";
            this.Load += new System.EventHandler(this.GeneticCode_Load);
            this.Resize += new System.EventHandler(this.GeneticCode_Resize);
            //((System.ComponentModel.ISupportInitialize)(this.rtCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.FlowLayoutPanel flpCodeHelpers;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox rtCode;
        private System.Windows.Forms.Button btnSaveNew;
    }
}