using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace UnE.SenarioMaker
{
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
                using (PopupNote form = new PopupNote(false))
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
                using (PopupNote form = new PopupNote(true))
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

    public class PropertyEndpoint : SectionPropertiesBase
    {

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("종류")]
        [Description("컴포넌트의 타입을 표시합니다.")]
        public string Desc
        {
            get { return m_szDesc; }
        }

        [Category("일반")]
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

        private string m_szEndType;

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("구분")]
        [Description("시작 또는 종료를 선택합니다.")]
        [TypeConverter(typeof(EndTypeConverter))]
        public string Type
        {
            get
            {
                if( mSection != null)
                {
                    Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)mSection.Data;
                    if( data != null)
                    {
                        bool bBeginPoint = data.IsBegin;
                        if (bBeginPoint)
                            m_szEndType = EndTypes.TypeList[0];
                        else
                            m_szEndType = EndTypes.TypeList[1];
                    }
                }
                return m_szEndType;
            }
            set 
            {
                if( m_szEndType != value)
                {
                    if (mSection != null)
                    {
                        Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)mSection.Data;
                        if (data != null)
                        {
                            UndoRedoManager.Instance.SaveSnapshot("컴포넌트 타입 변경");
                            data.IsBegin = !data.IsBegin;
                        }
                    }
                }
                m_szEndType = value;  
            }
        }

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("내용")]
        [Description("컴포넌트의 내용을 표시합니다.")]
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
       

    public class PropertyDecisionProcess : SectionPropertiesBase
    {
        [Category("일반")]
        [Browsable(true)]
        [DisplayName("종류")]
        [Description("컴포넌트의 타입을 표시합니다.")]
        public string Desc
        {
            get { return m_szDesc; }
        }

        [Category("일반")]
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

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("내용")]
        [Description("컴포넌트의 내용을 표시합니다.")]
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
                m_szText = value;
                if (mSection != null)
                {
                    UndoRedoManager.Instance.SaveSnapshot("컴포넌트 내용 편집");
                 
                    mSection.Title = m_szText;
                    if (mSection.GetParent() != null)
                        mSection.GetParent().Refresh();
                }

            }
        }

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("수식")]
        [Description("컴포넌트의 수식을 표시합니다.")]
        [Editor(typeof(ExprEditor), typeof(UITypeEditor))]
        public string Expr
        {
            get 
            {
                if (mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        m_szExpr = data.Expression;
                    }
                }
                return m_szExpr; 
            }
            set 
            {
                m_szExpr = value;
                if (mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 수식 편집");

                        data.Expression = m_szExpr;
                        if (mSection.GetParent() != null)
                            mSection.GetParent().Refresh();
                    }
                }
            }
        }

        [Category("일반")]
        [Browsable(true)]
        [DisplayName("표시 옵션")]
        [Description("컴포넌트를 화면에 표시하는 방법입니다.")]
        [TypeConverter(typeof(DisplayTypeConverter))]
        public string TextOption
        {
            get
            {
                if (mSection != null)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        if(data.ShowExpression)
                            m_szDisplay = DisplayTypes.TypeList[1];
                        else
                            m_szDisplay = DisplayTypes.TypeList[0];
                    }
                }
                return m_szDisplay;
            }
            set 
            {
                if (m_szDisplay != value)
                {
                    Sections.SectionData data = mSection.Data;
                    if (data != null)
                    {
                        UndoRedoManager.Instance.SaveSnapshot("컴포넌트 표시 타입 변경");

                        data.ShowExpression = !data.ShowExpression;

                        if (FormMain.Instance.ContentForm.ContentOption == FormContent.ShowOption.Component)
                        {
                            data.ResetShowExpression();
                        }

                        if (mSection.GetParent() != null)
                            mSection.GetParent().Refresh();
                    }
                }
                m_szDisplay = value;

            }
        }
    }

    public class PropertyAnnotation : SectionPropertiesBase
    {       
        [Category("일반")]
        [Browsable(true)]
        [DisplayName("종류")]
        [Description("컴포넌트의 타입을 표시합니다.")]
        public string Desc
        {
            get
            {
                return m_szDesc;
            }
        }
                
        [Category("일반")]
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
                    if( data != null)
                    {
                        m_szID = data.ComponentID;
                    }
                }
                return m_szID; 
            }
        }
        
        [Category("일반")]
        [Browsable(true)]
        [DisplayName("내용")]
        [Description("설명의 내용을 입력합니다.")]
        [Editor(typeof(TextEditor), typeof(UITypeEditor))]
        public string Text
        {            
            get
            { 
                if(mSection != null)
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
                    if (mSection.GetParent()!= null)
                        mSection.GetParent().Refresh();
                }
                m_szText = value; 
            }
        }
    }

    public class SectionPropertiesBase
    {
        protected string m_szDesc = "";
        protected string m_szText = "";
        protected string m_szID = "";
        protected string m_szExpr = "";
        protected string m_szDisplay = "";

        protected Sections.Section mSection = null;
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
                }
            }
        }  
    }
}
