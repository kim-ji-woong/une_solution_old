using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring.Popups
{ 
    public partial class BeginWorkSelectPipe : Form
    { 
        private int nTankID { get; set; }
        public int nPipeID = -1;
        private int nStableBeginWorkM { get; set; } // 작업 시작 후 ?분 기준

        private int m_nHorzButtonCount = 4;
        private int m_nVertButtonCount = 3;
        private Color m_btnForeColor = Color.Black;
         
        // m_isNormal이 true이면 등록된 Pipe Table의 값을 사용
        //              false이면 등록되지 않은 PO, 황산 배관을 사용
        private bool m_isNormal = true;
        private const int PO_PIPE = -100;
        private const int H2SO4_PIPE = -200;

        /// <summary>
        /// 탱크에서 작업시작할때
        /// </summary>
        /// <param name="selectedId"></param>
        public BeginWorkSelectPipe(int selectedId, bool isNormal = true)
        {
            InitializeComponent();

            m_isNormal = isNormal;
            this.nTankID = selectedId; 
            foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
            {
                if (item.nTankID == this.nTankID)
                {
                    label_pipeName.Text = "TK-" + item.strTankName + item.strType;
                    break;
                }
            }

            label_pipeName.Parent = pictureBox1;
            pictureBox_begin.Parent = pictureBox1;
            pictureBox_cancel.Parent = pictureBox1;

            BeginWorkSelectTank.ReadButtonCount(panelPipeButtonArea, ref m_nHorzButtonCount, ref m_nVertButtonCount);
            Display();
        }

        Image unSelectImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Normal;
        Image selectImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Click;

        private void Display()
        {
            int recentWorkId = -1;
            ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("select pipeid, AnotherLink from lastworkhistory where tankid = " + this.nTankID + " order by begintime desc limit 1", 0);
            if (arrList != null && arrList.Count == 2)
            {
                recentWorkId = DBUtility.WebDBManager.GetIntField(arrList[0].ToString(), -1);
                int anotherLink = DBUtility.WebDBManager.GetIntField(arrList[1].ToString(), -1);

                if (recentWorkId < 0 && anotherLink != -1)
                    recentWorkId = anotherLink;
            }

            int nSpaceHorz, nSpaceVert;
            int nImageWidth, nImageHeight;
            BeginWorkSelectTank.GetDisplayInfo(panelPipeButtonArea, unSelectImg.Size, m_nHorzButtonCount, m_nVertButtonCount, out nImageWidth, out nImageHeight, out nSpaceHorz, out nSpaceVert);

            List<CommonFunction.PipeInfo> pipes = null;

            if (m_isNormal)
            {
                pipes = MainForm.Instance.pipeInfo;
            }
            else
            {
                pipes = new List<CommonFunction.PipeInfo>();

                CommonFunction.PipeInfo itemPO = new CommonFunction.PipeInfo(PO_PIPE, "PO", "", 0, 0, 0, 0);
                CommonFunction.PipeInfo itemH2SO4 = new CommonFunction.PipeInfo(H2SO4_PIPE, "황산", "", 0, 0, 0, 0);
                pipes.Add(itemPO);
                pipes.Add(itemH2SO4);
            }

            int index = 0;

            for (int i = 0; i < pipes.Count; i++)
            {
                //CommonFunction.PipeInfo item = i < MainForm.Instance.pipeInfo.Count ? MainForm.Instance.pipeInfo[i] : null;
                CommonFunction.PipeInfo item = pipes[i];

                int x = index % m_nHorzButtonCount;
                int y = index / m_nHorzButtonCount;

                int x2 = panelPipeButtonArea.Location.X + x * (nImageWidth + nSpaceHorz);
                int y2 = panelPipeButtonArea.Location.Y + y * (nImageHeight + nSpaceVert);
                SettingBtn(item, x2, y2, nImageWidth, nImageHeight, (item.nPipeID == recentWorkId));

                index++;
            }
        } 
        PictureBox selectedBtn = null;
        void btn_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox btn = null;
            if (sender is PictureBox)
            {
                btn = sender as PictureBox;                
            }
            else if (sender is Label)
            {
                Label label = sender as Label;
                if (label.Parent is PictureBox)
                {
                    btn = label.Parent as PictureBox;
                    label.ForeColor = Color.White;
                }
            }

            if (btn == null) return;
            if (selectedBtn != null)
            {
                selectedBtn.Image = unSelectImg;
                if (selectedBtn.Controls != null && selectedBtn.Controls.Count == 1)
                {
                    //Child Label 글자 색상 바꾸기 
                    Label label = selectedBtn.Controls[0] as Label;
                    label.ForeColor = m_btnForeColor;//Color.FromArgb(243, 116, 33);
                }                
            }

            if (btn == selectedBtn)
            {
                btn.Image = unSelectImg;
                selectedBtn = null;
            }
            else
            {
                btn.Image = selectImg;
                selectedBtn = btn;
            }
        }

        private void SettingBtn(CommonFunction.PipeInfo item, int btnX, int btnY, int btnWidth, int btnHeight, bool isSelectedBtn)
        {
            //int btnWidth = 188;
            //int btnHeight = 82;

            PictureBox pic = new PictureBox();
            pic.Size = new Size(btnWidth, btnHeight);
            pic.Location = new Point(btnX, btnY);
            pic.BackColor = Color.Transparent;
            pic.Parent = pictureBox1;
            if (isSelectedBtn)
            {
                pic.Image = selectImg;
                selectedBtn = pic;
            }
            else
                pic.Image = unSelectImg;
            pic.MouseClick += btn_MouseClick;            
            pic.Tag = item;
            pic.Cursor = Cursors.Hand;
            pic.SizeMode = PictureBoxSizeMode.StretchImage;

            Label label = new Label();
            label.AutoSize = false;
            label.Font = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            label.Size = new Size(btnWidth, btnHeight);
            label.Location = new Point(0, 0);
            label.BackColor = Color.Transparent;
            label.Parent = pictureBox1;
            if (isSelectedBtn)
                label.ForeColor = Color.White;//Color.FromArgb(243, 116, 33);
            else
                label.ForeColor = m_btnForeColor;
            if (item == null)
                label.Text = "출 고";
            else
                label.Text = item.strPipeName;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.Tag = item;
            label.Cursor = Cursors.Hand;
            label.MouseClick += btn_MouseClick;

            pic.Controls.Add(label);
            pictureBox1.Controls.Add(pic);
        }

        private void pictureBox_begin_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedBtn == null || selectedBtn.Tag == null)
                    throw new ApplicationException("연결할 배관을 선택하세요.");
                                 
                CommonFunction.PipeInfo selectedInfo = null; 
                selectedInfo = (CommonFunction.PipeInfo)selectedBtn.Tag; 
                this.nPipeID = selectedInfo.nPipeID;

                if (MainForm.Instance.commonFunction.ReturnConnectTankIDs(selectedInfo.nPipeID) > 0)
                    throw new ApplicationException("이미 다른 탱크와 작업중입니다.");

                //if (m_isNormal)
                //{
                //    // •작업시 시작될 때 압력 또는 유량의 안정범위는 시작될 당시의 값을 기준으로 작성되는데, 이 옵션이 비율(%)일 경우는 값이 0이면 문제가 됨
                //    // •안정범위가 0~0이 되기 때문에 항상 알람이 발생하기 때문에 이에 대한 회피 방법이 필요함
                //    // •압력 또는 유량이 0이면서 옵션이 비율(%)일 경우에 작업시작 버튼을 누르면 경고문구 메시지 박스를 띄우고 작업은 시작시키지 않는다.
                //    StringBuilder sb = new StringBuilder();
                //    sb.Append("SELECT p.Pressure, Flow, PipeStableType, TankStableType ");
                //    sb.Append("  FROM Pipe as p, Tank as t INNER JOIN AlarmOptions as ao ON t.id=ao.tankid ");
                //    sb.Append(" WHERE p.id= " + this.nPipeID + " AND t.id=" + this.nTankID);
                //    ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                //    if (arrResult == null || arrResult.Count < 4) return;

                //    double nPressure = (arrResult[0].ToString() == "null") ? 0 : Convert.ToDouble(arrResult[0]);
                //    double nFlow = (arrResult[1].ToString() == "null") ? 0 : Convert.ToDouble(arrResult[1]);
                //    double nPipeStableType = DBUtility.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
                //    double nTankStableType = DBUtility.WebDBManager.GetIntField(arrResult[3].ToString(), -1);

                //    string msg = "";
                //    bool isPipeMsg = false;
                //    bool isTankMsg = false;
                //    if ((nPressure == 0 && nPipeStableType == 0) && (nFlow == 0 && nTankStableType == 0))
                //    {
                //        isPipeMsg = true;
                //        isTankMsg = true;
                //        msg = "현재 압력과 유량값이 0입니다.\r이 탱크의 압력, 유량에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r압력과 유량값이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //    }
                //    else if (nPressure == 0 && nPipeStableType == 0)
                //    {
                //        isPipeMsg = true;
                //        msg = "현재 압력값이 0입니다.\r이 탱크의 압력에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r압력값이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //    }
                //    else if (nFlow == 0 && nTankStableType == 0)
                //    {
                //        isTankMsg = true;
                //        if (isPipeMsg)
                //            msg += "\r";
                //        msg += "현재 유량값이 0입니다.\r이 탱크의 유량에 관한 정상범위는 비율(%)로 판단하도록 설정되어있어 안정범위를 산정할 수 없습니다.\r유량이 0보다 큰 값으로 나타날때까지 기다리거나 비율(%)대신 절대값으로 변경해야합니다.";
                //    }

                //    if (isPipeMsg || isTankMsg)
                //    {
                //        msg += "\r절대값으로 변경하시겠습니까?";
                //        if (UnE.Utility.UMessageBox.Show(msg, "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                //            return;

                //        int nCommandID = commonFunction.GetMaxTableID("command") + 1;
                //        int nCommandHistoryID = commonFunction.GetMaxTableID("commandHistory") + 1;
                //        if (isPipeMsg)
                //        {
                //            new EnvironmentPop().SaveOptionSql(nCommandID, nCommandHistoryID, 6, this.nTankID, "PipeStableType", 1);
                //            nCommandID++;
                //            nCommandHistoryID++;
                //        }
                //        if (isTankMsg)
                //        {
                //            new EnvironmentPop().SaveOptionSql(nCommandID, nCommandHistoryID, 6, this.nTankID, "TankStableType", 1);
                //        }
                //    }
                //}

                foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
                {
                    if (item.nTankID == this.nTankID)
                    {
                        this.nStableBeginWorkM = item.nStableBeginWorkM;
                        break;
                    }
                }

                if (UnE.Utility.UMessageBox.Show(this, "작업을 시작하시겠습니까?\r" + "작업 시작 후 " + this.nStableBeginWorkM + "분동안은 알람이 발생하지 않습니다.", "작업 시작", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                    return;

                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
        }

        private void pictureBox_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        Image beginNormalImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Begin_Normal;
        Image beginClickImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Begin_Click;
        Image cancelNormalImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Cancel_Normal;
        Image CancelClickImg = global::KpxPipeMonitoring.Properties.Resources.BeginWorkSelect_Cancel_Click;        
        
        private void pictureBox_begin_MouseEnter(object sender, EventArgs e)
        {
            pictureBox_begin.Image = beginClickImg;
        }

        private void pictureBox_begin_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_begin.Image = beginNormalImg;
        }

        private void pictureBox_cancel_MouseEnter(object sender, EventArgs e)
        {
            pictureBox_cancel.Image = CancelClickImg;
        }

        private void pictureBox_cancel_MouseLeave(object sender, EventArgs e)
        {
            pictureBox_cancel.Image = cancelNormalImg;
        }
    }
}
