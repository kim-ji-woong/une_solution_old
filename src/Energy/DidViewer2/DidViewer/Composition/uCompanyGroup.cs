using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace DidViewer.Composition
{
    public partial class uCompanyGroup : UserControl
    {
        /// <summary>
        /// SystemPage1 (현장 근로자 현황) 일 때 업체 정보는 
        /// 한번에 8개만 보여줄 수 있는데 8개가 넘어가면 페이지를 나눠서 보여준다
        /// 현재 보여준 마지막 m_companyInfo index
        /// </summary>
        public int ViewCompanyInfoIndex = -1;

        public uCompanyGroup()
        {
            InitializeComponent();

            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.EnableNotifyMessage, true);

            this.Size = new Size(1920, 760);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companies"></param>
        /// <param name="viewIndex"></param>
        /// <param name="lastIndex"></param>
        /// <param name="showPage">잔류가 0이면 업체 페이지 보여주지 않고 건너뛴다</param>
        public void MakePanel(Dictionary<string, Company> companies, int viewIndex, ref int lastIndex)
        {
            ArrayList arr = MakeData(companies);

            this.Controls.Clear();

            if (viewIndex < 0)
                viewIndex += 1; // 마지막 보여준 Index 다음꺼부터 보여줌

            int beginX = 0;
            int beginY = 0;

            int emptyX = 20;
            int emptyY = 40;

            int count = 0;

            //if (viewIndex > arr.Count / 4)
            //{
                int temp = viewIndex / 8; // 8개씩 한 개의 세트이고 arr는 4개가 한 개의 업체임


            //}

            Size formSize = new Size(0, 0);
            int index = temp * 8 * 4; //viewIndex * 4;
            
            
            for (int i = index; i < arr.Count; i+=4) // arr.Count / 4 > arr 4개가 한개의 업체 정보임
            {
                count++;

                ArrayList arr2 = new ArrayList();
                arr2.Add(arr[i]);
                arr2.Add(arr[i + 1]);
                arr2.Add(arr[i + 2]);
                arr2.Add(arr[i + 3]);
                
                uCompany company = new uCompany(arr2);
                company.Location = new Point(beginX, beginY);
                this.Controls.Add(company);

                beginX = beginX += company.Width + emptyX;
                if (count % 4 == 0)
                {
                    beginX = 0;
                    beginY = beginY += company.Height + emptyY;
                    formSize.Height += company.Width;
                }
                else
                    formSize.Width += company.Width;

                if (count == 8) // 최대 8개까지만 나타낼 수 있음
                    break;
            }

            lastIndex = index / 4 + count;
        }

        public ArrayList MakeData(Dictionary<string, Company> companies)
        {
            ArrayList arr = new ArrayList();
            
            foreach (var item in companies)
            {
                int cnt = item.Value.Workers.Where(p => p.InWork).Count();
                if (cnt <= 0)
                    continue;

                arr.Add(item.Value.Name);
                arr.Add(item.Value.Workers[0].Department.Name);
                arr.Add(item.Value.Workers[0].Location.Name);

                int hiCount = 0;
                int byeCount = 0;
                foreach (Worker item2 in item.Value.Workers)
                {
                    if (item2.InWork)
                        hiCount++;
                    else
                        byeCount++;
                }

                arr.Add(hiCount - byeCount);
            }

            return arr;
        }

        public void ClearInfo()
        {
            this.Controls.Clear();
        }
    }

    public class CompanyData
    {
        private string m_strName = "";
        /// <summary>
        /// 업체명
        /// </summary>
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        private string m_strWorkProcess = "";
        /// <summary>
        /// 작업 공정
        /// </summary>
        public string WorkProcess
        {
            get { return m_strWorkProcess; }
            set { m_strWorkProcess = value; }
        }

        private string m_strWorkZone = "";
        /// <summary>
        /// 작업 구역
        /// </summary>
        public string WorkZone
        {
            get { return m_strWorkZone; }
            set { m_strWorkZone = value; }
        }

        private int m_nStayMembers = -1;
        /// <summary>
        /// 잔류 인원
        /// </summary>
        public int StayMembers
        {
            get { return m_nStayMembers; }
            set { m_nStayMembers = value; }
        }
    }
}
