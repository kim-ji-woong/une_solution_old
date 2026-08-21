using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup.CreateFormulas
{
    public class LogicalTerm : CustomComboBox
    {
        public LogicalTerm()
        {
            this.ComboBox.Items.Add(new CustomComboBoxItem("and", "그리고"));
            this.ComboBox.Items.Add(new CustomComboBoxItem("or", "또는"));                     
            this.ComboBox.SelectedIndex = 0;

            this.Size = new System.Drawing.Size(50, this.ComboBox.Height);

            this.Label.ForeColor = System.Drawing.Color.White;
        }
    }
}
