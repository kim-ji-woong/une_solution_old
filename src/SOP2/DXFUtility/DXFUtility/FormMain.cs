using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace DXFUtility
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = new WebDBManager();
        private static FormMain m_instance = new FormMain();

        private FormMain()
        {
            InitializeComponent();
        }

        private void btnZoneBoundary_Click(object sender, EventArgs e)
        {
            ZoneBoundaryLoader loader = new ZoneBoundaryLoader();
            loader.Run();
        }

        private void btnBuildingBoundary_Click(object sender, EventArgs e)
        {
            BuildingBoundaryLoader loader = new BuildingBoundaryLoader();
            //loader.Run();
            loader.Run2(@"H:\Project\SOP\2차\도면\소화설비 Zone");
        }

        private void btnBuildingDXF_Click(object sender, EventArgs e)
        {
            DXFDBInput input = new DXFDBInput(@"H:\Project\SOP\2차\도면\소화설비 Zone", m_dbMgr);
            input.Run();
        }

        private void btnDXFToDB_Click(object sender, EventArgs e)
        {
            //DXFDBInput2 input = new DXFDBInput2(@"G:\UnESolution\trunk\bin\SOP\FEData\DXF", m_dbMgr);
            DXFDBInput2 input = new DXFDBInput2(@"C:\Users\김지웅\Documents\Messenger\Received File\기러기", m_dbMgr);
            input.Run();
        }

        private void btnMakeBuildingZone_Click(object sender, EventArgs e)
        {
            //BuildingZoneMaker maker = new BuildingZoneMaker(@"C:\Work\trunk\bin\SOP\FEData\DXF", m_dbMgr);
            //BuildingZoneMaker maker = new BuildingZoneMaker(@"C:\Users\김지웅\Documents\Messenger\Received File\도면 추가 및 수정\추가", m_dbMgr);
            BuildingZoneMaker maker = new BuildingZoneMaker(@"H:\Project\SOP\2차\도면\소화설비 Zone", m_dbMgr);
            maker.Run();
        }

        private void btnCSVToDB_Click(object sender, EventArgs e)
        {
            //CSVDBInput input = new CSVDBInput(@"H:\Project\SOP\소방설비\소방설비_RFID DB 작업_130514", @"H:\Project\SOP\소방설비\소방설비 RFID 테그 정보_소화전_발신기 정보_130515_kjw.xlsx", m_dbMgr);
            //CSVDBInput input = new CSVDBInput(@"H:\Project\SOP\소방설비\소방설비_RFID DB 작업_130516 최종", @"H:\Project\SOP\소방설비\소방설비 RFID 테그 정보_소화전_발신기 정보_130515_kjw.xlsx", m_dbMgr);
            CSVDBInput input = new CSVDBInput(@"H:\Project\SOP\1차(2012.07 ~ 2013.05)\소방설비\소방설비_RFID DB 작업_130607", @"H:\Project\SOP\1차(2012.07 ~ 2013.05)\소방설비\소방설비 RFID 테그 정보_소화기 정보_추가제작_130531_kjw(1차 최종).xlsx", m_dbMgr);
            input.Run();
        }

        private void btnFireEquipmentToDBFinal_Click(object sender, EventArgs e)
        {
            /*OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "fmf Files (*.txt)|*.txt| All Files (*.*)|*.*";
            dlg.Title = "설비ID 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CSVDBInput2 input = new CSVDBInput2(dlg.FileName, m_dbMgr);
                input.Run();
            }*/

            CSVDBInput2 input = new CSVDBInput2("C:\\Users\\김지웅\\Documents\\Messenger\\Received File\\소방설비관리자_FEData", m_dbMgr);
            input.Run();
        }

        private void btnMakeEquipmentZone_Click(object sender, EventArgs e)
        {
            //DXFEquipZoneDBInput equipZone = new DXFEquipZoneDBInput(@"H:\Project\SOP\2차\도면\소화설비 Zone", m_dbMgr);
            DXFEquipZoneDBInput equipZone = new DXFEquipZoneDBInput(@"C:\Work\trunk\bin\SOP\FEData\DXF", m_dbMgr);
            equipZone.Run();
        }

        private void btnUpdateFireEquipmentTemp_Click(object sender, EventArgs e)
        {
            DXFFireEquipmentTemp equipTemp = new DXFFireEquipmentTemp(m_dbMgr);
            equipTemp.Run();
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }
    }
}
