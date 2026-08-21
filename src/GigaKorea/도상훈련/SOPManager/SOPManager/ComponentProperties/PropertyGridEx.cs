using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager
{
    class PropertyGridEx : PropertyGrid
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.SelectedGridItem.PropertyDescriptor.DisplayName == "X" || this.SelectedGridItem.PropertyDescriptor.DisplayName == "Y")
            {
                if ((keyData >= Keys.D0 && keyData <= Keys.D9) || (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9) || 
                    keyData == Keys.Back || keyData == Keys.Enter)
                    return false;
                else
                    return true;
            }
            else if (this.SelectedGridItem.PropertyDescriptor.DisplayName == "문자 색상")
            {
                if ((keyData >= Keys.D0 && keyData <= Keys.D9) || (keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9) ||
                    keyData == Keys.Back || keyData == Keys.Enter || keyData == Keys.Oemcomma)
                    return false;
            }
            else if (this.SelectedGridItem.PropertyDescriptor.DisplayName == "임무제목" ||
                this.SelectedGridItem.PropertyDescriptor.DisplayName == "단계명" ||
                (this.SelectedGridItem.PropertyDescriptor.Category.Contains("일반") && this.SelectedGridItem.PropertyDescriptor.DisplayName == "내용") ||
                this.SelectedGridItem.PropertyDescriptor.DisplayName == "설명")
            {
                return false;
            }
            return true;
        }
    }
}
