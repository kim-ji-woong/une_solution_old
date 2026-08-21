using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertManager.Data
{
    class WeatherData
    {
    }
}

public class DataExpectTemp
{
    private int m_nID = -1;
    private string m_strAnnounceTime = "";
    private string m_strTempAfterOneDay = "";
    private string m_strTempAfterTwoDay = "";
    private string m_strTempAfterThreeDay = "";
    private string m_strTempAfterFourDay = "";
    private string m_strTempAfterFiveDay = "";
    private string m_strTempAfterSixDay = "";

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string AnnounceTime
    {
        get { return m_strAnnounceTime; }
        set { m_strAnnounceTime = value; }
    }

    public string AfterOneDay
    {
        get { return m_strTempAfterOneDay; }
        set { m_strTempAfterOneDay = value; }
    }

    public string AfterTwoDay
    {
        get { return m_strTempAfterTwoDay; }
        set { m_strTempAfterTwoDay = value; }
    }

    public string AfterThreeDay
    {
        get { return m_strTempAfterThreeDay; }
        set { m_strTempAfterThreeDay = value; }
    }

    public string AfterFourDay
    {
        get { return m_strTempAfterFourDay; }
        set { m_strTempAfterFourDay = value; }
    }

    public string AfterFiveDay
    {
        get { return m_strTempAfterFiveDay; }
        set { m_strTempAfterFiveDay = value; }
    }

    public string AfterSixDay
    {
        get { return m_strTempAfterSixDay; }
        set { m_strTempAfterSixDay = value; }
    }
}

public class DataBeforMaxTemp
{
    private int m_nID = -1;
    private string m_strTempBeforeOneDay = "";
    private string m_strTempBeforeTwoDay = "";
    private string m_strTempBeforeThreeDay = "";
    private string m_strTempBeforeFourDay = "";

    public int ID
    {
        get { return m_nID; }
        set { m_nID = value; }
    }

    public string BeforeOneDay
    {
        get { return m_strTempBeforeOneDay; }
        set { m_strTempBeforeOneDay = value; }
    }

    public string BeforeTwoDay
    {
        get { return m_strTempBeforeTwoDay; }
        set { m_strTempBeforeTwoDay = value; }
    }

    public string BeforeThreeDay
    {
        get { return m_strTempBeforeThreeDay; }
        set { m_strTempBeforeThreeDay = value; }
    }

    public string BeforeFourDay
    {
        get { return m_strTempBeforeFourDay; }
        set { m_strTempBeforeFourDay = value; }
    }

}
