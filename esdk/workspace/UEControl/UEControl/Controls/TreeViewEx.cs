using System.Drawing;
using System.Windows.Forms;

namespace UnE.Controls
{
    
    public class TreeViewEx : System.Windows.Forms.TreeView
    {
        public TreeViewEx()
            : base()
        {
            DoubleBuffered = true;
            m_AddTextLenth = 40;
        }

        private int m_AddTextLenth = 40;

        protected override bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }


        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            base.OnDrawNode(e);

            TreeNodeStates state = e.State;

            Font font = e.Node.NodeFont ?? e.Node.TreeView.Font;

            Color fore = e.Node.ForeColor;

            if (fore == Color.Empty)
                fore = e.Node.TreeView.ForeColor;

            if (e.Node == e.Node.TreeView.SelectedNode)
            {

                if (typeof(SOPNode).IsAssignableFrom(e.Node.GetType()))
                {
                    Rectangle rect = e.Bounds;
                    rect.Width += m_AddTextLenth;

                    string szText = e.Node.Text + ((SOPNode)e.Node).TypeText;
                    fore = SystemColors.HighlightText;
                    e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), rect);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rect, fore, Color.DarkGray);
                    TextRenderer.DrawText(e.Graphics, szText, font, rect, fore, Color.DarkGray, TextFormatFlags.GlyphOverhangPadding);
                }
                else
                {
                    fore = SystemColors.HighlightText;
                    e.Graphics.FillRectangle(new SolidBrush(Color.DarkGray), e.Bounds);
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, fore, Color.DarkGray);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, Color.DarkGray, TextFormatFlags.GlyphOverhangPadding);
                }
            }
            else
            {
                if (typeof(SOPNode).IsAssignableFrom(e.Node.GetType()))
                {
                    Rectangle rect = e.Bounds;
                    rect.Width += m_AddTextLenth;
                    string szText = e.Node.Text + ((SOPNode)e.Node).TypeText;
                    e.Graphics.FillRectangle(SystemBrushes.Window, rect);
                    TextRenderer.DrawText(e.Graphics, szText, font, rect, fore, TextFormatFlags.GlyphOverhangPadding);
                }
                else
                {
                    e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, TextFormatFlags.GlyphOverhangPadding);
                }
            }
        }
    }
}
