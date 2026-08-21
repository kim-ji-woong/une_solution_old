using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace HSMS
{
    public interface ITranslucentFormParent
    {
        void OnCloseTranslucentForm();
    }

    public partial class FormTranslucentForm : Form
    {
        private Form m_TopForm = null;
        private Form m_ParentForm = null;
        public new Form Parent
        {
            get { return m_ParentForm; }
            set { m_ParentForm = value; }
        }
        private Form m_InnerSubForm = null;
        public System.Windows.Forms.Form InnerSubForm
        {
            get { return m_InnerSubForm; }
            set { m_InnerSubForm = value; }
        }
        private Form m_InnerForm = null;
        public System.Windows.Forms.Form InnerForm
        {
            get { return m_InnerForm; }
            set { m_InnerForm = value; }
        }
        private Point m_InnerFormLocation;
        private Size m_InnerFormSize;

        public void Detach()
        {
            if (m_InnerForm != null)
            {
                m_InnerForm.Visible = false;
                m_InnerForm.FormClosing -= InnerForm_FormClosing;
                if (m_InnerForm.IsDisposed != false)
                {
                    m_InnerForm.Close();
                }
            }
            button1.Visible = false;
        }

        public DialogResult AddSubModalForm(Form form)
        {
            if (m_InnerFormLocation == null)
                return DialogResult.Cancel;

            Point pt = m_InnerForm.Location;
            Point ptRel = PointToClient(pt);
            Size size1 = m_InnerForm.Size;
            Size size2 = form.Size;

            Point ptLoc = pt;
            ptLoc.X = pt.X + size1.Width + 1;
            ptLoc.Y = pt.Y + (size1.Height - size2.Height);

            if (this.Width < (ptRel.X + size1.Width + 11 + size2.Width))
            {
                int dx = (ptRel.X + size1.Width + 11 + size2.Width) - this.Width;
                size2.Width = size2.Width - dx;
            }
            if (this.Height < (ptLoc.Y + 10 + size2.Height))
            {
                int dy = (ptRel.Y + 10 + size2.Height) - this.Height;
                size2.Height = size2.Height - dy;
            }

            form.SetBounds(ptLoc.X, ptLoc.Y, size2.Width, size2.Height);
            m_InnerSubForm = form;
            m_InnerForm.Enabled = false;
            DialogResult result = form.ShowDialog(this);
            m_InnerForm.Enabled = true;
            return result;
        }

        private int pos_x = 0;
        private int pos_y = 0;
        public void AddContentForm(Form form, int x, int y, int width, int height)
        {
            if (m_InnerForm != null && m_InnerForm.Visible == true)
            {
                m_InnerForm.Visible = false;
                m_InnerForm = null;
            }
            m_InnerSubForm = null;
            pos_x = x;
            pos_y = y;
            m_InnerForm = form;
            m_InnerForm.Visible = false;
            m_InnerFormLocation = new Point(x, y);
            m_InnerFormSize = new Size(width, height);

            Point loc = PointToScreen(m_InnerFormLocation);
            m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
            m_InnerForm.FormClosing += InnerForm_FormClosing;

            //button1.Visible = false;
            button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);
        }

        private void InnerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_InnerForm != null)
            {
                m_InnerForm.FormClosing -= InnerForm_FormClosing;
            }
            button1.Visible = false;
            this.Visible = false;

            if (m_ParentForm.GetType().GetInterface("ITranslucentFormParent") == typeof(ITranslucentFormParent))
            {
                ITranslucentFormParent t = (ITranslucentFormParent)m_ParentForm;
                t.OnCloseTranslucentForm();
            }

            //FormMain.Instance.PageHome.OnTranslucentFormClosing();
        }

        private void FormTranslucentForm_Resize(object sender, EventArgs e)
        {
            if (m_ParentForm != null)
            {
                this.Location = m_ParentForm.PointToScreen(new Point(0, 0));
                this.Size = m_ParentForm.Size;
                if (this.Visible == true)
                {
                    Activate();
                    Focus();
                }
            }            
        }

        private void FormTranslucentForm_Move(object sender, EventArgs e)
        {
            if (m_ParentForm != null)
            {
                this.Location = m_ParentForm.PointToScreen(new Point(0, 0));
                this.Size = m_ParentForm.Size;
                if (this.Visible == true)
                {
                    Activate();
                    Focus();
                }
            }
        }

        public FormTranslucentForm()
        {
            InitializeComponent();
            BackColor = Color.Gray;
        }

        public void CloseExternal()
        {
            if (InnerForm != null)
            {
                InnerForm.Close();
            }
            this.Visible = false;

            //FormMain.Instance.PageHome.OnTranslucentFormClosing();
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (InnerForm != null)
            {
                InnerForm.Close();
            }
            this.Visible = false;
        }        

        private void OnSizeInnerForm()
        {
            Rectangle pBounds = this.Bounds;
            if (m_InnerForm != null)
            {
                int width = m_InnerFormSize.Width;
                if ((m_InnerFormLocation.X + width + 60) > pBounds.Width)
                {
                    width = pBounds.Width - m_InnerFormLocation.X - 60;
                }
                int height = m_InnerFormSize.Height;
                if ((m_InnerFormLocation.Y + height + 10) > pBounds.Height)
                {
                    height = pBounds.Height - m_InnerFormLocation.Y - 10;
                }
                Point loc = PointToScreen(m_InnerFormLocation);
                m_InnerForm.SetBounds(loc.X, loc.Y, width, height);

                button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);

                m_InnerForm.Update();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            OnSizeInnerForm();
        }

        public void Show(Form form)
        {
            if (Visible == true)
            {
                Visible = false;
            }
            m_TopForm = form;
            button1.Visible = true;

            OnSizeInnerForm();
            base.Show((IWin32Window)form);
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            OnSizeInnerForm();
        }

        private void FormTranslucentForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                if (m_ParentForm != null)
                {
                    Location = m_ParentForm.PointToScreen(new Point(0, 0));
                    Size = m_ParentForm.Size;

                    if (m_InnerForm != null)
                    {
                        Point loc = m_ParentForm.PointToScreen(m_InnerFormLocation);
                        m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
                        
                        OnSizeInnerForm();
                        m_InnerForm.Show(this);
                        Invalidate();
                    }
                    m_TopForm.Resize += FormTranslucentForm_Resize;
                    m_TopForm.Move += FormTranslucentForm_Move;
                    Control c = m_TopForm.TopLevelControl;
                    c.Move += FormTranslucentForm_Move;
                }
            }
            else
            {
                if (m_ParentForm != null)
                {
                    m_TopForm.Resize -= FormTranslucentForm_Resize;
                    m_TopForm.Move -= FormTranslucentForm_Move;
                    Control c = m_TopForm.TopLevelControl;
                    c.Move -= FormTranslucentForm_Move;
                }
                if (m_InnerForm != null)
                {
                    if (m_InnerForm != null && m_InnerForm.Visible == true)
                    {
                        m_InnerForm.Visible = false;
                        //m_InnerForm.Close();
                    }                    
                } 
                
            }
        }

        private void FormTranslucentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        public void ResizeInner(int x, int y, int width, int height)
        {
            button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);
        }       
    }
}
