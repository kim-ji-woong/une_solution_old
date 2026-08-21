using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JubixNetwork
{
    public interface IJubixNetwork
    {
        JubixClientProvider ClientProvider
        {
            get;
        }

        int SiteID
        {
            get;
            set;
        }


        bool ShutdownSensorThread
        {
            get;
            set;
        }

        void RecvLog(byte[] bytes);

        void SensorRecvLog(byte[] bytes);
              
        int Send(byte[] bytes, JubixClientProvider provider);

        int Send_NoLengthByte(byte[] bytes, JubixClientProvider provider);
     
        void ReleaseThread();

        void OnDropConnection();

    }
}
