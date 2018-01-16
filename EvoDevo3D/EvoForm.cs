using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EvoDevo3D
{
    public class EvoForm : Form
    {
        //Controls.
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

            evoArea.Name = "evoArea";
            this.Controls.Add(evoArea);
            OnResize(e);
            KeyDown += evoArea.Keyboard_KeyDown;

            System.Windows.Forms.Timer refresher = new System.Windows.Forms.Timer();
            refresher.Interval = 200;
            refresher.Tick += new System.EventHandler(Refresh);
            refresher.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            evoArea.Width = ClientSize.Width;
            evoArea.Height = Math.Max(ClientSize.Height, 1);
        }

        private void Refresh(object sender, EventArgs e)
        {
            evoArea.Invalidate();
        }
    }
}

