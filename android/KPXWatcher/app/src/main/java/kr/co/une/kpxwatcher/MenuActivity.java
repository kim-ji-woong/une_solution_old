package kr.co.une.kpxwatcher;

import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutCompat;
import android.text.method.ScrollingMovementMethod;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.HorizontalScrollView;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.google.firebase.iid.FirebaseInstanceId;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;
import kr.co.une.kpxwatcher.Data.*;

public class MenuActivity extends AppCompatActivity {

    private static MenuActivity m_instance = null;

    //private TimerTask task = null;
    //private Timer timer = null;
    private String m_strNotice = "";

    private Thread m_thread = null;
    private boolean m_runThread = false;
    private boolean m_runActivity = false;

    private boolean m_readNotice = true;
    private boolean m_readPipe = false;
    private boolean m_readTank = false;

    // Key : Pipe ID
    private Map<Integer, Pipe> m_mapPipeStatus = new HashMap<Integer, Pipe>();
    // Key : Tank ID
    private Map<Integer, Tank> m_mapTankStatus = new HashMap<Integer, Tank>();
    private Pipe[] m_arrSortedPipe = null;
    private Tank[] m_arrSortedTank = null;

    private String m_initTag = "";
    private boolean m_initPipe = false;
    private boolean m_initTank = false;

    private Splash.UserType m_userType = Splash.UserType.UNKNOWN;

    private boolean m_pipeAccess = false;
    private List<Integer> m_tankAccessIDs = new ArrayList();
    private List<Integer> m_tankItems = new ArrayList();

    // Pipe 설정 권한
    // 1 : 알람해제
    private List<Integer> m_pipeItems = new ArrayList();

    public static MenuActivity Instance() {
        return m_instance;
    }

    public void setNotice(boolean enabled) {
        m_readNotice = enabled;
    }

    public void setPipe(boolean enabled) {
        m_readPipe = enabled;
    }

    public void setTank(boolean enabled) {
        m_readTank = enabled;
    }

    /*public static void setPipeAccess(boolean pipeAccess)
    {
        m_pipeAccess = pipeAccess;
    }

    public static void setTankAccess(List<Integer> tankAccessIDs)
    {
        for (int nTankID : tankAccessIDs)
        {
            m_tankAccessIDs.add(nTankID);
        }
    }

    public static void setTankItems(List<Integer> tankItems)
    {
        for (int itemID : tankItems)
        {
            m_tankItems.add(itemID);
        }
    }*/

    public boolean getTankItems(int itemID)
    {
        return m_tankItems.contains(itemID);
    }

    public boolean getPipeItems(int itemID)
    {
        return m_pipeItems.contains(itemID);
    }

    public Tank getTank(int nTankID) {return m_mapTankStatus.get(nTankID);}
    public Pipe getPipe(int nPipeID) {return m_mapPipeStatus.get(nPipeID);}

    public Tank getTankStatus(int index)
    {
        if (m_arrSortedTank == null)
            return null;

        if (index >= m_arrSortedTank.length)
            return null;

        return m_arrSortedTank[index - 1];
    }

    public Pipe getPipeStatus(int index)
    {
        if (m_arrSortedPipe == null)
            return null;

        if (index >= m_arrSortedPipe.length)
            return null;

        return m_arrSortedPipe[index - 1];
    }

    public String getNoticeString()
    {
        return m_strNotice;
    }

    public int getPipeCount() { return m_mapPipeStatus.size(); }
    public int getTankCount() { return m_mapTankStatus.size(); }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;

        Intent intent = getIntent();
        String userType = intent.getStringExtra("UserType");
        m_userType = Splash.getUserType(userType);

        setAccess(intent);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_menu);

        setImage((ImageView)findViewById(R.id.menuTop), R.drawable.menu_top2);
        setImage((ImageView)findViewById(R.id.btnPipeMonitoring), R.drawable.pipe_monitor2_normal);
        setImage((ImageView)findViewById(R.id.btnTankMonitoring), R.drawable.tank_monitor2_normal);
        //setImage((ImageView)findViewById(R.id.btnRecentHistory), R.drawable.recent_history_disabled);

        TextView text = (TextView)findViewById(R.id.textNotice);
        //text.setSelected(true);
        //text.setMovementMethod(new ScrollingMovementMethod());

        /*task = new TimerTask()
        {
            public void run()
            {
                readNotice();

                // 1초후 실행하고 Timer 종료
                //timer.schedule(task, 1000);
            }
        };

        timer = new Timer();
        // 1초후 실행하고 Timer 종료
        timer.schedule(task, 1000);*/
    }

    private void setAccess(Intent intent)
    {
        String pipeAccess = intent.getStringExtra("PipeAccess");

        if (pipeAccess != null)
        {
            if (pipeAccess.equals("1"))
                m_pipeAccess = true;
        }

        String tankAccess = intent.getStringExtra("TankAccess");

        if (tankAccess != null)
        {
            Splash.getIDs(tankAccess, m_tankAccessIDs);
        }

        String tankItems = intent.getStringExtra("TankItems");

        if (tankItems != null)
        {
            Splash.getIDs(tankItems, m_tankItems);
        }

        String pipeItems = intent.getStringExtra("PipeItems");
        if (pipeItems != null)
        {
            Splash.getIDs(pipeItems, m_pipeItems);
        }
     }

    @Override
    protected void onStart() {
        super.onStart();
        getDelegate().onStart();

        String initTag = m_initTag;
        m_initTag = "";

        if (initTag.contains("Tank"))
        {
            if (m_tankAccessIDs.size() > 0) {
                Intent intent = new Intent(MenuActivity.this, TankMonitoring.class);
                intent.putExtra("TankCount", Integer.toString(m_tankAccessIDs.size()));
                startActivity(intent);
            }
        }
        else if (initTag.contains("Pipe"))
        {
            if (m_pipeAccess) {
                Intent intent = new Intent(MenuActivity.this, PipeMonitoring.class);
                startActivity(intent);
            }
        }
        else {
            MainActivity.setCurrentActivity(MainActivity.ActivityType.Menu);

            m_runActivity = true;
            m_runThread = true;
            m_readNotice = true;

            if (m_thread == null) {
                m_thread = new Thread(new Runnable() {
                    public void run() {
                        boolean isFirst = true;

                        while (m_runThread) {
                            final TextView text = (TextView) findViewById(R.id.textNotice);

                            if (m_readNotice || isFirst)
                                readNotice();

                            if (m_readPipe || m_readTank || isFirst) {
                                readPipe();
                                readTank();
                            }

                            if (m_runActivity) {
                                text.post(new Runnable() {
                                    public void run() {
                                        text.setText(m_strNotice);
                                    }
                                });
                                /*if(getTankItems(10)) {
                                    text.post(new Runnable() {
                                        public void run() {
                                            text.setText(m_strNotice);
                                        }
                                    });
                                }*/
                            }

                            isFirst = false;

                            try {
                                Thread.sleep(1000);
                            } catch (Exception e) {
                            }
                        }
                    }
                });

                m_thread.start();
            }
        }
    }

    @Override
    protected void onStop() {
        super.onStop();
        getDelegate().onStop();

        m_runActivity = false;
    }

    @Override
    protected void onDestroy() {
        m_runThread = false;
        m_runActivity = false;
        m_thread.interrupt();

        super.onDestroy();
        getDelegate().onDestroy();
    }

    public static String readIgnoreAlarmMinute(Context context)
    {
        WebManager mgr = new WebManager(context);
        mgr.setQueryType(WebManager.QueryType.READ_OPTION);
        mgr.setParameter("PropertyName", "IgnoreTime");
        mgr.start();

        String ignoreAlarmMinute = "";
        int nTimeOut = 3000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
                return ignoreAlarmMinute;
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            int nResultCount = mgr.getResultSet().size();

            if (nResultCount > 0) {
                ignoreAlarmMinute = mgr.getResultSet().get(nResultCount - 1);
            }
        }

        return ignoreAlarmMinute;
    }

    private void readNotice()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.READ_NOTICE);
        mgr.start();

        int nTimeOut = 3000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
                return;
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            int nResultCount = mgr.getResultSet().size();

            if (nResultCount > 0) {
                m_strNotice = mgr.getResultSet().get(nResultCount - 1);
            }
        }
    }

    private void readPipe()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.READ_PIPE);
        mgr.start();

        int nTimeOut = 3000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
                return;
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            List<String> results = mgr.getResultSet();
            int nResultCount = results.size();

            boolean hasException = false;

            for (int i=0;i<nResultCount-15;i+=16)
            {
                try
                {
                    int id = Integer.parseInt(results.get(i).trim());
                    String strName = results.get(i + 1).trim();
                    float currentPressure = Float.parseFloat(results.get(i + 2).trim());
                    float prevPressure = Float.parseFloat(results.get(i + 3).trim());
                    int nStatus = Integer.parseInt(results.get(i + 4).trim());
                    int inWork = Integer.parseInt(results.get(i + 5).trim());
                    String pipeType = results.get(i + 6).trim();
                    String strPipeTypeBefore = results.get(i + 7).trim();
                    int nConTankID = Integer.parseInt(results.get(i + 8).trim());
                    int stableType = Integer.parseInt(results.get(i + 9).trim());
                    float stableRatio = Float.parseFloat(results.get(i + 10).trim());
                    float stableAbs = Float.parseFloat(results.get(i + 11).trim());
                    int alarmID = Integer.parseInt(results.get(i + 12).trim());
                    int alarmType = Integer.parseInt(results.get(i + 13).trim());
                    String strBeginTime = results.get(i + 14).trim();
                    String strServerTime = results.get(i + 15).trim();

                    if (strPipeTypeBefore.equals("null"))
                        strPipeTypeBefore = "";
                    else
                        strPipeTypeBefore = strPipeTypeBefore.replace(" ", "");

                    Pipe pipe = m_mapPipeStatus.get(id);

                    if (pipe == null)
                    {
                        pipe = new Pipe();
                        m_mapPipeStatus.put(id, pipe);
                    }
                    pipe.setLinkedTank(null, false);

                    pipe.setID(id);
                    pipe.setName(strName);
                    pipe.setPressure(currentPressure);
                    pipe.setPressureRangeType(stableType);
                    if(prevPressure <= -999)
                        prevPressure = 0;

                    if(stableType == 0) // %
                    {
                        pipe.setPressureRnageOrigin(stableRatio);
                        if(prevPressure == 0) {
                            pipe.setPressureMax(1.0f + stableRatio / 100.0f);
                            pipe.setPressureRange(stableRatio / 100.0f * 2);
                        }
                        else
                        {
                            pipe.setPressureMax(prevPressure * (1.0f + stableRatio / 100.0f));
                            pipe.setPressureRange(prevPressure * stableRatio / 100.0f * 2);
                        }

                    }
                    else // Abs
                    {
                        pipe.setPressureMax(prevPressure + stableAbs);
                        pipe.setPressureRange(stableAbs * 2);
                        pipe.setPressureRnageOrigin(stableAbs);
                    }

                    pipe.setStatus(nStatus);
                    pipe.setWork(inWork == 0 ? false : true);
                    pipe.setPipeType(pipeType);
                    pipe.setLiquid(strPipeTypeBefore);

                    m_mapPipeStatus.put(id, pipe);

                    Tank tank = m_mapTankStatus.get(nConTankID);
                    if(inWork == 1)
                    {
                        if(tank != null)
                        {
                            pipe.setLinkedTank(tank, true);
                        }

                        SimpleDateFormat transFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                        Date to = transFormat.parse(strBeginTime);
                        pipe.SetWorkBeginTime(to.getTime());
                    }
                    else {
                        if(tank != null)
                            pipe.setLinkedTank(tank, false);
                    }

                    pipe.setAlarmID(alarmID);
                    pipe.setAlarmType(alarmType);

                    SimpleDateFormat transFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                    Date to = transFormat.parse(strServerTime);
                    pipe.setServerTime(to.getTime());
                }
                catch (Exception e)
                {
                    hasException = true;
                    continue;
                }
            }

            if (hasException == false)
                m_initPipe = true;
        }

        int nSortedCount = m_arrSortedPipe == null ? 0 : m_arrSortedPipe.length - 1;
        int nMapCount = m_mapPipeStatus.size();

        if (nSortedCount != nMapCount)
        {
            if (nMapCount == 0)
                m_arrSortedPipe = null;
            else
            {
                String strLastPipeName = "";
                Pipe lastPipe = null;

                // 배열의 마지막 개체를 인식하지 못하는 오류가 있어 하나를 더 추가한다.
                m_arrSortedPipe = new Pipe[nMapCount + 1];
                int i=0;

                for (Map.Entry<Integer, Pipe> pair : m_mapPipeStatus.entrySet()) {
                    m_arrSortedPipe[i++] = pair.getValue();

                    if (strLastPipeName.compareTo(pair.getValue().getName()) < 0)
                    {
                        lastPipe = pair.getValue();
                        strLastPipeName = lastPipe.getName();
                    }
                }

                // 배열의 마지막 개체를 인식하지 못하는 오류가 있어 하나를 더 추가한다.
                m_arrSortedPipe[nMapCount] = lastPipe;
                Arrays.sort(m_arrSortedPipe);
            }
        }
    }

    private void readTank()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.READ_TANK);
        mgr.start();

        int nTimeOut = 3000, delay = 500, sum = 0;

        while (mgr.getResult() == WebManager.ResultType.UNKNOWN)
        {
            try {
                if (sum > nTimeOut)
                    break;

                Thread.sleep(delay);
                sum += delay;
            }
            catch (Exception e)
            {
                return;
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS)
        {
            boolean hasException = false;
            List<String> results = mgr.getResultSet();
            int nResultCount = results.size();

            for (int i=0;i<nResultCount-23;i+=24)
            {
                try
                {
                    int id = Integer.parseInt(results.get(i).trim());
                    String strName = results.get(i + 1).trim();
                    String strLiquid = results.get(i + 2).trim();
                    double level = Double.parseDouble(results.get(i + 3).trim());
                    double temp = Double.parseDouble(results.get(i + 4).trim());
                    double density = Double.parseDouble(results.get(i + 5).trim());
                    double mass = Double.parseDouble(results.get(i + 6).trim());
                    float flow = Float.parseFloat(results.get(i + 7).trim());
                    double capacity = Double.parseDouble(results.get(i + 8).trim());
                    String strTankType = results.get(i + 9).trim();
                    int status = Integer.parseInt(results.get(i + 10).trim());
                    double highLevel = Double.parseDouble(results.get(i + 11).trim());
                    double minTemp = Double.parseDouble(results.get(i + 12).trim());
                    double maxTemp = Double.parseDouble(results.get(i + 13).trim());
                    int nAlarmID1 = Integer.parseInt(results.get(i + 14).trim());
                    int nAlarmID2 = Integer.parseInt(results.get(i + 15).trim());
                    int isWork = Integer.parseInt(results.get(i + 16).trim());
                    float stdFlow = Float.parseFloat(results.get(i + 17).trim());
                    int stableType = Integer.parseInt(results.get(i + 18).trim());
                    float stableRatio = Float.parseFloat(results.get(i + 19).trim());
                    float stableAbs = Float.parseFloat(results.get(i + 20).trim());
                    int sulfuricLeak = Integer.parseInt(results.get(i + 21).trim());
                    int sulfuricObserve = Integer.parseInt(results.get(i + 22).trim());
                    String strBeginTime = results.get(i + 23).trim();

                    if (m_tankAccessIDs.contains(id) == false)
                        continue;

                    Tank tank = m_mapTankStatus.get(id);

                    if (tank == null)
                    {
                        tank = new Tank();
                        m_mapTankStatus.put(id, tank);
                    }

                    tank.setID(id);
                    tank.setName(strName);
                    tank.setLiquidType(strLiquid);
                    tank.setLevel(level);
                    tank.setTemp(temp);
                    tank.setGravity(density);
                    tank.setMass(mass);
                    tank.setFlow(flow);
                    tank.setFlowRangeType(stableType);
                    tank.setStatus(status);
                    tank.setCapacity(capacity);
                    tank.setTankType(strTankType);
                    tank.setHighLevel(highLevel);
                    tank.setMinTemp(minTemp);
                    tank.setMaxTemp(maxTemp);
                    tank.setAlarmID1(nAlarmID1);
                    tank.setAlarmID2(nAlarmID2);
                    tank.setWork(isWork == 0 ? false : true);
                    tank.setSulfuricLeak(sulfuricLeak);
                    tank.setSulfuricObserve(sulfuricObserve);

                    tank.setStdFlow(stdFlow);
                    if(stableType == 0) // %
                    {
                        if(stdFlow < 0)
                            tank.setFlowMax(stdFlow * (1.0f - stableRatio / 100.0f));
                        else
                            tank.setFlowMax(stdFlow * (1.0f + stableRatio / 100.0f));
                        tank.setFlowRange(stdFlow * stableRatio / 100.0f * 2);
                        tank.setFlowRangeOrigin(stableRatio);
                    }
                    else // Abs
                    {
                        if(stableAbs < 0)
                            stableAbs *= -1;
                        tank.setFlowMax(stdFlow + stableAbs);
                        tank.setFlowRange(stableAbs * 2);
                        tank.setFlowRangeOrigin(stableAbs);
                    }

                    if(isWork == 1)
                    {
                        SimpleDateFormat transFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
                        Date to = transFormat.parse(strBeginTime);
                        tank.SetWorkBeginTime(to.getTime());
                    }

                    m_mapTankStatus.put(id, tank);
                }
                catch (Exception e)
                {
                    hasException = true;
                    continue;
                }
            }

            if (hasException == false)
                m_initTank = true;
        }

        int nSortedCount = m_arrSortedTank == null ? 0 : m_arrSortedTank.length - 1;
        int nMapCount = m_mapTankStatus.size();

        if (nSortedCount != nMapCount)
        {
            if (nMapCount == 0)
                m_arrSortedTank = null;
            else
            {
                String strLastTankName = "";
                Tank lastTank = null;

                // 배열의 마지막 개체를 인식하지 못하는 오류가 있어 하나를 더 추가한다.
                m_arrSortedTank = new Tank[nMapCount + 1];
                int i=0;

                for (Map.Entry<Integer, Tank> pair : m_mapTankStatus.entrySet()) {
                    m_arrSortedTank[i++] = pair.getValue();

                    if (strLastTankName.compareTo(pair.getValue().getName()) < 0)
                    {
                        lastTank = pair.getValue();
                        strLastTankName = lastTank.getName();
                    }
                }

                // 배열의 마지막 개체를 인식하지 못하는 오류가 있어 하나를 더 추가한다.
                m_arrSortedTank[nMapCount] = lastTank;
                Arrays.sort(m_arrSortedTank);
            }
        }
    }

    private void setImage(ImageView view, int nImageID)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);
    }

    public void btnInputClick(View v)
    {
        if (v.getId() == R.id.btnPipeMonitoring)
        {
            if (m_initPipe == false)
                return;

            if (m_pipeAccess) {
                Intent intent = new Intent(MenuActivity.this, PipeMonitoring.class);
                startActivity(intent);
            }else
            {
                MainActivity.showAlert("배관 정보에 접속할 권한이 없습니다.", "알림", this);
            }

        }
        else if (v.getId() == R.id.btnTankMonitoring)
        {
            if (m_initTank == false)
                return;

            if (m_tankAccessIDs.size() > 0) {
                Intent intent = new Intent(MenuActivity.this, TankMonitoring.class);
                intent.putExtra("TankCount", Integer.toString(m_tankAccessIDs.size()));
                startActivity(intent);
            }
            else
            {
                MainActivity.showAlert("탱크 정보에 접속할 권한이 없습니다.", "알림", this);
            }
        }
    }

    @Override
    public void onBackPressed()
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);

        builder.setMessage("종료하시겠습니까?");
        builder.setTitle("확인");

        builder.setNegativeButton("네", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which)
            {
                MenuActivity.super.onBackPressed();
                dialog.dismiss();
            }
        });

        builder.setPositiveButton("아니오", new DialogInterface.OnClickListener()
        {
            public void onClick(DialogInterface dialog, int which)
            {
                // Do nothing
                dialog.dismiss();
            }
        });

        builder.show();
    }

    public void onNotify(String strTitle, String strMessage)
    {
        if (strTitle.equals("TankAlarm"))
        {
            if (m_tankAccessIDs.size() > 0) {
                Intent intent = new Intent(MenuActivity.this, TankMonitoring.class);
                intent.putExtra("TankCount", Integer.toString(m_tankAccessIDs.size()));
                startActivity(intent);
            }
        }
        else if (strTitle.equals("PipeAlarm"))
        {
            if (m_pipeAccess) {
                Intent intent = new Intent(MenuActivity.this, PipeMonitoring.class);
                startActivity(intent);
            }
        }
    }

    public void setInitTag(String tag)
    {
        m_initTag = tag;
    }

    // 알람 Off 권한이 있는가?
    public boolean canAlarmOff()
    {
        return m_userType == Splash.UserType.CERTIFICATED_USER;
    }

    public void PipeAlarmOff(final Pipe pipe, final int OccurType, final String comment)
    {
        if (pipe != null) {
            Tank tank = pipe.getLinkTank();
            if(tank != null) {
                WebManager mgr = new WebManager(getApplicationContext());
                mgr.setQueryType(WebManager.QueryType.CLEAR_PIPE_ALARM);

                mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
                mgr.setParameter("PipeID", String.valueOf(pipe.getID()));
                mgr.setParameter("TankID", String.valueOf(tank.getID()));
                mgr.setParameter("OccurType", String.valueOf(OccurType));
                mgr.setParameter("Comment", comment);
                mgr.start();
            }
        }
    }

    public void TankAlarmOff(final Tank tank, final int OccurType, final String comment)
    {
        if (tank != null) {
            WebManager mgr = new WebManager(getApplicationContext());
            mgr.setQueryType(WebManager.QueryType.CLEAR_TANK_ALARM);

            mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
            mgr.setParameter("TankID", String.valueOf(tank.getID()));
            mgr.setParameter("OccurType", String.valueOf(OccurType));
            mgr.setParameter("Comment", comment);
            mgr.start();
        }
    }
}
