using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;
using libSensorProcess;

namespace SDMS.PopupDialog
{
    public partial class FormNotice : Form
    {
        public List<ProcessIF> m_noticeListItem = new List<ProcessIF>();
        public Dictionary<ProcessIF, int> m_dicCountUpNoticeList = new Dictionary<ProcessIF, int>();
        private readonly int m_panelWidthLength = 340;
        private readonly int m_panelHeightLength = 40;
        private readonly int m_lineHeightLength = 4;

        public ProcessIF m_selectedItem { get; set; }

        public delegate void ChgSensorDectect(ProcessIF process);
        public event ChgSensorDectect chgSensorDectect;

        public FormNotice()
        {
            InitializeComponent();

            //this.AutoScrollMinSize = new Size(m_panelWidthLength + 10, 300);
        }

        public void RefreshList()
        {
            this.Controls.Clear();
            
            int curPanelHeight = 0;
             
            for (int i = 0; i < m_noticeListItem.Count; i++)
            {
                Panel panel = new Panel();
                panel.Location = new Point(5, curPanelHeight);
                panel.Size = new System.Drawing.Size(m_panelWidthLength, m_panelHeightLength);
                panel.BackColor = Color.FromArgb(0xc7, 0xc6, 0xc6);
                panel.Tag = m_noticeListItem[i];
                curPanelHeight += m_panelHeightLength;

                Label label = new Label();
                if (m_noticeListItem[i].ProcessType == ProcessType.FireAlarm)
                    label.Text = "[화재]";
                else if (m_noticeListItem[i].ProcessType == ProcessType.PSMAlarm)
                    label.Text = "[누출]";
                else if (m_noticeListItem[i].ProcessType == ProcessType.SecurityAlarm)
                    label.Text = "[방범]";
                else
                    label.Text = "[기타]";
                label.Location = new Point(7, 10);
                label.Font = new System.Drawing.Font(Program.prgFont, 13F);          
                label.Size = new System.Drawing.Size(57, 20);
                panel.Controls.Add(label);

                Label label1 = new Label();
                label1.Text = m_noticeListItem[i].ToString();
                label1.Location = new Point(60, 3);
                label1.Font = new System.Drawing.Font(Program.prgFont, 9F);      
                label1.Size = new System.Drawing.Size(229, 14);
                panel.Controls.Add(label1);

                Label label2 = new Label();
                //label2.Text = GetFacilityTypeStr(m_noticeListItem[i].TargetSensor.Type);

                label2.Text = SOPServer.EventTypeString.GetEventTypeDetectString(Convert.ToInt32(m_noticeListItem[i].TargetSensor.Type));

                label2.Location = new Point(60, 17);
                label2.Font = new System.Drawing.Font(Program.prgFont, 13F);           
                label2.Size = new System.Drawing.Size(229, 20);
                panel.Controls.Add(label2);

                Button btn = new Button();
                btn.Size = new Size(35, 35);
                btn.Location = new Point(300, 2); 
                btn.Image = global::SDMS.Properties.Resources.Notice_Power_Normal;
                btn.Tag = m_noticeListItem[i];
                btn.Click += btn_Click;
                panel.Controls.Add(btn);

                this.Controls.Add(panel);

                if (m_noticeListItem.Count - 1 > i)
                {
                    PictureBox picLine = new PictureBox();
                    picLine.Size = new System.Drawing.Size(m_panelWidthLength, m_lineHeightLength);
                    Bitmap bmLine = new Bitmap(global::SDMS.Properties.Resources.Notice_Line);
                    picLine.Image = (Image)bmLine;
                    picLine.SizeMode = PictureBoxSizeMode.StretchImage;
                    picLine.Location = new Point(0, curPanelHeight);
                    this.Controls.Add(picLine);
                    curPanelHeight += m_lineHeightLength; 
                }
            } 
            if (curPanelHeight >= 264)
            {
                this.Size = new Size(m_panelWidthLength + 30, 264);
                this.AutoScroll = true; 
                this.VScroll = true; 
                this.HScroll = false;
            }
            else
            {
                this.Size = new Size(m_panelWidthLength + 10, curPanelHeight);
                this.AutoScroll = false;
            }

            ChangeNotice();
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            this.m_selectedItem = (ProcessIF)btn.Tag;
            ChangeNotice();

            if (chgSensorDectect != null)
                chgSensorDectect(this.m_selectedItem);
        }

        public void ChangeNotice()
        {
            if (m_selectedItem == null) return;

            foreach (Control item in this.Controls)
            {
                if (item is Panel)
                {  
                    foreach (Control item2 in item.Controls)
                    {
                        if (item2 is Button)
                        {
                            if (item2.Tag == m_selectedItem)
                            {
                                ((Button)item2).Image = global::SDMS.Properties.Resources.Notice_Power_Click;
                                item.Focus();
                            }
                            else
                            {
                                ((Button)item2).Image = global::SDMS.Properties.Resources.Notice_Power_Normal;
                                
                            }
                        }
                        if (item2 is Label)
                        {
                            if (item.Tag != null && m_dicCountUpNoticeList.ContainsKey((ProcessIF)item.Tag))
                            {
                                int count = m_dicCountUpNoticeList[(ProcessIF)item.Tag];
                                if (count > 1) item.ForeColor = Color.Black;
                                else item.ForeColor = Color.Red;
                            }
                            else
                            {
                                item.ForeColor = Color.Red;
                            }
                        }
                    }                    
                }
            }
        }
        public void CountUpNotice(ProcessIF process)
        {
            if (m_dicCountUpNoticeList.ContainsKey(process))
                m_dicCountUpNoticeList[process] = m_dicCountUpNoticeList[process] + 1;
            else
                m_dicCountUpNoticeList.Add(process, 1); 
        } 
        public string GetFacilityTypeStr(UnE.Sensor.IFacility.FacilityType type)
        {
            string typeStr = string.Empty;

            switch (type)
            {
                case IFacility.FacilityType.FIRE_SENSOR: typeStr = "화재탐지센서"; break;       // 화재탐지센서(100번 ~ 199번)
                case IFacility.FacilityType.COOLER_SENSOR: typeStr = "스프링쿨러"; break;// 스프링쿨러
                case IFacility.FacilityType.PRESSURE_SENSOR: typeStr = "펌프압력센서"; break;    // 펌프압력센서
                case IFacility.FacilityType.CCTV: typeStr = "CCTV"; break;
                case IFacility.FacilityType.FE: typeStr = "소화기"; break;                 // 소화기(Fire Extinguisher)
                case IFacility.FacilityType.HD: typeStr = "소화전"; break;                 // 소화전(Hydrant)
                case IFacility.FacilityType.FA: typeStr = "발신기"; break;                 // 발신기(Fire Alarm)
                case IFacility.FacilityType.FR: typeStr = "수신반"; break;                 // 수신반(Fire Receiver)
                case IFacility.FacilityType.PSM_SENSOR: typeStr = "유해화학물질 누출감지 센서"; break;        // 유해화학물질 누출감지 센서
                case IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT: typeStr = "방재장비"; break; // 방재장비
                case IFacility.FacilityType.FireSensor_TypeA: typeStr = "화재감지기 A"; break;             // 화재감지기 A
                case IFacility.FacilityType.FireSensor_TypeB: typeStr = "화재감지기 B"; break;             // 화재감지기 B
                case IFacility.FacilityType.FireSensor_GasEmission: typeStr = "가스 방출신호"; break;       // 가스 방출신호
                case IFacility.FacilityType.FireSensor_ManualControl: typeStr = "수동조작함 신호"; break;     // 수동조작함 신호
                case IFacility.FacilityType.FireSensor_LightType: typeStr = "광선식"; break;        // 광선식
                case IFacility.FacilityType.FireSensor_SiemensType: typeStr = "지멘스 자탐"; break;       // 지멘스 자탐
                case IFacility.FacilityType.FireSensor_Monitoring: typeStr = "감시"; break;// 감시
                case IFacility.FacilityType.FireSensor_SensingLine: typeStr = "감지선"; break;// 감지선
                case IFacility.FacilityType.FireSensor_AnalogSmokeType: typeStr = "아날로그식 연기"; break;// 아날로그식 연기
                case IFacility.FacilityType.FireSensor_MonitoringType: typeStr = "감시센서"; break;// 감시센서 
                case IFacility.FacilityType.Security_Sensor: typeStr = "방범센서"; break;// 방범센서 
                case IFacility.FacilityType.Intrusion_S1: typeStr = "침입"; break;// SVMS 침입
                case IFacility.FacilityType.Loiter_S1: typeStr = "배회"; break;// SVMS 배회
                case IFacility.FacilityType.Collapse_S1: typeStr = "쓰러짐"; break;// SVMS 쓰러짐
                case IFacility.FacilityType.Theft_S1: typeStr = "도난"; break;// SVMS 도난
                case IFacility.FacilityType.Neglect_S1: typeStr = "방치"; break;// SVMS 방치
                case IFacility.FacilityType.VirtualFence_S1: typeStr = "가상펜스"; break;// SVMS 가상펜스
                case IFacility.FacilityType.Fire_S1: typeStr = "화재"; break;// SVMS 화재
                case IFacility.FacilityType.EmergencyBell_S1: typeStr = "비상벨"; break;// SVMS 비상벨
                case IFacility.FacilityType.GeneralIntrusionT1_S1: typeStr = "일반 침입1"; break;// S1Access 일반침입1
                case IFacility.FacilityType.GeneralIntrusionT2_S1: typeStr = "일반 침입2"; break;// S1Access 일반 침입2
                case IFacility.FacilityType.InternalIntrusionT3_S1: typeStr = "내부 침입"; break;// S1Access 내부침입
                case IFacility.FacilityType.VaultIntrusionT4_S1: typeStr = "금고 침입"; break;// S1Access 금고침입
                case IFacility.FacilityType.FireF1_S1: typeStr = "화재"; break;// S1Access 화재
                case IFacility.FacilityType.CustomerEmergencyC1_S1: typeStr = "여자화장실 비상벨"; break;// S1Access 고객비상
                case IFacility.FacilityType.CustomerEmergencyC2_S1: typeStr = "여자화장실 비상벨"; break;// S1Access 고객 비상
                case IFacility.FacilityType.RescueQQ_S1: typeStr = "구급"; break;// S1Access 구급
                case IFacility.FacilityType.GasG1_S1: typeStr = "가스"; break;// S1Access 가스
                case IFacility.FacilityType.BlackoutAbnormalityU1_S1: typeStr = "정전이상"; break;// S1Access 정전이상
                case IFacility.FacilityType.LeakAbnormalityU4_S1: typeStr = "누수이상"; break;// S1Access 누수이상
                case IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1: typeStr = "종합경보반 이상"; break;// S1Access 종합경보반 이상
                case IFacility.FacilityType.ExternalAlarmBell: typeStr = "외부 비상벨"; break;// 외부 비상벨
                default: typeStr = "기타"; break;
            }

            return typeStr;
        }
    } 
}
