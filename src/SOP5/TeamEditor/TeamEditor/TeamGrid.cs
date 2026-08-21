using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;
using UnE.Controls;
using TeamEditor.Command;
using System.Drawing;

namespace TeamEditor
{
    public class TeamGrid : MergedDataGridView
    {
        // 사번
        public class MemberID : IComparable
        {
            private string m_strMemberID = "";
            private bool m_isChanged = false;

            public string ID
            {
                get { return m_strMemberID; }
                set { m_strMemberID = value; }
            }

            public bool IsChanged
            {
                get { return m_isChanged; }
                set { m_isChanged = value; }
            }

            public MemberID()
            {
            }

            public MemberID(string strMemberID, bool isChanged)
            {
                m_strMemberID = strMemberID;
                m_isChanged = isChanged;
            }

            public int CompareTo(object obj)
            {
                MemberID id1 = this;
                MemberID id2 = (MemberID)obj;

                return id1.m_strMemberID.CompareTo(id2.m_strMemberID);
            }

            public override string ToString()
            {
                if (!m_isChanged && m_strMemberID.Length > 0)
                    return TeamGrid.SECRET_VALUE;

                return m_strMemberID;
            }
        }

        public class PhoneNumber : IComparable
        {
            private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
            private int m_nBodyLen = 0;
            private bool m_isChanged = false;
            private bool m_isBlank = false;

            public string Number
            {
                get { return GetPhoneNumber(); }
                set { SetPhoneNumber(value); }
            }

            public bool IsChanged
            {
                get { return m_isChanged; }
                set { m_isChanged = value; }
            }

            public bool IsValid
            {
                get
                {
                    if (m_nHeader == 0 && m_nBody == 0 && m_nTail == 0)
                        return false;

                    return true;
                }
            }

            public bool IsBlank
            {
                get { return this.m_isBlank; }
            }

            public PhoneNumber()
            {
            }

            public PhoneNumber(string strPhoneNumber, bool isChanged)
            {
                if (!String.IsNullOrWhiteSpace(strPhoneNumber))
                {
                    SetPhoneNumber(strPhoneNumber);
                }
                else
                {
                    m_isBlank = true;
                }


                m_isChanged = isChanged;
            }

            public int CompareTo(object obj)
            {
                PhoneNumber phone1 = this;
                PhoneNumber phone2 = (PhoneNumber)obj;

                if (phone1.m_nHeader < phone2.m_nHeader)
                    return -1;
                else if (phone1.m_nHeader > phone2.m_nHeader)
                    return 1;

                if (phone1.m_nBody < phone2.m_nHeader)
                    return -1;
                else if (phone1.m_nBody > phone2.m_nBody)
                    return 1;

                if (phone1.m_nTail < phone2.m_nTail)
                    return -1;
                else if (phone1.m_nTail > phone2.m_nTail)
                    return 1;

                return 0;
            }

            public override string ToString()
            {
                string strPhoneNumber = GetPhoneNumber();

                //if (!m_isChanged && strPhoneNumber.Length > 0)
                //    return TeamGrid.SECRET_VALUE;

                return strPhoneNumber;
            }

            private string GetPhoneNumber()
            {
                if (m_nBodyLen == 3)
                    return string.Format("01{0}-{1:000}-{2:0000}", m_nHeader, m_nBody, m_nTail);
                else if (m_nBodyLen == 4)
                    return string.Format("01{0}-{1:0000}-{2:0000}", m_nHeader, m_nBody, m_nTail);

                return "";
            }

            private void SetPhoneNumber(string strPhoneNumber)
            {
                string[] arrTokens = strPhoneNumber.Trim().Split('-');
                int nTokenCount = arrTokens.Count();
               
                m_nHeader = m_nBody = m_nTail = m_nBodyLen = 0;

                if (nTokenCount == 3)
                    SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
                else if (nTokenCount == 2)
                    SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
                else if (nTokenCount == 1)
                    SetPhoneNumber2(strPhoneNumber.Trim());
                else
                {
                    m_isChanged = false;
                } 
            }

            private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
            {
                if (!strHead.StartsWith("01") || strHead.Length != 3)
                    return false;

                char chHead = strHead.ElementAt(2);

                if (chHead < '0' || chHead > '9')
                    return false;

                int nBody = 0, nTail = 0;
                int nBodyLen = strBody.Length;
                int nTailLen = strTail.Length;

                if (nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
                    return false;

                if (!int.TryParse(strBody, out nBody))
                    return false;

                if (!int.TryParse(strTail, out nTail))
                    return false;

                m_nHeader = chHead - '0';
                m_nBody = nBody;
                m_nTail = nTail;
                m_nBodyLen = nBodyLen;

                return true;
            }

            private bool SetPhoneNumber2(string strPhoneNumber)
            {
                int len = strPhoneNumber.Length;

                bool readNum = false;
                int nIndex1 = -1, nIndex2 = -1;

                for (int i=0;i<len;i++)
                {
                    char ch = strPhoneNumber.ElementAt(i);

                    if (ch >= '0' && ch <= '9')
                    {
                        readNum = true;
                    }
                    else if (ch == ' ' || ch == '\t')
                    {
                        if (readNum)
                        {
                            readNum = false;

                            if (nIndex1 < 0)
                                nIndex1 = i;
                            else
                            {
                                nIndex2 = i;
                                break;
                            }
                        }
                    }
                }

                if (nIndex1 >= 0 && nIndex2 > nIndex1)
                {
                    string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                    string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1 - 1).Trim();
                    string str3 = strPhoneNumber.Substring(nIndex2).Trim();

                    return SetPhoneNumber2(str1, str2, str3);
                }
                else if (nIndex1 >= 0)
                {
                    string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                    string str2 = strPhoneNumber.Substring(nIndex1).Trim();

                    int len1 = str1.Length;
                    int len2 = str2.Length;

                    if (len1 == 3 && (len2 == 7 || len2 == 8))
                    {
                        return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
                    }
                    else if ((len1 == 6 || len1 == 7) || len2 == 4)
                    {
                        return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
                    }
                }
                else
                {
                    if (len == 10 || len == 11)
                    {
                        string str1 = strPhoneNumber.Substring(0, 3);
                        string str2 = strPhoneNumber.Substring(3, len - 7);
                        string str3 = strPhoneNumber.Substring(len - 4);

                        return SetPhoneNumber2(str1, str2, str3);
                    }
                }

                return false;
            }
        }

        public class OfficePhoneNumber : IComparable
        {
            string[] m_strfirstNums = { "070", "080", "010", "011", "019", "02", "031", "032", "033", "041", "042", "043", "044", "051", "052", "053", "054", "055", "061", "062", "063", "064" };

            private int m_nTotal = 0;
            private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
            private int m_nHeaderLen = 0, m_nBodyLen = 0;
            private bool m_isChanged = false;
            private bool m_isBlank = false;

            public string Number
            {
                get { return GetPhoneNumber(); }
                set { SetPhoneNumber(value); }
            }

            public bool IsChanged
            {
                get { return m_isChanged; }
                set { m_isChanged = value; }
            }

            public bool IsValid
            {
                get
                {
                    if (m_nHeader == 0 && m_nBody == 0 && m_nTail == 0)
                        return false;

                    return true;
                }
            }

            public bool IsBlank
            {
                get { return this.m_isBlank; }
            }


            public OfficePhoneNumber()
            {
            }

            public OfficePhoneNumber(string strPhoneNumber, bool isChanged)
            {
                if (!String.IsNullOrWhiteSpace(strPhoneNumber))
                {
                    SetPhoneNumber(strPhoneNumber);
                }
                else
                {
                    m_isBlank = true;
                }


                m_isChanged = isChanged;
            }

            public int CompareTo(object obj)
            {
                OfficePhoneNumber phone1 = this;
                OfficePhoneNumber phone2 = (OfficePhoneNumber)obj;

                if (phone1.m_nHeader < phone2.m_nHeader)
                    return -1;
                else if (phone1.m_nHeader > phone2.m_nHeader)
                    return 1;

                if (phone1.m_nBody < phone2.m_nHeader)
                    return -1;
                else if (phone1.m_nBody > phone2.m_nBody)
                    return 1;

                if (phone1.m_nTail < phone2.m_nTail)
                    return -1;
                else if (phone1.m_nTail > phone2.m_nTail)
                    return 1;

                return 0;
            }

            public override string ToString()
            {
                string strPhoneNumber = GetPhoneNumber();

                //if (!m_isChanged && strPhoneNumber.Length > 0)
                //    return TeamGrid.SECRET_VALUE;

                return strPhoneNumber;
            }

            private string GetPhoneNumber()
            {
                string strHead = "";
                if (m_nHeaderLen == 2)
                    strHead = "{0:00}";
                else if (m_nHeaderLen == 3)
                    strHead = "{0:000}";
                else if (m_nHeaderLen == 4)
                    strHead = "{0:0000}";
                else
                    return "";

                string strBody = "";
                if (m_nTotal > 1)
                {
                    if (m_nBodyLen == 3)
                        strBody = "-{1:000}";
                    else if (m_nBodyLen == 4)
                        strBody = "-{1:0000}";
                    else
                        return "";
                }

                string strTail = "";
                if (m_nTotal > 2)
                    strTail = "-{2:0000}";

                if (m_nTotal == 1)
                    return string.Format(strHead, m_nHeader);
                else if (m_nTotal == 2)
                    return string.Format(strHead + strBody, m_nHeader, m_nBody);
                else if (m_nTotal == 3)
                    return string.Format(strHead + strBody + strTail, m_nHeader, m_nBody, m_nTail); 

                return "";
            }

            private void SetPhoneNumber(string strPhoneNumber)
            {
                string[] arrTokens = strPhoneNumber.Trim().Split('-');
                int nTokenCount = arrTokens.Count();

                m_nHeader = m_nBody = m_nTail = m_nHeaderLen = m_nBodyLen = 0;

                if (nTokenCount == 3)
                    SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
                else if (nTokenCount == 2 && strPhoneNumber.Length > 9)
                    SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
                else if (nTokenCount == 2 && strPhoneNumber.Length == 9)
                    SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), "");
                else if (nTokenCount == 1)
                    SetPhoneNumber2(strPhoneNumber.Trim());
                else
                {
                    m_isChanged = false;
                }
            }

            private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
            {
                int nHead = 0, nBody = 0, nTail = 0;
                int nHeadLen = strHead.Length;
                int nBodyLen = strBody.Length;
                int nTailLen = strTail.Length;

                //양식
                //00-000-0000
                //00-0000-0000
                //000-000-0000
                //000-0000-0000
                //0000-0000
                //0000

                //if (nHeadLen < 2 || nHeadLen > 3 || nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
                //    return false;

                if (nHeadLen > 0 && !int.TryParse(strHead, out nHead))
                    return false;

                if (nBodyLen > 0 && !int.TryParse(strBody, out nBody))
                    return false;

                if (nTailLen > 0 && !int.TryParse(strTail, out nTail))
                    return false;

                m_nHeader = nHead;
                m_nBody = nBody;
                m_nTail = nTail;
                m_nHeaderLen = nHeadLen;
                m_nBodyLen = nBodyLen;
                if (nHeadLen > 0 && nBodyLen > 0 && nTailLen > 0)
                    m_nTotal = 3;
                else if (nHeadLen > 0 && nBodyLen > 0 && nTailLen == 0)
                    m_nTotal = 2;
                else if (nHeadLen > 0 && nBodyLen == 0 && nTailLen == 0)
                    m_nTotal = 1;
                return true;
            }

            private bool SetPhoneNumber2(string strPhoneNumber)
            {
                int len = strPhoneNumber.Length;

                bool readNum = false;
                int nIndex1 = -1, nIndex2 = -1;

                for (int i = 0; i < len; i++)
                {
                    char ch = strPhoneNumber.ElementAt(i);

                    if (ch >= '0' && ch <= '9')
                    {
                        readNum = true;
                    }
                    else if (ch == ' ' || ch == '\t')
                    {
                        if (readNum)
                        {
                            readNum = false;

                            if (nIndex1 < 0)
                                nIndex1 = i;
                            else
                            {
                                nIndex2 = i;
                                break;
                            }
                        }
                    }
                }

                if (nIndex1 >= 0 && nIndex2 > nIndex1)
                {
                    string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                    string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1).Trim();
                    string str3 = strPhoneNumber.Substring(nIndex2).Trim();

                    return SetPhoneNumber2(str1, str2, str3);
                }
                else if (nIndex1 >= 0)
                {
                    string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                    string str2 = strPhoneNumber.Substring(nIndex1).Trim();

                    int len1 = str1.Length;
                    int len2 = str2.Length;

                    if (len1 == 3 && (len2 == 7 || len2 == 8))
                    {
                        return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
                    }
                    else if ((len1 == 6 || len1 == 7) || len2 == 4)
                    {
                        return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
                    }
                }
                else
                {
                    string str1 = "";
                    string str2 = "";
                    string str3 = "";

                    if (len == 4)
                    {
                        str1 = strPhoneNumber;
                        return SetPhoneNumber2(str1, str2, str3);
                    }
                    else if (len == 8)
                    {
                        str1 = strPhoneNumber.Substring(0, 4);
                        str2 = strPhoneNumber.Substring(4, 4);
                        return SetPhoneNumber2(str1, str2, str3);
                    }
                    else if (len == 9 || len == 10 || len == 11)
                    {
                        string head2 = strPhoneNumber.Substring(0, 2);
                        string head3 = strPhoneNumber.Substring(0, 3);

                        if (m_strfirstNums.Contains(head2))
                        {
                            str1 = strPhoneNumber.Substring(0, 2);
                            str3 = strPhoneNumber.Substring(len - 4);
                            str2 = strPhoneNumber.Substring(str1.Length, len - str1.Length - str3.Length);
                            return SetPhoneNumber2(str1, str2, str3);
                        }
                        else if (m_strfirstNums.Contains(head3))
                        {
                            str1 = strPhoneNumber.Substring(0, 3);
                            str3 = strPhoneNumber.Substring(len - 4);
                            str2 = strPhoneNumber.Substring(str1.Length, len - str1.Length - str3.Length);
                            return SetPhoneNumber2(str1, str2, str3);
                        } 
                    }
                }

                return false;
            }
        } 

        public enum GridType { RegularMember = 0, TemporaryNormal, TemporaryEmergency, ExternalCompanyTeam, UserDefinedTeam, None };

        public const string SECRET_VALUE = "*******";
        private Team m_teamCurrent = null;
        private Team m_teamCurrentRow = null;
        private ContextMenuStrip contextMenuStripRegularMember;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem tsMenuRemoveCompanyMembers;
        private ToolStripMenuItem tsMoveRegularTeam;
        private TeamTreeView m_linkedTree = null;

        // 편집 도중 데이터가 정렬되는 오류를 막기위한 변수
        private bool m_noSort = false;

        private GridType m_gridType = GridType.RegularMember;

        // gridRegular
        private const int TeamNameIndex = 1;        
        private const int NameIndex = 2;
        private const int PositionIndex = 3;
        private const int ColumnLevelIndex = 4;
        private const int SubLevelIndex = 5;
        private const int PhoneNumberIndex = 6;
        private const int GroupPositionIndex = 7;
        private const int SubPositionIndex = 8;
        private const int MemberIDIndex = 9;
        private const int OfficePhoneNumberIndex = 10;

        public int TeamNameIndex2 { get { return TeamNameIndex; } }
        public int NameIndex2 { get { return NameIndex; } }
        public int PositionIndex2 { get { return PositionIndex; } }
        public int SubPositionIndex2 { get { return SubPositionIndex; } }
        public int ColumnLevelIndex2 { get { return ColumnLevelIndex; } }
        public int SubLevelIndex2 { get { return SubLevelIndex; } }
        public int PhoneNumberIndex2 { get { return PhoneNumberIndex; } }
        public int GroupPositionIndex2 { get { return GroupPositionIndex; } }
        public int MemberIDIndex2 { get { return MemberIDIndex; } }
        public int OfficePhoneNumberIndex2 { get { return OfficePhoneNumberIndex; } }

        // gridTemporary
        public const int RoleIndex = 1;
        public const int DisplayNameIndex = 2;
        public const int TeamIndex = 3;
        public const int TeamButtonIndex = 4;
        public const int TeamLeaderIndex = 5;
        public const int Manager2Index = 6;
        public const int Manager2ButtonIndex = 7;
        public const int MemberCountIndex = 8;
        private ContextMenuStrip contextMenuStripTemporaryMember;
        private ToolStripMenuItem tsRemoveTemporaryMember;
        private ToolStripMenuItem tsMoveTemporaryMember;
        public const int IncludeChildTeamIndex = 9;
        public const int IncludeChildTeamImageIndex = 10;

        // gridExternal
        private const int ExternalCompanyMemberTeamNameIndex = 1;
        private const int ExternalCompanyMemberNameIndex = 2;
        private const int ExternalCompanyMemberLevelIndex = 3;
        private const int ExternalCompanyMemberPositionIndex = 4;
        private const int ExternalCompanyMemberPhoneNumberIndex = 5;
        private const int ExternalCompanyMemberDescriptionIndex = 6;
        private ContextMenuStrip contextMenuStripExternalMember;
        private ToolStripMenuItem tsMenuRemoveExternalMember;
        private ToolStripMenuItem tsMenuMoveExternalMember; 

        // gridUserDefinedTeam
        private const int UserDefinedTeamName = 1;
        private const int UserDefinedTeamPhoneNumber = 2;
        private ContextMenuStrip contextMenuStripUserDefinedTeam;
        private ToolStripMenuItem tsMenuRemoveUserDefinedTeam;
        private const int UserDefinedTeamFaxNumber = 3;


        public Team CurrentTeam
        {
            get { return m_teamCurrent; }
            set { m_teamCurrent = value; }
        }
        public Team CurrentTeamRow
        {
            get { return m_teamCurrentRow; }
            set { m_teamCurrentRow = value; }
        }

        public TeamTreeView LinkedTree
        {
            get { return m_linkedTree; }
            set { m_linkedTree = value; }
        }

        // 편집 도중 데이터가 정렬되는 오류를 막기위한 변수
        public bool NoSort
        {
            get { return m_noSort; }
            set { m_noSort = value; }
        }

        public GridType Type
        {
            get { return m_gridType; }
            set { m_gridType = value; }
        }

        private Int32 m_RowHeight;
        public Int32 RowHeight
        {
            get { return m_RowHeight; }
            set { m_RowHeight = value; }
        }

        public Int32 HorizentalScrollValue
        {
            get
            {
                return this.HorizontalScrollBar.Value;
            }
        }

        public ScrollBar VerticalScroll
        {
            get
            {
                return this.VerticalScrollBar;
            }
        }

        public TeamGrid()
        {
            InitializeComponent();                        

            this.Sorted += new System.EventHandler(this.OnSorted);
            this.ReadOnlyChanged += new System.EventHandler(this.OnReadOnlyChanged);
            this.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.OnRowsAdded);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TeamGrid_CellMouseDown);
            this.CellClick += new DataGridViewCellEventHandler(TeamGrid_CellClick);
            this.CellContentClick += new DataGridViewCellEventHandler(TeamGrid_CellContentClick);
            this.CellMouseLeave += TeamGrid_CellMouseLeave;
            this.CellMouseEnter += TeamGrid_CellMouseEnter;
            this.AllowUserToAddRowsChanged += TeamGrid_AllowUserToAddRowsChanged;            
        }

        public void AddWinRateChangeEvent()
        {
            FormMain.Instance.event_WinRateChanged += Instance_event_WinRateChanged;
        }

        void Instance_event_WinRateChanged()
        {
            FormMain.Instance.UpdateWindowRate(contextMenuStripRegularMember, FormMain.Instance.WindowWidthRate, FormMain.Instance.WindowHeightRate);
            FormMain.Instance.UpdateWindowRate(contextMenuStripTemporaryMember, FormMain.Instance.WindowWidthRate, FormMain.Instance.WindowHeightRate);
            FormMain.Instance.UpdateWindowRate(contextMenuStripExternalMember, FormMain.Instance.WindowWidthRate, FormMain.Instance.WindowHeightRate);
            FormMain.Instance.UpdateWindowRate(contextMenuStripUserDefinedTeam, FormMain.Instance.WindowWidthRate, FormMain.Instance.WindowHeightRate);  
        }

        public Bitmap GetCheckBoxImg(bool pbEnable)
        {
            Bitmap bmp = null;

            if (pbEnable)            
                bmp = global::TeamEditor.Properties.Resources.__COMMON_ckb_enableWhite;
            else
                bmp = global::TeamEditor.Properties.Resources.__COMMON_ckb_disableWhite;

            double[] dWinRate = FormMain.Instance.GetCurWindowRate();
            return new Bitmap(bmp, (int)(bmp.Width * dWinRate[0]), (int)(bmp.Height * dWinRate[1]));
        }

        void TeamGrid_AllowUserToAddRowsChanged(object sender, EventArgs e)
        {
            if(Type == GridType.RegularMember || Type == GridType.ExternalCompanyTeam)
            {
                TeamGrid tg = (TeamGrid)sender as TeamGrid;
                if (tg == null) return;

                if (tg.Rows.Count <= 0) return;

                if (tg.Rows[tg.Rows.Count - 1].IsNewRow && m_teamCurrent != null)
                {
                    tg.Rows[tg.Rows.Count - 1].Cells[1].Value = m_teamCurrent.TeamID;
                }
            }
        }

        public void SetColumnsAlignment(DataGridViewContentAlignment align)
        {
            foreach (DataGridViewColumn column in this.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public int GetLevelID(string strLevelName)
        {
            DataGridViewComboBoxColumn columnLevel = (DataGridViewComboBoxColumn)this.Columns[ColumnLevelIndex];
            int nItemCount = columnLevel.Items.Count;

            for (int i=0;i<nItemCount;i++)
            {
                object obj = columnLevel.Items[i];

                if (obj.ToString() == strLevelName)
                    return i;
            }

            return -1;
        }

        public void RefreshGrid()
        {
            this.Rows.Clear();

            if (m_teamCurrent == null && m_gridType == GridType.UserDefinedTeam)
                SelectUserDefinedTeam();
            else if (m_teamCurrent is RegularTeam)                            
                SelectRegularTeam((RegularTeam)m_teamCurrent,FormMain.Instance.IsEditMode);            
            else if (m_teamCurrent is TemporaryNormalTeam)
                SelectTemporaryTeam(m_teamCurrent, true);
            else if (m_teamCurrent is TemporaryEmergencyTeam)
                SelectTemporaryTeam(m_teamCurrent, false);
            else if (m_teamCurrent is ExternalTeam)
                SelectExternalCompanyTeam((ExternalTeam)m_teamCurrent, FormMain.Instance.IsEditMode);
        }

        public void SelectTeam(Team team, bool alwaysDo = false)
        {
            if (!alwaysDo && team == m_teamCurrent)
                return;

            m_teamCurrent = team;
            this.Rows.Clear();

            if (team == null && m_gridType == GridType.UserDefinedTeam)
                SelectUserDefinedTeam();
            else if (team is RegularTeam)
            {                     
                SelectRegularTeam((RegularTeam)team, FormMain.Instance.IsEditMode);

                if (this.Rows != null && this.Rows.Count > 0)
                    m_teamCurrentRow = (Team)this.Rows[0].Cells[TeamNameIndex].Tag;
            }
            else if (team is TemporaryNormalTeam)
            {
                SelectTemporaryTeam(team, true);
            }                
            else if (team is TemporaryEmergencyTeam)
            {
                SelectTemporaryTeam(team, false);
            }                
            else if (team is ExternalTeam)
            {
                SelectExternalCompanyTeam((ExternalTeam)team, FormMain.Instance.IsEditMode);
            }                
        }

        private void SelectTemporaryTeam(Team team, bool isNormal)
        {
            List<TemporaryMember> members = null;
            
            if (isNormal)
                members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)team);
            else
                members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)team);

            if (members == null)
                return;

            if (!m_noSort)
                members.Sort();

            foreach (TemporaryMember member in members)
            {
                DataGridViewRow row = MakeNewRow();

                // Cell들의 Tag를 지정하는 이유는 Cell Data 편집시 데이터가 수정되었는지 여부를 확인하기 위해서다.
                row.Cells[RoleIndex].Value = TemporaryMember.GetManagerTypeString(member.TemporaryManagerType);
                row.Cells[DisplayNameIndex].Value = member.DisplayName;
                row.Cells[TeamIndex].Value = member.GetTeamRealName();
                row.Cells[Manager2Index].Value = member.GetMemberRealName();
                
                if (member.IsTeamLeader)
                    row.Cells[TeamLeaderIndex].Value = "책임자";
                else if (member.MemberCount >= 0)
                    row.Cells[TeamLeaderIndex].Value = "팀원";
                else
                    row.Cells[TeamLeaderIndex].Value = "팀전체";

                if (member.MemberCount < 0)
                    row.Cells[MemberCountIndex].Value = null;
                else
                    row.Cells[MemberCountIndex].Value = member.MemberCount;

                row.Cells[IncludeChildTeamIndex].Value = member.IncludeChildTeam;
                row.Cells[IncludeChildTeamImageIndex].Value = GetCheckBoxImg(member.IncludeChildTeam);

                //if (member.IncludeChildTeam == true)
                //    row.Cells[IncludeChildTeamImageIndex].Value = global::TeamEditor.Properties.Resources.__COMMON_ckb_enable;
                //else
                //    row.Cells[IncludeChildTeamImageIndex].Value = global::TeamEditor.Properties.Resources.__COMMON_ckb_disable;

                row.Tag = member;

                AfterTemporaryCellEndEdit(row, member);
            }
        }

        private void AfterTemporaryCellEndEdit(DataGridViewRow row, TemporaryMember member)
        {
            if (member.TemporaryMemberType == TemporaryMember.MemberType.CompanyMember ||
                    member.TemporaryMemberType == TemporaryMember.MemberType.ExternalCompanyMember)
            {
                row.Cells[TeamLeaderIndex].Value = null;
                row.Cells[MemberCountIndex].Value = null;
                row.Cells[IncludeChildTeamIndex].Value = false;

                row.Cells[TeamLeaderIndex].ReadOnly = row.Cells[MemberCountIndex].ReadOnly = row.Cells[IncludeChildTeamIndex].ReadOnly = true;
            }
            else if (member.TemporaryMemberType == TemporaryMember.MemberType.LevelID)
            {
                row.Cells[TeamLeaderIndex].Value = null;
                row.Cells[IncludeChildTeamIndex].Value = false;

                row.Cells[MemberCountIndex].ReadOnly = false;
                row.Cells[TeamLeaderIndex].ReadOnly = row.Cells[IncludeChildTeamIndex].ReadOnly = true;
            }
            else
            {
                // Team일 경우
                if (row.Cells[TeamLeaderIndex].Value == null)
                {
                    row.Cells[TeamLeaderIndex].ReadOnly = row.Cells[MemberCountIndex].ReadOnly = false;
                }
                else if (row.Cells[TeamLeaderIndex].Value.ToString() == "책임자" || row.Cells[TeamLeaderIndex].Value.ToString() == "팀전체")
                {
                    row.Cells[MemberCountIndex].Value = null;

                    row.Cells[TeamLeaderIndex].ReadOnly = false;
                    row.Cells[MemberCountIndex].ReadOnly = true;
                }
                else// if (row.Cells[TeamLeaderIndex].Value.ToString() == "팀원")
                {
                    row.Cells[MemberCountIndex].ReadOnly = false;
                }

                row.Cells[IncludeChildTeamIndex].ReadOnly = false;

                row.Cells[IncludeChildTeamImageIndex].Value = GetCheckBoxImg(member.IncludeChildTeam);
                //if ((bool)row.Cells[IncludeChildTeamIndex].Value == true)
                //    row.Cells[IncludeChildTeamImageIndex].Value = global::TeamEditor.Properties.Resources.__COMMON_ckb_enable;
                //else
                //    row.Cells[IncludeChildTeamImageIndex].Value = global::TeamEditor.Properties.Resources.__COMMON_ckb_disable;

                row.Cells[Manager2Index].Value = member.GetMemberRealName();
            }

            for (int i = RoleIndex; i <= IncludeChildTeamIndex; i++)
            {
                row.Cells[i].Tag = row.Cells[i].Value;
            }

            row.Cells[TeamIndex].Tag = member.Team;
            row.Cells[Manager2Index].Tag = member.Member;
        }

        private void SelectExternalCompanyTeam(ExternalTeam team, bool isEditMode = false)
        {
            //List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(team);
            List<ExternalCompanyMember> members = null;
            if(isEditMode == false)
                members = DataManager.GetChildExternalCompanyMembers(team);
            else
                members = DataManager.GetChildExternalCompanyMember(team);

            if (members == null)
                return;

            members.Sort((a, b) => a.Team.TeamID.CompareTo(b.Team.TeamID));

            DataGridViewComboBoxColumn columnTeamName = (DataGridViewComboBoxColumn)this.Columns[ExternalCompanyMemberTeamNameIndex];
            DataGridViewComboBoxColumn columnLevel = (DataGridViewComboBoxColumn)this.Columns[ExternalCompanyMemberLevelIndex];
            DataGridViewComboBoxColumn columnPosition = (DataGridViewComboBoxColumn)this.Columns[ExternalCompanyMemberPositionIndex];

            foreach (ExternalCompanyMember member in members)
            {
                DataGridViewRow row = MakeNewRow();

                // Cell들의 Tag를 지정하는 이유는 Cell Data 편집시 데이터가 수정되었는지 여부를 확인하기 위해서다.
                row.Cells[ExternalCompanyMemberTeamNameIndex].Value = DataManager.GetExternalTeam(member.Team.TeamID);
                row.Cells[ExternalCompanyMemberTeamNameIndex].Tag = row.Cells[ExternalCompanyMemberTeamNameIndex].Value;
                 
                row.Cells[ExternalCompanyMemberNameIndex].Value = member.Name;
                row.Cells[ExternalCompanyMemberNameIndex].Tag = member.Name;

                if (member.ExternalJobLevel != null)
                {
                    if (!columnLevel.Items.Contains(member.ExternalJobLevel))
                        columnLevel.Items.Add(member.ExternalJobLevel);

                    row.Cells[ExternalCompanyMemberLevelIndex].Value = member.ExternalJobLevel;
                    row.Cells[ExternalCompanyMemberLevelIndex].Tag = row.Cells[ExternalCompanyMemberLevelIndex].Value;
                }
                else
                {
                    row.Cells[ExternalCompanyMemberLevelIndex].Value = null;
                    row.Cells[ExternalCompanyMemberLevelIndex].Tag = row.Cells[ExternalCompanyMemberLevelIndex].Value;
                }

                if (member.ExternalJobPosition != null)
                {
                    if (!columnPosition.Items.Contains(member.ExternalJobPosition))
                        columnPosition.Items.Add(member.ExternalJobPosition);

                    row.Cells[ExternalCompanyMemberPositionIndex].Value = member.ExternalJobPosition;
                    row.Cells[ExternalCompanyMemberPositionIndex].Tag = row.Cells[ExternalCompanyMemberPositionIndex].Value;
                }
                else
                {
                    row.Cells[ExternalCompanyMemberPositionIndex].Value = null;
                    row.Cells[ExternalCompanyMemberPositionIndex].Tag = row.Cells[ExternalCompanyMemberPositionIndex].Value;
                }

                bool isChanged = DataManager.GetExternalCompanyMemberPhoneNumberChanged(member);
                row.Cells[ExternalCompanyMemberPhoneNumberIndex].Value = new PhoneNumber(member.PhoneNumber, isChanged);
                row.Cells[ExternalCompanyMemberPhoneNumberIndex].Tag = row.Cells[ExternalCompanyMemberPhoneNumberIndex].Value;

                row.Cells[ExternalCompanyMemberDescriptionIndex].Value = member.Description;
                row.Cells[ExternalCompanyMemberDescriptionIndex].Tag = member.Description;

                row.Tag = member;
            }
        }
        
        private List<CompanyMember> MemberSort(RegularTeam team, List<CompanyMember> members)
        {
            if (members.Count == 0)
                return members;

            //1. 최상위 소속팀순 정렬
            SortedDictionary<int, Team> sortTeams = DataManager.GetTeamsSort(team, TeamTreeView.TeamType.REGULAR);

            SortedDictionary<int, List<CompanyMember>> dddd = new SortedDictionary<int, List<CompanyMember>>();
            
            foreach (CompanyMember member in members)
            {
                foreach (KeyValuePair<int, Team> sortTeam in sortTeams)
                {
                    if (member.Team == null)
                        continue;

                    if (member.Team.TeamID == sortTeam.Value.TeamID)
                    {
                        if (!dddd.ContainsKey(sortTeam.Key))
                            dddd.Add(sortTeam.Key, new List<CompanyMember>());
                        dddd[sortTeam.Key].Add(member);
                    }
                }
            }

            //2. 본부장->처장->실장->팀장->파트장-> 센터장->팀원순
            //3. 직위별 2명이상일 때 이름순
            foreach (KeyValuePair<int, List<CompanyMember>> dd in dddd)
            { 
                dd.Value.Sort((first, second)
                    =>
                    {
                        // 2 센터장
                        // 3 파트장
                        // 4 팀장 

                        int firstPositionID = first.PositionID;
                        if (first.PositionID == 2) // 팀장이면 파트장 상위로
                            firstPositionID = 4;
                        else if (first.PositionID == 3) // 파트장이면 팀장 하위로
                            firstPositionID = 3;
                        else if (first.PositionID == 4) // 센터장이면 파트장 하위로
                            firstPositionID = 2; 

                        int secondPositionID = second.PositionID;
                        if (second.PositionID == 2)
                            secondPositionID = 4;
                        else if (second.PositionID == 3)
                            secondPositionID = 3;
                        else if (second.PositionID == 4)
                            secondPositionID = 2; 

                        var positionId = -1 * firstPositionID.CompareTo(secondPositionID);
                        if (firstPositionID != secondPositionID)
                            return positionId;
                        else return first.Name.CompareTo(second.Name); // 이름순
                    });
            }

            List<CompanyMember> returnMember = new List<CompanyMember>();
            foreach (KeyValuePair<int, List<CompanyMember>> dd in dddd)
            {
                returnMember.AddRange(dd.Value);
            }
             
            return returnMember;
        }

        private void SelectRegularTeam(RegularTeam team, Boolean OnlySelectTeam = false)
        {
            List<CompanyMember> members = DataManager.GetChildRegularMembers(team, OnlySelectTeam);

            if (members == null || members.Count == 0)
                return;

            members = MemberSort(team, members);
             
            //DataGridViewComboBoxColumn columnTeamName = (DataGridViewComboBoxColumn)this.Columns[TeamNameIndex];
            DataGridViewComboBoxColumn columnLevel = (DataGridViewComboBoxColumn)this.Columns[ColumnLevelIndex];
            DataGridViewComboBoxColumn columnSubLevel = (DataGridViewComboBoxColumn)this.Columns[SubLevelIndex];
            DataGridViewComboBoxColumn columnSubPosition = (DataGridViewComboBoxColumn)this.Columns[SubPositionIndex];
            DataGridViewComboBoxColumn columnGroupPosition = (DataGridViewComboBoxColumn)this.Columns[GroupPositionIndex];

            foreach (CompanyMember member in members)
            {
                DataGridViewRow row = MakeNewRow();

                // Cell들의 Tag를 지정하는 이유는 Cell Data 편집시 데이터가 수정되었는지 여부를 확인하기 위해서다.
                if (member.Team != null)
                {
                    /*foreach (RegularTeam item in columnTeamName.Items)
                    { 
                        if (item.TeamID == member.Team.TeamID)
                        {
                            row.Cells[TeamNameIndex].Value = item;
                            row.Cells[TeamNameIndex].Tag = item;
                            break;
                        }
                    }*/
                    row.Cells[TeamNameIndex].Value = member.Team;
                    row.Cells[TeamNameIndex].Tag = member.Team;
                }
                else
                {
                    row.Cells[TeamNameIndex].Value = null;
                    row.Cells[TeamNameIndex].Tag = null;
                }
                 
                row.Cells[NameIndex].Value = member.Name;
                row.Cells[NameIndex].Tag = member.Name;

                if (member.LevelID >= 0 && member.LevelID < columnLevel.Items.Count)
                {
                    row.Cells[ColumnLevelIndex].Value = columnLevel.Items[member.LevelID];
                    row.Cells[ColumnLevelIndex].Tag = row.Cells[ColumnLevelIndex].Value;
                }
                
                if (member.SubJobLevel != null)
                {
                    if (!columnSubLevel.Items.Contains(member.SubJobLevel))
                        columnSubLevel.Items.Add(member.SubJobLevel);

                    row.Cells[SubLevelIndex].Value = member.SubJobLevel;
                    row.Cells[SubLevelIndex].Tag = row.Cells[SubLevelIndex].Value;
                }

                string strPositionName = DataManager.GetJobPositionName(member.PositionID);

                if (strPositionName != null)
                {
                    row.Cells[PositionIndex].Value = strPositionName;
                    row.Cells[PositionIndex].Tag = strPositionName;
                }

                if (member.SubJobPosition != null)
                {
                    if (!columnSubPosition.Items.Contains(member.SubJobPosition))
                        columnSubPosition.Items.Add(member.SubJobPosition);

                    row.Cells[SubPositionIndex].Value = member.SubJobPosition;
                    row.Cells[SubPositionIndex].Tag = row.Cells[SubPositionIndex].Value;
                }

                if (member.GroupPosition != null)
                {
                    if (!columnGroupPosition.Items.Contains(member.GroupPosition))
                        columnGroupPosition.Items.Add(member.GroupPosition);

                    row.Cells[GroupPositionIndex].Value = member.GroupPosition;
                    row.Cells[GroupPositionIndex].Tag = row.Cells[GroupPositionIndex].Value;
                }

                bool isChanged = DataManager.GetCompanyMemberMemberIDChanged(member);
                row.Cells[MemberIDIndex].Value = new MemberID(member.MemberID, isChanged);
                row.Cells[MemberIDIndex].Tag = row.Cells[MemberIDIndex].Value;

                isChanged = DataManager.GetCompanyMemberOfficePhoneNumberChanged(member);
                row.Cells[OfficePhoneNumberIndex].Value = new OfficePhoneNumber(member.OfficePhoneNumber, isChanged);
                row.Cells[OfficePhoneNumberIndex].Tag = row.Cells[OfficePhoneNumberIndex].Value;

                isChanged = DataManager.GetCompanyMemberPhoneNumberChanged(member);
                row.Cells[PhoneNumberIndex].Value = new PhoneNumber(member.PhoneNumber, isChanged);
                row.Cells[PhoneNumberIndex].Tag = row.Cells[PhoneNumberIndex].Value;

                row.Tag = member;
            }
        }

        private void SelectUserDefinedTeam()
        {
            List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

            if (teams == null)
                return;

            //teams.Sort();

            foreach (UserDefinedTeam team in teams)
            {
                DataGridViewRow row = MakeNewRow();

                // Cell들의 Tag를 지정하는 이유는 Cell Data 편집시 데이터가 수정되었는지 여부를 확인하기 위해서다.
                row.Cells[UserDefinedTeamName].Value = team.TeamName;
                row.Cells[UserDefinedTeamName].Tag = team.TeamName;

                row.Cells[UserDefinedTeamPhoneNumber].Value = team.PhoneNumber;
                row.Cells[UserDefinedTeamPhoneNumber].Tag = team.PhoneNumber;

                row.Cells[UserDefinedTeamFaxNumber].Value = team.FaxNumber;
                row.Cells[UserDefinedTeamFaxNumber].Tag = team.FaxNumber;

                row.Tag = team;
            }
        }

        public DataGridViewRow MakeNewRow()
        {
            if (this.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)this.Rows[this.Rows.Count - 1].Clone();
                this.Rows.Add(row);

                return this.Rows[this.Rows.Count - 2];
            }
            else
            {
                this.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)this.Rows[this.Rows.Count - 1].Clone();
                this.Rows.Add(row);

                this.AllowUserToAddRows = false;
            }

            return this.Rows[this.Rows.Count - 1];
        }

        private void SetComboBoxItems(DataGridViewComboBoxCell cell, DataGridViewComboBoxColumn column)
        {
            foreach (object obj in column.Items)
            {
                cell.Items.Add(obj);
            }
        }

        // 정렬이 끝난후 행번호를 새로 지정한다.
        private void OnSorted(object sender, EventArgs e)
        {
            ResetIndeces();
        }

        public void ResetIndeces()
        {
            foreach (DataGridViewRow row in this.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].Value = row.Index + 1;
            }
        }

        private void OnReadOnlyChanged(object sender, EventArgs e)
        {
            if (this.Columns.Count > 0)
            {
                this.Columns[0].ReadOnly = true;

                if (m_gridType == GridType.RegularMember || m_gridType == GridType.ExternalCompanyTeam)
                {
                    this.Columns[TeamNameIndex].ReadOnly = true;
                }
            }

            if (m_gridType == GridType.TemporaryNormal || m_gridType == GridType.TemporaryEmergency)
            {
                foreach (DataGridViewRow row in this.Rows)
                {
                    if (row.IsNewRow || row.Tag == null)
                        continue;

                    TemporaryMember member = (TemporaryMember)row.Tag;

                    // 팀장이거나 팀전체
                    if (member.IsTeamLeader || (!member.IsTeamLeader && member.MemberCount < 0))
                        row.Cells[MemberCountIndex].ReadOnly = true;
                    else
                        row.Cells[MemberCountIndex].ReadOnly = false;

                    if (member.TemporaryMemberType == TemporaryMember.MemberType.RegularTeam ||
                        member.TemporaryMemberType == TemporaryMember.MemberType.ExternalTeam ||
                        member.TemporaryMemberType == TemporaryMember.MemberType.UserDefinedTeam)
                    {
                        row.Cells[IncludeChildTeamIndex].ReadOnly = false;
                        row.Cells[TeamLeaderIndex].ReadOnly = false;
                    }
                    else
                    {
                        row.Cells[IncludeChildTeamIndex].ReadOnly = true;

                        row.Cells[TeamLeaderIndex].ReadOnly = true;
                    }
                }
            }
        }

        private void OnRowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                DataGridViewRow row = this.Rows[e.RowIndex - 1];
                row.Cells[0].Value = row.Index + 1;

                if (this.m_gridType == GridType.RegularMember && FormMain.Instance.IsEditMode)
                    row.Cells[TeamNameIndex].Value = (RegularTeam)m_teamCurrent;
            }

            DataGridViewRow rowCurrent = this.Rows[e.RowIndex];

            if (!rowCurrent.IsNewRow)
                rowCurrent.Cells[0].Value = rowCurrent.Index + 1;

            if (this.m_gridType == GridType.TemporaryEmergency || this.m_gridType == GridType.TemporaryNormal)
            {
                this.Rows[e.RowIndex].Cells[IncludeChildTeamImageIndex].Value = GetCheckBoxImg(false);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (m_teamCurrent != null && m_teamCurrent is RegularTeam)
                {
                    Search(FormMain.Instance.SearchStr);
                }
            }

            if (this.ReadOnly) return;
            if (e.KeyCode == Keys.Delete)
            {
                if (m_teamCurrent != null)
                {
                    if (m_teamCurrent is RegularTeam)
                        RemoveCompanyMembers();
                    else if ((m_teamCurrent is TemporaryNormalTeam) || (m_teamCurrent is TemporaryEmergencyTeam))
                        RemoveTemporaryMembers();
                    else if (m_teamCurrent is ExternalTeam)
                        RemoveExternalMember();
                }
                else if(m_gridType == GridType.UserDefinedTeam)
                {
                    RemoveUserDefinedTeams();
                }
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                PasteClipboard();
            }  
        }

        public void Search(string searchText)
        {
            if (searchText.Length == 0) return;
              
            foreach (DataGridViewRow row in this.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex == 0) 
                        continue;

                    if (cell.Value == null)
                        continue;

                    if (cell.Value.ToString().Contains(searchText))
                        cell.Style.BackColor = System.Drawing.Color.Yellow;
                    else
                        cell.Style.BackColor = System.Drawing.Color.Empty; 
                }
            }
        }

        private void PasteClipboard()
        {
            if (this.SelectedCells == null || this.SelectedCells.Count == 0) 
                return; 

            DataObject o = (DataObject)Clipboard.GetDataObject();
            if (o.GetDataPresent(DataFormats.Text))
            { 
                string[] pastedRows = System.Text.RegularExpressions.Regex.Split(o.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");

                int columnMinIndex = -1; 
                int rowMinIndex = -1; 

                foreach (DataGridViewCell cell in this.SelectedCells)
                {
                    columnMinIndex = (columnMinIndex < 0) ? cell.ColumnIndex : Math.Min(cell.ColumnIndex, columnMinIndex); 
                    rowMinIndex = (rowMinIndex < 0) ? cell.RowIndex : Math.Min(cell.RowIndex, rowMinIndex); 
                }   

                if (columnMinIndex < 0 || rowMinIndex < 0) 
                    return;

                if (m_gridType == GridType.RegularMember)
                {
                    // 핸드폰번호 중복검사  
                    string overlapMsg = OverlapPasted(pastedRows, rowMinIndex, columnMinIndex);
                    if (overlapMsg.Length > 0)
                    {
                        //MessageBox.Show(overlapMsg);
                        UnE.Utility.UMessageBoxRibbon.Show(overlapMsg, "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    CommandPasteRegularMember cmd = new CommandPasteRegularMember(this.m_linkedTree, this, pastedRows, rowMinIndex, columnMinIndex);
                    cmd.Do();

                    if (cmd.IsPasted)
                        FormMain.Instance.AddCommand(cmd, false); 
                } 
            }
        }  

        private string OverlapPasted(string[] strPastedRows, int rowMinIdx, int colMinIdx)
        {
            int tempRowMinIndex = rowMinIdx;
            int tempColMinIndex = colMinIdx;

            for (int row = 0; row < strPastedRows.Length; row++)
            {
                string[] pastedRowCells = strPastedRows[row].Split(new char[] { '\t' });
                string pastedRowLength = string.Join("", pastedRowCells);
                if (pastedRowLength.Length == 0)
                    continue;

                int columnIndex = tempColMinIndex;

                DataGridViewRow curRow = null;
                if (this.Rows.Count > tempRowMinIndex)
                    curRow = this.Rows[tempRowMinIndex];

                CompanyMember member = null;
                if (curRow == null || curRow.Tag == null)
                    member = new CompanyMember();
                else
                    member = (CompanyMember)curRow.Tag;

                for (int cell = 0; cell < pastedRowCells.Length; cell++)
                {
                    string pastedValue = pastedRowCells[cell].Trim();
                    if (columnIndex == PhoneNumberIndex)
                    {
                        string OverlapMsg = DataManager.OverlapRegularMember(member, pastedValue);
                        if (OverlapMsg.Length > 0)
                        {
                            return OverlapMsg;
                        }
                    }
                    columnIndex++;
                }

                tempRowMinIndex++;
            }

            return "";
        }

        private void RemoveTemporaryMembers()
        {
            List<int> rowIndeces = new List<int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (!rowIndeces.Contains(cell.RowIndex))
                    rowIndeces.Add(cell.RowIndex);
            }

            string strMembers = "";
            List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex> memberIndeces = new List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex>();

            foreach (int nRowIndex in rowIndeces)
            {
                DataGridViewRow row = this.Rows[nRowIndex];

                if (row.IsNewRow || row.Tag == null || (row.Tag is TemporaryMember) == false)
                    continue;

                TemporaryMember member = (TemporaryMember)row.Tag;

                Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex memberIndex = new Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex(member, nRowIndex);
                memberIndeces.Add(memberIndex);

                if (strMembers.Length == 0)
                    strMembers = member.DisplayName.Length > 0 ? member.DisplayName : (member.GetMemberRealName().Length > 0 ? member.GetMemberRealName() : member.GetTeamRealName());
            }

            if (memberIndeces.Count > 1)
                strMembers += "외 " + (memberIndeces.Count - 1).ToString() + "명";

            string strMessage = string.Format("[{0}]을 삭제하시겠습니까?", strMembers);

            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMessage, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (MessageBox.Show(strMessage, "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (_result == DialogResult.Yes)
            {
                Command.CommandRemoveTemporaryMembers cmd = new Command.CommandRemoveTemporaryMembers(m_teamCurrent, memberIndeces, this);
                FormMain.Instance.AddCommand(cmd);
            }
        }

        private void RemoveExternalMember()
        {
            List<int> rowIndeces = new List<int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (!rowIndeces.Contains(cell.RowIndex))
                    rowIndeces.Add(cell.RowIndex);
            }

            string strMembers = "";
            List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> memberIndeces = new List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex>();

            foreach (int nRowIndex in rowIndeces)
            {
                DataGridViewRow row = this.Rows[nRowIndex];

                if (row.IsNewRow || row.Tag == null || (row.Tag is ExternalCompanyMember) == false)
                    continue;

                ExternalCompanyMember member = (ExternalCompanyMember)row.Tag;

                Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex memberIndex = new Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex(member, nRowIndex);
                memberIndeces.Add(memberIndex);

                if (strMembers.Length == 0)
                    strMembers = member.Name;
            }

            if (memberIndeces.Count > 1)
                strMembers += "외 " + (memberIndeces.Count - 1).ToString() + "명";

            string strMessage = string.Format("[{0}]을 삭제하시겠습니까?\r\n해당 직원정보 및 그와 연관된 정보들이 모두 삭제됩니다.\r\n계속 하시겠습니까?", strMembers);
            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMessage, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (MessageBox.Show(strMessage, "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (_result == DialogResult.Yes)
            {
                Command.CommandRemoveExternalCompanyMembers cmd = new Command.CommandRemoveExternalCompanyMembers((ExternalTeam)m_teamCurrent, memberIndeces, this);
                FormMain.Instance.AddCommand(cmd);
            }
        }

        private void RemoveCompanyMembers()
        {
            List<int> rowIndeces = new List<int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (!rowIndeces.Contains(cell.RowIndex))
                    rowIndeces.Add(cell.RowIndex);
            }

            string strMembers = "";
            List<Command.CommandMoveRegularMembers.CompanyMemberNIndex> memberIndeces = new List<Command.CommandMoveRegularMembers.CompanyMemberNIndex>();

            foreach (int nRowIndex in rowIndeces)
            {
                DataGridViewRow row = this.Rows[nRowIndex];

                if (row.IsNewRow || row.Tag == null || (row.Tag is CompanyMember) == false)
                    continue;

                CompanyMember member = (CompanyMember)row.Tag;

                Command.CommandMoveRegularMembers.CompanyMemberNIndex memberIndex = new Command.CommandMoveRegularMembers.CompanyMemberNIndex(member, nRowIndex);
                memberIndeces.Add(memberIndex);

                if (strMembers.Length == 0)
                    strMembers = member.Name;
            }

            if (memberIndeces.Count > 1)
                strMembers += "외 " + (memberIndeces.Count - 1).ToString() + "명";

            string strMessage = string.Format("[{0}]을 삭제하시겠습니까?\r\n해당 직원정보 및 그와 연관된 정보들이 모두 삭제됩니다.\r\n계속 하시겠습니까?", strMembers);
            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMessage, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (MessageBox.Show(strMessage, "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (_result == DialogResult.Yes)
            {
                Command.CommandRemoveRegularMembers cmd = new Command.CommandRemoveRegularMembers((RegularTeam)m_teamCurrent, memberIndeces, this);
                FormMain.Instance.AddCommand(cmd);
            } 
        }

        private void RemoveUserDefinedTeams()
        {
            List<int> rowIndeces = new List<int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (!rowIndeces.Contains(cell.RowIndex))
                    rowIndeces.Add(cell.RowIndex);
            }

            string strTeam = "";
            List<UserDefinedTeam> teams = new List<UserDefinedTeam>();

            foreach (int nRowIndex in rowIndeces)
            {
                DataGridViewRow row = this.Rows[nRowIndex];

                if (row.IsNewRow || row.Tag == null || (row.Tag is UserDefinedTeam) == false)
                    continue;

                UserDefinedTeam team = (UserDefinedTeam)row.Tag;

                teams.Add(team);

                if (strTeam.Length == 0)
                    strTeam = team.TeamName;
            }

            if (teams.Count > 1)
                strTeam += "외 " + (teams.Count - 1).ToString() + "조직";

            string strMessage = string.Format("[{0}]을 삭제하시겠습니까?\r\n해당 조직 및 조직에 연관된 정보들이 모두 삭제됩니다.\r\n계속 하시겠습니까?", strTeam);
            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMessage, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (MessageBox.Show(strMessage, "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (_result == DialogResult.Yes)
            {
                Command.CommandRemoveUserDefinedTeam cmd = new Command.CommandRemoveUserDefinedTeam(teams, this);
                FormMain.Instance.AddCommand(cmd);
            }
        }

        private void TeamGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (this.ReadOnly)
                    return;

                if (m_teamCurrent == null || m_linkedTree == null)
                    return;

                string strMemberNames = "";
                Command.CommandEx cmd = null;
                TeamTreeView.DropDataType dropType = TeamTreeView.DropDataType.NONE;

                // Grid To Tree Drag & Drop
                if (m_teamCurrent is RegularTeam)
                {
                    cmd = MakeMoveRegularMembersCommand(out strMemberNames);
                    dropType = TeamTreeView.DropDataType.REGULAR_MEMBER;
                    RefreshGrid();
                }
                else if ((m_teamCurrent is TemporaryNormalTeam) || (m_teamCurrent is TemporaryNormalTeam))
                {
                    cmd = MakeMoveTemporaryMembersCommand(out strMemberNames);
                    dropType = TeamTreeView.DropDataType.TEMPORARY_MEMBER;
                }
                else if (m_teamCurrent is ExternalTeam)
                {
                    cmd = MakeMoveExternalMembersCommand(out strMemberNames);
                    dropType = TeamTreeView.DropDataType.EXTERNAL_MEMBER;
                }

                if (cmd == null)
                    return;

                System.Drawing.Bitmap bmp = m_linkedTree.MakeDragBitmap(strMemberNames);

                if (bmp == null)
                    return;

                m_linkedTree.DropData = cmd;

                m_linkedTree.BeginDragDrop(bmp, DragDropEffects.Move, dropType);

            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (this.ReadOnly)
                {
                    tsMenuMoveExternalMember.Visible =
                    tsMenuRemoveCompanyMembers.Visible =
                    tsMenuRemoveExternalMember.Visible =
                    tsMenuRemoveUserDefinedTeam.Visible =
                    tsMoveRegularTeam.Visible =
                    tsMoveTemporaryMember.Visible =
                    tsRemoveTemporaryMember.Visible = false;
                }
                else
                {
                    tsMenuMoveExternalMember.Visible =
                    tsMenuRemoveCompanyMembers.Visible =
                    tsMenuRemoveExternalMember.Visible =
                    tsMenuRemoveUserDefinedTeam.Visible =
                    tsMoveRegularTeam.Visible =
                    tsMoveTemporaryMember.Visible =
                    tsRemoveTemporaryMember.Visible = true;
                }

                System.Drawing.Rectangle rect = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (m_gridType == GridType.RegularMember)
                    contextMenuStripRegularMember.Show(this, e.X + rect.Left, e.Y + rect.Top);
                else if (m_gridType == GridType.TemporaryNormal || m_gridType == GridType.TemporaryEmergency)
                    contextMenuStripTemporaryMember.Show(this, e.X + rect.Left, e.Y + rect.Top);
                else if (m_gridType == GridType.ExternalCompanyTeam)
                    contextMenuStripExternalMember.Show(this, e.X + rect.Left, e.Y + rect.Top);
                else if (m_gridType == GridType.UserDefinedTeam)
                    contextMenuStripUserDefinedTeam.Show(this, e.X + rect.Left, e.Y + rect.Top);

            }
        }

        private void TeamGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (m_teamCurrent == null || !FormMain.Instance.IsEditMode || this.ReadOnly)
            //    return;
                        
            //if ((m_teamCurrent is TemporaryNormalTeam) || (m_teamCurrent is TemporaryEmergencyTeam))
            //{
            //    if (e.ColumnIndex == Manager2Index || e.ColumnIndex == TeamIndex)
            //    {
            //        FormMain.Instance.ShowTemporaryMemberForm();
            //    }
            //}
        }

        private void TeamGrid_MouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (m_teamCurrent == null || FormMain.Instance.IsEditMode == false ) return; 

            // Index && TeamName
            if (e.ColumnIndex != 0) return;// Index, 팀명이 아닐경우 Return

            if (e.RowIndex < 0 || this.Rows[e.RowIndex].IsNewRow == true) return; //수정 신규Row일 경우 Return

            if (m_gridType == GridType.UserDefinedTeam) return; //사용자정의조직일 경우 Return

            if (LinkedTree == null) return;

            DataGridViewRow row = this.Rows[e.RowIndex];

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1,1);
            TeamTreeView.DropDataType dropType;

            if (m_gridType == GridType.RegularMember)
            {
                bmp = this.LinkedTree.MakeDragBitmap(row.Cells[2].Value.ToString());//이름
                if (bmp == null) return;

                Command.CommandMoveRegularMembers cmd = MakeMoveRegularMemberCommand(row);
                LinkedTree.DropData = cmd;
                dropType = TeamTreeView.DropDataType.REGULAR_MEMBER;
            }
            else if (m_gridType == GridType.TemporaryNormal || m_gridType == GridType.TemporaryEmergency)
            {
                bmp = this.LinkedTree.MakeDragBitmap(row.Cells[6].Value.ToString());//이름
                if (bmp == null) return;

                Command.CommandMoveTemporaryMembers cmd = MakeMoveTemporaryMemberCommand(row);
                LinkedTree.DropData = cmd;
                dropType = TeamTreeView.DropDataType.TEMPORARY_MEMBER;
            }
            else if (m_gridType == GridType.ExternalCompanyTeam)
            {
                bmp = this.LinkedTree.MakeDragBitmap(row.Cells[2].Value.ToString());//이름
                if (bmp == null) return;

                Command.CommandMoveExternalMembers cmd = MakeMoveExternalMemberCommand(row);
                LinkedTree.DropData = cmd;
                dropType = TeamTreeView.DropDataType.EXTERNAL_MEMBER;
            }
            else
            {
                return;
            }

            LinkedTree.BeginDragDrop(bmp, DragDropEffects.Move, dropType);
        }

        private void TeamGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_teamCurrent == null || m_linkedTree == null || this.ReadOnly)
                return;

            if (m_teamCurrent is RegularTeam)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = this.Rows[e.RowIndex];
                if (row.Cells[TeamNameIndex].Tag != null)
                {
                    m_teamCurrentRow = (RegularTeam)row.Cells[TeamNameIndex].Tag;
                }

                if (row.IsNewRow)
                {
                    if (e.ColumnIndex == TeamNameIndex)
                    {
                        //row.Cells[e.ColumnIndex].Value = m_teamCurrent.TeamID;
                    }

                    return;
                }

                if (e.ColumnIndex == SubLevelIndex || e.ColumnIndex == SubPositionIndex || e.ColumnIndex == GroupPositionIndex)
                {
                    this.BeginEdit(true);

                    ComboBox comboBox = (ComboBox)this.EditingControl;
                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                    if (comboBox.Tag == null)
                        comboBox.Leave += new EventHandler(cellComboBox_Leave);

                    comboBox.Tag = true;
                }
                //else if (e.ColumnIndex == MemberIDIndex)
                //{ 
                //    MemberID id = (MemberID)this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                //    if (id == null) return;
                //    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = id.ID;
                //}
            }
            else if (m_teamCurrent is ExternalTeam)
            {
                if (e.RowIndex < 0)
                    return;

                DataGridViewRow row = this.Rows[e.RowIndex];

                if (row.Cells[ExternalCompanyMemberTeamNameIndex].Tag != null)
                {
                    m_teamCurrentRow = (ExternalTeam)row.Cells[ExternalCompanyMemberTeamNameIndex].Tag;
                }

                if (row.IsNewRow)
                {
                    if (e.ColumnIndex == 1)
                    {
                        row.Cells[e.ColumnIndex].Value = m_teamCurrent.TeamID;
                    }
                    return;
                }
                    

                if (e.ColumnIndex == ExternalCompanyMemberLevelIndex || e.ColumnIndex == ExternalCompanyMemberPositionIndex)
                {
                    this.BeginEdit(true);

                    ComboBox comboBox = (ComboBox)this.EditingControl;
                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                    if (comboBox.Tag == null)
                        comboBox.Leave += new EventHandler(cellComboBox_Leave);

                    comboBox.Tag = true;
                }
            }
        }

        private void TeamGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_teamCurrent == null || !FormMain.Instance.IsEditMode)
                return;
             
            if (Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= -1)
            {
                if ((m_teamCurrent is TemporaryNormalTeam) || (m_teamCurrent is TemporaryEmergencyTeam))
                {
                    if (e.ColumnIndex == Manager2ButtonIndex || e.ColumnIndex == TeamButtonIndex)
                    {
                        FormMain.Instance.ShowTemporaryMemberForm();
                    }
                }
            }

            if (this.m_gridType == GridType.TemporaryEmergency || this.m_gridType == GridType.TemporaryNormal)
            {
                if (e.ColumnIndex == IncludeChildTeamImageIndex || e.ColumnIndex == IncludeChildTeamIndex)
                {
                    try
                    {
                        if ((bool)this.Rows[e.RowIndex].Cells[IncludeChildTeamIndex].Value == true)
                        {                            
                            this.Rows[e.RowIndex].Cells[IncludeChildTeamImageIndex].Value = GetCheckBoxImg(false);
                            this.Rows[e.RowIndex].Cells[IncludeChildTeamIndex].Value = false;
                        }
                        else
                        {                            
                            this.Rows[e.RowIndex].Cells[IncludeChildTeamImageIndex].Value = GetCheckBoxImg(true);
                            this.Rows[e.RowIndex].Cells[IncludeChildTeamIndex].Value = true;
                        }
                    }
                    catch{ return; }

                    if (this.m_gridType == GridType.TemporaryNormal)
                    {
                        TemporaryMemberCellEndEdit(this.Rows[e.RowIndex], this.Rows[e.RowIndex].Cells[IncludeChildTeamIndex], true);
                    }
                    else
                    {
                        TemporaryMemberCellEndEdit(this.Rows[e.RowIndex], this.Rows[e.RowIndex].Cells[IncludeChildTeamIndex], false);
                    }

                }
            }
        }

        void TeamGrid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == MemberIDIndex)
            {
                if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null && this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag is MemberID)
                {
                    MemberID id = (MemberID)this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    if (id == null) return;
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = id.ID;
                }
            }
        } 

        void TeamGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == MemberIDIndex)
            {
                if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag != null && this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag is MemberID)
                {
                    MemberID id = (MemberID)this.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    if (id == null || id.ID.Length == 0) return;
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = TeamGrid.SECRET_VALUE;// id.ToString();
                }
            }
        }

        private void OnFinishRegularMemberCellEdit(DataGridViewCell cell, ComboBox cbo)
        {
            DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)this.Columns[cell.ColumnIndex];

            DataGridViewRow row = this.Rows[cell.RowIndex];

            Command.CommandChangeRegularMemberInfo cmd = null;
            
            if (cell.ColumnIndex == SubLevelIndex)
            {
                bool isNewMember;
                CompanyMember member = GetRowCompanyMember(row, out isNewMember);

                Command.CommandChangeRegularMemberInfo.InfoType type = Command.CommandChangeRegularMemberInfo.ToInfoType(cell.ColumnIndex);

                cmd = new Command.CommandChangeRegularMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (type == Command.CommandChangeRegularMemberInfo.InfoType.Unknown)
                    return;

                CompanyMember.JobLevelSubInfo subLevel = null;

                if (cbo.Text.Length > 0)
                {
                    subLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(cbo.Text);

                    if (subLevel == null)
                    {
                        subLevel = new CompanyMember.JobLevelSubInfo();
                        subLevel.Name = cbo.Text;
                        column.Items.Add(subLevel);
                    }
                    else if (!column.Items.Contains(subLevel))
                        column.Items.Add(subLevel);
                }

                if (cell.Value == subLevel)
                    return;

                cmd.Origin = cell.Tag;
                cmd.Changed = subLevel;

                cell.Value = subLevel;
                cell.Tag = cell.Value;
            }
            else if (cell.ColumnIndex == SubPositionIndex)
            {
                bool isNewMember;
                CompanyMember member = GetRowCompanyMember(row, out isNewMember);

                Command.CommandChangeRegularMemberInfo.InfoType type = Command.CommandChangeRegularMemberInfo.ToInfoType(cell.ColumnIndex);

                cmd = new Command.CommandChangeRegularMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (type == Command.CommandChangeRegularMemberInfo.InfoType.Unknown)
                    return;

                CompanyMember.JobPositionSubInfo subPosition = null;

                if (cbo.Text.Length > 0)
                {
                    subPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(cbo.Text);

                    if (subPosition == null)
                    {
                        subPosition = new CompanyMember.JobPositionSubInfo();
                        subPosition.Name = cbo.Text;
                        column.Items.Add(subPosition);
                    }
                    else if (!column.Items.Contains(subPosition))
                        column.Items.Add(subPosition);
                }

                if (cell.Value == subPosition)
                    return;

                cmd.Origin = cell.Tag;
                cmd.Changed = subPosition;

                cell.Value = subPosition;
                cell.Tag = cell.Value;
            }
            else if (cell.ColumnIndex == GroupPositionIndex)
            {
                bool isNewMember;
                CompanyMember member = GetRowCompanyMember(row, out isNewMember);

                Command.CommandChangeRegularMemberInfo.InfoType type = Command.CommandChangeRegularMemberInfo.ToInfoType(cell.ColumnIndex);

                cmd = new Command.CommandChangeRegularMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (type == Command.CommandChangeRegularMemberInfo.InfoType.Unknown)
                    return;

                CompanyMember.JobGroupPosition groupPosition = null;

                if (cbo.Text.Length > 0)
                {
                    groupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(cbo.Text);

                    if (groupPosition == null)
                    {
                        groupPosition = new CompanyMember.JobGroupPosition();
                        groupPosition.Name = cbo.Text;
                        column.Items.Add(groupPosition);
                    }
                    else if (!column.Items.Contains(groupPosition))
                        column.Items.Add(groupPosition);
                }

                if (cell.Value == groupPosition)
                    return;

                cmd.Origin = cell.Tag;
                cmd.Changed = groupPosition;

                cell.Value = groupPosition;
                cell.Tag = cell.Value;
            }

            if (cmd != null)
            {
                // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
                FormMain.Instance.AddCommand(cmd, false);
                cmd.SetMemberData(cmd.Changed);
            }
        }

        private void OnFinishExternalMemberCellEdit(DataGridViewCell cell, ComboBox cbo)
        {
            DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)this.Columns[cell.ColumnIndex];

            DataGridViewRow row = this.Rows[cell.RowIndex];

            Command.CommandChangeExternalMemberInfo cmd = null;

            if (cell.ColumnIndex == ExternalCompanyMemberLevelIndex)
            {
                bool isNewMember;
                ExternalCompanyMember member = GetRowExternalCompanyMember(row, out isNewMember);

                Command.CommandChangeExternalMemberInfo.InfoType type = Command.CommandChangeExternalMemberInfo.ToInfoType(cell.ColumnIndex);

                cmd = new Command.CommandChangeExternalMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (type == Command.CommandChangeExternalMemberInfo.InfoType.Unknown)
                    return;

                ExternalCompanyMember.ExternalJobLevelInfo externalJobLevel = null;

                if (cbo.Text.Length > 0)
                {
                    externalJobLevel = ExternalCompanyMember.ExternalJobLevelInfo.GetExternalJobLevel(cbo.Text);

                    if (externalJobLevel == null)
                    {
                        externalJobLevel = new ExternalCompanyMember.ExternalJobLevelInfo();
                        externalJobLevel.Name = cbo.Text;
                        column.Items.Add(externalJobLevel);
                    }
                    else if (!column.Items.Contains(externalJobLevel))
                        column.Items.Add(externalJobLevel);
                }

                if (cell.Value == externalJobLevel)
                    return;

                cmd.Origin = cell.Tag;
                cmd.Changed = externalJobLevel;

                cell.Value = externalJobLevel;
                cell.Tag = cell.Value;
            }
            else if (cell.ColumnIndex == ExternalCompanyMemberPositionIndex)
            {
                bool isNewMember;
                ExternalCompanyMember member = GetRowExternalCompanyMember(row, out isNewMember);

                Command.CommandChangeExternalMemberInfo.InfoType type = Command.CommandChangeExternalMemberInfo.ToInfoType(cell.ColumnIndex);

                cmd = new Command.CommandChangeExternalMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (type == Command.CommandChangeExternalMemberInfo.InfoType.Unknown)
                    return;

                ExternalCompanyMember.ExternalJobPositionInfo externalJobPosition = null;

                if (cbo.Text.Length > 0)
                {
                    externalJobPosition = ExternalCompanyMember.ExternalJobPositionInfo.GetExternalJobPosition(cbo.Text);

                    if (externalJobPosition == null)
                    {
                        externalJobPosition = new ExternalCompanyMember.ExternalJobPositionInfo();
                        externalJobPosition.Name = cbo.Text;
                        column.Items.Add(externalJobPosition);
                    }
                    else if (!column.Items.Contains(externalJobPosition))
                        column.Items.Add(externalJobPosition);
                }

                if (cell.Value == externalJobPosition)
                    return;

                cmd.Origin = cell.Tag;
                cmd.Changed = externalJobPosition;

                cell.Value = externalJobPosition;
                cell.Tag = cell.Value;
            }

            if (cmd != null)
            {
                // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
                FormMain.Instance.AddCommand(cmd, false);
                cmd.SetMemberData(cmd.Changed);
            }
        }

        private void cellComboBox_Leave(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.Tag == null)
                return;

            if (this.SelectedCells.Count != 1)
                return;

            DataGridViewCell cell = this.SelectedCells[0];

            if (m_teamCurrent is RegularTeam)
                OnFinishRegularMemberCellEdit(cell, cbo);
            else if (m_teamCurrent is ExternalTeam)
                OnFinishExternalMemberCellEdit(cell, cbo);
        }

        private Command.CommandMoveExternalMembers MakeMoveExternalMemberCommand(DataGridViewRow pRow)
        {
            List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> memberIndeces = new List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex>();
            DataGridViewRow _row = pRow;

            if (_row.Tag != null && _row.Tag is ExternalCompanyMember)
            {
                ExternalCompanyMember member = (ExternalCompanyMember)pRow.Tag;
                memberIndeces.Add(new Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex(member, pRow.Index));
            }

            Command.CommandMoveExternalMembers cmd = new Command.CommandMoveExternalMembers((ExternalTeam)m_teamCurrent, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private Command.CommandMoveExternalMembers MakeMoveExternalMembersCommand(out string strMemberNames)
        {
            strMemberNames = "";

            Dictionary<int, int> dicRowIndeces = new Dictionary<int, int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (dicRowIndeces.ContainsKey(cell.RowIndex))
                    continue;

                dicRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> memberIndeces = new List<Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex>();

            foreach (KeyValuePair<int, int> pair in dicRowIndeces)
            {
                if (pair.Value < 0)
                    continue;

                DataGridViewRow row = this.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                if (row.Tag != null && row.Tag is ExternalCompanyMember)
                {
                    ExternalCompanyMember member = (ExternalCompanyMember)row.Tag;
                    memberIndeces.Add(new Command.CommandRemoveExternalCompanyMembers.ExternalMemberNIndex(member, pair.Value));

                    if (strMemberNames.Length == 0)
                        strMemberNames = member.Name;
                }
            }

            if (strMemberNames.Length > 0 && memberIndeces.Count > 1)
                strMemberNames += string.Format("외 {0}명", memberIndeces.Count - 1);

            Command.CommandMoveExternalMembers cmd = new Command.CommandMoveExternalMembers((ExternalTeam)m_teamCurrentRow, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private Command.CommandMoveTemporaryMembers MakeMoveTemporaryMemberCommand(DataGridViewRow pRow)
        {
            List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex> memberIndeces = new List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex>();
            DataGridViewRow _row = pRow;

            if (_row.Tag != null && _row.Tag is TemporaryMember)
            {
                TemporaryMember member = (TemporaryMember)_row.Tag;
                memberIndeces.Add(new Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex(member, _row.Index));
            }

            Command.CommandMoveTemporaryMembers cmd = new Command.CommandMoveTemporaryMembers(m_teamCurrent, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private Command.CommandMoveTemporaryMembers MakeMoveTemporaryMembersCommand(out string strMemberNames)
        {
            strMemberNames = "";

            Dictionary<int, int> dicRowIndeces = new Dictionary<int, int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (dicRowIndeces.ContainsKey(cell.RowIndex))
                    continue;

                dicRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex> memberIndeces = new List<Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex>();

            foreach (KeyValuePair<int, int> pair in dicRowIndeces)
            {
                if (pair.Value < 0)
                    continue;

                DataGridViewRow row = this.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                if (row.Tag != null && row.Tag is TemporaryMember)
                {
                    TemporaryMember member = (TemporaryMember)row.Tag;
                    memberIndeces.Add(new Command.CommandMoveTemporaryMembers.TemporaryMemberNIndex(member, pair.Value));

                    if (strMemberNames.Length == 0)
                        strMemberNames = member.DisplayName.Length > 0 ? member.DisplayName : (member.GetMemberRealName().Length > 0 ? member.GetMemberRealName() : member.GetTeamRealName());
                }
            }

            if (strMemberNames.Length > 0 && memberIndeces.Count > 1)
                strMemberNames += string.Format("외 {0}명", memberIndeces.Count - 1);

            Command.CommandMoveTemporaryMembers cmd = new Command.CommandMoveTemporaryMembers(m_teamCurrent, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private Command.CommandMoveRegularMembers MakeMoveRegularMemberCommand(DataGridViewRow pRow)
        {
            List<Command.CommandMoveRegularMembers.CompanyMemberNIndex> memberIndeces = new List<Command.CommandMoveRegularMembers.CompanyMemberNIndex>();
            DataGridViewRow _row = pRow;

            if (_row.Tag != null && _row.Tag is CompanyMember)
            {
                CompanyMember member = (CompanyMember)_row.Tag;
                memberIndeces.Add(new Command.CommandMoveRegularMembers.CompanyMemberNIndex(member, _row.Index));
            }

            Command.CommandMoveRegularMembers cmd = new Command.CommandMoveRegularMembers((RegularTeam)m_teamCurrentRow, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private Command.CommandMoveRegularMembers MakeMoveRegularMembersCommand(out string strMemberNames)
        {
            strMemberNames = "";

            Dictionary<int, int> dicRowIndeces = new Dictionary<int, int>();

            foreach (DataGridViewCell cell in this.SelectedCells)
            {
                if (dicRowIndeces.ContainsKey(cell.RowIndex))
                    continue;

                dicRowIndeces[cell.RowIndex] = cell.RowIndex;
            }

            List<Command.CommandMoveRegularMembers.CompanyMemberNIndex> memberIndeces = new List<Command.CommandMoveRegularMembers.CompanyMemberNIndex>();

            foreach (KeyValuePair<int, int> pair in dicRowIndeces)
            {
                if (pair.Value < 0)
                    continue;

                DataGridViewRow row = this.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                if (row.Tag != null && row.Tag is CompanyMember)
                {
                    CompanyMember member = (CompanyMember)row.Tag;
                    //members.Add(member);
                    memberIndeces.Add(new Command.CommandMoveRegularMembers.CompanyMemberNIndex(member, pair.Value));

                    if (strMemberNames.Length == 0)
                        strMemberNames = member.Name;
                }
            }

            if (strMemberNames.Length > 0 && memberIndeces.Count > 1)
                strMemberNames += string.Format("외 {0}명", memberIndeces.Count - 1);

            Command.CommandMoveRegularMembers cmd = new Command.CommandMoveRegularMembers((RegularTeam)m_teamCurrentRow, null, memberIndeces, this, m_linkedTree);            
            return cmd;
        }

        private Command.CommandMoveRegularMembers MakeMoveRegularMembersCommand2(DataGridViewCell cell)
        { 
            Dictionary<int, int> dicRowIndeces = new Dictionary<int, int>();
            
            if (!dicRowIndeces.ContainsKey(cell.RowIndex))  
                dicRowIndeces[cell.RowIndex] = cell.RowIndex;

            //foreach (DataGridViewCell cell in this.SelectedCells)
            //{
            //    if (dicRowIndeces.ContainsKey(cell.RowIndex))
            //        continue;

            //    dicRowIndeces[cell.RowIndex] = cell.RowIndex;
            //}

            List<Command.CommandMoveRegularMembers.CompanyMemberNIndex> memberIndeces = new List<Command.CommandMoveRegularMembers.CompanyMemberNIndex>();

            foreach (KeyValuePair<int, int> pair in dicRowIndeces)
            {
                if (pair.Value < 0)
                    continue;

                DataGridViewRow row = this.Rows[pair.Value];

                if (row.IsNewRow)
                    continue;

                if (row.Tag != null && row.Tag is CompanyMember)
                {
                    CompanyMember member = (CompanyMember)row.Tag; 
                    memberIndeces.Add(new Command.CommandMoveRegularMembers.CompanyMemberNIndex(member, pair.Value)); 
                }
            }

            Command.CommandMoveRegularMembers cmd = new Command.CommandMoveRegularMembers((RegularTeam)cell.Tag, null, memberIndeces, this, m_linkedTree);
            return cmd;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStripRegularMember = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuRemoveCompanyMembers = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMoveRegularTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTemporaryMember = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsRemoveTemporaryMember = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMoveTemporaryMember = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripExternalMember = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuRemoveExternalMember = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuMoveExternalMember = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUserDefinedTeam = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuRemoveUserDefinedTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripRegularMember.SuspendLayout();
            this.contextMenuStripTemporaryMember.SuspendLayout();
            this.contextMenuStripExternalMember.SuspendLayout();
            this.contextMenuStripUserDefinedTeam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStripRegularMember
            // 
            this.contextMenuStripRegularMember.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuRemoveCompanyMembers,
            this.tsMoveRegularTeam});
            this.contextMenuStripRegularMember.Name = "contextMenuStrip1";
            this.contextMenuStripRegularMember.Size = new System.Drawing.Size(115, 48);
            // 
            // tsMenuRemoveCompanyMembers
            // 
            this.tsMenuRemoveCompanyMembers.Name = "tsMenuRemoveCompanyMembers";
            this.tsMenuRemoveCompanyMembers.Size = new System.Drawing.Size(114, 22);
            this.tsMenuRemoveCompanyMembers.Text = "삭제";
            this.tsMenuRemoveCompanyMembers.Click += new System.EventHandler(this.tsMenuRemoveCompanyMembers_Click);
            // 
            // tsMoveRegularTeam
            // 
            this.tsMoveRegularTeam.Name = "tsMoveRegularTeam";
            this.tsMoveRegularTeam.Size = new System.Drawing.Size(114, 22);
            this.tsMoveRegularTeam.Text = "팀 이동";
            this.tsMoveRegularTeam.Click += new System.EventHandler(this.tsMoveRegularTeam_Click);
            // 
            // contextMenuStripTemporaryMember
            // 
            this.contextMenuStripTemporaryMember.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsRemoveTemporaryMember,
            this.tsMoveTemporaryMember});
            this.contextMenuStripTemporaryMember.Name = "contextMenuStripTemporaryMember";
            this.contextMenuStripTemporaryMember.Size = new System.Drawing.Size(115, 48);
            // 
            // tsRemoveTemporaryMember
            // 
            this.tsRemoveTemporaryMember.Name = "tsRemoveTemporaryMember";
            this.tsRemoveTemporaryMember.Size = new System.Drawing.Size(114, 22);
            this.tsRemoveTemporaryMember.Text = "삭제";
            this.tsRemoveTemporaryMember.Click += new System.EventHandler(this.tsRemoveTemporaryMember_Click);
            // 
            // tsMoveTemporaryMember
            // 
            this.tsMoveTemporaryMember.Name = "tsMoveTemporaryMember";
            this.tsMoveTemporaryMember.Size = new System.Drawing.Size(114, 22);
            this.tsMoveTemporaryMember.Text = "팀 이동";
            this.tsMoveTemporaryMember.Click += new System.EventHandler(this.tsMoveTemporaryMember_Click);
            // 
            // contextMenuStripExternalMember
            // 
            this.contextMenuStripExternalMember.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuRemoveExternalMember,
            this.tsMenuMoveExternalMember});
            this.contextMenuStripExternalMember.Name = "contextMenuStripExternalMember";
            this.contextMenuStripExternalMember.Size = new System.Drawing.Size(115, 48);
            // 
            // tsMenuRemoveExternalMember
            // 
            this.tsMenuRemoveExternalMember.Name = "tsMenuRemoveExternalMember";
            this.tsMenuRemoveExternalMember.Size = new System.Drawing.Size(114, 22);
            this.tsMenuRemoveExternalMember.Text = "삭제";
            this.tsMenuRemoveExternalMember.Click += new System.EventHandler(this.tsMenuRemoveExternalMember_Click);
            // 
            // tsMenuMoveExternalMember
            // 
            this.tsMenuMoveExternalMember.Name = "tsMenuMoveExternalMember";
            this.tsMenuMoveExternalMember.Size = new System.Drawing.Size(114, 22);
            this.tsMenuMoveExternalMember.Text = "팀 이동";
            this.tsMenuMoveExternalMember.Click += new System.EventHandler(this.tsMenuMoveExternalMember_Click);
            // 
            // contextMenuStripUserDefinedTeam
            // 
            this.contextMenuStripUserDefinedTeam.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuRemoveUserDefinedTeam});
            this.contextMenuStripUserDefinedTeam.Name = "contextMenuStripUserDefinedTeam";
            this.contextMenuStripUserDefinedTeam.Size = new System.Drawing.Size(99, 26);
            // 
            // tsMenuRemoveUserDefinedTeam
            // 
            this.tsMenuRemoveUserDefinedTeam.Name = "tsMenuRemoveUserDefinedTeam";
            this.tsMenuRemoveUserDefinedTeam.Size = new System.Drawing.Size(98, 22);
            this.tsMenuRemoveUserDefinedTeam.Text = "삭제";
            this.tsMenuRemoveUserDefinedTeam.Click += new System.EventHandler(this.tsMenuRemoveUserDefinedTeam_Click);
            // 
            // TeamGrid
            // 
            this.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.RowTemplate.Height = 23;
            this.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.TeamGrid_CellEndEdit);
            this.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TeamGrid_CellMouseDoubleClick);
            this.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.TeamGrid_MouseDown);
            this.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.TeamGrid_CellPainting);
            this.contextMenuStripRegularMember.ResumeLayout(false);
            this.contextMenuStripTemporaryMember.ResumeLayout(false);
            this.contextMenuStripExternalMember.ResumeLayout(false);
            this.contextMenuStripUserDefinedTeam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void tsMenuRemoveExternalMember_Click(object sender, EventArgs e)
        {
            RemoveExternalMember();
        }

        private void tsMenuMoveExternalMember_Click(object sender, EventArgs e)
        {
            Popup.FormSelectTeam frm = new Popup.FormSelectTeam("이동할 팀을 선택해 주세요.", m_linkedTree, m_teamCurrentRow);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm);
            frame.TitleBarBackColor = System.Drawing.Color.FromArgb(246, 169, 43);
            frame.TitleTextColor = System.Drawing.Color.Black;
            frame.ShowMaxButton = false;
            frame.ShowMinButton = false;
            frame.Sizable = false;
            if (frame.ShowDialog(this) != DialogResult.OK)            
                return;            

            //if (frm.ShowDialog() == DialogResult.OK)
            {
                if (frm.SelectedTeam == null)
                    return;

                if (frm.SelectedTeam is ExternalTeam)
                {
                    ExternalTeam team = (ExternalTeam)frm.SelectedTeam;
                    //int nTeamID = (int)frm.SelectedTeam;
                    //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

                    if (team != null && team != m_teamCurrentRow)
                    {
                        string strMemberNames;
                        Command.CommandMoveExternalMembers cmd = MakeMoveExternalMembersCommand(out strMemberNames);

                        if (cmd == null)
                            return;

                        TreeNode dropNode = GetTreeNode(team);
                        //TreeNode dropNode = GetTreeNode(nTeamID);

                        if (dropNode != null)
                            FormMain.Instance.OnDropExternalMembers(cmd, dropNode);

                        RefreshGrid();
                    }
                }
            }
        }

        private void tsMenuRemoveCompanyMembers_Click(object sender, EventArgs e)
        {
            RemoveCompanyMembers();
        }

        private void tsMoveRegularTeam_Click(object sender, EventArgs e)
        { 
            Popup.FormSelectTeam frm = new Popup.FormSelectTeam("이동할 팀을 선택해 주세요.", m_linkedTree, m_teamCurrentRow);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm);
            frame.TitleBarBackColor = System.Drawing.Color.FromArgb(246, 169, 43);
            frame.TitleTextColor = System.Drawing.Color.Black;
            frame.ShowMaxButton = false;
            frame.ShowMinButton = false;
            frame.Sizable = false;
            if (frame.ShowDialog(this) != DialogResult.OK)
                return;   
                        
            //if (frm.ShowDialog() == DialogResult.OK)
            {
                if (frm.SelectedTeam == null)
                    return;

                if (frm.SelectedTeam is RegularTeam)
                {
                    RegularTeam team = (RegularTeam)frm.SelectedTeam;
                    //int nTeamID = (int)frm.SelectedTeam;
                    //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

                    if (team != null && team != m_teamCurrentRow)
                    {
                        string strMemberNames;
                        Command.CommandMoveRegularMembers cmd = MakeMoveRegularMembersCommand(out strMemberNames);

                        if (cmd == null)
                            return;

                        TreeNode dropNode = GetTreeNode(team);
                        //TreeNode dropNode = GetTreeNode(nTeamID);

                        if (dropNode != null)
                            FormMain.Instance.OnDropRegularMembers(cmd, dropNode);

                        RefreshGrid();
                    }
                }
            }
        }

        private void tsMoveTemporaryMember_Click(object sender, EventArgs e)
        {
            Popup.FormSelectTeam frm = new Popup.FormSelectTeam("이동할 팀을 선택해 주세요.", m_linkedTree);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm);
            frame.TitleBarBackColor = System.Drawing.Color.FromArgb(246, 169, 43);
            frame.TitleTextColor = System.Drawing.Color.Black;
            frame.ShowMaxButton = false;
            frame.ShowMinButton = false;
            frame.Sizable = false;
            if (frame.ShowDialog(this) != DialogResult.OK)
                return;

            //if (frm.ShowDialog() == DialogResult.OK)
            {
                if (frm.SelectedTeam == null)
                    return;

                bool isNormal;

                if (IsTemporaryTeam((Team)frm.SelectedTeam, out isNormal))
                {
                    Team team = (Team)frm.SelectedTeam;

                    if (team != null && team != m_teamCurrent)
                    {
                        string strMemberNames;
                        Command.CommandMoveTemporaryMembers cmd = MakeMoveTemporaryMembersCommand(out strMemberNames);

                        if (cmd == null)
                            return;

                        TreeNode dropNode = GetTreeNode(team);

                        if (dropNode != null)
                            FormMain.Instance.OnDropTemporaryMembers(cmd, dropNode);

                        RefreshGrid();
                    }
                }
            }
        }

        private void tsMenuRemoveUserDefinedTeam_Click(object sender, EventArgs e)
        {
            RemoveUserDefinedTeams();
        }

        private TreeNode GetTreeNode(Team team, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
            {
                if (m_linkedTree == null)
                    return null;

                nodes = m_linkedTree.Nodes;
            }

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null)
                {
                    if (team == node.Tag)
                        return node;

                    TreeNode findNode = GetTreeNode(team, node.Nodes);

                    if (findNode != null)
                        return findNode;
                }
            }

            return null;
        }

        private void TeamGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                if (m_gridType == GridType.TemporaryEmergency || m_gridType == GridType.TemporaryNormal)
                {
                    if (e.ColumnIndex >= 0 &&
                        e.ColumnIndex != TeamIndex &&
                        e.ColumnIndex != TeamButtonIndex &&
                        e.ColumnIndex != Manager2Index &&
                        e.ColumnIndex != Manager2ButtonIndex)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }

                        e.Handled = true;
                    }

                    if (e.ColumnIndex == 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                                e.CellBounds.Left, e.CellBounds.Bottom);
                        }
                    }
                    else if (e.ColumnIndex == IncludeChildTeamIndex)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                            e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }
                    }
                }
                else if (m_gridType == GridType.RegularMember)
                {
                    using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                    {
                        e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                        e.CellBounds.Right - 1, e.CellBounds.Top);
                    }


                    if (e.ColumnIndex >= 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }

                        e.Handled = true;
                    }

                    if (e.ColumnIndex == 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                                e.CellBounds.Left, e.CellBounds.Bottom);
                        }
                    }
                    else if (e.ColumnIndex == OfficePhoneNumberIndex)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                            e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }
                    }

                }
                else if (m_gridType == GridType.ExternalCompanyTeam)
                {
                    using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                    {
                        e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                        e.CellBounds.Right - 1, e.CellBounds.Top);
                    }

                    if (e.ColumnIndex >= 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }

                        e.Handled = true;
                    }

                    if (e.ColumnIndex == 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                                e.CellBounds.Left, e.CellBounds.Bottom);
                        }
                    }
                    else if (e.ColumnIndex == ExternalCompanyMemberDescriptionIndex)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                            e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }
                    }
                    
                }
                else if (m_gridType == GridType.UserDefinedTeam)
                {
                    if (e.ColumnIndex >= 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                                e.CellBounds.Right, e.CellBounds.Bottom - 1);

                            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                                e.CellBounds.Right - 1, e.CellBounds.Bottom);
                        }

                        e.Handled = true;
                    }

                    if (e.ColumnIndex == 0)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                                e.CellBounds.Left, e.CellBounds.Bottom);
                        }
                    }
                    else if (e.ColumnIndex == UserDefinedTeamFaxNumber)
                    {
                        using (System.Drawing.Pen p = new System.Drawing.Pen(this.GridColor))
                        {
                            e.Graphics.DrawLine(p, e.CellBounds.Right, e.CellBounds.Top,
                            e.CellBounds.Right, e.CellBounds.Bottom);
                        }
                    }

                }

            }

            if (m_gridType != GridType.TemporaryEmergency && m_gridType != GridType.TemporaryNormal)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == Manager2ButtonIndex || e.ColumnIndex == TeamButtonIndex)
            {
                Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "편집";

                DataGridViewButtonCell cell = (Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell);

            }
        }

        private void TeamGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            if (m_teamCurrent != null)
            {
                if (m_teamCurrent is RegularTeam)
                    RegularMemberCellEndEdit(row, cell);
                else if (m_teamCurrent is TemporaryNormalTeam)
                    TemporaryMemberCellEndEdit(row, cell, true);
                else if (m_teamCurrent is TemporaryEmergencyTeam)
                    TemporaryMemberCellEndEdit(row, cell, false);
                else if (m_teamCurrent is ExternalTeam)
                    ExternalMemberCellEndEdit(row, cell);
            }
            else if (m_gridType == GridType.UserDefinedTeam)
            {
                UserDefinedTeamCellEndEdit(row, cell);
            }
        }

        private void TemporaryMemberCellEndEdit(DataGridViewRow row, DataGridViewCell cell, bool isNormal)
        {
            TemporaryMember member = null;
            bool isNewMember = false;

            if (row.Tag == null)
            {
                List<TemporaryMember> members = null;

                // 신규 행
                if (isNormal)
                {
                    member = new TemporaryNormalMember();
                    members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_teamCurrent);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryNormalMembers((TemporaryNormalTeam)m_teamCurrent, members);
                    }
                }
                else
                {
                    member = new TemporaryEmergencyMember();
                    members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamCurrent);

                    if (members == null)
                    {
                        members = new List<TemporaryMember>();
                        DataManager.SetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamCurrent, members);
                    }
                }

                if (members != null)
                    members.Add(member);

                row.Tag = member;
                isNewMember = true;
            }
            else
                member = (TemporaryMember)row.Tag;

            if (cell.Value == cell.Tag)
                return;

            object cellData = null;
            
            if (!GetTemporaryCellData(cell, out cellData))
            {
                cell.Value = cell.Tag;
                return;
            }

            Command.CommandChangeTemporaryMemberInfo.InfoType type = Command.CommandChangeTemporaryMemberInfo.ToInfoType(cell.ColumnIndex);

            if (type == Command.CommandChangeTemporaryMemberInfo.InfoType.Unknown)
                return;

            Command.CommandChangeTemporaryMemberInfo cmd = new Command.CommandChangeTemporaryMemberInfo(this, member);

            cmd.DataType = type;
            cmd.IsNewMember = isNewMember;
            cmd.SetChangedData(member, cellData);

            // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
            FormMain.Instance.AddCommand(cmd, false);
            cmd.SetMemberData(cmd.Changed);

            AfterTemporaryCellEndEdit(row, member);
        }

        private bool GetTemporaryCellData(DataGridViewCell cell, out object cellData)
        {
            cellData = null;

            if (cell.ColumnIndex == (int)Command.CommandChangeTemporaryMemberInfo.InfoType.Member)
            {
                if (cell.Tag == null)
                    return true;
                else
                {
                    cellData = cell.Tag;
                    return true;
                }
            }
            else if (cell.ColumnIndex == (int)Command.CommandChangeTemporaryMemberInfo.InfoType.Position)
            {
                TemporaryMember member = new TemporaryMember();
                TemporaryMember memberRow = (TemporaryMember)cell.OwningRow.Tag;

                if (cell.Value == null || cell.Value.ToString() == "팀전체")
                {
                    member.IsTeamLeader = false;
                    member.MemberCount = -1;
                }
                else if (cell.Value.ToString() == "팀원")
                {
                    member.IsTeamLeader = false;
                    member.MemberCount = memberRow.MemberCount;
                }
                else if (cell.Value.ToString() == "책임자")
                {
                    member.IsTeamLeader = true;
                }
                else
                    return true;

                cellData = member;
                return true;
            }
            else if (cell.ColumnIndex == (int)Command.CommandChangeTemporaryMemberInfo.InfoType.MemberCount)
            {
                if (cell.Value == null)
                    return true;
                
                int nMemberCount;

                if (int.TryParse(cell.Value.ToString(), out nMemberCount))
                {
                    cellData = nMemberCount;
                    cell.Value = nMemberCount;
                    return true;
                }
                else
                    return false;
            }
            else if (cell.ColumnIndex == (int)Command.CommandChangeTemporaryMemberInfo.InfoType.IncludeChildTeams)
            {
                cellData = cell.Value;
                return true;
            }

            if (cell.Value != null)
                cellData = cell.Value.ToString();

            return true;
        }

        ///<summary>신규 멤버인지 판단</summary> 
        private bool IsNewMember(DataGridViewRow row)
        {
            bool isNewMember = false;
            if (row.Tag == null)
                isNewMember = true;

            return isNewMember;
        }

        private CompanyMember GetRowCompanyMember(DataGridViewRow row, out bool isNewMember)
        {
            CompanyMember member = null;
            isNewMember = false;

            if (row.Tag == null)
            {
                // 신규 행 
                member = new CompanyMember();

                List<CompanyMember> members = null;

                if (row.Cells[TeamNameIndex].Value == null)
                    members = DataManager.GetRegularMembers((RegularTeam)this.m_teamCurrent);
                else
                {
                    members = DataManager.GetRegularMembers((RegularTeam)row.Cells[TeamNameIndex].Value);
                    //members = DataManager.GetRegularMembers(DataManager.GetRegularTeam((int)row.Cells[TeamNameIndex].Value));
                }

                if (members != null)
                    members.Add(member);

                row.Tag = member;
                //isNewMember = true;
            }
            else
                member = (CompanyMember)row.Tag;

            if (member != null && member.ID < 0)
                isNewMember = true;
            else
                isNewMember = false;

            return member;
        }

        private ExternalCompanyMember GetRowExternalCompanyMember(DataGridViewRow row, out bool isNewMember)
        {
            ExternalCompanyMember member = null;
            isNewMember = false;

            if (row.Tag == null)
            {
                // 신규 행
                member = new ExternalCompanyMember();
                List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers((ExternalTeam)this.m_teamCurrent);

                if (members != null)
                    members.Add(member);

                row.Tag = member;
                //isNewMember = true;
            }
            else
                member = (ExternalCompanyMember)row.Tag;

            if (member != null && member.ID < 0)
                isNewMember = true;
            else
                isNewMember = false;

            return member;
        }

        private UserDefinedTeam GetRowUserDefinedTeam(DataGridViewRow row, out bool isNewTeam)
        {
            UserDefinedTeam team = null;
            isNewTeam = false;

            if (row.Tag == null)
            {
                // 신규 행
                team = new UserDefinedTeam();
                List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

                if (teams != null)
                    teams.Add(team);

                row.Tag = team;
                //isNewTeam = true;
            }
            else
                team = (UserDefinedTeam)row.Tag;

            if (team != null && team.TeamID < 0)
                isNewTeam = true;
            else
                isNewTeam = false;

            return team;
        }

        private void ExternalMemberCellEndEdit(DataGridViewRow row, DataGridViewCell cell)
        {
            bool isNewMember;
            ExternalCompanyMember member = GetRowExternalCompanyMember(row, out isNewMember);

            if (cell.Value == cell.Tag)
                return;

            // cellComboBox_Leave(...)에서 이미 처리되었으므로 다시 값을 변경하거나 Command를 생성하지 않는다.
            if (cell.Value != null && cell.Tag != null && cell.Value is string && (cell.Tag is string) == false)
            {
                if (cell.Tag.GetType() != typeof(PhoneNumber))
                {
                    cell.Value = cell.Tag;
                    return;
                }
            }

            Command.CommandChangeExternalMemberInfo.InfoType type = Command.CommandChangeExternalMemberInfo.ToInfoType(cell.ColumnIndex);

            if (type == Command.CommandChangeExternalMemberInfo.InfoType.Unknown)
                return;

            if (!isNewMember && cell.ColumnIndex == ExternalCompanyMemberTeamNameIndex) // 기존 직원 팀이동은 팀이동 Command 사용
            {
                if ((int)cell.Value != ((ExternalTeam)cell.Tag).TeamID)
                {
                    string strMemberNames;
                    Command.CommandMoveExternalMembers cmd = MakeMoveExternalMembersCommand(out strMemberNames);

                    if (cmd == null)
                        return;

                    ExternalTeam team = DataManager.GetExternalTeam((int)cell.Value);
                    TreeNode dropNode = GetTreeNode(team);

                    if (dropNode != null)
                        FormMain.Instance.OnDropExternalMembers(cmd, dropNode);
                }
            }
            else
            {
                Command.CommandChangeExternalMemberInfo cmd = new Command.CommandChangeExternalMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (cell.ColumnIndex == ExternalCompanyMemberPhoneNumberIndex)
                {
                    PhoneNumber phoneNumber = new PhoneNumber((cell.Value == null) ? String.Empty : cell.Value.ToString().Trim(), true);

                    if (phoneNumber.IsBlank)
                    {
                        phoneNumber = null;
                    }
                    else if (!phoneNumber.IsValid)
                    {
                        //MessageBox.Show(String.Format("'{0}'은 휴대전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value));
                        UnE.Utility.UMessageBoxRibbon.Show(String.Format("'{0}'은 휴대전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value)
                                                                                                  , "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cell.Value = cell.Tag;
                        return;
                    }

                    if (cell.Tag == null)
                    {
                        cell.Value = phoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Changed = cell.Value;
                    }
                    else if (phoneNumber == null)
                    {
                        PhoneNumber number = null;

                        if (cell.Tag is string)
                            number = new PhoneNumber(cell.Tag.ToString(), true);
                        else if (cell.Tag is PhoneNumber)
                            number = (PhoneNumber)cell.Tag;
                        else
                            return;


                        number.IsChanged = DataManager.GetExternalCompanyMemberPhoneNumberChanged(member);

                        cell.Value = null;
                        cell.Tag = null;

                        cmd.Origin = number;
                        cmd.Changed = null;

                    }
                    else // if(cell.Tag != null && phoneNumber != null)
                    {
                        PhoneNumber number = null;

                        if (cell.Tag is string)
                            number = new PhoneNumber(cell.Tag.ToString(), true);
                        else if (cell.Tag is PhoneNumber)
                            number = (PhoneNumber)cell.Tag;
                        else
                            return;

                        number.IsChanged = DataManager.GetExternalCompanyMemberPhoneNumberChanged(member);

                        if (number.Number == phoneNumber.Number)
                        {
                            cell.Value = cell.Tag;
                            return;
                        }

                        cell.Value = phoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Origin = number;
                        cmd.Changed = cell.Value;
                    }

                    DataManager.SetExternalCompanyMemberPhoneNumberChanged(member, true);
                }
                else if (cell.ColumnIndex == ExternalCompanyMemberTeamNameIndex)
                {
                    cmd.Origin = cell.Tag;
                    ExternalTeam team = DataManager.GetExternalTeam((int)cell.Value);
                    cmd.Changed = team;
                    cell.Tag = team;
                }
                else
                {
                    cmd.Origin = cell.Tag;
                    cmd.Changed = cell.Value;
                    //cmd.Origin = cell.Tag == null ? "" : cell.Tag.ToString();
                    //cmd.Changed = cell.Value.ToString();
                    cell.Tag = cell.Value;
                }

                // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
                FormMain.Instance.AddCommand(cmd, false);
                cmd.SetMemberData(cmd.Changed);
            }
        }

        private void RegularMemberCellEndEdit(DataGridViewRow row, DataGridViewCell cell)
        {
            bool isNewMember;
            CompanyMember member = GetRowCompanyMember(row, out isNewMember);

            if (cell.Value == cell.Tag)
                return;

            // cellComboBox_Leave(...)에서 이미 처리되었으므로 다시 값을 변경하거나 Command를 생성하지 않는다.
            if (cell.Value != null && cell.Tag != null && cell.Value is string && (cell.Tag is string) == false)
            {
                // MemberID / PhoneNumber Class의 경우에는 예외처리
                if (cell.Tag.GetType() != typeof(MemberID)
                    && cell.Tag.GetType() != typeof(PhoneNumber)
                    && cell.Tag.GetType() != typeof(OfficePhoneNumber))
                {
                    cell.Value = cell.Tag;
                    return;
                }
            }

            Command.CommandChangeRegularMemberInfo.InfoType type = Command.CommandChangeRegularMemberInfo.ToInfoType(cell.ColumnIndex);

            if (type == Command.CommandChangeRegularMemberInfo.InfoType.Unknown)
                return;

            if (!isNewMember && cell.ColumnIndex == TeamNameIndex) // 기존 직원 팀이동은 팀이동 Command 사용
            {
                if ((int)cell.Value != ((RegularTeam)cell.Tag).TeamID)
                { 
                    Command.CommandMoveRegularMembers cmd = MakeMoveRegularMembersCommand2(cell);

                    if (cmd == null)
                        return;

                    RegularTeam team = DataManager.GetRegularTeam((int)cell.Value);
                    TreeNode dropNode = GetTreeNode(team);

                    if (dropNode != null)
                        FormMain.Instance.OnDropRegularMembers(cmd, dropNode);
                }
            }
            else
            {
                Command.CommandChangeRegularMemberInfo cmd = new Command.CommandChangeRegularMemberInfo(this, member);
                cmd.DataType = type;
                cmd.IsNewMember = isNewMember;

                if (cell.ColumnIndex == MemberIDIndex)
                {
                    if (cell.Tag == null)
                    {
                        cell.Value = new MemberID(cell.Value.ToString(), true);
                        cell.Tag = cell.Value;

                        cmd.Changed = cell.Value;
                    }
                    else
                    {
                        MemberID id = (MemberID)cell.Tag;
                        id.IsChanged = DataManager.GetCompanyMemberMemberIDChanged(member);

                        if (cell.Value != null && id.ID == cell.Value.ToString())
                        {
                            cell.Value = cell.Tag;
                            return;
                        }

                        cell.Value = new MemberID(cell.Value == null ? "" : cell.Value.ToString(), true);
                        cell.Tag = cell.Value;

                        cmd.Origin = id;
                        cmd.Changed = cell.Value;
                    }

                    DataManager.SetCompanyMemberMemberIDChanged(member, true);
                }
                else if (cell.ColumnIndex == PhoneNumberIndex)
                {
                    PhoneNumber phoneNumber = new PhoneNumber((cell.Value == null ? String.Empty : cell.Value.ToString().Trim()), true);

                    if (phoneNumber.IsBlank)
                    {
                        phoneNumber = null;
                    }
                    else if (!phoneNumber.IsValid)
                    {
                        //MessageBox.Show(String.Format("'{0}'은 휴대전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value));
                        UnE.Utility.UMessageBoxRibbon.Show(String.Format("'{0}'은 휴대전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value)
                                                                          , "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cell.Value = cell.Tag;
                        return;
                    }

                    if (phoneNumber != null)
                    {
                        string OverlapMsg = DataManager.OverlapRegularMember((CompanyMember)row.Tag, phoneNumber.Number);
                        if (OverlapMsg.Length > 0)
                        {
                            MessageBox.Show(OverlapMsg);
                            cell.Value = cell.Tag;
                            return;
                        } 
                    }

                    if (cell.Tag == null)
                    {
                        cell.Value = phoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Changed = cell.Value;
                    }
                    else if (phoneNumber == null)
                    {
                        PhoneNumber number = null;

                        if (cell.Tag is string)
                            number = new PhoneNumber(cell.Tag.ToString(), true);
                        else if (cell.Tag is PhoneNumber)
                            number = (PhoneNumber)cell.Tag;
                        else
                            return;


                        number.IsChanged = DataManager.GetCompanyMemberPhoneNumberChanged(member);

                        cell.Value = null;
                        cell.Tag = null;

                        cmd.Origin = number;
                        cmd.Changed = null;

                    }
                    else // if(cell.Tag != null && phoneNumber != null)
                    {
                        PhoneNumber number = (PhoneNumber)cell.Tag;
                        number.IsChanged = DataManager.GetCompanyMemberPhoneNumberChanged(member);

                        if (number.Number == phoneNumber.Number)
                        {
                            cell.Value = cell.Tag;
                            return;
                        } 

                        cell.Value = phoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Origin = number;
                        cmd.Changed = cell.Value;
                    }

                    DataManager.SetCompanyMemberPhoneNumberChanged(member, true);
                }
                else if (cell.ColumnIndex == OfficePhoneNumberIndex)
                {
                    OfficePhoneNumber officePhoneNumber = new OfficePhoneNumber((cell.Value == null ? String.Empty : cell.Value.ToString().Trim()), true);

                    if (officePhoneNumber.IsBlank)
                    {
                        officePhoneNumber = null;
                    }
                    else if (!officePhoneNumber.IsValid)
                    {
                        //MessageBox.Show(String.Format("'{0}'은 전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value));
                        UnE.Utility.UMessageBoxRibbon.Show(String.Format("'{0}'은  전화번호 형식에 맞지 않습니다.\r\n다시 입력해 주세요", cell.Value)
                                                  , "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        cell.Value = cell.Tag;
                        return;
                    } 

                    if (cell.Tag == null)
                    {
                        cell.Value = officePhoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Changed = cell.Value;
                    }
                    else if (officePhoneNumber == null)
                    {
                        OfficePhoneNumber number = null;

                        if (cell.Tag is string)
                            number = new OfficePhoneNumber(cell.Tag.ToString(), true);
                        else if (cell.Tag is OfficePhoneNumber)
                            number = (OfficePhoneNumber)cell.Tag;
                        else
                            return;


                        number.IsChanged = DataManager.GetCompanyMemberOfficePhoneNumberChanged(member);

                        cell.Value = null;
                        cell.Tag = null;

                        cmd.Origin = number;
                        cmd.Changed = null;

                    }
                    else // if(cell.Tag != null && phoneNumber != null)
                    {
                        OfficePhoneNumber number = (OfficePhoneNumber)cell.Tag;
                        number.IsChanged = DataManager.GetCompanyMemberOfficePhoneNumberChanged(member);

                        if (number.Number == officePhoneNumber.Number)
                        {
                            cell.Value = cell.Tag;
                            return;
                        }

                        cell.Value = officePhoneNumber;
                        cell.Tag = cell.Value;

                        cmd.Origin = number;
                        cmd.Changed = cell.Value;
                    }

                    DataManager.SetCompanyMemberOfficePhoneNumberChanged(member, true);
                }
                else if (cell.ColumnIndex == SubLevelIndex)
                {
                    if (cell.Value == null || cell.Value.ToString().Length == 0)
                    {
                        cmd.Origin = cell.Tag;
                        cmd.Changed = cell.Value;

                        cell.Value = cell.Tag = null;
                    }
                    else
                    {
                        CompanyMember.JobLevelSubInfo subLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(cell.Value.ToString());

                        if (subLevel == null)
                        {
                            subLevel = new CompanyMember.JobLevelSubInfo();
                            subLevel.Name = cell.Value.ToString();
                        }

                        cmd.Origin = cell.Tag;
                        cmd.Changed = subLevel;

                        cell.Value = cell.Tag = subLevel;
                    }
                }
                else if (cell.ColumnIndex == SubPositionIndex)
                {
                    if (cell.Value == null || cell.Value.ToString().Length == 0)
                    {
                        cmd.Origin = cell.Tag;
                        cmd.Changed = cell.Value;

                        cell.Value = cell.Tag = null;
                    }
                    else
                    {
                        CompanyMember.JobPositionSubInfo subPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(cell.Value.ToString());

                        if (subPosition == null)
                        {
                            subPosition = new CompanyMember.JobPositionSubInfo();
                            subPosition.Name = cell.Value.ToString();
                        }

                        cmd.Origin = cell.Tag;
                        cmd.Changed = subPosition;

                        cell.Value = cell.Tag = subPosition;
                    }
                }
                else if (cell.ColumnIndex == GroupPositionIndex)
                {
                    if (cell.Value == null || cell.Value.ToString().Length == 0)
                    {
                        cmd.Origin = cell.Tag;
                        cmd.Changed = cell.Value;

                        cell.Value = cell.Tag = null;
                    }
                    else
                    {
                        CompanyMember.JobGroupPosition groupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(cell.Value.ToString());

                        if (groupPosition == null)
                        {
                            groupPosition = new CompanyMember.JobGroupPosition();
                            groupPosition.Name = cell.Value.ToString();
                        }

                        cmd.Origin = cell.Tag;
                        cmd.Changed = groupPosition;

                        cell.Value = cell.Tag = groupPosition;
                    }
                }
                else if (cell.ColumnIndex == TeamNameIndex)
                {
                    cmd.Origin = cell.Tag;
                    RegularTeam team = DataManager.GetRegularTeam((int)cell.Value);
                    cmd.Changed = team;
                    cell.Tag = team;

                    RegularTeam orgTeam = cmd.Origin as RegularTeam;
                    if (orgTeam != null)
                    {
                        if (orgTeam.TeamID != team.TeamID)
                            DataManager.SetMoveRegularTeamMemberInfo(orgTeam, team, member);
                    }
                }
                else
                { 
                    cmd.Origin = cell.Tag;
                    cmd.Changed = cell.Value;

                    if (cmd.Origin == cmd.Changed) 
                        return;

                    //cmd.Origin = cell.Tag == null ? "" : cell.Tag.ToString();
                    //cmd.Changed = cell.Value.ToString();
                    cell.Tag = cell.Value;
                }

                // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
                FormMain.Instance.AddCommand(cmd, false);
                cmd.SetMemberData(cmd.Changed);
            } 
        }

        private void UserDefinedTeamCellEndEdit(DataGridViewRow row, DataGridViewCell cell)
        {
            bool isNewTeam;
            UserDefinedTeam team = GetRowUserDefinedTeam(row, out isNewTeam);

            if (cell.Value == cell.Tag)
                return;

            // cellComboBox_Leave(...)에서 이미 처리되었으므로 다시 값을 변경하거나 Command를 생성하지 않는다.
            if (cell.Value != null && cell.Tag != null && cell.Value is string && (cell.Tag is string) == false)
            {
                cell.Value = cell.Tag;
            }

            Command.CommandChangeUserDefinedTeamInfo.InfoType type = Command.CommandChangeUserDefinedTeamInfo.ToInfoType(cell.ColumnIndex);

            if (type == Command.CommandChangeUserDefinedTeamInfo.InfoType.Unknown)
                return;

            //전화번호가 없으면 공백을 입력
            if (row.Cells[2].Value == null)
            {
                row.Cells[2].Value = "";
                row.Cells[2].Tag = "";
            }
            
            Command.CommandChangeUserDefinedTeamInfo cmd = new Command.CommandChangeUserDefinedTeamInfo(this, team);
            cmd.DataType = type;
            cmd.IsNewTeam = isNewTeam;

            cmd.Origin = cell.Tag;
            cmd.Changed = cell.Value;
            cell.Tag = cell.Value;

            // SelectTeam이 호출되어 rows.Clear()로 인하여 무한루프로 빠지는 오류를 제거하기 위하여 Do()는 호출하지 않도록 한다.
            FormMain.Instance.AddCommand(cmd, false);
            cmd.SetMemberData(cmd.Changed);
        }
         
        public Command.CommandChangeTemporaryMemberInfo GetTemporaryMemberChangingCommand(object dataTeam, object dataMember, Command.CommandChangeTemporaryMemberInfo.InfoType type)
        {
            if (m_teamCurrent == null)
                return null;

            if ((m_teamCurrent is TemporaryNormalTeam) || (m_teamCurrent is TemporaryEmergencyTeam))
            {
                if (this.SelectedCells.Count == 0)
                    return null;

                int nRowIndex = this.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return null;

                DataGridViewRow row = this.Rows[nRowIndex];

                TemporaryMember member = null;

                if (!row.IsNewRow)
                    member = (TemporaryMember)row.Tag;
                else
                {
                    if (m_teamCurrent is TemporaryNormalTeam)
                        member = new TemporaryNormalMember();
                    else
                        member = new TemporaryEmergencyMember();
                }

                Command.CommandChangeTemporaryMemberInfo cmd = new Command.CommandChangeTemporaryMemberInfo(this, member);

                bool isTeamLeader = false;

                if ((dataTeam is RegularTeam && dataMember is CompanyMember == false) ||
                    (dataTeam is ExternalTeam && dataMember is ExternalCompanyMember == false) ||
                    (dataTeam is UserDefinedTeam))
                {
                    isTeamLeader = true;
                }

                cmd.DataType = type;
                cmd.SetChangedData(member, new object[] { dataTeam, dataMember, isTeamLeader, -1 });
                cmd.IsNewMember = row.IsNewRow;

                return cmd;
            }

            return null;
        }

        private void tsRemoveTemporaryMember_Click(object sender, EventArgs e)
        {
            RemoveTemporaryMembers();
        }

        public static bool IsTemporaryTeam(Team team, out bool isNormal)
        {
            isNormal = false;

            if (team == null)
                return false;

            if (team is TemporaryNormalTeam)
            {
                isNormal = true;
                return true;
            }
            else if (team is TemporaryEmergencyTeam)
            {
                isNormal = false;
                return true;
            }

            return false;
        }

        public CompanyMember.JobGroupPosition groupPosition { get; set; }

    }
}
