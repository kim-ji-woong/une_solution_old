using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Common
{
    public class ParameterManager
    {
        public const string ActionStepHistoryID = "lastActionStepHistoryID";
        public const string UserID = "lastUserID";

        private const int Padding = 400000;
        private const int SystemNumber = 26;

        private static readonly char[] OrderArr = { '*', '!', '_', '(', ')' };
        private static readonly char[] PositiveArr = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
        private static readonly char[] NegativeArr = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public const string MissionList = "missionList";
        public const string LastIndex = "lastIndex";
        public const string StepMemberID = "stepMemberID";
        public const string MissionLastViewCount = "missionLastViewCount";

        public static bool SetAccount(ref int ash, ref int uid, HttpSessionStateBase session)
        {
            if (ash > 0)
                session[ParameterManager.ActionStepHistoryID] = ash;
            else
            {
                object id = session[ParameterManager.ActionStepHistoryID];

                if (id != null && id is int)
                    ash = (int)id;
            }

            if (uid > 0)
                session[ParameterManager.UserID] = uid;
            else
            {
                object id = session[ParameterManager.UserID];

                if (id != null && id is int)
                    uid = (int)id;
            }

            return ash > 0 && uid > 0;
        }

        public static bool SetAccount(string ash, string uid, HttpSessionStateBase session, out int nActionStepHistoryID, out int nUserID)
        {
            nActionStepHistoryID = -1;
            nUserID = -1;

            if (ash.Length > 0)
            {
                if (StringToID(ash, out nActionStepHistoryID) == false)
                    return false;

                session[ParameterManager.ActionStepHistoryID] = nActionStepHistoryID;
            }
            else
            {
                object id = session[ParameterManager.ActionStepHistoryID];

                if (id != null && id is int)
                    nActionStepHistoryID = (int)id;
            }

            if (uid.Length > 0)
            {
                if (StringToID(uid, out nUserID) == false)
                    return false;

                session[ParameterManager.UserID] = nUserID;
            }
            else
            {
                object id = session[ParameterManager.UserID];

                if (id != null && id is int)
                    nUserID = (int)id;
            }

            return nActionStepHistoryID > 0 && nUserID > 0;
        }

        /// <summary>
        /// 26진수(알파벳 갯수)를 사용하여 정수값을 문자열로 치환한다.
        /// 첫번째 char : 순서바꿈 char(OrderArr)
        /// 두번째 char : offset. 소문자(정방향), 대문자(역방향)
        ///              1 ~ 26 : 정방향, 27 ~ 52 : 역방향
        /// 세번째부터 : 26진수(Little Endian)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string IDtoString(int id)
        {
            int num = id + Padding;
            int offset = id % (SystemNumber * 2);

            if (offset == 0)
                offset = SystemNumber * 2 - 1;
            else
                offset--;

            int order = id % OrderArr.Length;
            char first = OrderArr[order];
            char second = offset < SystemNumber ? PositiveArr[offset] : NegativeArr[offset - SystemNumber];

            string strValue = first.ToString();
            strValue += second;
            strValue += NumberToString(num, offset, order);

            return strValue;
        }

        public static bool StringToID(string strValue, out int id)
        {
            id = -1;

            if (int.TryParse(strValue, out id))
                return true;

            if (strValue.Length < 3)
                return false;

            char first = strValue.ElementAt(0);
            char second = strValue.ElementAt(1);

            int order = Array.IndexOf(OrderArr, first);

            if (order < 0)
                return false;

            int offset = -1;

            if (PositiveArr.Contains(second))
            {
                offset = Array.IndexOf(PositiveArr, second);
            }
            else if (NegativeArr.Contains(second))
            {
                offset = Array.IndexOf(NegativeArr, second) + SystemNumber;
            }

            if (offset < 0)
                return false;

            int num = StringToNumber(strValue.Substring(2), offset, order);

            if (num < 0)
                return false;

            id = num - Padding;
            return id >= 0;
        }

        private static int StringToNumber(string strValue, int offset, int order)
        {
            int len = strValue.Length;

            while (len <= order)
            {
                order -= len;
            }

            if (order > 0)
            {
                string strHead = strValue.Substring(len - order);
                string strTail = strValue.Substring(0, len - order);
                strValue = strHead + strTail;
            }

            if (offset > SystemNumber)
                offset = SystemNumber - offset;

            int result = 0, multiply = 1;

            for (int i=0;i<len;i++)
            {
                char ch = strValue.ElementAt(i);
                int index = Array.IndexOf(PositiveArr, ch);

                if (index < 0)
                    return -1;

                int num = NumberToIndex(index, -offset);
                result += multiply * num;
                multiply *= SystemNumber;
            }

            return result;
        }

        private static string NumberToString(int num, int offset, int order)
        {
            if (offset > SystemNumber)
                offset = SystemNumber - offset;

            string strValue = "";

            do
            {
                int r = num % SystemNumber;
                int index = NumberToIndex(r, offset);
                char ch = PositiveArr[index];

                strValue += ch;
                num = num / SystemNumber;
            }
            while (num > 0);

            int len = strValue.Length;

            while (len <= order)
            {
                order -= len;
            }

            if (order == 0)
                return strValue;

            string strHead = strValue.Substring(order);
            string strTail = strValue.Substring(0, order);
            return strHead + strTail;
        }

        private static int NumberToIndex(int num, int offset)
        {
            num += offset;

            if (num < 0)
                num += SystemNumber;
            else if (num >= SystemNumber)
                num -= SystemNumber;

            return num;
        }
    }
}