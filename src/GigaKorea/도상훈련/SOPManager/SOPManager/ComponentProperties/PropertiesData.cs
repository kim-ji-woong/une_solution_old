using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Globalization;

namespace SOPManager
{

	enum TextType
	{
		MissionItem = 1,
		TaskItem = 2,
		AnnotationDescription = 3,
		EndPointDescription = 4,
		LinkDescription,
		ContentItem,
		TransmissionContentItem,
		TransmissionDescription,
		TransSOPDescription,
		InternalDescription,
		ExternalDescription,
		GroupDescription
	}
	
	public static class PropertyGridExtensions
	{
		/// <summary>
		/// Gets the (private) PropertyGridView instance.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <returns>The PropertyGridView instance.</returns>
		private static object GetPropertyGridView(PropertyGrid propertyGrid)
		{
			//private PropertyGridView GetPropertyGridView();
			//PropertyGridView is an internal class...
			MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance);
			return methodInfo.Invoke(propertyGrid, new object[] { });
		}

		/// <summary>
		/// Gets the width of the left column.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <returns>
		/// The width of the left column.
		/// </returns>
		public static int GetInternalLabelWidth(this PropertyGrid propertyGrid)
		{
			//System.Windows.Forms.PropertyGridInternal.PropertyGridView
			object gridView = GetPropertyGridView(propertyGrid);

			//protected int InternalLabelWidth
			PropertyInfo propInfo = gridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
			return (int)propInfo.GetValue(gridView);
		}

		/// <summary>
		/// Moves the splitter to the supplied horizontal position.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <param name="xpos">The horizontal position.</param>
		public static void MoveSplitterTo(this PropertyGrid propertyGrid, int xpos)
		{
			//System.Windows.Forms.PropertyGridInternal.PropertyGridView
			object gridView = GetPropertyGridView(propertyGrid);

			//private void MoveSplitterTo(int xpos);
			MethodInfo methodInfo = gridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
			methodInfo.Invoke(gridView, new object[] { xpos });
		}
	}

	internal class DisplayTypes
	{
		static DisplayTypes()
		{
			DisplayTypes.TypeList = new string[2];
			DisplayTypes.TypeList[0] = "내용으로 표시";
			DisplayTypes.TypeList[1] = "수식으로 표시";
		}

		internal static string[] TypeList;
	}

	internal class MsgSendType
	{
		internal static string[] TypeList;
		static MsgSendType()
		{
			MsgSendType.TypeList = new string[2];
			MsgSendType.TypeList[0] = "팀장에게만 전송";
			MsgSendType.TypeList[1] = "팀 전체에게 전송";		
		}
	}

	internal class UsingType
	{
		internal static string[] TypeList;
		static UsingType()
		{
			UsingType.TypeList = new string[2];
			UsingType.TypeList[0] = "사용안함";
			UsingType.TypeList[1] = "사용";
		}
	}

    internal class WeekTypes
    {
        static WeekTypes()
        {
            WeekTypes.TypeList = new string[2];
            WeekTypes.TypeList[0] = "주간";
            WeekTypes.TypeList[1] = "야간";
        }

        internal static string[] TypeList;
    }

	internal class EndTypes
	{
		static EndTypes()
		{
			EndTypes.TypeList = new string[2];
			EndTypes.TypeList[0] = "시작";
			EndTypes.TypeList[1] = "종료";
		}

		internal static string[] TypeList;
	}

    internal class HAlignmentTypes
    {
        internal static string[] TypeList;
        static HAlignmentTypes()
        {
            HAlignmentTypes.TypeList = new string[3];
            HAlignmentTypes.TypeList[0] = "왼쪽";
            HAlignmentTypes.TypeList[1] = "가운데";
            HAlignmentTypes.TypeList[2] = "오른쪽";
        }
    }

    internal class VAlignmentTypes
    {
        internal static string[] TypeList;
        static VAlignmentTypes()
        {
            VAlignmentTypes.TypeList = new string[3];
            VAlignmentTypes.TypeList[0] = "위쪽";
            VAlignmentTypes.TypeList[1] = "가운데";
            VAlignmentTypes.TypeList[2] = "아래쪽";
        }
    }

	public class DisplayTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
			System.ComponentModel.TypeConverter.StandardValuesCollection
			GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(DisplayTypes.TypeList);
		}
	}

	public class MsgSendTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
			System.ComponentModel.TypeConverter.StandardValuesCollection
			GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(MsgSendType.TypeList);
		}
	}

	public class UsingTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
			System.ComponentModel.TypeConverter.StandardValuesCollection
			GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(UsingType.TypeList);
		}
	}

	public class PopupUsingTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
			System.ComponentModel.TypeConverter.StandardValuesCollection
			GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(UsingType.TypeList);
		}
	}

	public class SmsUsingTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
			System.ComponentModel.TypeConverter.StandardValuesCollection
			GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(UsingType.TypeList);
		}
	}
    
	public class EndTypeConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override
		   System.ComponentModel.TypeConverter.StandardValuesCollection
		   GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(EndTypes.TypeList);
		}
	}

    public class HAlignmentTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override
           System.ComponentModel.TypeConverter.StandardValuesCollection
           GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(HAlignmentTypes.TypeList);
        }
    }

    public class VAlignmentTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override
           System.ComponentModel.TypeConverter.StandardValuesCollection
           GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(VAlignmentTypes.TypeList);
        }
    }

	public class BroadcastEditor : UITypeEditor
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
				using (PopupBroadcastMessage form = new PopupBroadcastMessage())
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                    frame.Sizable = false;
                    form.PropertiesInternal = (PropertiesInternal)context.Instance;
					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						//value = (string)form.GetMessage();                        
					}
				}
			}
			return value;
		}
	}

	public class TextEditor : UITypeEditor
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
				//if (context.PropertyDescriptor.IsReadOnly == true)
				//	return value;			
				
				using (PopupNote form = new PopupNote())
				{
					string szText = context.PropertyDescriptor.DisplayName + " 입력";
					form.Text = szText;
                    form.Content = szValue;
                    form.InitText();

                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                    frame.Sizable = false;					

					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						value = form.Content;
					}
				}
			}
			return value;
		}
	}

	public class TextEditor2 : UITypeEditor
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
				//if (context.PropertyDescriptor.IsReadOnly == true)
				//	return value;			

				using (PopupNoteEx form = new PopupNoteEx())
				{
					string szText = context.PropertyDescriptor.DisplayName + " 입력";
					form.Text = szText;

                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
					form.Content = szValue;
					form.InitText();

					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						value = form.Content;
					}
				}
			}
			return value;
		}
	}

	public class MissionEditor : UITypeEditor
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
				using (PopupMission form = new PopupMission())
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                    frame.Sizable = false;
					form.PropertiesProcess = (PropertiesProcess)context.Instance;
					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
                        form.PropertiesProcess.EnabledSnapshot = false;                        
						form.PropertiesProcess.MissionList = (ArrayList)form.MissionList.Clone();
						value = form.TitleText;
                        form.PropertiesProcess.EnabledSnapshot = true;
					}
				}
			}
			return value;
		}
	}

	public class ExprEditor : UITypeEditor
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
				using (PopupNoteExpr form = new PopupNoteExpr(true))
				{
					form.Text = szValue;
					if (svc.ShowDialog(form) == DialogResult.OK)
					{
						value = form.Text;
					}
				}
			}
			return value;
		}
	}

	public class TeamNameConverter : CollectionConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context,
									System.Globalization.CultureInfo culture,
									object value, Type destType)
		{
			if (destType == typeof(string) && value is ArrayList)
			{
				string szResult = "";
				ArrayList arTeam = (ArrayList)value;
				if (arTeam != null)
				{
					for (int i = 0; i < arTeam.Count; i++)
					{
						if (i != 0)
							szResult += ",";

						if(arTeam[i].GetType() == typeof(Sections.ExternalTeamData))
						{
							Sections.ExternalTeamData team = (Sections.ExternalTeamData)arTeam[i];
							szResult += team.TeamName;
						}
						else if (arTeam[i].GetType() == typeof(Sections.SOPTeam))
						{
							Sections.SOPTeam team = (Sections.SOPTeam)arTeam[i];
							szResult += team.TeamName;
						}
					
					}

					return szResult;
				}
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}

	public class ProcessTeamEditor : CollectionEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public ProcessTeamEditor(Type type)
			: base(type)
		{

		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			ArrayList szValue = value as ArrayList;
			if (svc != null && szValue != null)
			{
                bool bReadOnly = context.PropertyDescriptor.IsReadOnly;
                if (bReadOnly == true)
                    return value;

				using (PopupSelectTeam form = new PopupSelectTeam(true))
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                    frame.Sizable = false;
					PropertiesProcess process = (PropertiesProcess)context.Instance;
                    form.TeamList = szValue;
					form.PropertiesProcess = process;
					
					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						value = form.TeamList;
					}
				}
			}
			return value;
		}
	}
    
    public class SMSCommanderEditor : CollectionEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public SMSCommanderEditor(Type type)
            : base(type)
        {

        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            Sections.SectionCommander commander = value as Sections.SectionCommander;
            if (svc != null)
            {
                if (commander == null)
                    commander = new Sections.SectionCommander();
                
                bool bReadOnly = context.PropertyDescriptor.IsReadOnly;
                if (bReadOnly == true)
                    return value;

                SectionPropertiesBase properties = (SectionPropertiesBase)context.Instance;
                // 171207 KYJ
                //using (Popup.PopupSelectCommander frm = new Popup.PopupSelectCommander(commander))
                using (PopupSelectTeam frm = new PopupSelectTeam(commander))
                {
                    using (UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm))
                    {
                        frame.Text = "발신자 선택";
                        frame.Sizable = false;
                        if (svc.ShowDialog(frame) == System.Windows.Forms.DialogResult.OK)
                        {
                            Sections.SectionCommander Commander = new Sections.SectionCommander();

                            commander.Team = frm.SelectedTeam;
                            commander.IsTeamMember = false;
                            commander.DisplayText = frm.DisplayText;

                            
                            if( properties.GetType() == typeof(PropertiesInternal))
                            {
                                ((PropertiesInternal)properties).SectionCommander = commander;
                            }
                            else if (properties.GetType() == typeof(PropertiesProcess))
                            {
                                ((PropertiesProcess)properties).SectionCommander = commander;
                            }
                        }
                    }                    
                }
            }
            return value;
        }
    }

    public class InternalReciverEditor : CollectionEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public InternalReciverEditor(Type type)
            : base(type)
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            ArrayList szValue = value as ArrayList;
            if (svc != null && szValue != null)
            {

                bool bReadOnly = context.PropertyDescriptor.IsReadOnly;
                if (bReadOnly == true)
                    return value;

                using (PopupSelectTeam form = new PopupSelectTeam(true))
                {
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
                    frame.Sizable = false;
                    PropertiesInternal process = (PropertiesInternal)context.Instance;
                    form.TeamList = szValue;
                    form.PropertiesInternal = process;
                   

                    if (svc.ShowDialog(frame) == DialogResult.OK)
                    {
                        value = form.TeamList;
                    }
                }
            }
            return value;
        }
    }



	public class ReciveTeamEditor : CollectionEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public ReciveTeamEditor(Type type)
			: base(type)
		{

		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			ArrayList szValue = value as ArrayList;
			if (svc != null && szValue != null)
			{

                bool bReadOnly = context.PropertyDescriptor.IsReadOnly;
                if (bReadOnly == true)
                    return value;

				if(context.Instance.GetType() == typeof(PropertiesExternal))
				{
					PropertiesExternal process = (PropertiesExternal)context.Instance;

					using (PopupSelectExternalReceive form = new PopupSelectExternalReceive())
					{
                        UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
						form.SelectedTeamList = szValue;
                        if (svc.ShowDialog(frame) == DialogResult.OK)
						{
							value = form.SelectedTeamList;
						}
					}
				}

				else if (context.Instance.GetType() == typeof(PropertiesTransmission))
				{
					PropertiesTransmission process = (PropertiesTransmission)context.Instance;
                   
					using (PopupSelectExternalReceive form = new PopupSelectExternalReceive())
					{
                        UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
						form.SelectedTeamList = szValue;
                        if (svc.ShowDialog(frame) == DialogResult.OK)
						{
							value = form.SelectedTeamList;
						}
					}
				}
				
			}
			return value;
		}
	}

	public class TermEditor : UITypeEditor
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
				using (PopupProcessTerm form = new PopupProcessTerm())
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);

					PropertiesLevel levels = (PropertiesLevel)context.Instance;
					form.SetTerm(levels, szValue);
					
					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						value = form.Term;
					}
				}
			}
			return value;
		}
	}

    public class WeekTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override
           System.ComponentModel.TypeConverter.StandardValuesCollection
           GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(WeekTypes.TypeList);
        }
    }

    public class RepeatTimeEditor : UITypeEditor
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
				using (PopupProcessNumber form = new PopupProcessNumber())
				{
                    UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);

					PropertiesLevel levels = (PropertiesLevel)context.Instance;
					form.ProcessNubmer = szValue;
					form.NumberType = levels.NumberType;

					if (svc.ShowDialog(frame) == DialogResult.OK)
					{
						value = form.ProcessNubmer;
						levels.NumberType = form.NumberType;
					}
				}
			}
			return value;
		}
	}

	public class PeriodEditor : UITypeEditor
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
				using (PopupProcessTime form = new PopupProcessTime())
				{
					if(context.Instance.GetType() == typeof(PropertiesLevel))
					{
						PropertiesLevel properties = (PropertiesLevel)context.Instance; ;
                        UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
						form.SetProcessingTime(szValue);

						if (svc.ShowDialog(frame) == DialogResult.OK)
						{
							properties.ProcessType = form.ProcessTimeType;
							value = form.ProcessTime;							
						}
					}
					else if (context.Instance.GetType() == typeof(PropertiesProcess))
					{
						PropertiesProcess properties = (PropertiesProcess)context.Instance; ;
                        UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(form);
						form.SetProcessingTime(szValue);

						if (svc.ShowDialog(frame) == DialogResult.OK)
						{
							value = form.ProcessTime;							
						}
					}
				}
			}
			return value;
		}
	}

    public class FontEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            System.Drawing.Font szValue = value as System.Drawing.Font;
            if (svc != null && szValue != null)
            {
                using (FontDialog form = new FontDialog())
                {
                    form.Font = szValue;
                    form.ShowColor = false;
                    form.FontMustExist = true;
                    form.ShowEffects = true;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        value = form.Font;
                    }                    
                }
            }
            return value;
        }
    }

    public class FontConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SectionPropertiesBase propery = (SectionPropertiesBase)context.Instance;
            System.Drawing.Font font = propery.TextFont;
            return font;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            destinationType = typeof(string);
            System.Drawing.Font font = (System.Drawing.Font)value;

            string szResult = font.Name;
            
            float size = font.Size;
            szResult += string.Format(", {0}pt, ", size);

            bool bStyle = false;
            if( font.Bold == true)
            {
                if (bStyle == false)
                {
                    szResult += "스타일=";
                    bStyle = true;
                }
                else
                    szResult += ", ";                

                if(font.Italic == true)
                {
                    szResult += "굵은";
                }
                else
                    szResult += "굵게";
            }

            if( font.Italic == true)
            {
                if( bStyle == false)
                {
                    szResult += "스타일=";
                    bStyle = true;
                }
                else                
                    szResult += " ";
                
                szResult += "기울임꼴";
            }

            if( font.Underline == true)
            {
                if( bStyle == false)
                {
                    szResult += "스타일=";
                    bStyle = true;
                }
                else                
                    szResult += " ";
                
                szResult += "밑줄";
            }

            if( font.Strikeout == true)
            {
                if( bStyle == false)
                {
                    szResult += "스타일=";
                    bStyle = true;
                }
                else                
                    szResult += " ";
                
                szResult += "최소선";
            }
            return szResult;
        }
    }
       
	
	public class SectionPropertiesBase
	{
        protected bool m_bEnabledSnapshot = true;

        [Browsable(false)]
        public bool EnabledSnapshot
        {
            get { return m_bEnabledSnapshot; }
            set { m_bEnabledSnapshot = value; }
        }

		protected string m_szDesc = "";
		protected string m_szText = "";
		protected string m_szID = "";
		protected string m_szExpr = "";
		protected string m_szDisplay = "";
		protected string m_szProcessTime = "";

        private float m_nLineSpace = 0.0f;
        private System.Drawing.Font m_Font = null;


		protected Sections.Section mSection = null;

        public PropertyGrid formParent = null;
				
		public Sections.Section GetSection()
		{
			return mSection; 
		}

        public void SetParent(PropertyGrid form)
        {
            formParent = form;
        }
		
		public void SetData(Sections.Section section)
		{
			mSection = section;

			if (mSection != null)
			{
				Sections.Section.ComponentType nType = mSection.GetComponentType();
				switch (nType)
				{
					case Sections.Section.ComponentType.ANNOTATION:
						m_szDesc = "설명";
						break;
					case Sections.Section.ComponentType.DECISION:
						m_szDesc = "판단";
						break;
					case Sections.Section.ComponentType.ENDPOINT:
						m_szDesc = "시작/종료";
						break;
					case Sections.Section.ComponentType.PROCESS:
						m_szDesc = "프로세스";
						break;
					case Sections.Section.ComponentType.EXTERNAL:
						m_szDesc = "외부상황전파";
						break;
					case Sections.Section.ComponentType.INTERNAL:
						m_szDesc = "내부상황전파";
						break;
					case Sections.Section.ComponentType.LINK:
						m_szDesc = "프로세스 연결";
						break;
					case Sections.Section.ComponentType.TRANSMISSION:
						m_szDesc = "상황전파";
						break;
					case Sections.Section.ComponentType.TRANSSOP:
						m_szDesc = "시나리오로 전환";
						break;
					case Sections.Section.ComponentType.GROUP:
						m_szDesc = "컴포턴트 그룹";
						break;
				}
                
                Sections.SectionData data = section.Data;
                GetTextAlign(data);

				m_X = section.Position.X;
				m_Y = section.Position.Y;
                
                m_Font = section.GetFont();

                if (data.LineSpace != 0.0f)
                    m_nLineSpace = data.LineSpace;
                else
                    m_nLineSpace = m_Font.Height - m_Font.Size;

                m_TextColor = section.TextColor;
			}
		}

        private void SetTextVAlign(Sections.SectionData data, string szvalue)
        {
            if (szvalue == VAlignmentTypes.TypeList[0])
            {
                data.TextVerticalAlign = Sections.SectionData.TextVAlign.UP;
            }
            else if (szvalue == VAlignmentTypes.TypeList[1])
            {
                data.TextVerticalAlign = Sections.SectionData.TextVAlign.MIDDLE;
            }
            else if (szvalue == VAlignmentTypes.TypeList[2])
            {
                data.TextVerticalAlign = Sections.SectionData.TextVAlign.BOTTOM;
            }
            else
            {
                data.TextVerticalAlign = Sections.SectionData.TextVAlign.NONE;
            }

            mSection.AdjustStringFormat();
            Sections.PanelSection pane = mSection.GetParent();
			if( pane != null)
			{
                pane.Invalidate();
            }            
        }
        
        private void SetTextHAlign(Sections.SectionData data, string szvalue)
        {
            if (szvalue == HAlignmentTypes.TypeList[0])
            {
                data.TextHorizontalAlign = Sections.SectionData.TextHAlign.LEFT;
            }
            else if (szvalue == HAlignmentTypes.TypeList[1])
            {
                data.TextHorizontalAlign = Sections.SectionData.TextHAlign.MIDDLE;
            }
            else if (szvalue == HAlignmentTypes.TypeList[2])
            {
                data.TextHorizontalAlign = Sections.SectionData.TextHAlign.RIGHT;
            }    
            else
            {
                data.TextHorizontalAlign = Sections.SectionData.TextHAlign.NONE;
            }

            mSection.AdjustStringFormat();
            Sections.PanelSection pane = mSection.GetParent();
            if (pane != null)
            {
                pane.Invalidate();
            }   

        }

        private void GetTextAlign(Sections.SectionData data)
        {
            m_TextHAlign = HAlignmentTypes.TypeList[0];
            m_TextVAlign = VAlignmentTypes.TypeList[1];

            if (data != null)
            {
                if (data.TextVerticalAlign == Sections.SectionData.TextVAlign.UP)
                {
                    m_TextVAlign = VAlignmentTypes.TypeList[0];
                }
                else if (data.TextVerticalAlign == Sections.SectionData.TextVAlign.MIDDLE)
                {
                    m_TextVAlign = VAlignmentTypes.TypeList[1];
                }
                else if (data.TextVerticalAlign == Sections.SectionData.TextVAlign.BOTTOM)
                {
                    m_TextVAlign = VAlignmentTypes.TypeList[2];
                }

                if (data.TextHorizontalAlign == Sections.SectionData.TextHAlign.LEFT)
                {
                    m_TextHAlign = HAlignmentTypes.TypeList[0];
                }
                else if (data.TextHorizontalAlign == Sections.SectionData.TextHAlign.MIDDLE)
                {
                    m_TextHAlign = HAlignmentTypes.TypeList[1];
                }
                else if (data.TextHorizontalAlign == Sections.SectionData.TextHAlign.RIGHT)
                {
                    m_TextHAlign = HAlignmentTypes.TypeList[2];
                }
            }
        }

		private float m_X = 0;
		private float m_Y = 0;

        [Category("속성")]
        [Browsable(true)]
        [DisplayName("종류")]
        [Description("컴포넌트의 타입을 표시합니다.")]
        public string Desc
        {
            get { return m_szDesc; }
        }

        [Category("속성")]
        [Browsable(true)]
        [DisplayName("이름")]
        [Description("컴포넌트의 ID를 표시합니다.")]
        public string ID
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        m_szID = data.ComponentID;
                    }
                }
                return m_szID;
            }
        }


        private string m_TextHAlign = "";
        private string m_TextVAlign = "";

        [Category("속성")]
        [Browsable(true)]
        [DisplayName("\t문자열 수직정렬")]
        [TypeConverter(typeof(VAlignmentTypeConverter))]
        [Description("컴포넌트의 문자열의 수직정렬을 지정합니다.")]
        public string TextVAlign
        {
            get { return m_TextVAlign; }
            set
            {
                if (this.mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if( data != null)
                    {
                        if (m_TextVAlign != value)
                        {
                            UndoRedoManager.Instance.SaveSnapshot("수직정렬변경");
                            SetTextVAlign(data, value);
                        }
                    }
                }
                m_TextVAlign = value;
            }
        }
        
        [Category("속성")]
        [Browsable(true)]
        [DisplayName("\t문자열 수평정렬")]
        [TypeConverter(typeof(HAlignmentTypeConverter))]
        [Description("컴포넌트의 문자열의 수평정렬을 지정합니다.")]
        public string TextHAlign
        {
            get { return m_TextHAlign; }
            set
            {
                if (this.mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        if( m_TextHAlign != value)
                        {
                            UndoRedoManager.Instance.SaveSnapshot("수평정렬변경");
                            SetTextHAlign(data, value);
                        }
                    }
                }
                m_TextHAlign = value;
            }
        }

		[Category("속성")]
		[Browsable(true)]
		[DisplayName("X")]
		[Description("컴포넌트의 X위치를 표시합니다.")]		
		public float X
		{
			get { return m_X; }
			set 			
			{

				if( this.mSection != null)
				{
					
					Sections.PanelSection pane = mSection.GetParent();
					if( pane != null)
					{
						UndoRedoManager.Instance.SaveSnapshot("위치 이동");

						pane.SectionMove(mSection, new System.Drawing.PointF(value, mSection.Position.Y));
						pane.Invalidate();
					}
					
				}
				m_X = value; 

			}
		}
        [Category("속성")]
		[Browsable(true)]
		[DisplayName("Y")]
		[Description("컴포넌트의 Y위치를 표시합니다.")]		
		public float Y
		{
			get { return m_Y; }
			set
			{
				if (this.mSection != null)
				{
					
					Sections.PanelSection pane = mSection.GetParent();
					if (pane != null)
					{
						UndoRedoManager.Instance.SaveSnapshot("위치 이동");

						pane.SectionMove(mSection, new System.Drawing.PointF(mSection.Position.X, value));
						pane.Invalidate();
					}
				}
				m_Y = value;

			}
		}

        [Category("속성")]
        [Browsable(true)]
        [DisplayName("폰트")]
        [Description("컴포넌트의 텍스트 폰트를 지정합니다.")]
        [TypeConverter(typeof(FontConverter))]
        [Editor(typeof(FontEditor), typeof(UITypeEditor))]
        public System.Drawing.Font TextFont
        {
            get { return m_Font; }
            set
            {
                if (this.mSection != null)
                {
                    if (CompareFont(m_Font, value))
                    {
                        UndoRedoManager.Instance.SaveSnapshot("폰트 변경");
                        m_Font = value;
                        mSection.SetFont(value);

                        Sections.PanelSection pane = mSection.GetParent();
                        if (pane != null)
                            pane.Invalidate();
                    }
                }
            }
        }

        private bool CompareFont(System.Drawing.Font org, System.Drawing.Font oth)
        {
            if (org.FontFamily != oth.FontFamily)
                return true;
            if (org.Name != oth.Name)
                return true;
            if (org.Height != oth.Height)
                return true;
            if (org.Style != oth.Style)
                return true;
            if (org.Size != oth.Size)
                return true;
            if (org.Strikeout != oth.Strikeout)
                return true;
            if (org.Underline != oth.Underline)
                return true;
            if (org.Bold != oth.Bold)
                return true;
            if (org.Italic != oth.Italic)
                return true;
            return false;
        }

        protected System.Drawing.Color m_TextColor = System.Drawing.Color.Black;
        [Category("속성")]
        [Browsable(true)]
        [DisplayName("문자 색상")]
        [Description("컴포넌트의 텍스트의 색상 지정합니다.")]
        public System.Drawing.Color TextColor
        {
            get 
            { 
                return m_TextColor;
            }
            set
            {
                if (this.mSection != null)
                {
                    if (m_TextColor != value)
                    {
                        UndoRedoManager.Instance.SaveSnapshot("폰트 변경");

                       
                        m_TextColor = value;
                        mSection.TextColor = value;

                        Sections.PanelSection pane = mSection.GetParent();
                        if (pane != null)
                            pane.Invalidate();
                    }
                }
            }
        }

        [Category("속성")]
        [Browsable(false)]
        [DisplayName("줄간격")]        
        [Description("컴포넌트의 텍스트 줄간격을 지정합니다.")]
        public float LineSpace
        {
            get { return m_nLineSpace; }
            set
            {
                if (this.mSection != null)
                {
                    if (m_nLineSpace != value)
                    {

                        UndoRedoManager.Instance.SaveSnapshot("줄간격 변경");

                        mSection.Data.LineSpace = value;
                        m_nLineSpace = value;
                    }
                }
            }
        }

	}
}
