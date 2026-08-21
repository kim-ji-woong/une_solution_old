using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace SOPManager
{

	public class PropertiesAnnotation : SectionPropertiesBase
	{
        //[Category("일반")]
        //[Browsable(true)]
        //[DisplayName("종류")]
        //[Description("컴포넌트의 타입을 표시합니다.")]
        //public string Desc
        //{
        //    get
        //    {
        //        return m_szDesc;
        //    }
        //}

        //[Category("일반")]
        //[Browsable(true)]
        //[DisplayName("이름")]
        //[Description("컴포넌트의 ID를 표시합니다.")]
        //public string ID
        //{
        //    get
        //    {
        //        if (mSection != null)
        //        {
        //            Sections.SectionData data = mSection.Data;
        //            if (data != null)
        //            {
        //                m_szID = data.ComponentID;
        //            }
        //        }
        //        return m_szID;
        //    }
        //}

        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("내용")]
		[Description("설명의 내용을 입력합니다.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		public string Text
		{
			get
			{
				if (mSection != null)
				{
					m_szText = mSection.Title;
				}
				return m_szText;
			}

			set
			{
				if (mSection != null)
				{
					UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

					mSection.Title = value;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
				m_szText = value;
			}
		}
	}
}