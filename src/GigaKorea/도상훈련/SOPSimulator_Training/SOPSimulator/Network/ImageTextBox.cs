using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SOPSimulator.Network
{
    public class ImageTextBox : TextBox
    {
        private ITextBoxOwner m_owner = null;

        public ITextBoxOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }
        
        public ImageTextBox()
        {
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (m_owner != null)
                m_owner.GetFocus(this);
        }
    }

    public interface ITextBoxOwner
    {
        void GetFocus(ImageTextBox textBox);
    }
}
