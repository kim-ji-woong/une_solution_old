using System;
using System.Collections.Generic;

namespace Vacation.Model
{
    public class Date : IComparable
    {
        // Normal : 하루 휴가
        // AM : 오전 반차
        // PM : 오후 반차
        public enum DateTypes { Normal = 1, AM, PM }

        public const int Quater1st = 4;
        public const int Quater2nd = 8;
        public const int Quater3rd = 16;
        public const int Quater4th = 32;
        public const int AllDay = Date.Quater1st | Date.Quater2nd | Date.Quater3rd | Date.Quater4th;

        private int m_nYear = -1;
        private int m_nMonth = -1;
        private int m_nDay = -1;
        private int m_nDateType = (int)DateTypes.Normal;
        //private DateType m_type = DateType.Normal;

        public int Year
        {
            get { return m_nYear; }
            set { m_nYear = value; }
        }

        public int Month
        {
            get { return m_nMonth; }
            set { m_nMonth = value; }
        }

        public int Day
        {
            get { return m_nDay; }
            set { m_nDay = value; }
        }

        public int DateType
        {
            get { return m_nDateType; }
            set { m_nDateType = value; }
        }
        /*public DateType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }*/

        public Date()
        {
        }

        public Date(Date date)
        {
            this.m_nYear = date.m_nYear;
            this.m_nMonth = date.m_nMonth;
            this.m_nDay = date.m_nDay;
            this.m_nDateType = date.m_nDateType;
        }

        public static string DateListToString(List<Date> dates)
        {
            string strDateList = "";

            foreach (Date date in dates)
            {
                string strDate = string.Format("{0}{1:00}", date.Month, date.Day);

                if (date.DateType != (int)DateTypes.Normal && date.DateType != AllDay)
                    strDate += ":" + date.DateType.ToString();
                /*if (date.Type == Date.DateType.AM || date.Type == DateType.PM)
                    strDate += ":" + ((int)date.Type).ToString();*/

                if (strDateList.Length == 0)
                    strDateList = strDate;
                else
                    strDateList += " " + strDate;
            }

            return strDateList;
        }

        public static List<Date> StringToDateList(string strDates, int year)
        {
            List<Date> dates = new List<Date>();
            string[] tokens = strDates.Trim().Split(' ');

            int month, day;
            int prev = 0;

            foreach (string strToken in tokens)
            {
                if (strToken.Length == 0)
                    continue;

                string strDate = strToken;
                int dateType = AllDay;
                //DateType type = DateType.Normal;

                int nIndex = strToken.LastIndexOf(':');

                if (nIndex > 0)
                {
                    string strType = strToken.Substring(nIndex + 1).Trim();

                    if (int.TryParse(strType, out dateType))
                    {
                        if (dateType == (int)DateTypes.Normal)
                            dateType = AllDay;
                        else if (dateType == (int)DateTypes.AM)
                            dateType = (Quater1st | Quater2nd);
                        else if (dateType == (int)DateTypes.PM)
                            dateType = (Quater3rd | Quater4th);
                    }

                    /*if (strType == ((int)DateType.AM).ToString())
                        type = DateType.AM;
                    else if (strType == ((int)DateType.PM).ToString())
                        type = DateType.PM;
                    else if (strType == ((int)DateType.Normal).ToString())
                        type = DateType.Normal;*/

                    strDate = strToken.Substring(0, nIndex).Trim();
                }

                string strDay = strDate.Substring(strDate.Length - 2);
                string strMonth = strDate.Substring(0, strDate.Length - 2);

                if (int.TryParse(strMonth, out month) == false ||
                    int.TryParse(strDay, out day) == false)
                    continue;

                Date date = new Date();
                date.Year = year;
                date.Month = month;
                date.Day = day;
                date.DateType = dateType;

                int dateNumber = year * 10000 + month * 100 + day;

                if (dateNumber < prev)
                {
                    year++;
                    date.Year = year;
                    prev = year * 10000 + month * 100 + day;
                }
                else
                    prev = dateNumber;

                dates.Add(date);
            }

            return dates;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is Date)
            {
                Date date1 = this;
                Date date2 = (Date)obj;

                if (date1.Year < date2.Year)
                    return -1;
                else if (date1.Year > date2.Year)
                    return 1;
                else
                {
                    if (date1.Month < date2.Month)
                        return -1;
                    else if (date1.Month > date2.Month)
                        return 1;
                    else
                    {
                        if (date1.Day < date2.Day)
                            return -1;
                        else if (date1.Day > date2.Day)
                            return 1;
                        else
                        {
                            if (date1.DateType == (int)DateTypes.Normal || date1.DateType == AllDay)
                            {
                                if (date2.DateType != (int)DateTypes.Normal && date2.DateType != AllDay)
                                    return 1;
                            }
                            else
                            {
                                int dateType1 = GetDateType(date1.DateType);
                                int dateType2 = GetDateType(date2.DateType);

                                if (dateType2 == AllDay)
                                    return -1;
                                else
                                    return dateType1.CompareTo(dateType2);
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private int GetDateType(int dateType)
        {
            if (dateType == (int)DateTypes.AM)
                return Quater1st | Quater2nd;
            else if (dateType == (int)DateTypes.PM)
                return Quater3rd | Quater4th;
            else if (dateType == (int)DateTypes.Normal)
                return AllDay;

            return dateType;
        }

        public static string GetDateTypeString(int dateType, string strTag = "")
        {
            if (dateType == (int)Date.DateTypes.AM)
                return "(오전" + strTag + ")";
            else if (dateType == (int)Date.DateTypes.PM)
                return "(오후" + strTag + ")";
            else if (dateType == (int)Date.DateTypes.Normal)
                return "";

            if ((dateType & Date.Quater1st) == Date.Quater1st)
            {
                if ((dateType & Date.Quater2nd) == Date.Quater2nd)
                {
                    if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "";
                        }
                        else
                        {
                            return "(1Q,2Q,3Q)";
                        }
                    }
                    else
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(1Q,2Q,4Q)";
                        }
                        else
                        {
                            return "(오전" + strTag + ")";
                        }
                    }
                }
                else
                {
                    if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(1Q,3Q,4Q)";
                        }
                        else
                        {
                            return "(1Q,3Q)";
                        }
                    }
                    else
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(1Q,4Q)";
                        }
                        else
                        {
                            return "(1Q)";
                        }
                    }
                }
            }
            else
            {
                if ((dateType & Date.Quater2nd) == Date.Quater2nd)
                {
                    if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(2Q,3Q,4Q)";
                        }
                        else
                        {
                            return "(2Q,3Q)";
                        }
                    }
                    else
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(2Q,4Q)";
                        }
                        else
                        {
                            return "(2Q)";
                        }
                    }
                }
                else
                {
                    if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(오후" + strTag + ")";
                        }
                        else
                        {
                            return "(3Q)";
                        }
                    }
                    else
                    {
                        if ((dateType & Date.Quater4th) == Date.Quater4th)
                        {
                            return "(4Q)";
                        }
                        /*else {
                            return "";
                        }*/
                    }
                }
            }

            return "";
        }

        public static bool BeforeNoon(int dateType)
        {
            if (dateType == (int)Date.DateTypes.AM)
                return true;
            else if ((dateType & Date.Quater1st) == Date.Quater1st)
                return true;
            else if ((dateType & Date.Quater2nd) == Date.Quater2nd)
                return true;

            return false;
        }

        public static float GetDateCount(int dateType)
        {
            if (dateType == (int)Date.DateTypes.AM || dateType == (int)Date.DateTypes.PM)
                return 0.5f;
            else if (dateType == (int)Date.DateTypes.Normal)
                return 1;

            float fCount = 0.0f;

            if ((dateType & Date.Quater1st) == Date.Quater1st)
                fCount += 0.25f;
            if ((dateType & Date.Quater2nd) == Date.Quater2nd)
                fCount += 0.25f;
            if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                fCount += 0.25f;
            if ((dateType & Date.Quater4th) == Date.Quater4th)
                fCount += 0.25f;

            return fCount;
        }

        public static bool IsFullDay(int dateType)
        {
            if (dateType == (int)Date.DateTypes.Normal || dateType == Date.AllDay)
                return true;

            return false;
        }

        public static bool IsContinuous(int dateType)
        {
            if (dateType == (int)Date.DateTypes.Normal || dateType == (int)Date.DateTypes.AM || dateType == (int)Date.DateTypes.PM)
                return true;

            if ((dateType & Date.Quater1st) == Date.Quater1st)
            {
                if ((dateType & Date.Quater2nd) == Date.Quater2nd)
                {
                    if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                        return true;
                    else if ((dateType & Date.Quater4th) == Date.Quater4th)
                        return false;
                    else
                        return true;
                }
                else if ((dateType & Date.Quater3rd) == Date.Quater3rd || (dateType & Date.Quater4th) == Date.Quater4th)
                    return false;
                else
                    return true;
            }
            else if ((dateType & Date.Quater2nd) == Date.Quater2nd)
            {
                if ((dateType & Date.Quater3rd) == Date.Quater3rd)
                    return true;
                else if ((dateType & Date.Quater4th) == Date.Quater4th)
                    return false;
                else
                    return true;
            }

            return true;
        }

        public static bool IsContinuous(int dateType1, int dateType2)
        {
            if (dateType1 == (int)Date.DateTypes.AM)
            {
                if (dateType2 == (int)Date.DateTypes.PM || (dateType2 & Date.Quater3rd) == Date.Quater3rd)
                    return true;
            }
            else if ((dateType1 & Date.Quater1st) == Date.Quater1st)
            {
                if ((dateType2 & Date.Quater2nd) == Date.Quater2nd)
                    return true;
            }
            else if ((dateType1 & Date.Quater2nd) == Date.Quater2nd)
            {
                if (dateType2 == (int)Date.DateTypes.PM || (dateType2 & Date.Quater3rd) == Date.Quater3rd)
                    return true;
            }
            else if ((dateType1 & Date.Quater3rd) == Date.Quater3rd)
            {
                if ((dateType2 & Date.Quater4th) == Date.Quater4th)
                    return true;
            }

            return false;
        }
    }
}
