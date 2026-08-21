using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnE
{
    namespace SOP
    {
        /// <summary>
        /// ProcessSection가 실행될때 사용되는 Form과 ProcessSection의  Message Interface
        /// </summary>
        public interface IAnnounceMessage
        {
            string Message
            {
                get;
                set;
            }
            int Count
            {
                get;
                set;
            }
            bool UseSenarioMessage
            {
                get;
                set;
            }

            bool UseSystemMessage
            {
                get;
                set;
            }
            string SystemMessage
            {
                get;
                set;
            }
            string SenarioMessage
            {
                get;
                set;
            }
            bool UseSiren
            {
                get;
                set;
            }
        }
    }
}