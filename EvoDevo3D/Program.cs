using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EvoDevo4
{
    class Program
    {
        static RenderWindow rw;
        [STAThread]
        static void Main(string[] args)
        {


            Application.Run(new RenderWindow());
            
        }
        /*private static void RenderTherad()
        {
            rw = new RenderWindow();
            //Application.Run(rw);            
            rw.Show();
            
            while (true) Thread.Sleep(300);
        }*/
        
    }
}
