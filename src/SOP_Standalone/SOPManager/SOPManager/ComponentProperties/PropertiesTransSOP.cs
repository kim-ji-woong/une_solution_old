using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SOPManager
{
	public class SOPSelectionEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			string szValue = value as string;
			if (svc != null && szValue != null)
			{
				using (PopupTransSOP form = new PopupTransSOP())
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
					
					PropertiesTransSOP properties = (PropertiesTransSOP)context.Instance;
					form.Section = (Sections.SectionTransSOP)properties.GetSection();
					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						properties.Text = form.Title;
						properties.LinkedActionStepID = form.LinkedActionStepID;
						value = form.FullPath;
					}
										
				}
			}
			return value;
		}
	}

	public class PropertiesTransSOP : SectionPropertiesBase
	{
  		[Category("\t\t일반")]
		[Browsable(false)]
		[DisplayName("내용")]
		[Description("컴포넌트의 내용을 편집합니다.")]
		[ReadOnly(false)]
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
                    if (m_bEnabledSnapshot == true)
					    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

					mSection.Title = value;
					if (mSection.GetParent() != null)
						mSection.GetParent().Refresh();
				}
				m_szText = value;
			}
		}

		private string m_szFullPath = "";
        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("SOP")]
		[Description("전환할 SOP를 지정합니다.")]
		[Editor(typeof(SOPSelectionEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string FullPath
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					m_szFullPath = data.Title;
				}
				return m_szFullPath;
			}

			set
			{
				m_szFullPath = value;
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					if( m_szFullPath != data.Title)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");

						data.Title = value;
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}				
				}				
			}
		}


		private string m_szDescription = "";
        [Category("\t\t일반")]
		[Browsable(true)]
		[DisplayName("설명")]
		[Description("설명의 내용을 입력합니다.")]
		[Editor(typeof(TextEditor), typeof(UITypeEditor))]
		[ReadOnly(false)]
		public string Description
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					m_szDescription = data.Description;
				}
				return m_szDescription;
			}

			set
			{
				m_szDescription = value;
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					if (m_szDescription != data.Description)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 설명 편집");
						data.Description = m_szDescription;
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}					
				}				
			}
		}

		private int m_nLinkedSOPID = -1;

        [Category("\t\t일반")]
		[Browsable(false)]
		[DisplayName("연결된 SOP ID")]
		[Description("전환할 SOP ID입니다.")]
		[ReadOnly(false)]
		public int LinkedActionStepID
		{
			get
			{
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					m_nLinkedSOPID = data.LinkedActionStepID;
				}
				return m_nLinkedSOPID;
			}
			set
			{
				m_nLinkedSOPID = value;
				if (mSection != null)
				{
					Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)mSection.Data;
					if( m_nLinkedSOPID != data.LinkedActionStepID)
					{
                        if (m_bEnabledSnapshot == true)
						    UndoRedoManager.Instance.SaveSnapshot("전활될 SOP 지정");
						data.LinkedActionStepID = value;
						if (mSection.GetParent() != null)
							mSection.GetParent().Refresh();
					}				
				}				
			}
		}
	}
}