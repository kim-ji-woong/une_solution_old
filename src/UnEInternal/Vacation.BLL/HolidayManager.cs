using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace Vacation.BLL
{
    using Models.Vacation;

    public class HolidayManager
    {
        public static ResponseHolidays GetHolidays(int year, string strBaseUrl, string strLicenseKey, List<string> customHolidays)
        {
            string strUrl = strBaseUrl + string.Format("?solYear={0}&numOfRows=60&ServiceKey={1}", year, strLicenseKey);
            return GetHolidays(strUrl, customHolidays);
        }

        public static ResponseHolidays GetHolidays(int year, int month, string strBaseUrl, string strLicenseKey, List<string> customHolidays)
        {
            string strUrl = strBaseUrl + string.Format("?solYear={0}&solMonth={1}&numOfRows=20&ServiceKey={2}", year, month, strLicenseKey);
            return GetHolidays(strUrl, customHolidays);
        }

        private static ResponseHolidays GetHolidays(string strUrl, List<string> customHolidays)
        {
            List<int> holidays = null;

            try
            {
                WebClient client = new WebClient();

                using (Stream data = client.OpenRead(strUrl))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        string strResponse = reader.ReadToEnd();
                        holidays = ParseHolidays(strResponse, customHolidays);

                        reader.Close();
                        data.Close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("GetHolidays Error : " + e.Message);
                return new ResponseHolidays(false, "공휴일 목록 얻어오기 실패");
            }

            ResponseHolidays response = new ResponseHolidays(true, "");

            if (holidays != null)
                response.Holidays = holidays;

            return response;
        }

        private static List<int> ParseHolidays(string strXml, List<string> customHolidays)
        {
            XDocument doc = XDocument.Parse(strXml);
            List<int> holidays = new List<int>();

            IEnumerable<XElement> response = doc.Root.Elements();

            if (response == null)
                return holidays;

            foreach (var element in response)
            {
                if (element.Name.ToString().ToLower() == "body")
                {
                    if (ReadBody(element, holidays))
                    {
                        AddCustomHolidays(holidays, customHolidays);

                        holidays.Sort();
                        return holidays;
                    }
                }
            }

            return holidays;
        }

        private static void AddCustomHolidays(List<int> holidays, List<string> customHolidays)
        {
            if (customHolidays == null || holidays.Count == 0)
                return;

            int firstHoliday = holidays[0];
            int year = firstHoliday / 10000;

            int holiday;

            foreach (string strDay in customHolidays)
            {
                string strHoliday = year.ToString() + strDay;

                if (int.TryParse(strHoliday, out holiday))
                    holidays.Add(holiday);
            }
        }

        private static bool ReadBody(XElement element, List<int> holidays)
        {
            if (element.HasElements)
            {
                foreach (var child in element.Elements())
                {
                    if (child.Name.ToString().ToLower() == "items")
                    {
                        if (child.HasElements)
                        {
                            foreach (var item in child.Elements())
                            {
                                if (ReadItem(item, holidays) == false)
                                    return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static bool ReadItem(XElement item, List<int> holidays)
        {
            if (item.HasElements)
            {
                foreach (var data in item.Elements())
                {
                    string strName = data.Name.ToString().ToLower();

                    if (strName == "isholiday")
                    {
                        if (data.Value.ToString().ToLower() != "y")
                            return true;
                    }
                    else if (strName == "locdate")
                    {
                        int date;

                        if (int.TryParse(data.Value.ToString(), out date))
                        {
                            holidays.Add(date);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
