using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisisAlertServer.Data
{
    class WeatherData
    {
    }
}

public class DataMidTemp
{
    private int m_nID = -1;
    private string m_strAnnounceTime = "";
    private string m_strTempAfterOneDay = "";
    private string m_strTempAfterTwoDay = "";

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
}

public class DataMidWeather
{
    private string m_strAnnounceTime = "";
    private string m_strNumEf = "";
    private string m_strTa = "";

    public string AnnounceTime
    {
        get { return m_strAnnounceTime; }
        set { m_strAnnounceTime = value; }
    }

    public string NumEf
    {
        get { return m_strNumEf; }
        set { m_strNumEf = value; }
    }

    public string Ta
    {
        get { return m_strTa; }
        set { m_strTa = value; }
    }
}

public class DataLongWeather
{
    private string m_strAnnounceTime = "";
    private string m_strtaMax3 = "";
    private string m_strtaMax4 = "";
    private string m_strtaMax5 = "";
    private string m_strtaMax6 = "";
    private string m_strtaMax7 = "";
    private string m_strtaMax8 = "";
    private string m_strtaMax9 = "";
    private string m_strtaMax10 = "";

    public string AnnounceTime
    {
        get { return m_strAnnounceTime; }
        set { m_strAnnounceTime = value; }
    }

    public string TaMax3
    {
        get { return m_strtaMax3; }
        set { m_strtaMax3 = value; }
    }

    public string TaMax4
    {
        get { return m_strtaMax4; }
        set { m_strtaMax4 = value; }
    }

    public string TaMax5
    {
        get { return m_strtaMax5; }
        set { m_strtaMax5 = value; }
    }

    public string TaMax6
    {
        get { return m_strtaMax6; }
        set { m_strtaMax6 = value; }
    }

    public string TaMax7
    {
        get { return m_strtaMax7; }
        set { m_strtaMax7 = value; }
    }

    public string TaMax8
    {
        get { return m_strtaMax8; }
        set { m_strtaMax8 = value; }
    }

    public string TaMax9
    {
        get { return m_strtaMax9; }
        set { m_strtaMax9 = value; }
    }

    public string TaMax10
    {
        get { return m_strtaMax10; }
        set { m_strtaMax10 = value; }
    }
}

public class DataLongTemp
{
    private int m_nID = -1;
    private string m_strAnnounceTime = "";
    private string m_strTempAfterThreeDay = "";
    private string m_strTempAfterFourDay = "";
    private string m_strTempAfterFiveDay = "";
    private string m_strTempAfterSixDay = "";

    private string m_strTempAfterSevenDay = "";
    private string m_strTempAfterEightDay = "";
    private string m_strTempAfterNineDay = "";
    private string m_strTempAfterTenDay = "";

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


    public string AfterSevenDay
    {
        get { return m_strTempAfterSevenDay; }
        set { m_strTempAfterSevenDay = value; }
    }

    public string AfterEightDay
    {
        get { return m_strTempAfterEightDay; }
        set { m_strTempAfterEightDay = value; }
    }

    public string AfterNineDay
    {
        get { return m_strTempAfterNineDay; }
        set { m_strTempAfterNineDay = value; }
    }

    public string AfterTenDay
    {
        get { return m_strTempAfterTenDay; }
        set { m_strTempAfterTenDay = value; }
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