using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication5
{
	public partial class Form1 : Form
	{
		Form2 form2 = new Form2();
		public Form1()
		{
			InitializeComponent();

			
			form2.TopLevel = false;
			//form2.Dock = DockStyle.Fill;
			panel1.Controls.Add(form2);

			form2.Location = new Point(0, 0);
			form2.Size = new Size(panel1.Width, panel1.Height);
			form2.Visible = true;						
			RemoveResizeEventHander(form2);
			form2.SizeChanged += Form2_SizeChanged;
		}


		private MethodInfo mSizeChange = null;
		private object mTargetForm = null;
		//System.ComponentModel.EventHandlerList
		private void RemoveResizeEventHander(Form form)
		{
			FieldInfo f1 = typeof(Control).GetField("EventSize", BindingFlags.Static | BindingFlags.NonPublic);
			object obj = f1.GetValue(form);
			PropertyInfo pi = form.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
			EventHandlerList list = (EventHandlerList)pi.GetValue(form, null);

			mSizeChange = list[obj].Method;
			mTargetForm = list[obj].Target;
			
			list.RemoveHandler(obj, list[obj]);

		}

		private bool m_bPreventInnerFormResize = false;
		private bool m_bPreventFrameResize = false;


		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			// InnerFormResize가 제한경우
			if (m_bPreventInnerFormResize == true)
				return;

			// InnerForm Resize로 인한 FrameResize를 제한합니다.
			m_bPreventFrameResize = true;

			// InnerForm의 Resize를 수행합니다.
			// 여기서 수행되는 Resize는 FormFrame에 영향을 미치지 않습니다.
			form2.Location = new Point(0, 0);
			form2.Size = new Size(panel1.Width, panel1.Height);

			// InnerForm Resize로 인한 FrameResize제한을 해제합니다.
			m_bPreventFrameResize = false;
		}

		private void Form2_SizeChanged(object sender, EventArgs e)
		{
			// 부모 프레임을 리사이즈 하지 않는 경우만 사이즈 변환을 수행
			if (m_bPreventFrameResize == false)
			{
				// 이벤트가 순환되지 않도록 InnerFormResize를 중지합니다.
				m_bPreventInnerFormResize = true;
				// 부모폼의 Size변경에 대한 처리를 추가합니다.
				// 자식폼의 크기를 클라이언트 영역의 사이즈로 지정합니다.
				this.ClientSize = new Size(form2.Size.Width, form2.Size.Height);
				
				// 이벤트가 순환되지 않도록 InnerFormResize를 중지합니다.
				m_bPreventInnerFormResize = false;
			}

			// InnerForm에 Resize 이벤트가 설정된 경우 전달합니다.
			if (mSizeChange != null)
				mSizeChange.Invoke(mTargetForm, new object[] { mTargetForm, e });
			
			
		}
	}
}
