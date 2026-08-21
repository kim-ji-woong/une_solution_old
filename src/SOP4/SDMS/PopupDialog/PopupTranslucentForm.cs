using System;
using System.Drawing;
using System.Windows.Forms;

namespace SDMS
{
	public partial class PopupTranslucentForm : Form
	{
		private Form m_TopForm = null;
		private Form m_ParentForm = null;

		public new Form Parent
		{
			get { return m_ParentForm; }
			set { m_ParentForm = value; }
		}

        private bool m_bUseCloseButton = true;
        public bool UseCloseButton
        {
            get { return m_bUseCloseButton; }
            set { m_bUseCloseButton = value; }
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

				if (dx > 0)
					size2.Width = size2.Width - dx;
			}
			if (this.Height < (ptLoc.Y + 10 + size2.Height))
			{
				int dy = (ptRel.Y + 10 + size2.Height) - this.Height;

				if (dy > 0)
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

            if (m_bUseCloseButton == false)
            {
                m_InnerFormSize = new Size(this.Width, this.Height);
            }
            else
                m_InnerFormSize = new Size(width, height);

			Point loc = PointToScreen(m_InnerFormLocation);

            m_InnerForm.Location = loc;
            m_InnerForm.Size = new Size(m_InnerFormSize.Width, m_InnerFormSize.Height);
			//m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
			m_InnerForm.FormClosing += InnerForm_FormClosing;



			button1.Visible = m_bUseCloseButton;

            if (m_bUseCloseButton == true)
            {
                if(! Controls.Contains(button1))
                    Controls.Add(button1);
                button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);
            }
            else
            {
                this.Controls.Remove(button1);
            }

			//button1.Invalidate();
			//Point.button1.Visible = true;
		}

		private void InnerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_InnerForm != null)
			{
				m_InnerForm.FormClosing -= InnerForm_FormClosing;
			}
			button1.Visible = false;
			this.Visible = false;

			FormMain.Instance.PageHome.OnTranslucentFormClosing();
            FormMain.Instance.Activate();
		}

		private void PopupTranslucentForm_Resize(object sender, EventArgs e)
		{
			this.Location = m_ParentForm.PointToScreen(new Point(0, 0));
			this.Size = m_ParentForm.Size;
			if (this.Visible == true)
			{
				Activate();
				Focus();
			}
		}

		private void PopupTranslucentForm_Move(object sender, EventArgs e)
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

		public PopupTranslucentForm()
		{
			InitializeComponent();
			BackColor = Color.Gray;
		}

		public void CloseExternal()
		{
			this.Visible = false;
			FormMain.Instance.PageHome.OnTranslucentFormClosing();
            FormMain.Instance.Activate();
        }

		private void button1_Click(object sender, EventArgs e)
		{
			this.Visible = false;
			FormMain.Instance.PageHome.OnTranslucentFormClosing();
            FormMain.Instance.Activate();
		}

		private void OnSizeInnerForm()
		{
			Rectangle pBounds = this.Bounds;
			if (m_InnerForm != null)
			{
				int width = m_InnerFormSize.Width;
                int height = m_InnerFormSize.Height;

                int nExtraWidt = 60;
                if (m_bUseCloseButton == true)
                {
                    if ((m_InnerFormLocation.X + width + nExtraWidt) > pBounds.Width)
                    {
                        width = pBounds.Width - m_InnerFormLocation.X - nExtraWidt;
                    }
                    
                    if ((m_InnerFormLocation.Y + height + 10) > pBounds.Height)
                    {
                        height = pBounds.Height - m_InnerFormLocation.Y - 10;
                    }
                }  
                else
                {
                    width = this.Width;
                    height = this.Height - 10;
                }

				Point loc = PointToScreen(m_InnerFormLocation);
                m_InnerForm.Location = loc;
                m_InnerForm.Size = new Size(width, width);

                if (m_bUseCloseButton == true)
                {
                    button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);
                }
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

            if (m_bUseCloseButton  == true)
			    button1.Visible = true;
			base.Show((IWin32Window)form);
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			OnSizeInnerForm();
		}

		private void PopupTranslucentForm_VisibleChanged(object sender, EventArgs e)
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
                        if( m_bUseCloseButton == true)
                            m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
                        else
                        {
                            m_InnerForm.Location = loc;
                            m_InnerForm.Size = new Size(Width, Height);

                        }
						m_InnerForm.Show(this);
						Invalidate();
					}
					m_TopForm.Resize += PopupTranslucentForm_Resize;
					m_TopForm.Move += PopupTranslucentForm_Move;
					Control c = m_TopForm.TopLevelControl;
                    if( c != null)
                        c.Move += PopupTranslucentForm_Move;
				}
			}
			else
			{
				if (m_ParentForm != null)
				{
					m_TopForm.Resize -= PopupTranslucentForm_Resize;
					m_TopForm.Move -= PopupTranslucentForm_Move;
					Control c = m_TopForm.TopLevelControl;
                    if( c != null)
                        c.Move -= PopupTranslucentForm_Move;
				}
				if (m_InnerForm != null)
					m_InnerForm.Visible = false;
			}
		}

		private void PopupTranslucentForm_FormClosing(object sender, FormClosingEventArgs e)
		{
		}

		public void ResizeInner(int x, int y, int width, int height)
		{
			button1.SetBounds(pos_x + width + 15, pos_y + 5, 40, 40);
		}
	}
}