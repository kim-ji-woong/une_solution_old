using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace SOPMonitoringSystem
{
	public partial class PopupTranslucentForm : Form
	{
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

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

		public void AddContentForm(Form form, int x, int y, int width, int height, Control parentForm)
		{
			if (m_InnerForm != null && m_InnerForm.Visible == true)
			{
				m_InnerForm.Visible = false;
				m_InnerForm = null;
			}
			m_InnerSubForm = null;
			m_InnerForm = form;

            //if (CheckInnerFormLocation(ref x, ref y))
            //{
            //    Point ptClient = parentForm.PointToClient(new Point(x, y));
            //    pos_x = ptClient.X;
            //    pos_y = ptClient.Y;
            //}
            //else
            {
                pos_x = x;
                pos_y = y;
            }

			m_InnerForm.Visible = false;
            m_InnerFormLocation = new Point(pos_x, pos_y);
			m_InnerFormSize = new Size(width, height);

			Point loc = PointToScreen(m_InnerFormLocation);
			m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
			m_InnerForm.FormClosing += InnerForm_FormClosing;

			button1.Visible = false;

            int dy = pos_y + 5;
            if (dy < 0)
                dy = 0;
			button1.SetBounds(pos_x + width + 15, dy, 40, 40);
			//button1.Invalidate();
			//Point.button1.Visible = true;
		}

        private bool CheckInnerFormLocation(ref int x, ref int y)
        {
            Size szInnerForm = m_InnerForm.Size;
            if (szInnerForm.Width == 0 || szInnerForm.Height == 0)
                return false;


            Rectangle rectScreen = GetScreenSize();

            // InnerForm의 크기가 화면 크기보다 크면 그냥 리턴한다.
            //if (szInnerForm.Width > rectScreen.Width || szInnerForm.Height > rectScreen.Height)
            //    return false;

            // InnerForm의 위치 및 크기가 화면 내에 모두 나타날 경우 그냥 리턴한다.
            if (x >= 0 && szInnerForm.Width + x <= rectScreen.Width &&
                y >= 0 && szInnerForm.Height + y <= rectScreen.Height)
                return false;

            // InnerForm을 화면 중앙에 위치시킨다.
            x = (rectScreen.Width - szInnerForm.Width) / 2;
            y = (rectScreen.Height - szInnerForm.Height) / 2;
            return true;
        }

        private Rectangle GetScreenSize()
        {
            return Screen.FromControl(this).Bounds;
        }

		private void InnerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_InnerForm != null)
			{
				m_InnerForm.FormClosing -= InnerForm_FormClosing;
			}
			button1.Visible = false;
			this.Visible = false;

            SetForegroundWindow(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
        }



		private void PopupTranslucentForm_Resize(object sender, EventArgs e)
		{


            if (m_ParentForm != null)
            {
                Point ptLoc = new Point();
                Size size = new System.Drawing.Size();

                m_TopForm.Invoke((MethodInvoker)delegate
                {
                    ptLoc = m_ParentForm.PointToScreen(new Point(0, 0));
                    size = m_ParentForm.Size;
                });

                Invoke((MethodInvoker)delegate
                {
                    Location = ptLoc;
                    Size = size;
                    if (this.Visible == true)
                    {
                        Activate();
                        Focus();
                    }
                });
            }
			
			
		}

		private void PopupTranslucentForm_Move(object sender, EventArgs e)
		{
			if (m_ParentForm != null)
			{
                Point ptLoc = new Point();
                Size size = new System.Drawing.Size();

                m_ParentForm.Invoke((MethodInvoker)delegate
                {
                    ptLoc = m_ParentForm.PointToScreen(new Point(0, 0));
                    size = m_ParentForm.Size;
                });

                Invoke((MethodInvoker)delegate
                {
                    Location = ptLoc;
                    Size = size;
                    if (this.Visible == true)
                    {
                        Activate();
                        Focus();
                    }
                });

               
			}
		}

		public PopupTranslucentForm()
		{
			InitializeComponent();
			BackColor = Color.Gray;
		}


		public void CloseExternal()
		{
            if (InnerForm != null)
            {
                InnerForm.DialogResult = DialogResult.Cancel;

                Invoke(new Action(() => button1.PerformClick()));
                
            }
		}


		private void button1_Click(object sender, EventArgs e)
		{
			this.Visible = false;
            if(InnerForm != null)
            {
                InnerForm.Close();
            }
            //FormSOP.Instance.GetPageHome().OnTranslucentFormClosing();
		}

		private void OnSizeInnerForm()
		{
			Rectangle pBounds = this.Bounds;
			if (m_InnerForm != null)
			{
				int width = m_InnerFormSize.Width;
				if ((m_InnerFormLocation.X + width + 10) > pBounds.Width)
				{
					width = pBounds.Width - m_InnerFormLocation.X - 10;
				}
				int height = m_InnerFormSize.Height;
				if ((m_InnerFormLocation.Y + height + 10) > pBounds.Height)
				{
					height = pBounds.Height - m_InnerFormLocation.Y - 10;
				}
				Point loc = PointToScreen(m_InnerFormLocation);
				m_InnerForm.SetBounds(loc.X, loc.Y, width, height);

                int dy = pos_y + 5;
                if (dy < 0)
                    dy = 0;
				button1.SetBounds(pos_x + width + 15, dy, 40, 40);
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
			base.Show((IWin32Window)form);
		}


        private static PopupTranslucentForm mCurrentForm = null;
        public static DialogResult ShowModalTranslucentForm(Form targetForm, int x, int y, int width, int height)
        {
           
            
            if (targetForm == null)
                return DialogResult.Cancel;
            PopupTranslucentForm mTranslucentForm = null;
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                mTranslucentForm = new PopupTranslucentForm();
                if (mTranslucentForm == null || mTranslucentForm.IsDisposed)
                    mTranslucentForm = new PopupTranslucentForm();


                // m_nTranslucentCommandID = nCommandID;
               
                if (mTranslucentForm.Visible == true)
                {
                    mTranslucentForm.Detach();
                }
            });

            targetForm.ShowInTaskbar = false;
            targetForm.StartPosition = FormStartPosition.Manual;

            
            PageBackstageSOP pForm = null;
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                pForm = FormSOP.Instance.GetPageHome();

                mTranslucentForm.AddContentForm(targetForm, x, y, targetForm.Size.Width, targetForm.Size.Height, pForm);
                mTranslucentForm.ShowInTaskbar = false;
                mTranslucentForm.TopMost = false;            

                mTranslucentForm.Parent = pForm;
                mTranslucentForm.TopLevel = false;
                pForm.Controls.Add(mTranslucentForm);

                mTranslucentForm.Text = targetForm.Text;
            });
           

           
            mCurrentForm = mTranslucentForm;

            m_FormOwnerThread = Thread.CurrentThread;

            DialogResult result = mTranslucentForm.ShowDialog(pForm);
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                pForm = FormSOP.Instance.GetPageHome();
                pForm.Controls.Remove(mTranslucentForm);
            });

            mCurrentForm = null;
            return result;
        }

        private static Thread m_FormOwnerThread = null;

        public static bool IsShowDialog()
        {
            if(mCurrentForm != null)
            {
                return true;
            }
            return false;
        }

        public DialogResult ShowDialog(Form form)
        {
            if (Visible == true)
            {
                Visible = false;
            }
            m_TopForm = form;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {


                Parent = form;
                this.BringToFront();
                button1.Visible = true;
                this.Visible = true;

                Show();
            });

            bool bVisible = true;
            while (bVisible == true)
            {
                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                    bVisible = InnerForm.Visible;
                });


                Thread.Sleep(60);
            }
            // while check
            
            return InnerForm.DialogResult ;
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
                    Point ptLoc = new Point();
                    Size size = new Size();
                    m_ParentForm.Invoke((MethodInvoker)delegate
                    {
                        ptLoc = m_ParentForm.PointToScreen(new Point(0, 0));
                        size = m_ParentForm.Size;
                    });


                    this.Location = ptLoc;
                    this.Size = size;

					if (m_InnerForm != null)
					{
                        Point loc = new Point();
                        m_ParentForm.Invoke((MethodInvoker)delegate
                        {
                            loc = m_ParentForm.PointToScreen(m_InnerFormLocation);


                            //int dx = loc.X;
                            //int dy = loc.Y;
                            //if (CheckInnerFormLocation(ref dx, ref dy))
                            //{
                            //    loc = new Point(dx, dy);
                                Point ptClient = m_ParentForm.PointToClient(loc);

                                //pos_x = (ptClient.X < 0) ? 0 : ptClient.X;
                                //pos_x = (ptClient.Y < 0) ? 0 : ptClient.Y;

                                pos_x = ptClient.X;
                                pos_y = ptClient.Y;

                                ResizeInner(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
                            //}                           
                        });

						m_InnerForm.SetBounds(loc.X, loc.Y, m_InnerFormSize.Width, m_InnerFormSize.Height);
						m_InnerForm.Show(this);
						Invalidate();
					}

                    if (m_TopForm != null)
                    {
                        m_TopForm.Invoke((MethodInvoker)delegate
                        {
                            m_TopForm.Resize += PopupTranslucentForm_Resize;
                            m_TopForm.Move += PopupTranslucentForm_Move;

                            Control c = m_TopForm.TopLevelControl;

                            if (c != null)
                                c.Move += PopupTranslucentForm_Move;	
                        });
                        	
                    }
                    
                }
			}
			else
			{
                if (m_TopForm != null)
				{
                    m_TopForm.Invoke((MethodInvoker)delegate
                    {
					    m_TopForm.Resize -= PopupTranslucentForm_Resize;
					    m_TopForm.Move -= PopupTranslucentForm_Move;
					    Control c = m_TopForm.TopLevelControl;
                        if (c != null)
					        c.Move -= PopupTranslucentForm_Move;
                    });
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

            int dy = pos_y + 5;
            if (dy < 0)
                dy = 0;
			button1.SetBounds(pos_x + width + 15, dy, 40, 40);
		}

        private void PopupTranslucentForm_Activated(object sender, EventArgs e)
        {
            int i = 0;
            i++;
            
            //this.BringToFront();
            //FormSOP.Instance.Invoke((MethodInvoker)delegate
            //{
            //    FormSOP.Instance.ParentForm.BringToFront();
            //});
        }

        private void PopupTranslucentForm_Deactivate(object sender, EventArgs e)
        {
            //this.BringToFront();

            
            if (timer1.Enabled == false)
            {
                //bWaitTimer = true;
                timer1.Interval = 1200;
               // timer1.Enabled = true;
               // timer1.Start();
            }
           
        }

        //private bool bWaitTimer = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //bWaitTimer = false;
            timer1.Stop();
            timer1.Enabled = false;
           
            if(this.IsDisposed == false && this.Visible == true)
            {
                this.Activate();
                //this.Focus();
                typeof(Control).GetMethod("OnResize",
                  BindingFlags.Instance | BindingFlags.NonPublic)
                  .Invoke(this, new object[] { EventArgs.Empty });
            }
        }
          
	}
}