using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Network
{
    public abstract class Message
    {
        public abstract int GetHeader();
        public abstract byte[] GetBytes();

        public virtual bool SendToSOPSimulator()
        {
            return false;
        }

        public virtual bool SendToEtc()
        {
            return false;
        }

        public virtual bool SendToFire()
        {
            return false;
        }
    }
}