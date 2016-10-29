using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EvoDevo4
{
    public partial class WorldController : Form
    {
        private GeneticCode RENDERWINDOW;
        private GeneticCode renderWindow
        {
            get
            {
                return RENDERWINDOW;
            }
        }
        private Thread heartbeatThread;
        public WorldController(GeneticCode rw)
        {
            InitializeComponent();
            RENDERWINDOW = rw;
            RENDERWINDOW.Show();

            GeneticCode gc = new GeneticCode();
            gc.Show();

            tmFPSChecker.Start();
            heartbeatThread = new Thread(World.Instance.ActionsManager);
            heartbeatThread.Start();
                        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);            
        }

        private void tmFPSChecker_Tick(object sender, EventArgs e)
        {
        }

        private void WorldController_Load(object sender, EventArgs e)
        {

        }
        
        private void tmWorldHeartbeat_Tick(object sender, EventArgs e)
        {
            World.Instance.newActionAllowed = true;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            World.Instance.paused = !World.Instance.paused;
            ((Button)sender).Text= ((World.Instance.paused)?"Run":"Pause");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will reset the world to initial state. Are you sure?", "EvoDevo IV", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                World.Instance.Reset();
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            World.Instance.AwaitingQueue.Enqueue('s');
            World.Instance.paused = false;
            World.Instance.newActionAllowed = true;
        }

        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            RENDERWINDOW.screenshotAwaiting = true;
        }
    }
}
