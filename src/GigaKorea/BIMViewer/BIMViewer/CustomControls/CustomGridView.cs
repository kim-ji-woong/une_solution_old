using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer.CustomControls
{
    public class CustomGridView : DataGridView
    {
        public CustomGridView()
        {
            this.AllowUserToAddRows = false;            
            this.BackgroundColor = System.Drawing.Color.FromArgb(32, 36, 39);
            this.BorderStyle = BorderStyle.None;
            this.CellBorderStyle = DataGridViewCellBorderStyle.None;
            this.ColumnHeadersVisible = false;
            this.RowHeadersVisible = false;
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.BackColor = System.Drawing.Color.FromArgb(32, 36, 39);
            cellStyle.ForeColor = System.Drawing.Color.White;
            cellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(222, 235, 247);
            cellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(156, 36, 107);
            this.RowsDefaultCellStyle = cellStyle;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
    }
}
