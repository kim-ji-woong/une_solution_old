using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS.WeatherDisplay
{
    public interface IWeatherForm
    {
        void UpdateData(List<WeatherSimulator.WeatherData> weatherDatas);
        void Show();
        void Hide();
        void SendStatus();
        bool ApplyBeforeWeatherData();

        bool Visible { get; set; }
    }
}
