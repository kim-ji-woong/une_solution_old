package kr.co.une.kpxwatcher.Data;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Iterator;
import java.util.List;

/**
 * Created by 지웅 on 2017-06-26.
 */

public class Tank implements Comparable<Tank> {
    private int m_nID = -1;
    private String m_strName = "";
    // 유종
    private String m_strLiquidType = "";
    // 레벨
    private double m_dLevel = 0.0;
    // 온도
    private double m_dTemp = 0.0;
    // 비중
    private double m_dGravity = 0.0;
    // 수량
    private double m_dMass = 0.0;
    // 유량
    private double m_dFlow = 0.0;
    private float m_fStdFlow = 0;
    private float m_fFlowMax = 0;
    private float m_fFlowRange = 0;
    private int m_nFlowRangeType = 0;
    private float m_fFlowRangeOrigin = 0;
    // 알람상태
    private int m_nStatus = 0;
    // 용량(KL)
    private double m_dCapacity = 0.0;
    // Tank Type : C(Carbon), S(SUS)
    private String m_strTankType = "";
    private double m_dHighLevel = 0.0;
    private double m_dMinTemp = 0.0;
    private double m_dMaxTemp = 0.0;
    private int m_nAlarmID1 = 0;
    private int m_nAlarmID2 = 0;
    // 연결된 배관
    private List<Pipe> m_LinkedPipe = new ArrayList<>();
    // 작업중
    private boolean m_bWork = false;
    // 작업시작 시간
    private long m_workBeginTime;

    // 황산
    private boolean m_bSulfuricLeak = false;    // 누출 여부
    private boolean m_bSulfuricObserve = false;         // 누출 감시

    // 황산
    public void setSulfuricLeak(int nLeak) {m_bSulfuricLeak = nLeak == 1 ? true : false;}
    public boolean getSulfuricLeak() {return m_bSulfuricLeak;}
    public void setSulfuricObserve(int nObserve) {m_bSulfuricObserve = nObserve == 1 ? true : false;}
    public boolean getSulfuricObserve() {return m_bSulfuricObserve;}

    // 탱크 ID
    public void setID(int nID)
    {
        m_nID = nID;
    }
    public int getID()
    {
        return m_nID;
    }

    // 탱크명
    public void setName(String name)
    {
        m_strName = name;
    }
    public String getName()
    {
        return m_strName;
    }

    // 유종
    public void setLiquidType(String type)
    {
        m_strLiquidType = type;
    }
    public String getLiquidType()
    {
        return m_strLiquidType;
    }

    // 레벨
    public void setLevel(double level)
    {
        m_dLevel = level;
    }
    public double getLevel() { return m_dLevel; }
    public String getLevelString()
    {
        if(m_dLevel <= -999)
            return "";

        return String.format("%.1f", m_dLevel);
    }
    public void setHighLevel(double highLevel)
    {
        m_dHighLevel = highLevel;
    }
    public double getHighLevel()
    {
        return m_dHighLevel;
    }
    public String getHighLevelText()
    {
        String strHighLevel = String.format("%.1f", m_dHighLevel);
        return "(" + removeTailZero(strHighLevel) + ")";
    }
    public String getTempRangeText()
    {
        String strMaxTemp = String.format("%.1f", m_dMaxTemp);
        String strMinTemp = String.format("%.1f", m_dMinTemp);

        strMaxTemp = removeTailZero(strMaxTemp);
        strMinTemp = removeTailZero(strMinTemp);

        return "(" + strMinTemp + "~" + strMaxTemp + ")";
    }

    // 온도
    public void setTemp(double temp)
    {
        m_dTemp = temp;
    }
    public double getTemp()
    {
        return m_dTemp;
    }
    public String getTempString()
    {
        if(m_dTemp <= -999)
            return "";

        return String.format("%.1f", m_dTemp);
        //return removeTailZero(strTemp);
    }
    public String getTempUnit()
    {
        return "  ̊C";
    }
    public void setMinTemp(double minTemp)
    {
        m_dMinTemp = minTemp;
    }
    public double getMinTemp()
    {
        return m_dMinTemp;
    }
    public void setMaxTemp(double maxTemp)
    {
        m_dMaxTemp = maxTemp;
    }
    public double getMaxTemp()
    {
        return m_dMaxTemp;
    }

    // 비중
    public void setGravity(double gravity)
    {
        m_dGravity = gravity;
    }
    public double getGravity()
    {
        return m_dGravity;
    }
    public String getGravityString()
    {
        String strGravity = String.format("%.2f", m_dGravity);
        return removeTailZero(strGravity);
        //return String.format("%.2f", m_dGravity);
    }

    // 재고
    public void setMass(double mass)
    {
        m_dMass = mass;
    }
    public double getMass()
    {
        return m_dMass;
    }
    public String getMassString()
    {
        String strMass = String.format("%,.0f", m_dMass);
        if(m_dMass <= 0)
            return "";

        return removeTailZero(strMass);
        //return String.format("%,.0f", m_dMass) + getMassUnit();
    }
    public String getMassUnit()
    {
        return " TON";
    }

    // 유량
    public void setFlow(double flow)
    {
        m_dFlow = flow;
    }
    public double getFlow()
    {
        return m_dFlow;
    }
    public String getFlowString()
    {
        String strFlow = "";
        if(m_dFlow > -999) {
            strFlow = String.format("%,.1f", m_dFlow);
        }
        return strFlow;
    }
    public void setStdFlow(float flow) {m_fStdFlow = flow;}
    public void setFlowRangeType(int nType) {m_nFlowRangeType = nType;}
    public void setFlowMax(float flow) {m_fFlowMax = flow;}
    public void setFlowRange(float range)
    {
        m_fFlowRange = range;
        if(m_fFlowRange < 0)
            m_fFlowRange *= -1;
    }
    public void setFlowRangeOrigin(float fRange) {m_fFlowRangeOrigin = fRange;}
    public String getFlowRangeString()
    {
        if(m_fStdFlow <= -999)
            return "";

        float minFlow = m_fFlowMax - m_fFlowRange;

        String strMinFlow = String.format("%.1f", minFlow);
        String strMaxFlow = String.format("%.1f", m_fFlowMax);

        strMinFlow = Tank.removeTailZero(strMinFlow);
        strMaxFlow = Tank.removeTailZero(strMaxFlow);

        if(strMinFlow == "-0")
            strMinFlow = "0";
        if(strMaxFlow == "-0")
            strMaxFlow = "0";

        return strMinFlow + "~" + strMaxFlow;
    }
    public String getFlowRangeTypeString()
    {
        String strRange = String.format("%.0f", m_fFlowRangeOrigin);
        if(m_nFlowRangeType == 0) //%
            strRange += "%";

        return strRange;
    }
    public String getFlowUnit()
    {
        return " KL/h";
    }

    // 상태
    public void setStatus(int status)
    {
        m_nStatus = status;
    }
    public int getStatus()
    {
        return m_nStatus;
    }

    // 용량
    public double getCapacity()
    {
        return m_dCapacity;
    }
    public void setCapacity(double capacity)
    {
        m_dCapacity = capacity;
    }
    public String getCapacityString()
    {
        return String.format("(%,.0f KL)", m_dCapacity);
        //return String.format("%s / %,.0f (KL)", m_strTankType, m_dCapacity);
        //return String.format("용량 : %,.0f KL", m_dCapacity);
    }

    // 타입
    public String getTankType()
    {
        return m_strTankType;
    }
    public void setTankType(String type)
    {
        m_strTankType = type;
    }

    // 연결된 배관
    public int getLinkPipeSize()
    {
        return m_LinkedPipe.size();
    }
    public Pipe getLinkPipe()
    {
        int size = m_LinkedPipe.size();
        if(size == 0)
            return null;
        else if(size == 1)
            return m_LinkedPipe.get(0);

        if(m_LinkedPipe.get(0).GetWorkBeginTime() < m_LinkedPipe.get(1).GetWorkBeginTime())
            return m_LinkedPipe.get(0);
        else
            return m_LinkedPipe.get(1);
    }

    public Pipe getLinkPipe2()
    {
        int size = m_LinkedPipe.size();
        if(size < 2)
            return null;

        if(m_LinkedPipe.get(0).GetWorkBeginTime() < m_LinkedPipe.get(1).GetWorkBeginTime())
            return m_LinkedPipe.get(1);
        else
            return m_LinkedPipe.get(0);
    }

    public void setLinkedPipe(Pipe pipe, boolean bSet)
    {
        if(bSet)
        {
            if(!m_LinkedPipe.contains(pipe))
                m_LinkedPipe.add(pipe);
        }
        else
        {
            for(Iterator<Pipe> it = m_LinkedPipe.iterator(); it.hasNext();)
            {
                if(it.next() == pipe) {
                    it.remove();
                    return;
                }
            }
            //if(m_LinkedPipe.contains(pipe))
            //    m_LinkedPipe.remove(pipe);
        }
    }

    // 알람 정보
    public void setAlarmID1(int nID) {m_nAlarmID1 = nID;}
    public void setAlarmID2(int nID) {m_nAlarmID2 = nID;}
    public int getAlarmID1() {return m_nAlarmID1;}
    public int getAlarmID2() {return m_nAlarmID2;}

    // 작업 시작 시간
    public void SetWorkBeginTime(long time) {m_workBeginTime = time;}
    public long GetWorkBeginTime() {return m_workBeginTime;}
    public String GetWorkTime()
    {
        Calendar now = Calendar.getInstance();

        long due = (now.getTimeInMillis() - m_workBeginTime) / 1000;
        long h = due / 60 / 60;
        long min = ((long)(due / 60)) % 60;

        //String strTime = String.format("%d시간 %02d분", h, min);
        return String.format("%d,%02d", h, min);
    }


    // 숫자 Text가 소숫점 이하를 표현하고 있을 경우, 마지막이 0으로 끝나게 되면 0을 없앤다.
    public static String removeTailZero(String number)
    {
        if (number.contains(".") == false)
            return number;

        while (number.endsWith("0"))
        {
            number = number.substring(0, number.length() - 1);
        }

        if (number.endsWith("."))
            number = number.substring(0, number.length() - 1);

        return number;
    }

    public int compareTo(Tank tank)
    {
        if (tank == null)
            return 1;

        return m_strName.compareTo(tank.m_strName);
    }

    // 작업중 상태
    public void setWork(boolean bWork) {m_bWork = bWork;}
    public boolean isWork() {return m_bWork;}
}
