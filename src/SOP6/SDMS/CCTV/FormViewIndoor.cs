using System.Windows.Forms;

namespace SDMS
{
	public partial class FormViewIndoor : Form
	{
        private Control m_view = null;

        public FormViewIndoor()
		{
			InitializeComponent();
			MouseWheel += new MouseEventHandler(OnMouseWheel);
		}

        int nType = -1;
        public void AttachView(Control view)
		{

            if (view.GetType() == typeof(UnE.Util.Unity.Panel4Unity))
            {
                nType = 1;
            }

			this.Controls.Add(view);
			view.Dock = DockStyle.Fill;
			m_view = view;
			view.Show();
		}

        public Control DetachView()
		{
			this.Controls.Remove(m_view);

            Control view = m_view;
			m_view = null;
            nType = -1;
			return view;
		}

		public void OnMouseWheel(object sender, MouseEventArgs e)
		{
			if (m_view != null)
            {
                if (nType == 1)
                {
                    ((UnE.Util.Unity.Panel4Unity)m_view).OnMouseWheel(e.X, e.Y, e.Delta);
                }
                //if (nType == 2)
                //{
                //    ((BaseViewEx2)m_view).OnMouseWheel(e.X, e.Y, e.Delta);
                //}
                //else if (nType == 3)
                //{
                //    ((ImageViewCtrl)m_view).OnMouseWheel(sender, e);
                //}
				
			}
		}
	}
}