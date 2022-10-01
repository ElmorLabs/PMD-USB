using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KTH {
	public class DBPanel : System.Windows.Forms.Panel {
		public DBPanel() {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint | 
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint | 
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, 
                true);
		}

	}
}
