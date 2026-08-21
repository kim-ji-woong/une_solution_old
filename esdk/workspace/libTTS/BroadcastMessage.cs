using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libTTS
{
    public class BroadcastMessage
    {
        public enum MesageOption
        {
            STOP = 0,
            PLAY,
            RESUME,
            PAUSE
        }

        protected int mID;
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        protected string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        protected bool bUseSiren;
        public bool UseSiren
        {
            get { return bUseSiren; }
            set { bUseSiren = value; }
        }
        protected MesageOption mplayOption;
        public MesageOption PlayOption
        {
            get { return mplayOption; }
            set { mplayOption = value; }
        }
        protected int mRepeatCount;
        public int RepeatCount
        {
            get { return mRepeatCount; }
            set { mRepeatCount = value; }
        }

        protected DateTime mAddedTime;
        public System.DateTime AddTime
        {
            get { return mAddedTime; }
            set { mAddedTime = value; }
        }
    }
}
