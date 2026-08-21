using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SenarioWeb
{
	public partial class PreSafe : System.Web.UI.Page
	{
        private string m_szSelectedSenairo = String.Empty;

        private List<int> m_nSelectCategories = new List<int>();

        private int m_nLocation = 0;
        private int m_nHeartBeat = 0;
        private int m_nAcc = 0;
        private int m_nAlcohol = 0;
        private int m_nSound = 0;
        private int m_nImpact = 0;

        private bool m_bUseLocation = false;
        private bool m_bUseHeartBeat = false;
        private bool m_bUseAcc = false;
        private bool m_bUseAlcohol = false;
        private bool m_bUseSound = false;
		private bool m_bUseImpact = false;


        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                LoadEBDeviceData();
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GetSenarioList();

            if (!this.Page.IsPostBack)
            {
            }
            else
            {
                //m_szSelectedSenairo = listBoxSenario.SelectedValue;

                //m_szLocation = cmbLocation.SelectedValue;
                //int nHeartRate = 0;
                //if (int.TryParse(txtHeartRate.Text, out nHeartRate))
                //{
                //    m_nHeartRate = nHeartRate;
                //}
                //m_szAcc = cmbAcc.SelectedValue;
                //m_szAlcohol = cmbAlcohol.SelectedValue;
                //m_szScream = cmbScream.SelectedValue;
                //m_szImpact = cmbImpact.SelectedValue;

                //m_bUseLocation = chkLocation.Checked;
                //m_bUseHeartRate = chkHeartRate.Checked;
                //m_bUseAcc = chkAcc.Checked;
                //m_bUseAlcohol = chkAlcohol.Checked;
                //m_bUseScream = chkScream.Checked;
                //m_bUseImpact = chkImpact.Checked;
            }

        }


        private void GetSenarioList()
        {
            m_nSelectCategories.Clear();

            foreach (ListItem item in chkCategory.Items)
            {
                if (item.Selected)
                {
                    m_nSelectCategories.Add(Convert.ToInt32(item.Value));
                }
            }

            m_szSelectedSenairo = listBoxSenario.SelectedValue;

            listBoxSenario.Items.Clear();
            SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe();

            string[] szResult = service.SenarioListForType(m_nSelectCategories.ToArray());

            if (szResult != null)
            {
                foreach (string szItem in szResult)
                {
                    listBoxSenario.Items.Add(szItem);
                }
            }

            if (!String.IsNullOrEmpty(m_szSelectedSenairo))
            {
                foreach (ListItem item in listBoxSenario.Items)
                {
                    if (item.Value == m_szSelectedSenairo)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }


        private void LoadEBDeviceData()
        {
            object[] arr = null;

            using (SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe())
            {
                arr = service.LoadSenarioData("UNES", 1);

                if (arr[4].ToString() == "NULL")
                {
                    m_bUseLocation = false;
                    m_nLocation = 0;

                    lblLocation.Visible = lblLocationValue.Visible = false;
                }
                else
                {
                    m_bUseLocation = true;
                    m_nLocation = Convert.ToInt32(arr[4]);

                    lblLocation.Visible = lblLocationValue.Visible = true;
                    lblLocationValue.Text = (arr[4].ToString() == "0" ? "실내" : "실외");

                }

                if (arr[5].ToString() == "NULL")
                {
                    m_bUseHeartBeat = false;
                    m_nHeartBeat = 0;

                    lblHeartBeat.Visible = lblHeartBeatValue.Visible = false;
                }
                else
                {
                    m_bUseHeartBeat = true;
                    m_nHeartBeat = Convert.ToInt32(arr[5]);

                    lblHeartBeat.Visible = lblHeartBeatValue.Visible = true;
                    lblHeartBeatValue.Text = String.Format("{0} 회/분", arr[5]);
                }

                if (arr[6].ToString() == "NULL")
                {
                    m_bUseAcc = false;
                    m_nAcc = 0;

                    lblAcc.Visible = lblAccValue.Visible = false;
                }
                else
                {
                    m_bUseAcc = true;
                    m_nAcc = Convert.ToInt32(arr[6]);

                    lblAcc.Visible = lblAccValue.Visible = true;
                    lblAccValue.Text = (arr[6].ToString() == "0" ? "정지" : (arr[6].ToString() == "1" ? "걷기" : "뛰기"));
                }

                if (arr[7].ToString() == "NULL")
                {
                    m_bUseAlcohol = false;
                    m_nAlcohol = 0;

                    lblAlcohol.Visible = lblAlcoholValue.Visible = false;
                }
                else
                {
                    m_bUseAlcohol = true;
                    m_nAlcohol = Convert.ToInt32(arr[7]);

                    lblAlcohol.Visible = lblAlcoholValue.Visible = true;
                    lblAlcoholValue.Text = (arr[7].ToString() == "0" ? "유" : "무");
                }

                if (arr[8].ToString() == "NULL")
                {
                    m_bUseSound = false;
                    m_nSound = 0;

                    lblSound.Visible = lblSoundValue.Visible = false;
                }
                else
                {
                    m_bUseSound = true;
                    m_nSound = Convert.ToInt32(arr[8]);

                    lblSound.Visible = lblSoundValue.Visible = true;
                    lblSoundValue.Text = (arr[8].ToString() == "0" ? "유" : "무");
                }

                if (arr[9].ToString() == "NULL")
                {
                    m_bUseImpact = false;
                    m_nImpact = 0;

                    lblImpact.Visible = lblImpactValue.Visible = false;
                }
                else
                {
                    m_bUseImpact = true;
                    m_nImpact = Convert.ToInt32(arr[9]);

                    lblImpact.Visible = lblImpactValue.Visible = true;
                    lblImpactValue.Text = (arr[9].ToString() == "0" ? "유" : "무");
                }


            }
        }


        protected void listBoxSenario_SelectedIndexChanged(object sender, EventArgs e)
		{
            string item = listBoxSenario.SelectedValue;
			if( item != null)
			{
				Label5.Text = "선택된 시나리오 : " + item;
				m_szSelectedSenairo = item;
			}
		}


		protected void Button2_Click(object sender, EventArgs e)
		{
			string szName = TextBoxSearch.Text;
            ListItemCollection col = listBoxSenario.Items;
			foreach (ListItem item in col)
			{
				if (item.Text == szName || item.Text.IndexOf(szName) != -1 || szName.IndexOf(item.Text) != -1)
				{
					item.Selected = true;
					Label5.Text = "선택된 시나리오 : " + item.Text;
					m_szSelectedSenairo = item.Text;
					TextBoxSearch.Text = "";
				}
			}
		}


        protected void Button1_Click(object sender, EventArgs e)
        {
            if (m_szSelectedSenairo == "")
                return;

            //if( m_bUseSoundLevel )
            //{
            //    if( !float.TryParse(tbSoundLevel.Text, out fSoundLevel))
            //    {
            //        m_bUseSoundLevel = false;
            //    }


            //}
            //if(m_bUseSoundLevel)
            //{
            //    lbInput1.Text = string.Format("소리크기 : {0} db", fSoundLevel);
            //}
            //else
            //{
            //    lbInput1.Text = "소리크기 : 사용안함";
            //}



            //if( m_bUseVelocity )
            //{
            //    if( !float.TryParse(tbVelocity.Text, out fVelocity))
            //    {
            //        m_bUseVelocity = false;
            //    }
            //}

            //if (m_bUseVelocity)
            //{
            //    lbInput2.Text = string.Format("속도 : {0} km/h", fVelocity);
            //}
            //else
            //{
            //    lbInput2.Text = "속도 : 사용안함";
            //}


            //if( m_bUseHeartBeat )
            //{
            //    if( !int.TryParse(tbHeartBeat.Text, out nHeartBeat))
            //    {
            //        m_bUseHeartBeat = false;
            //    }
            //}

            //if (m_bUseHeartBeat)
            //{
            //    lbInput3.Text = string.Format("맥박 : {0} 회/분", nHeartBeat);
            //}
            //else
            //{
            //    lbInput3.Text = "맥박 : 사용안함";
            //}

            //if( m_bUseAlcole )
            //{
            //    if( !float.TryParse(tbAlcole.Text , out fAlcole))
            //    {
            //        m_bUseAlcole = false;
            //    }
            //}

            //if (m_bUseAlcole)
            //{
            //    lbInput4.Text = string.Format("알콜수치 : {0} %", fAlcole);
            //}
            //else
            //{
            //    lbInput4.Text = "알콜수치 : 사용안함";
            //}

            //if( m_bUseAcc )
            //{
            //    if (!float.TryParse(tbACC.Text, out fAcc))
            //    {
            //        m_bUseAcc = false;
            //    }
            //}

            //if (m_bUseAcc)
            //{
            //    lbInput5.Text = string.Format("가속도 : {0} m/sec^2", fAcc);
            //}
            //else
            //{
            //    lbInput5.Text = "가속도 : 사용안함";
            //}

            //if (m_bUseLocation)
            //{
            //    if (!int.TryParse(m_szLocation, out m_nLocationType))
            //    {
            //        m_bUseLocation = false;
            //    }
            //}

            //if (m_bUseLocation)
            //{
            //    switch(m_nLocationType)
            //    {
            //        case 1:
            //            lbInput6.Text = string.Format("위치 : {0}", "집");
            //            break;
            //        case 2:
            //            lbInput6.Text = string.Format("위치 : {0}", "직장");
            //            break;						
            //        case 3:
            //            lbInput6.Text = string.Format("위치 : {0}", "외곽지");
            //            break;						
            //        case 4:
            //            lbInput6.Text = string.Format("위치 : {0}", "접근금지구역");
            //            break;
            //        case 5:
            //            lbInput6.Text = string.Format("위치 : {0}", "기타");
            //            break;
            //    }				
            //}
            //else
            //{
            //    lbInput6.Text = "위치 : 사용안함";
            //}

            //if (m_bUseImpact)
            //{
            //    if (!bool.TryParse(m_szImpact, out m_bImpact))
            //    {
            //        m_bUseImpact = false;
            //    }
            //}
            //if (m_bUseImpact)
            //{
            //    if (m_bImpact == true)
            //    {
            //        lbInput7.Text = string.Format("충격여부 : {0}", "충격있음");
            //    }
            //    else
            //    {
            //        lbInput7.Text = string.Format("충격여부 : {0}", "충격없음");

            //    }

            //}
            //else
            //{
            //    lbInput7.Text = "충격여부 : 사용안함";
            //}


            LoadEBDeviceData();

            using (SenarioWeb.SenarioService.PreSafe service = new SenarioWeb.SenarioService.PreSafe())
            {
                //object[] arr = service.LoadSenarioData("UNES", 1);

                //if (arr[4].ToString() == "NULL")
                //{
                //    m_bUseLocation = false;
                //    m_nLocation = 0;
                //}
                //else
                //{
                //    m_bUseLocation = true;
                //    m_nLocation = Convert.ToInt32(arr[4]);
                //}

                //if (arr[5].ToString() == "NULL")
                //{
                //    m_bUseHeartBeat = false;
                //    m_nHeartBeat = 0;
                //}
                //else
                //{
                //    m_bUseHeartBeat = true;
                //    m_nHeartBeat = Convert.ToInt32(arr[5]);
                //}

                //if (arr[6].ToString() == "NULL")
                //{
                //    m_bUseAcc = false;
                //    m_nAcc = 0;
                //}
                //else
                //{
                //    m_bUseAcc = true;
                //    m_nAcc = Convert.ToInt32(arr[6]);
                //}

                //if (arr[7].ToString() == "NULL")
                //{
                //    m_bUseAlcohol = false;
                //    m_nAlcohol = 0;
                //}
                //else
                //{
                //    m_bUseAlcohol = true;
                //    m_nAlcohol = Convert.ToInt32(arr[7]);
                //}

                //if (arr[8].ToString() == "NULL")
                //{
                //    m_bUseSound = false;
                //    m_nSound = 0;
                //}
                //else
                //{
                //    m_bUseSound = true;
                //    m_nSound = Convert.ToInt32(arr[8]);
                //}

                //if (arr[9].ToString() == "NULL")
                //{
                //    m_bUseImpact = false;
                //    m_nImpact = 0;
                //}
                //else
                //{
                //    m_bUseImpact = true;
                //    m_nImpact = Convert.ToInt32(arr[9]);
                //}

                string[] szResult = service.RunSenario2(m_szSelectedSenairo,
                 m_bUseLocation, m_nLocation,
                 m_bUseHeartBeat, m_nHeartBeat,
                 m_bUseAcc, m_nAcc,
                 m_bUseAlcohol, m_nAlcohol,
                 m_bUseSound, m_nSound,
                 m_bUseImpact, m_nImpact);
                Label2.Text = szResult[2] + "점";
            }
        }

		
		protected void ListBox1_TextChanged(object sender, EventArgs e)
		{
			int i = 0;
			i++;
		}


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"./PreSafeInputData.aspx");
        }


        protected void Timer_Tick(object sender, EventArgs e)
        {
            LoadEBDeviceData();
        }

	}
}