package kr.co.une.kpxwatcher.Data;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;

/**
 * Created by 지웅 on 2017-06-26.
 */

public class Pipe implements Comparable<Pipe> {

    private int m_nID = -1;
    private String m_strName = "";

    // 압력
    private float m_currentPressure = 0.0f;
    private float m_pressureMax = 0.0f;
    private float m_pressureRange = 0.0f;
    private float m_pressureRangeType = 0;
    private float m_pressureRangeOrigin = 0;

    private int m_nStatus = 0;
    private int m_nIgnoreAlarmMinute = 0;
    // 알람무시 시간이 얼마나 남아있는가?
    private String m_strIgnoreElapsedTime = "";
    private String m_strType = "";
    private String m_strLiquid = "";

    // 작업중인가?
    private boolean m_isInWork = false;
    // 연결된 탱크
    private Tank m_LinkedTank;
    // 알람 유무
    private int m_nAlarmID = 0;
    private int m_nAlarmType = 0;
    // 작업시작 시간
    private long m_workBeginTime;
    // 현재 서버 시간
    private long m_serverTime;

    // 연결된 탱크
    public Tank getLinkTank() {return m_LinkedTank;}
    public void setLinkedTank(Tank tank, boolean bSet) {
        if(!bSet){
            if(m_LinkedTank != null)
                m_LinkedTank.setLinkedPipe(this, false);

            m_LinkedTank = null;
        }
        else {
            m_LinkedTank = tank;
            tank.setLinkedPipe(this, true);
        }
    }

    // Pipe ID
    public int getID()
    {
        return m_nID;
    }
    public void setID(int nID)
    {
        m_nID = nID;
    }

    // Pipe Name
    public String getName()
    {
        return m_strName;
    }
    public void setName(String name)
    {
        m_strName = name;
    }

    // 압력
    public void setPressure(float pressure)
    {
        m_currentPressure = pressure;
    }
    public String getPressureString()
    {
        if(m_currentPressure <= -999)
            return "";

        return String.format("%.1f", m_currentPressure);
    }
    public void setPressureMax(float max)
    {
        m_pressureMax = max;
    }
    public void setPressureRange(float range) { m_pressureRange = range; }
    public void setPressureRnageOrigin(float nRange) {m_pressureRangeOrigin = nRange;}
    public String getRangeString()
    {
        if(m_currentPressure <= -999)
            return "";

        float minPressure = m_pressureMax - m_pressureRange;

        if (minPressure < 0.0f) {
            minPressure = 0.0f;
        }

        String strMinPressure = String.format("%.1f", minPressure);
        String strMaxPressure = String.format("%.1f", m_pressureMax);

        return strMinPressure + "~" + strMaxPressure;
    }
    public void setPressureRangeType(int nType) {m_pressureRangeType = nType;}
    public String getPressureRangeString()
    {
        String strRange = String.format("(%.0f", m_pressureRangeOrigin);
        if(m_pressureRangeType == 0) // %
            strRange += "%";
        return strRange + ")";
    }

    // 상태
    public int getStatus()
    {
        return m_nStatus;
    }
    public void setStatus(int status)
    {
        m_nStatus = status;
    }

    // 알람 정보
    public void setAlarmID(int nID) {m_nAlarmID = nID;}
    public int getAlarmID() {return m_nAlarmID;}
    public void setAlarmType(int nType) {m_nAlarmType = nType;}
    public int getAlarmType() {return m_nAlarmType;}

    // 작업중 상태
    public boolean getWork()
    {
        return m_isInWork;
    }
    public void setWork(boolean isInWork)
    {
        m_isInWork = isInWork;
    }

    // 타입
    public String getPipeType()
    {
        return m_strType;
    }
    public void setPipeType(String type)
    {
        m_strType = type;
    }

    // 유종
    public void setLiquid(String strLiquid)
    {
        m_strLiquid = strLiquid;
    }
    public String getLiquid()
    {
        return m_strLiquid;
    }

    // 작업 시작 시간
    public void SetWorkBeginTime(long time) {m_workBeginTime = time;}
    public long GetWorkBeginTime() {return m_workBeginTime;}
    public String GetWorkTime()
    {
        //Calendar now = Calendar.getInstance();

        long due = (m_serverTime - m_workBeginTime) / 1000;
        long h = due / 60 / 60;
        long min = ((long)(due / 60)) % 60;

        //String strTime = String.format("%d시간 %02d분", h, min);
        return String.format("%d,%02d", h, min);
    }

    // 현재 서버 시간
    public void setServerTime(long time) {m_serverTime = time;}

    public int getIgnoreAlarmMinute()
    {
        return m_nIgnoreAlarmMinute;
    }

    public int compareTo(Pipe pipe)
    {
        if (pipe == null)
            return 1;

        return m_strName.compareTo(pipe.m_strName);
    }
}
