using System;
using System.Windows.Forms;

namespace WeatherMaster
{
    public partial class FormMain : Form
    {
        //private KrWeatherReader m_krReader = new KrWeatherReader();
        private CityReader m_cityReader = new CityReader();
        private SpecialReportReader m_reportReader = new SpecialReportReader();

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            //m_krReader.ReadData();
            m_cityReader.ReadData();
        }

        private void btnSpecialReport_Click(object sender, EventArgs e)
        {
            m_reportReader.ReadData();
        }
    }
}
