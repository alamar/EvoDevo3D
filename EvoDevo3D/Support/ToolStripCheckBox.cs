using System;
using System.Windows.Forms;

namespace EvoDevo3D.Support
{
    public class ToolStripCheckBox : ToolStripControlHost
    {
        public ToolStripCheckBox()
            : base(new CheckBox())
        {
        }

        public CheckBox CheckBox
        {
            get
            {
                return this.Control as CheckBox;
            }
        }
    }
}
 