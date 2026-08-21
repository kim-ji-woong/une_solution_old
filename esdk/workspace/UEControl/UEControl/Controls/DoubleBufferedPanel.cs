
namespace UnE.Controls
{
	public class DoubleBufferedPanel : System.Windows.Forms.Panel
	{
		public DoubleBufferedPanel()
			: base()
		{
			base.DoubleBuffered = true;
			base.ResizeRedraw = true;
		}
	}
}
