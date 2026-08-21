using System;
using System.Drawing;
using System.Windows.Forms;

namespace UnE
{
    namespace GUI
    {
        public class ImageComboBox : ComboBox
        {
            private PictureBox m_picBtn = new PictureBox();
            public PictureBox PicBtn
            {
                get { return m_picBtn; }
            }
            private Image m_imgNormal = null;
            private Image m_imgClicked = null;
            private Image m_imgMouseOver = null;
            private Image m_imgDisabled = null;

            protected bool m_isLClicked = false;
            protected bool m_isMouseOver = false;
            
            protected IImageComboBoxOwner m_owner = null;

            protected static SolidBrush m_defBrush = new SolidBrush(Color.Gray);
            protected SolidBrush m_ownBrush = new SolidBrush(Color.Black);
            protected System.Drawing.Font m_font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            public Image ImageNormal
            {
                get { return m_imgNormal; }
                set
                {
                    m_imgNormal = value; 
                }
            }

            public Image ImageClicked
            {
                get { return m_imgClicked; }
                set { m_imgClicked = value; }
            }

            public Image ImageMouseOver
            {
                get { return m_imgMouseOver; }
                set { m_imgMouseOver = value; }
            }

            public Image ImageDisabled
            {
                get { return m_imgDisabled; }
                set { m_imgDisabled = value; }
            }

            public IImageComboBoxOwner Owner
            {
                get { return m_owner; }
                set { m_owner = value; }
            }

            public Color TextColor
            {
                get { return m_ownBrush.Color; }
                set { m_ownBrush.Color = value; }
            }

            public System.Drawing.Font TextFont
            {
                get { return base.Font; }
                set { base.Font = value; }
            } 

            public ImageComboBox()
            {  
                this.DrawItem += ImageComboBox_DrawItem;                
                this.MouseDown += ImageComboBox_MouseDown;
                this.MouseHover += ImageComboBox_MouseHover;                
                this.MouseLeave += ImageComboBox_MouseLeave;
                this.SelectionChangeCommitted += ImageComboBox_SelectionChangeCommitted;

                m_picBtn.MouseHover += ImageComboBox_MouseHover;
                m_picBtn.MouseLeave += ImageComboBox_MouseLeave;
                m_picBtn.Click += picBtn_Click;

                this.Resize += ImageComboBox_Resize;                 
                 
                this.DropDownStyle = ComboBoxStyle.DropDownList;
                m_picBtn.Parent = this;
                m_picBtn.SizeMode = PictureBoxSizeMode.StretchImage;
                m_picBtn.Name = this.Name + "_pic";
            }

            void ImageComboBox_SelectionChangeCommitted(object sender, EventArgs e)
            {
                m_picBtn.Image = m_imgNormal;
            }  

            void ImageComboBox_Resize(object sender, EventArgs e)
            {
                if (m_imgNormal == null)
                    return;

                int width = SystemInformation.VerticalScrollBarWidth;
                int height = this.Size.Height;

                m_picBtn.Size = new Size(width, height);
                m_picBtn.Location = new Point(this.Size.Width - width, 0);

                if (m_picBtn.Image == null)
                    m_picBtn.Image = m_imgNormal; 
            } 

            void picBtn_Click(object sender, EventArgs e)
            {
                if (this.DroppedDown)
                    this.DroppedDown = false;
                else
                    this.DroppedDown = true; 
            }

            private void ImageComboBox_MouseHover(object sender, EventArgs e)
            {
                m_picBtn.Image = m_imgClicked;
            }

            private void ImageComboBox_MouseLeave(object sender, EventArgs e)
            {
                if (!this.DroppedDown)
                    m_picBtn.Image = m_imgNormal;

                //if (m_isMouseOver)
                //{
                //    m_isMouseOver = false;
                //    Refresh();
                //}
                //else
                //    m_isMouseOver = false;
            }

            void ImageComboBox_DrawItem(object sender, DrawItemEventArgs e)
            {
                if (e.Index >= 0)
                {
                    //if (e.State == DrawItemState.Selected)
                    {
                        ComboBox box = ((ComboBox)sender);
                        if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
                        {
                            this.BackColor = Color.White;
                        }
                        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        {

                            e.Graphics.FillRectangle(new SolidBrush(Color.Orange), e.Bounds);
                        }
                        else if (e.State == DrawItemState.ComboBoxEdit)
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x3d, 0x3d, 0x3d)), e.Bounds);
                        }

                        e.Graphics.DrawString(box.Items[e.Index].ToString(),
                             e.Font, new SolidBrush(Color.White),
                             new Point(e.Bounds.X, e.Bounds.Y));
                        e.DrawFocusRectangle();
                    }
                }
            } 

            private void ImageComboBox_MouseDown(object sender, MouseEventArgs e)
            {
                m_picBtn.Image = m_imgClicked;
                //m_isLClicked = true;

                //if (m_owner != null)
                //    m_owner.OnImageComboBoxMouseDown(sender, e);

                //Refresh();
            } 

            private void ImageComboBox_MouseEnter(object sender, EventArgs e)
            {
                m_picBtn.Image = m_imgClicked;
            } 
        }

        public interface IImageComboBoxOwner
        {
            void OnImageComboBoxMouseDown(object sender, MouseEventArgs e);
            void OnImageComboBoxMouseUp(object sender, MouseEventArgs e);
        }
    }
}
