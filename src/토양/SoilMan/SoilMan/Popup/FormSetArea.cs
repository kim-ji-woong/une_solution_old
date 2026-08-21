using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.Popup
{
    public partial class FormSetArea : Form
    {
        public Overlay.OverlayPainter.DrawingType DrawingType
        {
            get
            {
                if (btnDelete.IsChecked)
                    return Overlay.OverlayPainter.DrawingType.DELETE;
                else if (btnDraw.IsChecked)
                {
                    if (radioCircle.Checked)
                        return Overlay.OverlayPainter.DrawingType.CIRCLE;
                    else if (radioRectangle.Checked)
                        return Overlay.OverlayPainter.DrawingType.RECTANGLE;
                    //else if (radioPolygon.Checked)
                        return Overlay.OverlayPainter.DrawingType.POLYLINE;
                }

                return Overlay.OverlayPainter.DrawingType.NONE;
            }
        }

        public FormSetArea()
        {
            InitializeComponent();
            CheckButton(btnDraw, true);
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            if (radio.Checked)
            {
                if (radio == radioCircle)
                    FormMain.Instance.OverlayPainter.DrawType = Overlay.OverlayPainter.DrawingType.CIRCLE;
                else if (radio == radioPolygon)
                    FormMain.Instance.OverlayPainter.DrawType = Overlay.OverlayPainter.DrawingType.POLYLINE;
                else// if (radio == radioRectangle)
                    FormMain.Instance.OverlayPainter.DrawType = Overlay.OverlayPainter.DrawingType.RECTANGLE;
            }
        }

        private void CheckButton(UnE.GUI.RibbonButton btn, bool isChecked)
        {
            UnE.GUI.RibbonButton otherButton = null;

            if (btn == btnDelete)
                otherButton = btnDraw;
            else
                otherButton = btnDelete;

            btn.IsChecked = isChecked;

            if (isChecked)
            {
                btn.BackColor = Color.FromArgb(0, 111, 196);
                otherButton.BackColor = Color.FromArgb(75, 71, 86);

                otherButton.IsChecked = false;
            }
            else
            {
                btn.BackColor = Color.FromArgb(75, 71, 86);
            }

            if (FormMain.Instance != null)
                FormMain.Instance.OverlayPainter.DrawType = DrawingType;

            btn.Refresh();
            otherButton.Refresh();
        }

        private void btnDrawType_Click(object sender, EventArgs e)
        {
            UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;
            CheckButton(btn, !btn.IsChecked);
        }

        private void btnDelete_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;
            lbText.Text = btn.ToolTipText;
        }

        private void btnDraw_MouseEnter(object sender, EventArgs e)
        {
            UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;
            lbText.Text = btn.ToolTipText;
        }

        private void btnDraw_MouseLeave(object sender, EventArgs e)
        {
            lbText.Text = "";
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            lbText.Text = "";
        }
    }
}
