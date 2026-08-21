using System.Windows.Forms;

namespace SDMS
{

	public partial class FormViewOutdoor : Form
	{
        private Control m_view = null;

		public FormViewOutdoor()
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
            //if(view.GetType() == typeof(BaseViewEx2))
            //{
            //    nType = 2;
            //}
            //else if(view.GetType() == typeof(ImageViewCtrl))
            //{
            //    nType = 3;
            //}
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
                if( nType == 1)
                {
                    ((UnE.Util.Unity.Panel4Unity)m_view).OnMouseWheel(e.X, e.Y, e.Delta);
                }
                //if (nType == 2)
                //{
                //    ((BaseViewEx2)m_view).OnMouseWheel(e.X, e.Y, e.Delta);
                //}
                //else if (nType == 3)
                //{
                //    ((ImageViewCtrl)m_view).OnMouseWheel(m_view,e);
                //}
				
			}
		}
	}
}