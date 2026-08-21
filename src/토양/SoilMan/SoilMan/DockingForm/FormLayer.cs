using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.DockingForm
{
    public partial class FormLayer : Form
    {
        public enum LayerType { 수치지도 = 0, 토지이용계획도, 지적도, UNKNOWN };

        private bool m_noRefresh = false;

        public FormLayer()
        {
            InitializeComponent();
        }

        public bool IsChecked(LayerType type)
        {
            CheckBox checkBox = null;

            if (type == LayerType.수치지도)
                checkBox = checkBoxSuchi;
            else if (type == LayerType.토지이용계획도)
                checkBox = checkBoxTozi;
            else if (type == LayerType.지적도)
                checkBox = checkBoxZigeoc;
            else
                return false;

            return checkBox.Checked;
        }

        public bool IsEnabled(LayerType type)
        {
            CheckBox checkBox = null;

            if (type == LayerType.수치지도)
                checkBox = checkBoxSuchi;
            else if (type == LayerType.토지이용계획도)
                checkBox = checkBoxTozi;
            else if (type == LayerType.지적도)
                checkBox = checkBoxZigeoc;
            else
                return false;

            return checkBox.Enabled;
        }

        public void SetLayer(LayerType type, bool _checked, bool enabled)
        {
            CheckBox checkBox = null;

            if (type == LayerType.수치지도)
                checkBox = checkBoxSuchi;
            else if (type == LayerType.토지이용계획도)
                checkBox = checkBoxTozi;
            else if (type == LayerType.지적도)
                checkBox = checkBoxZigeoc;
            else
                return;

            m_noRefresh = true;
            checkBox.Checked = _checked;
            checkBox.Enabled = enabled;
            m_noRefresh = false;
        }

        private void checkBoxZigeoc_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_noRefresh)
                FormMain.Instance.ShowLayer(LayerType.지적도, checkBoxZigeoc.Checked);
        }

        private void checkBoxTozi_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_noRefresh)
                FormMain.Instance.ShowLayer(LayerType.토지이용계획도, checkBoxTozi.Checked);
        }

        private void checkBoxSuchi_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_noRefresh)
                FormMain.Instance.ShowLayer(LayerType.수치지도, checkBoxSuchi.Checked);
        }
    }
}
