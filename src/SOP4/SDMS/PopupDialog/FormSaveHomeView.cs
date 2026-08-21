using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormSaveHomeView : Form
    {
        public System.Windows.Forms.Button BtnMainHome
        {
            get { return btnHome; }
            set { btnHome = value; }
        }

        public System.Windows.Forms.Button Btn14Home
        {
            get { return btn14Home; }
            set { btn14Home = value; }
        }

        public System.Windows.Forms.Button Btn56Home
        {
            get { return btn56Home; }
            set { btn56Home = value; }
        }

        public System.Windows.Forms.Button BtnCoalHome
        {
            get { return btnCoalHome; }
            set { btnCoalHome = value; }
        }

        public FormSaveHomeView()
        {
            InitializeComponent();

          
        }
    }
}
