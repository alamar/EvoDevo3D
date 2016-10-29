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
        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new GeneticCode());
        }
    }
}
