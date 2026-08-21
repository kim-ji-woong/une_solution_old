using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ScenarioEditor
{
    public partial class FormMain : Form
    {
        private FormDisaster m_frmDisaster = new FormDisaster();
        private FormDisaster m_frmSpread = new FormDisaster();
        private FormDisaster m_frmTrans = new FormDisaster();
        private FormDisaster m_frmControl = new FormDisaster();
        private FormDisaster m_frmInitial = new FormDisaster();
        private FormDisaster m_frmEvacuation = new FormDisaster();
        private FormDisaster m_frmCommit = new FormDisaster();
        private FormDisaster m_frmSuppress = new FormDisaster();
        private FormDisaster m_frmRescue = new FormDisaster();
        private FormVariable m_frmVariable = new FormVariable();
        private FormVariable2 m_frmVariable2 = new FormVariable2();

        private bool m_closeApplication = false;
        private static FormMain m_instance = null;

        private List<string> m_xmlLines = new List<string>();

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public bool IsDayLight
        {
            get { return radioDay.Checked; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_frmDisaster.Text = "재난발생기";
            m_frmSpread.Text = "재난확대기";
            m_frmTrans.Text = "신고접수 및 상황전파";
            m_frmControl.Text = "지휘체계 확립 및 초기대응";
            m_frmInitial.Text = "초기대응";
            m_frmEvacuation.Text = "주민대피";
            m_frmCommit.Text = "현장투입";
            m_frmSuppress.Text = "사고진압";
            m_frmRescue.Text = "진압 및 구조";

            m_frmDisaster.CheckBox = checkBoxDisaster;
            m_frmSpread.CheckBox = checkBoxSpread;
            m_frmTrans.CheckBox = checkBoxTrans;
            m_frmControl.CheckBox = checkBoxControl;
            m_frmInitial.CheckBox = checkBoxInitial;
            m_frmEvacuation.CheckBox = checkBoxEvacuation;
            m_frmCommit.CheckBox = checkBoxCommit;
            m_frmSuppress.CheckBox = checkBoxSuppress;
            m_frmRescue.CheckBox = checkBoxRescue;

            m_frmVariable.CheckBox = checkBoxVariable;
            m_frmVariable2.CheckBox = checkBoxVariable2;

            radioDay.Checked = true;
        }

        private void btnLoadOrigin_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
			dlg.FilterIndex = 0;
			dlg.Title = "XML 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxOriginXML.Text = dlg.FileName;
                LoadFile();
            }
        }

        private void LoadFile()
        {
            StreamReader reader = new StreamReader(textBoxOriginXML.Text);
            FormDisaster frm = null;

            m_xmlLines.Clear();

            while (!reader.EndOfStream)
            {
                string strOrigin = reader.ReadLine();
                m_xmlLines.Add(strOrigin);

                string strLine = strOrigin.Trim();

                if (strLine == "<Component id=\"7\">")
                {
                    frm = m_frmDisaster;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"9\">")
                {
                    frm = m_frmSpread;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"3\">")
                {
                    frm = m_frmTrans;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"15\">")
                {
                    frm = m_frmControl;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"16\">")
                {
                    frm = m_frmInitial;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"6\">")
                {
                    frm = m_frmEvacuation;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"22\">")
                {
                    frm = m_frmCommit;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"32\">")
                {
                    frm = m_frmSuppress;
                    frm.ComponentID = strLine;
                }
                else if (strLine == "<Component id=\"49\">")
                {
                    frm = m_frmRescue;
                    frm.ComponentID = strLine;
                }

                if (strLine.StartsWith("<Mission transmissionType"))
                {
                    string strMission = GetMissionItem(strLine);

                    if (strMission.Length > 0 && frm != null)
                        frm.AddItem(strMission);
                }
            }

            reader.Close();

            m_frmDisaster.Reload();
            m_frmSpread.Reload();
            m_frmTrans.Reload();
            m_frmControl.Reload();
            m_frmInitial.Reload();
            m_frmEvacuation.Reload();
            m_frmCommit.Reload();
            m_frmSuppress.Reload();
            m_frmRescue.Reload();
        }

        private string GetMissionItem(string strLine)
        {
            int nIndex1 = strLine.IndexOf('>');
            int nIndex2 = strLine.LastIndexOf("</");

            if (nIndex1 < 0 || nIndex2 < 0)
                return "";

            return strLine.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            //<Mission transmissionType="2" target="화학구조팀">(사고 원인) 진압</Mission>
        }

        private void btnLoadSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "XML Files|*.xml|All FIles|*.*";
			dlg.FilterIndex = 0;
			dlg.Title = "시나리오 저장";
			dlg.OverwritePrompt = true;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxSaveXML.Text = dlg.FileName;
            }
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    CloseApplication = true;
                    break;
            }

            base.WndProc(ref m);
        }

        public void Reset(IVariable variable)
        {
            m_frmDisaster.Reset(variable);
            m_frmSpread.Reset(variable);
            m_frmTrans.Reset(variable);
            m_frmControl.Reset(variable);
            m_frmInitial.Reset(variable);
            m_frmEvacuation.Reset(variable);
            m_frmCommit.Reset(variable);
            m_frmSuppress.Reset(variable);
            m_frmRescue.Reset(variable);
        }

        private void SaveFile()
        {
            IVariable variable = null;

            if (checkBoxVariable.Checked)
                variable = m_frmVariable;
            else if (checkBoxVariable2.Checked)
                variable = m_frmVariable2;
            else
                return;

            if (textBoxSaveXML.Text.Length == 0)
            {
                MessageBox.Show("저장할 파일의 경로를 입력하세요");
                return;
            }

            try
            {
                string strCategory = "", strSubCategory = "", strSOPVersion = "";
                MakeHeaderString(variable, textBoxSaveXML.Text, ref strCategory, ref strSubCategory, ref strSOPVersion);

                StreamWriter writer = new StreamWriter(textBoxSaveXML.Text, false, Encoding.UTF8);

                foreach (string strXMLLine in m_xmlLines)
                {
                    if (!CheckHeader(strXMLLine, writer, variable, strCategory, strSubCategory, strSOPVersion))
                    {
                        List<string> lines = ChangeString(strXMLLine, variable);

                        foreach (string strLine in lines)
                        {
                            writer.WriteLine(strLine);
                        }
                    }
                }

                writer.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            MessageBox.Show("저장이 완료되었습니다.");
        }

        private void MakeHeaderString(IVariable variable, string strPath, ref string strCategory, ref string strSubCategory, ref string strSOPVersion)
        {
            if (variable.Reason == "화재")
            {
                strCategory = "화재";
                strSubCategory = "화재";
            }
            else if (variable.Reason == "누출")
            {
                strCategory = "유출사고";
                strSubCategory = "오염";
            }

            int nIndex1 = strPath.LastIndexOf('\\');
            int nIndex2 = strPath.LastIndexOf('.');

            if (nIndex2 < 0 || nIndex2 < nIndex1)
            {
                nIndex2 = strPath.Length;
            }

            strSOPVersion = strPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
        }

        private bool CheckHeader(string strLine, StreamWriter writer, IVariable variable, string strCategory, string strSubCategory, string strSOPVersion)
        {
            string strCategoryTag = "Category>";
            string strSubCategoryTag = "SubCategory>";
            string strDisasterTag = "Disaster>";
            string strNormalTag = "Normal>";
            string strVersionTag = "SOPVersion>";

            if (CheckHeader(ref strLine, strCategoryTag, strCategory))
            {
                writer.WriteLine(strLine);
                return true;
            }
            else if (CheckHeader(ref strLine, strSubCategoryTag, strSubCategory))
            {
                writer.WriteLine(strLine);
                return true;
            }
            else if (CheckHeader(ref strLine, strDisasterTag, variable.MaterialName))
            {
                writer.WriteLine(strLine);
                return true;
            }
            else if (CheckHeader(ref strLine, strNormalTag, this.IsDayLight ? "1" : "0"))
            {
                writer.WriteLine(strLine);
                return true;
            }
            else if (CheckHeader(ref strLine, strVersionTag, strSOPVersion))
            {
                writer.WriteLine(strLine);
                return true;
            }

            return false;
        }

        private bool CheckHeader(ref string strLine, string strTag, string strTarget)
        {
            string strBeginTag = "<" + strTag;
            string strEndTag = "</" + strTag;

            int nIndex1 = strLine.IndexOf(strBeginTag);
            int nIndex2 = strLine.IndexOf(strEndTag);

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                strLine = strLine.Substring(0, nIndex1) + strBeginTag + strTarget + strEndTag;
                return true;
            }

            return false;
        }

        private List<string> ChangeString(string strLine, IVariable variable)
        {
            ChangeString(ref strLine, "(물질명)", variable.MaterialName);
            ChangeString(ref strLine, "(장소)", variable.Place);
            ChangeString(ref strLine, "(사고 원인)", variable.Reason);
            ChangeString(ref strLine, "(사고원인)", variable.Reason);
            ChangeString(ref strLine, "(기상)", variable.Weather);
            ChangeString(ref strLine, "(발생 물질)", variable.Material);
            ChangeString(ref strLine, "(발생물질)", variable.Material);
            ChangeString(ref strLine, "(사상자 인원)", variable.CountOfDeath);
            ChangeString(ref strLine, "(사상자인원)", variable.CountOfDeath);
            ChangeString(ref strLine, "(건물 숫자)", variable.CountOfBuilding);
            ChangeString(ref strLine, "(건물숫자)", variable.CountOfBuilding);
            ChangeString(ref strLine, "(초기 이격거리)", variable.InitialDistance);
            ChangeString(ref strLine, "(초기 이격 거리)", variable.InitialDistance);
            ChangeString(ref strLine, "(초기이격거리)", variable.InitialDistance);
            ChangeString(ref strLine, "(지휘 체계)", variable.Control);
            ChangeString(ref strLine, "(지휘체계)", variable.Control);
            ChangeString(ref strLine, "(대피 거리)", variable.Distance);
            ChangeString(ref strLine, "(대피거리)", variable.Distance);
            ChangeString(ref strLine, "(반응 물질)", variable.MixedFactor);
            ChangeString(ref strLine, "(반응물질)", variable.MixedFactor);

            List<string> lines = new List<string>();

            if (!ChangeString(strLine, lines, "(대응 내용)", variable.Actions))
            {
                if (!ChangeString(strLine, lines, "(대응내용)", variable.Actions))
                {
                    if (!ChangeString(strLine, lines, "(환자 응급조치)", variable.PatientItems))
                    {
                        if (!ChangeString(strLine, lines, "(환자 응급 조치)", variable.PatientItems))
                            ChangeString(strLine, lines, "(환자응급조치)", variable.PatientItems);
                    }
                }
            }

            if (lines.Count == 0)
                lines.Add(strLine);
            else if (lines.Count == 1 && lines[0].Length == 0)
                lines.Clear();

            return lines;
        }

        private bool ChangeString(string strOrigin, List<string> lines, string strTag, List<string> targets)
        {
            int nIndex = strOrigin.IndexOf(strTag);

            if (nIndex >= 0)
            {
                if (targets == null || targets.Count == 0)
                {
                    lines.Add("");
                    return true;
                }

                string strLeft = strOrigin.Substring(0, nIndex);
                string strRight = strOrigin.Substring(nIndex + strTag.Length);

                foreach (string strTarget in targets)
                {
                    lines.Add(strLeft + strTarget + strRight);
                }

                return true;
            }

            return false;
        }

        private void ChangeString(ref string strOrigin, string strTag, string strTarget)
        {
            if (strTarget == null)
                return;

            int nIndex = strOrigin.IndexOf(strTag);

            while (nIndex >= 0)
            {
                string strLeft = strOrigin.Substring(0, nIndex);
                string strRight = strOrigin.Substring(nIndex + strTag.Length);

                strOrigin = strLeft + strTarget + strRight;
                nIndex = strOrigin.IndexOf(strTag);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
            /*if (textBoxSaveXML.Text.Length == 0)
            {
                MessageBox.Show("저장할 파일의 경로를 입력하세요");
                return;
            }

            try
            {
                string strTime = "";
                FormDisaster frm = null;
                StreamWriter writer = new StreamWriter(textBoxSaveXML.Text, false, Encoding.UTF8);

                foreach (string strXMLLine in m_xmlLines)
                {
                    string strLine = strXMLLine.Trim();

                    if (strLine.StartsWith("<Component id="))
                    {
                        if (strLine == m_frmDisaster.ComponentID)
                            frm = m_frmDisaster;
                        else if (strLine == m_frmSpread.ComponentID)
                            frm = m_frmSpread;
                        else if (strLine == m_frmTrans.ComponentID)
                            frm = m_frmTrans;
                        else if (strLine == m_frmControl.ComponentID)
                            frm = m_frmControl;
                        else if (strLine == m_frmInitial.ComponentID)
                            frm = m_frmInitial;
                        else if (strLine == m_frmEvacuation.ComponentID)
                            frm = m_frmEvacuation;
                        else if (strLine == m_frmCommit.ComponentID)
                            frm = m_frmCommit;
                        else if (strLine == m_frmSuppress.ComponentID)
                            frm = m_frmSuppress;
                        else if (strLine == m_frmRescue.ComponentID)
                            frm = m_frmRescue;
                        else
                            frm = null;
                    }
                    else if (strLine == "</Component>")
                        frm = null;

                    if (frm != null && strLine == "<MissionList>")
                    {
                        int nIndex = strXMLLine.IndexOf('<');

                        if (nIndex < 0)
                            continue;

                        writer.WriteLine(strXMLLine);

                        string strTab = strXMLLine.Substring(0, nIndex) + "\t";
                        WriteMissions(writer, frm, strTab);
                    }
                    else if (frm != null && strLine.StartsWith("<Mission transmissionType"))
                        continue;
                    else if (IsTimeText(strLine, ref strTime))
                    {
                        if (radioDay.Checked)
                            writer.WriteLine(strXMLLine);
                        else
                            WriteNightTime(writer, strXMLLine, strTime);
                    }
                    else
                        writer.WriteLine(strXMLLine);
                }

                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            MessageBox.Show("저장이 완료되었습니다.");*/
        }

        private void WriteNightTime(StreamWriter writer, string strXMLLine, string strTime)
        {
            int nIndex = strTime.IndexOf(':');

            if (nIndex < 0)
            {
                writer.WriteLine(strXMLLine);
                return;
            }

            string strHour = strTime.Substring(0, nIndex);

            int nHour;

            if (!int.TryParse(strHour, out nHour))
            {
                writer.WriteLine(strXMLLine);
                return;
            }

            nHour += 8;

            strTime = nHour.ToString() + strTime.Substring(nIndex);

            int nIndex2 = strXMLLine.IndexOf("<Text>");

            if (nIndex2 < 0)
            {
                writer.WriteLine("<Text>" + strTime + "</Text>");
                return;
            }

            string strTab = strXMLLine.Substring(0, nIndex2);

            writer.Write(strTab);
            writer.WriteLine("<Text>" + strTime + "</Text>");
        }

        private bool IsTimeText(string strLine, ref string strTime)
        {
            string strTag1 = "<Text>";
            string strTag2 = "</Text>";
            int nIndex1 = strLine.IndexOf(strTag1);

            if (nIndex1 < 0)
                return false;

            int nIndex2 = strLine.IndexOf(strTag2);

            if (nIndex2 < 0)
                return false;

            int nBeginIndex = nIndex1 + strTag1.Length;
            string strItem = strLine.Substring(nBeginIndex, nIndex2 - nBeginIndex);

            strTime = strItem;

            if (strItem.Length == 5 && strItem.Contains(':'))
                return true;

            if (strItem.Contains("D+1"))
                return true;

            return false;
        }

        private void WriteMissions(StreamWriter writer, FormDisaster frm, string strTab)
        {
            List<string> items = frm.Items;

            foreach (string strItem in items)
            {
                writer.Write(strTab);
                writer.Write("<Mission transmissionType=\"2\" target=\"\">");
                writer.Write(strItem);
                writer.WriteLine("</Mission>");
            }
        }
    }
}
