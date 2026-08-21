using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingSystem
{
    public class DisasterStep
    {
        public enum ReactionType
        {
            BEGIN_STATUS = 0,
            RUN_BROADCAST = 10,
            SEND_SMS = 11,

            MALFUNCTION = 21,
            NOTIFY_FIRE = 22,
            IGNORE_FIRE = 23,
            TRAINNING_FIRE = 24,

            RUN_SOP = 30,
            RUN_N_CANCEL_SOP = 31,
            FINISH_SOP = 32,
            IGNORE_SOP = 33,
            END_STATUS = 50,

            BEGIN_PSM_STATUS = 60,
            IGNORE_PSM_DETECT = 61,
            CHANGE_PSM_ALARM_DEPTH = 62,
            NOTIFY_PSM = 63,
            PSM_USER_RESET = 64,
            END_PSM_STATUS = 70,
            ETC = 100,

            NOTIFY_SECURITY = 898,
            BEGIN_S1SVMS_STATUS = 899,
            //NOTIFY_Intrusion_S1 = 900,  // SVMS 침입            
            // NOTIFY_Loiter_S1 = 901,     // SVMS 배회 
            //NOTIFY_Slip_S1 = 902,   // SVMS 쓰러짐 
            // NOTIFY_Steal_S1 = 903,       // SVMS 도난 
            // NOTIFY_Abandoned_S1 = 904,           // SVMS 방치 
            //  NOTIFY_VirtualFence_S1 = 905,      // SVMS 가상펜스 
            //  NOTIFY_Fire_S1 = 906,              // SVMS 화재 
            //  NOTIFY_EmergencyBell_S1 = 907,     // SVMS 비상벨 
            IGNORE_S1SVMS_STATUS = 919,
            END_S1SVMS_STATUS = 920,
            BEGIN_S1ACCESS_STATUS = 921,
            IGNORE_S1ACCESS_STATUS = 939,
            END_S1ACCESS_STATUS = 940
        }

        int id = 0;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        string time = "";

        public string Time
        {
            get { return time; }
            set { time = value; }
        }



        List<ReactionType> reactionList = new List<ReactionType>();
        Dictionary<ReactionType, String> reactionMap = new Dictionary<ReactionType, string>();

        public void addReaction(ReactionType reactionType,string message)
        {
            if(reactionMap.ContainsKey(reactionType))
            {
                reactionMap[reactionType] = message;
            }
            else
                reactionMap.Add(reactionType, message);
        }

        public string getMessage(ReactionType reactionType)
        {
            if (reactionMap.ContainsKey(reactionType))
                return reactionMap[reactionType];

            return "";
        }

        public string getStatusMessage()
        {
            bool isEnded = false;

            foreach(KeyValuePair<ReactionType,String> pair in reactionMap)
            {
                if (pair.Key == ReactionType.MALFUNCTION)
                    return "오작동";
                else if (pair.Key == ReactionType.IGNORE_FIRE)
                    return "신호 무시";

                if (pair.Key == ReactionType.END_STATUS)
                    isEnded = true;
            }

            if (isEnded)
                return "상황 종료";

            return "";
        }
    }
}