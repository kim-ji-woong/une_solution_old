using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Sections
{
    public abstract class SectionData
    {
        public enum TextHAlign 
        {
            NONE = 0,
            LEFT,
            MIDDLE,
            RIGHT
        }

        public enum TextVAlign
        {
            NONE = 0,
            UP,
            MIDDLE,
            BOTTOM
        }

        protected TextVAlign m_TextVerticalAlign = TextVAlign.MIDDLE;
        public TextVAlign TextVerticalAlign
        {
            get { return m_TextVerticalAlign; }
            set 
            {
                m_TextVerticalAlign = value;
                if (mOwner != null)
                {
                    mOwner.AdjustStringFormat();
                }
            }
        }

        protected TextHAlign m_TextHorizontalAlign = TextHAlign.MIDDLE;
        public TextHAlign TextHorizontalAlign
        {
            get { return m_TextHorizontalAlign; }
            set 
            {
                m_TextHorizontalAlign = value; 
                if( mOwner != null)
                {
                    mOwner.AdjustStringFormat();
                }
            }
        }


        protected float m_fLineSpace = 0.0f;
        public float LineSpace
        {
            get { return m_fLineSpace; }
            set { m_fLineSpace = value; }
        }

        private bool m_showMessageBox = true;

        private string m_strTitle = "";
        protected string m_strComponentID = "";

        protected Section mOwner = null;
        public Section Owner
        {
            get { return mOwner; }
            set
            {
                mOwner = value; 
                if( mOwner != null)
                {
                    mOwner.AdjustStringFormat();
                }
            }
        }

        protected Section mAggSection = null;
        public Section AggSection
        {
            get { return mAggSection; }
            set { mAggSection = value; }
        }

        protected Section mFlowLinkSection = null;
        public Sections.Section FlowLinkSection
        {
            get { return mFlowLinkSection; }
            set { mFlowLinkSection = value; }
        }

        protected int m_nSectionNumber = -1;
        public int SectionNumber
        {
            get { return m_nSectionNumber; }
            set { m_nSectionNumber = value; }
        }

        // DB Component ID
        private int m_nID = -1;

        // Key : Component ID
        // Value : Section ID
        // Section은 타입이 다르면 같은 ID가 존재할 수도 있는데, 어차피 Section 타입이 다르면 Component ID도 다르게 사용한다.
        protected static Dictionary<string, int> ID_LIST = new Dictionary<string, int>();
        //protected static ArrayList ID_LIST = new ArrayList();

        public static void ClearIDList()
        {
            ID_LIST.Clear();

            SectionDataAnnotation.ClearIDCount();
            SectionDataDecision.ClearIDCount();
            SectionDataEndPoint.ClearIDCount();
            SectionDataExternal.ClearIDCount();
            SectionDataInternal.ClearIDCount();
            SectionDataLink.ClearIDCount();
            SectionDataProcess.ClearIDCount();
            SectionDataTransSOP.ClearIDCount();
            SectionDataTransmission.ClearIDCount();
        }

        protected void MakeDefaultID(string strStepName, string strTeamName, Dictionary<string, int> dicIDCount, string strComponentType)
        {
            string strTag = strStepName + "_" + strTeamName + "_" + strComponentType;

            if (dicIDCount.ContainsKey(strTag))
            {
                int nTagCount = dicIDCount[strTag];

                m_strComponentID = string.Format("{0}_{1}", strTag, nTagCount + 1);
                dicIDCount[strTag] = nTagCount + 1;
            }
            else
            {
                m_strComponentID = strTag + "_1";
                dicIDCount[strTag] = 1;
            }

            ID_LIST[m_strComponentID] = this.m_nID;
            //ID_LIST.Add(m_strComponentID);
        }

        // strID가 이미 존재하는 ID인지 검사한다.
        // 존재하지 않으면 true, 존재하면 false를 리턴한다.
        protected bool CheckExist(string strID)
        {
            int nSectionID;

            if (ID_LIST.TryGetValue(strID, out nSectionID) == false)
                return true;

            if (nSectionID == m_nID)
                return true;

            return false;
            //return !ID_LIST.Contains(strID);
        }

        // Default ID는 [Component 고유 문자열 + '_' + 숫자]의 형식을 따른다.
        // strID가 Default ID Type인지 알려준다.
        protected static bool CheckDefaultStringType(string strID, out string strTag, out int nTagCount)
        {
            strTag = "";
            nTagCount = 0;

            int nLastIndex = strID.LastIndexOf('_');
            if (nLastIndex < 0)
                return false;

            string str = strID.Substring(nLastIndex + 1);

            try
            {
                nTagCount = int.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }

            strTag = strID.Substring(0, nLastIndex);
            return true;
        }

        public abstract void SetDefaultID(string strStepName, string strTeamName);
        protected abstract void AddDefaultID(string strTag, int nTagCount);
        protected abstract void RemoveMaxDefaultCount(string strTag, int nTagCount);

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        // 문자열 ID
        public string ComponentID
        {
            get { return m_strComponentID; }
            set
            {
                if (m_strComponentID != value)
                {
                    if (!CheckExist(value))
                    {
                        if (m_showMessageBox)
                            MessageBox.Show(value + "\r\n이미 존재하는 ID입니다.");
                    }
                    else
                    {
                        string strTag;
                        int nTagCount;

                        if (CheckDefaultStringType(value, out strTag, out nTagCount))
                            AddDefaultID(strTag, nTagCount);
                        else
                        {
                            // 기존 ID가 Default String Type인지 검사한다.
                            if (CheckDefaultStringType(m_strComponentID, out strTag, out nTagCount))
                            {
                                // 기존 ID의 Tag Count가 최대값이면 최대값을 1 낮춰준다.
                                RemoveMaxDefaultCount(strTag, nTagCount);
                            }
                        }

                        ID_LIST[value] = this.m_nID;
                        //ID_LIST.Add(value);
                        m_strComponentID = value;
                    }
                }
            }
        }

        // DB Component ID
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public bool ShowMessageBox
        {
            get { return m_showMessageBox; }
            set { m_showMessageBox = value; }
        }

        private string m_strExprOrigin = "";
        public string ExpressionOrigin
        {
            get { return m_strExprOrigin; }
            set { m_strExprOrigin = value; }
        }

        private string m_strExpr = "";
        public string Expression
        {
            get { return m_strExpr; }
            set { m_strExpr = value; }
        }
        private bool m_bShowExpr = false;
        public bool ShowExpression
        {
            get { return m_bShowExpr; }
            set 
            {
                m_bShowExpr = value;
                //m_bShowTempExpr = value;
            }
        }

        private bool m_bShowTempExpr = false;
        public bool ShowTempExpression
        {
            get { return m_bShowTempExpr; }
            set { m_bShowTempExpr = value; }
        }

        public void ResetShowExpression()
        {
            m_bShowTempExpr = m_bShowExpr;
        }
    }
}
