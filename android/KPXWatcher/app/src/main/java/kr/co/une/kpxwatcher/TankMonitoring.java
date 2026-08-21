package kr.co.une.kpxwatcher;

import android.app.ActionBar;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.support.annotation.RequiresApi;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.TextUtils;
import android.text.style.AbsoluteSizeSpan;
import android.text.style.ForegroundColorSpan;
import android.text.style.RelativeSizeSpan;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.google.firebase.iid.FirebaseInstanceId;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import kr.co.une.kpxwatcher.Data.Pipe;
import kr.co.une.kpxwatcher.Data.Tank;

public class TankMonitoring extends AppCompatActivity {

    private final int TANK_NORMAL = 0;
    private final int TEMP_INCREASE = 1;
    private final int TEMP_DECREASE = 2;
    private final int LEVEL_MAX = 4;
    private final int FLOW_INCREASE = 8;
    private final int FLOW_DECREASE = 8;

    private String LiquidTag = "유종:";
    private String TemperatureTag = "온도:";
    private String LevelTag = "레벨:";
    private String GravityTag = "비중:";
    private String RemainTag = "재고:";
    private String FlowTag = "유량:";

    private List<LayoutAlarmStatus> m_listAlarmStatus = new ArrayList<>();

    private DlgAlarmClear m_dlgAlarmClear = null;

    // 입출고를 나누는 기준
    private final double FlowStandard = 10.0;

    private Thread m_thread = null;
    private boolean m_runThread = false;
    // Key : View ID
    // Value : Image ID
    private Map<Integer, Integer> m_mapViewImage = new HashMap<Integer, Integer>();

    public static boolean UseAlarmStatus2 = false;

    private int m_nTankCount = 0;

    private List<RelativeLayout> m_listTankLayout = new ArrayList<>();
    // Key : View ID
    // Value : View ID에 해당하는 Tank Index
    private HashMap<Integer, Integer> m_mapViewTankIndex = new HashMap<Integer, Integer>();
    // Key : Tank Index
    private HashMap<Integer, TankTextSet> m_mapTankTextSet = new HashMap<Integer, TankTextSet>();

    private static TankMonitoring m_instance = null;

    public static TankMonitoring Instance()
    {
        return m_instance;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_tank_monitoring);

        Intent intent = getIntent();
        String tankCount = intent.getStringExtra("TankCount");
        m_nTankCount = Integer.parseInt(tankCount.trim());

        setImage((ImageView)findViewById(R.id.imgTankTitle), R.drawable.tank_title);

        if (MenuActivity.Instance().getTankItems(9))
            setImage((ImageView)findViewById(R.id.imgHeaderBackground), R.drawable.tank_column_header);
        else
            setImage((ImageView)findViewById(R.id.imgHeaderBackground), R.drawable.tank_column_header_no_alarm);

        List<LinearLayout> alarmStatusList = new ArrayList<>();
        InitView(alarmStatusList);
        /*setImage((ImageView)findViewById(R.id.imgTank1), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank1Liquid, R.id.textTank1Temp, R.id.textTank1Level, R.id.textTank1Gravity, R.id.textTank1Remain, R.id.textTank1Flow);

        setImage((ImageView)findViewById(R.id.imgTank2), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank2Liquid, R.id.textTank2Temp, R.id.textTank2Level, R.id.textTank2Gravity, R.id.textTank2Remain, R.id.textTank2Flow);

        setImage((ImageView)findViewById(R.id.imgTank3), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank3Liquid, R.id.textTank3Temp, R.id.textTank3Level, R.id.textTank3Gravity, R.id.textTank3Remain, R.id.textTank3Flow);

        setImage((ImageView)findViewById(R.id.imgTank4), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank4Liquid, R.id.textTank4Temp, R.id.textTank4Level, R.id.textTank4Gravity, R.id.textTank4Remain, R.id.textTank4Flow);

        setImage((ImageView)findViewById(R.id.imgTank5), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank5Liquid, R.id.textTank5Temp, R.id.textTank5Level, R.id.textTank5Gravity, R.id.textTank5Remain, R.id.textTank5Flow);

        setImage((ImageView)findViewById(R.id.imgTank6), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank6Liquid, R.id.textTank6Temp, R.id.textTank6Level, R.id.textTank6Gravity, R.id.textTank6Remain, R.id.textTank6Flow);

        setImage((ImageView)findViewById(R.id.imgTank7), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank7Liquid, R.id.textTank7Temp, R.id.textTank7Level, R.id.textTank7Gravity, R.id.textTank7Remain, R.id.textTank7Flow);

        setImage((ImageView)findViewById(R.id.imgTank8), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank8Liquid, R.id.textTank8Temp, R.id.textTank8Level, R.id.textTank8Gravity, R.id.textTank8Remain, R.id.textTank8Flow);

        setImage((ImageView)findViewById(R.id.imgTank9), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank9Liquid, R.id.textTank9Temp, R.id.textTank9Level, R.id.textTank9Gravity, R.id.textTank9Remain, R.id.textTank9Flow);

        setImage((ImageView)findViewById(R.id.imgTank10), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank10Liquid, R.id.textTank10Temp, R.id.textTank10Level, R.id.textTank10Gravity, R.id.textTank10Remain, R.id.textTank10Flow);

        setImage((ImageView)findViewById(R.id.imgTank11), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank11Liquid, R.id.textTank11Temp, R.id.textTank11Level, R.id.textTank11Gravity, R.id.textTank11Remain, R.id.textTank11Flow);

        setImage((ImageView)findViewById(R.id.imgTank12), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank12Liquid, R.id.textTank12Temp, R.id.textTank12Level, R.id.textTank12Gravity, R.id.textTank12Remain, R.id.textTank12Flow);

        setImage((ImageView)findViewById(R.id.imgTank13), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank13Liquid, R.id.textTank13Temp, R.id.textTank13Level, R.id.textTank13Gravity, R.id.textTank13Remain, R.id.textTank13Flow);

        setImage((ImageView)findViewById(R.id.imgTank14), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank14Liquid, R.id.textTank14Temp, R.id.textTank14Level, R.id.textTank14Gravity, R.id.textTank14Remain, R.id.textTank14Flow);

        setImage((ImageView)findViewById(R.id.imgTank15), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank15Liquid, R.id.textTank15Temp, R.id.textTank15Level, R.id.textTank15Gravity, R.id.textTank15Remain, R.id.textTank15Flow);

        setImage((ImageView)findViewById(R.id.imgTank16), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank16Liquid, R.id.textTank16Temp, R.id.textTank16Level, R.id.textTank16Gravity, R.id.textTank16Remain, R.id.textTank16Flow);

        setImage((ImageView)findViewById(R.id.imgTank17), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank17Liquid, R.id.textTank17Temp, R.id.textTank17Level, R.id.textTank17Gravity, R.id.textTank17Remain, R.id.textTank17Flow);

        setImage((ImageView)findViewById(R.id.imgTank18), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank18Liquid, R.id.textTank18Temp, R.id.textTank18Level, R.id.textTank18Gravity, R.id.textTank18Remain, R.id.textTank18Flow);

        setImage((ImageView)findViewById(R.id.imgTank19), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank19Liquid, R.id.textTank19Temp, R.id.textTank19Level, R.id.textTank19Gravity, R.id.textTank19Remain, R.id.textTank19Flow);

        setImage((ImageView)findViewById(R.id.imgTank20), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank20Liquid, R.id.textTank20Temp, R.id.textTank20Level, R.id.textTank20Gravity, R.id.textTank20Remain, R.id.textTank20Flow);

        setImage((ImageView)findViewById(R.id.imgTank21), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank21Liquid, R.id.textTank21Temp, R.id.textTank21Level, R.id.textTank21Gravity, R.id.textTank21Remain, R.id.textTank21Flow);

        setImage((ImageView)findViewById(R.id.imgTank22), R.drawable.item_panel_dot_tank);
        initText(R.id.textTank22Liquid, R.id.textTank22Temp, R.id.textTank22Level, R.id.textTank22Gravity, R.id.textTank22Remain, R.id.textTank22Flow);*/

        if (alarmStatusList.size() != m_nTankCount)
            return;

        InitAlarmStatus(alarmStatusList);

        //removeUnusingTanks();

        //initAlarmStatus2();
        setLayerSize();

        for (int i=1;i<=m_nTankCount;i++) {
            final TankTextSet tankSet = m_mapTankTextSet.get(i - 1);
            //final TankTextSet tankSet = new TankTextSet(i);
            final TextView textNotice = (TextView) findViewById(R.id.textNotice);

            final Tank tank = MenuActivity.Instance().getTankStatus(i);

            if (tank != null)
            {
                setTankName(tankSet.getNameText(), tankSet.getCapacityText(), tankSet.getLinkText(), tankSet.getLinkText2(), tank);
                //tankSet.getNameText().setText(" " + tank.getName()/* + tank.getTankType()*/);
                setFlowText(tankSet.getFlowText(), tankSet.getFlowRangeText(), tank);
                setGravityText(tankSet.getGravityText(), tank);
                setLiquidText(tankSet.getLiquidTypeText(), tank);
                setRemainText(tankSet.getMassText(), tank);
                setLevelText(tankSet.getLevelText(), tank);
                setTempText(tankSet.getTempText(), tankSet.getTempRangeText(), tank);
                setAlarmButton(tankSet.getWorkStatusButton(), m_listAlarmStatus.get(i-1), tank);
                setPanel(tankSet.getPanel(), tank);
                if(MenuActivity.Instance().getTankItems(10)) {
                    String strNotice = MenuActivity.Instance().getNoticeString();
                    textNotice.setText(MenuActivity.Instance().getNoticeString());
                }
            }
        }
    }

    private void InitView(List<LinearLayout> alarmStatusList)
    {
        // 사용하지 않는 리소스는 감춘다.
        InitVisible();

        List<ImageView> images = new ArrayList<>();
        int nTankCount = m_listTankLayout.size();

        for (int i=0;i<nTankCount;i++)
        {
            RelativeLayout tank = m_listTankLayout.get(i);

            images.clear();
            List<Integer> results = GetTankInfoLayout(tank, images, alarmStatusList, i);

            if (results == null)
                continue;

            int nLiquidID = (int)results.get(0);
            int nRemainID = (int)results.get(1);
            int nTempID = (int)results.get(2);
            int nGravityID = (int)results.get(3);
            int nLevelID = (int)results.get(4);
            int nFlowID = (int)results.get(5);

            setImage(images.get(0), R.drawable.item_panel_dot_tank);
            initText(nLiquidID, nTempID, nLevelID, nGravityID, nRemainID, nFlowID);
        }
    }

    private List<Integer> GetTankInfoLayout(RelativeLayout layout, List<ImageView> images, List<LinearLayout> alarmStatusList, int nTankIndex)
    {
        List<Integer> results = new ArrayList<>();

        TankTextSet tankSet = null;

        if (m_mapTankTextSet.containsKey(nTankIndex) == false)
        {
            tankSet = new TankTextSet();
            m_mapTankTextSet.put(nTankIndex, tankSet);
        }
        else
            tankSet = m_mapTankTextSet.get(nTankIndex);

        ImageView img = null;
        LinearLayout parent = null;
        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof ImageView)
            {
                if (img == null)
                {
                    img = (ImageView)view;
                    tankSet.setPanel(img);
                }
            }
            else if (view instanceof LinearLayout)
            {
                parent = (LinearLayout)view;
                break;
            }
        }

        if (parent == null || img == null)
            return null;

        ImageView imgWorkStatus = null;
        int nLayoutIndex = 0;
        nChildCount = parent.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = parent.getChildAt(i);

            if (view instanceof LinearLayout) {
                if (nLayoutIndex == 0) {
                    if (ReadNameLayout((LinearLayout)view, tankSet) == false)
                        return null;
                }
                else if (nLayoutIndex == 1) {
                    // Liquid, Remain, Temp
                    if (ReadLiquidLayout((LinearLayout) view, results, tankSet) == false)
                        return null;
                } else if (nLayoutIndex == 2) {
                    // Gravity, Level, Flow
                    if (ReadGravityLayout((LinearLayout) view, results, tankSet) == false)
                        return null;
                } else if (nLayoutIndex == 5) {
                    alarmStatusList.add((LinearLayout) view);
                    m_mapViewTankIndex.put(imgWorkStatus.getId(), nTankIndex);
                }

                nLayoutIndex++;
            }
            else if (view instanceof ImageView)
            {
                if (imgWorkStatus == null)
                {
                    imgWorkStatus = (ImageView)view;
                    m_mapViewTankIndex.put(imgWorkStatus.getId(), nTankIndex);
                    tankSet.setWorkStatusButton(imgWorkStatus);
                }
            }
        }

        if (results.size() != 6)
            return null;

        images.add(img);
        return results;
    }

    private boolean ReadNameLayout(LinearLayout layout, TankTextSet tankSet)
    {
        int nTextIndex = 0;
        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof TextView)
            {
                if (nTextIndex == 0)
                    tankSet.setNameText((TextView)view);
                else if (nTextIndex == 1)
                    tankSet.setCapacityText((TextView)view);
                else if (nTextIndex == 2)
                    tankSet.setLinkText((TextView)view);
                else if (nTextIndex == 3)
                    tankSet.setLinkText2((TextView)view);

                nTextIndex++;
            }
        }

        if (tankSet.getNameText() == null || tankSet.getCapacityText() == null || tankSet.getLinkText() == null)
            return false;

        return true;
    }

    // Gravity, Level, Flow ID를 순서대로 추출하여 reslts에 담는다.
    private boolean ReadGravityLayout(LinearLayout layout, List<Integer> results, TankTextSet tankSet)
    {
        int nTextIndex = 0;
        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof TextView)
            {
                if (nTextIndex == 0) {
                    results.add((view.getId()));
                    tankSet.setGravityText((TextView)view);
                }
                else if (nTextIndex == 1) {
                    results.add((view.getId()));
                    tankSet.setLevelText((TextView)view);
                }
                else if (nTextIndex == 2)
                {
                    results.add((view.getId()));
                    tankSet.setFlowText((TextView)view);
                }
                else if (nTextIndex == 3)
                {
                    tankSet.setFlowRangeText((TextView)view);
                }

                nTextIndex++;
            }
        }

        if (tankSet.getGravityText() == null || tankSet.getLevelText() == null || tankSet.getFlowText() == null || tankSet.getFlowRangeText() == null)
            return false;

        return true;
    }

    // Liquid, Remain, Temp ID를 순서대로 추출하여 reslts에 담는다.
    private boolean ReadLiquidLayout(LinearLayout layout, List<Integer> results, TankTextSet tankSet)
    {
        int nTextIndex = 0;
        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof TextView)
            {
                if (nTextIndex == 0) {
                    results.add((view.getId()));
                    tankSet.setLiquidTypeText((TextView)view);
                }
                else if (nTextIndex == 1) {
                    results.add((view.getId()));
                    tankSet.setMassText((TextView)view);
                }
                else if (nTextIndex == 2)
                {
                    results.add((view.getId()));
                    tankSet.setTempText((TextView)view);
                }
                else if (nTextIndex == 3)
                {
                    tankSet.setTempRangeText((TextView)view);
                }

                nTextIndex++;
            }
        }

        if (tankSet.getLiquidTypeText() == null || tankSet.getMassText() == null || tankSet.getTempText() == null || tankSet.getTempRangeText() == null)
            return false;

        return true;
    }

    private void InitVisible()
    {
        //m_nTankCount = MenuActivity.Instance().getTankCount();

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.bodyLayout);
        int nChildCount = bodyLayout.getChildCount();

        int nTankIndex = 0;
        View lastView = null;

        for (int i=nChildCount-1;i>=0;i--)
        {
            View view = bodyLayout.getChildAt(i);

            if (view instanceof RelativeLayout)
            {
                lastView = view;
                view.setVisibility(View.VISIBLE);
                break;
            }
        }

        for (int i=0;i<nChildCount;i++)
        {
            View view = bodyLayout.getChildAt(i);

            if (view instanceof RelativeLayout)
            {
                if (nTankIndex++ < m_nTankCount - 1) {
                    m_listTankLayout.add((RelativeLayout) view);
                    view.setVisibility(View.VISIBLE);
                }
                else
                    break;
            }
        }

        m_listTankLayout.add((RelativeLayout)lastView);
    }
    /*private void InitVisible()
    {
        //m_nTankCount = MenuActivity.Instance().getTankCount();

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.bodyLayout);
        int nChildCount = bodyLayout.getChildCount();

        int nTankIndex = 0;
        View lastView = null;

        for (int i=nChildCount-1;i>=0;i--)
        {
            View view = bodyLayout.getChildAt(i);

            if (view instanceof RelativeLayout)
            {
                lastView = view;
                break;
            }
        }

        for (int i=0;i<nChildCount;i++)
        {
            View view = bodyLayout.getChildAt(i);

            if (view instanceof RelativeLayout)
            {
                if (nTankIndex++ < m_nTankCount - 1)
                    m_listTankLayout.add((RelativeLayout)view);
                else {
                    if (lastView != view)
                        view.setVisibility(View.GONE);
                    else
                        m_listTankLayout.add((RelativeLayout)view);
                }
            }
        }
    }*/

    private void InitAlarmStatus(List<LinearLayout> alarmStatusList)
    {
        int nLayoutCount = alarmStatusList.size();

        for (int i=0;i<nLayoutCount;i++)
        {
            LinearLayout layout = alarmStatusList.get(i);
            CreateAlarmStatus(layout);
        }
        /*LinearLayout layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus1);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus2);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus3);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus4);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus5);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus6);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus7);
        if(layout == null)
            return;
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus8);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus9);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus10);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus11);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus12);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus13);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus14);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus15);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus16);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus17);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus18);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus19);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus20);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus21);
        CreateAlarmStatus(layout);

        layout = (LinearLayout)findViewById(R.id.layout_tankAlarmStatus22);
        CreateAlarmStatus(layout);*/
    }

    private void CreateAlarmStatus(LinearLayout layout)
    {
        ImageView iv = CreateAlarmStatus_img();
        layout.addView(iv);
        TextView tv = CreateAlarmStatus_text();
        layout.addView(tv);

        LayoutAlarmStatus status = new LayoutAlarmStatus(iv, tv);
        m_listAlarmStatus.add(status);
    }

    @RequiresApi(api = Build.VERSION_CODES.JELLY_BEAN_MR1)
    private TextView CreateAlarmStatus_text()
    {
        TextView view = new TextView(this);
        DisplayMetrics dm = getResources().getDisplayMetrics();

        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.gravity = Gravity.CENTER;
        params.topMargin = Math.round(-3 * dm.density);
        view.setLayoutParams(params);

        view.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        view.setTextColor(getResources().getColor(R.color.colorItemNormal));
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        return view;
    }

    private ImageView CreateAlarmStatus_img()
    {
        ImageView view = new ImageView(this);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.gravity = Gravity.CENTER_HORIZONTAL | Gravity.TOP;
        view.setLayoutParams(params);
        view.setAdjustViewBounds(true);
        view.setBackgroundColor(0x00000000);
        view.setOnClickListener(OnAlarmOffClick);

        return view;
    }

    View.OnClickListener OnAlarmOffClick = new View.OnClickListener(){
        @Override
        public void onClick(View v) {
            btnAlarmOffClick(v);
        }
    };

    /*private void removeUnusingTanks()
    {
        if (m_nTankCount >= 22)
            return;

        RelativeLayout lastLayout = (RelativeLayout)((ImageView)findViewById(R.id.imgTank22)).getParent();
        LinearLayout.LayoutParams lastLayoutParam = (LinearLayout.LayoutParams)lastLayout.getLayoutParams();
        int nLastBottomMargin = lastLayoutParam.bottomMargin;

        LinearLayout parentLayout = (LinearLayout)lastLayout.getParent();

        if (m_nTankCount < 22)
            removeTank((ImageView)findViewById(R.id.imgTank22));

        if (m_nTankCount < 21)
            removeTank((ImageView)findViewById(R.id.imgTank21));

        if (m_nTankCount < 20)
            removeTank((ImageView)findViewById(R.id.imgTank20));

        if (m_nTankCount < 19)
            removeTank((ImageView)findViewById(R.id.imgTank19));

        if (m_nTankCount < 18)
            removeTank((ImageView)findViewById(R.id.imgTank18));

        if (m_nTankCount < 17)
            removeTank((ImageView)findViewById(R.id.imgTank17));

        if (m_nTankCount < 16)
            removeTank((ImageView)findViewById(R.id.imgTank16));

        if (m_nTankCount < 15)
            removeTank((ImageView)findViewById(R.id.imgTank15));

        if (m_nTankCount < 14)
            removeTank((ImageView)findViewById(R.id.imgTank14));

        if (m_nTankCount < 13)
            removeTank((ImageView)findViewById(R.id.imgTank13));

        if (m_nTankCount < 12)
            removeTank((ImageView)findViewById(R.id.imgTank12));

        if (m_nTankCount < 11)
            removeTank((ImageView)findViewById(R.id.imgTank11));

        if (m_nTankCount < 10)
            removeTank((ImageView)findViewById(R.id.imgTank10));

        if (m_nTankCount < 9)
            removeTank((ImageView)findViewById(R.id.imgTank9));

        if (m_nTankCount < 8)
            removeTank((ImageView)findViewById(R.id.imgTank8));

        if (m_nTankCount < 7)
            removeTank((ImageView)findViewById(R.id.imgTank7));

        if (m_nTankCount < 6)
            removeTank((ImageView)findViewById(R.id.imgTank6));

        if (m_nTankCount < 5)
            removeTank((ImageView)findViewById(R.id.imgTank5));

        if (m_nTankCount < 4)
            removeTank((ImageView)findViewById(R.id.imgTank4));

        if (m_nTankCount < 3)
            removeTank((ImageView)findViewById(R.id.imgTank3));

        if (m_nTankCount < 2)
            removeTank((ImageView)findViewById(R.id.imgTank2));

        if (m_nTankCount < 1)
            removeTank((ImageView)findViewById(R.id.imgTank1));

        int nChildCount = parentLayout.getChildCount();

        if (nChildCount > 1)
        {
            RelativeLayout layout = (RelativeLayout)parentLayout.getChildAt(nChildCount - 1);
            LinearLayout.LayoutParams param = (LinearLayout.LayoutParams)layout.getLayoutParams();
            param.bottomMargin = nLastBottomMargin;
        }
    }

    private void removeTank(ImageView view)
    {
        RelativeLayout layout = (RelativeLayout)view.getParent();
        LinearLayout parent = (LinearLayout)layout.getParent();
        parent.removeView(layout);
    }*/

    private String getLiquidType(String strLiquid)
    {
        if (strLiquid.equals("N-BUTANOL"))
            return "BUTANOL";
        else if (strLiquid.equals("메틸렌클로라이드"))
            return "MC";

        return strLiquid;
    }

    @Override
    protected void onStart() {
        super.onStart();
        getDelegate().onStart();

        MenuActivity.Instance().setTank(true);
        MainActivity.setCurrentActivity(MainActivity.ActivityType.TankMonitor);

        m_runThread = true;

        m_thread = new Thread(new Runnable() {
            public void run() {
                while (m_runThread) {
                    for (int i=1;i<=m_nTankCount;i++) {
                        final TankTextSet tankSet = m_mapTankTextSet.get(i - 1);
                        //final TankTextSet tankSet = new TankTextSet(i);
                        final TextView textNotice = (TextView) findViewById(R.id.textNotice);
                        final int idx = i-1;

                        final Tank tank = MenuActivity.Instance().getTankStatus(i);

                        if (tank != null) {
                            tankSet.getNameText().post(new Runnable() {
                                public void run() {
                                    setTankName(tankSet.getNameText(), tankSet.getCapacityText(), tankSet.getLinkText(), tankSet.getLinkText2(), tank);
                                    //tankSet.getNameText().setText(" " + tank.getName()/* + tank.getTankType()*/);
                                    //tankSet.getCapacityText().setText("     " + tank.getCapacityString());
                                    setFlowText(tankSet.getFlowText(), tankSet.getFlowRangeText(), tank);
                                    setGravityText(tankSet.getGravityText(), tank);
                                    setLiquidText(tankSet.getLiquidTypeText(), tank);
                                    setRemainText(tankSet.getMassText(), tank);
                                    setLevelText(tankSet.getLevelText(), tank);
                                    setTempText(tankSet.getTempText(), tankSet.getTempRangeText(), tank);
                                    setAlarmButton(tankSet.getWorkStatusButton(), m_listAlarmStatus.get(idx), tank);
                                    setPanel(tankSet.getPanel(), tank);
                                }
                            });

                            if (i == m_nTankCount)
                            {
                                textNotice.post(new Runnable() {
                                    public void run() {
                                        textNotice.setText(MenuActivity.Instance().getNoticeString());
                                    }
                                });
                            }
                            /*if(MenuActivity.Instance().getTankItems(10)) {
                                textNotice.post(new Runnable() {
                                    public void run() {
                                        textNotice.setText(MenuActivity.Instance().getNoticeString());
                                    }
                                });
                            }*/
                        }
                    }

                    try {
                        Thread.sleep(1000);
                    } catch (Exception e) {
                    }
                }
            }
        });

        m_thread.start();
    }

    @Override
    protected void onStop() {
        MenuActivity.Instance().setPipe(false);

        m_runThread = false;

        super.onStop();
        getDelegate().onStop();
    }

    // 탱크 패널
    private void setPanel(ImageView panel, Tank tank)
    {
        int nAlarmID1 = tank.getAlarmID1();
        int nAlarmID2 = tank.getAlarmID2();

        Pipe pipe = tank.getLinkPipe();
        int nPipeAlarm = 0;
        if(pipe != null)
        {
            nPipeAlarm = pipe.getAlarmID();
        }

        // 황산 알람
        if(tank.getLiquidType().equals("황산") && !tank.getSulfuricLeak() && !tank.getSulfuricObserve()) {
            setImage(panel, R.drawable.item_panel_no_comm_tank);
        }
        else
        {
            if (nAlarmID1 != 0 || nAlarmID2 != 0 || nPipeAlarm != 0 || (MenuActivity.Instance().getTankItems(11) && tank.getSulfuricLeak()))
                setImage(panel, R.drawable.item_panel_alarm_dot_tank);
            else
                setImage(panel, R.drawable.item_panel_dot_tank);
        }
    }

    // 탱크명
    private void setTankName(TextView textName, TextView textCapacity, TextView textLink, TextView textLink2, Tank tank)
    {
        String strName = tank.getName();
        String strType = tank.getTankType();
        String strCapacity = tank.getCapacityString();
        String total = strName + strType;

        Spannable wordtoSpan = new SpannableString(total);
        wordtoSpan.setSpan(new RelativeSizeSpan(1.0f), 0, strName.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        wordtoSpan.setSpan(new RelativeSizeSpan(0.7f), strName.length(), total.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

        textName.setText(wordtoSpan);
        textCapacity.setText("    " + strCapacity);
        textLink2.setText("");;

        // 황산 알람
        if(MenuActivity.Instance().getTankItems(11) && tank.getLiquidType().equals("황산")) {
            if (tank.getSulfuricLeak()) {
                textName.setTextColor(Color.WHITE);
                textLink.setText("     누출 감지");
                textLink.setTextColor(Color.RED);
            } else if(tank.getSulfuricObserve()) {
                textName.setTextColor(Color.rgb(200, 200, 0));
                textLink.setText("     감시중");
                textLink.setTextColor(Color.rgb(200, 200, 0));
            }
            else
            {
                textName.setTextColor(Color.rgb(200, 200, 0));
                textLink.setText("     통신불능");
                textLink.setTextColor(Color.WHITE);
            }
        }
        else {
            Pipe pipe = tank.getLinkPipe();
            if (pipe == null)
                textLink.setText("");
            else {
                textLink.setText("     " + pipe.getName());

                //if(tank.getStatus() == 0)
                textLink.setTextColor(Color.GREEN);
                //else
                //    textLink.setTextColor(Color.RED);
            }

            Pipe pipe2 = tank.getLinkPipe2();

            if (pipe2 != null)
            {
                textLink2.setText("     " + pipe2.getName());
            }
        }
    }

    // 재고
    private void setRemainText(TextView text, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(2) == false) {
            text.setText("");
            return;
        }

        String strRemain = tank.getMassString();
        String strRemainUnit = tank.getMassUnit();
        String str = RemainTag;
        if(strRemain != "")
            str = RemainTag + strRemain + strRemainUnit;

        Spannable wordtoSpan = new SpannableString(str);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, RemainTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        if(strRemain != "") {
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), RemainTag.length(), str.length() - strRemainUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), str.length() - strRemainUnit.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        }

        text.setText(wordtoSpan);
    }

    // 비중
    private void setGravityText(TextView text, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(5) == false) {
            text.setText("");
            return;
        }

        String strGravity = tank.getGravityString();
        String str = GravityTag + strGravity;

        Spannable wordtoSpan = new SpannableString(str);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, GravityTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), GravityTag.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

        text.setText(wordtoSpan);
    }

    // 유량
    private void setFlowText(TextView text, TextView textRange, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(6) == false) {
            text.setText("");
            return;
        }

        String strFlow = tank.getFlowString();
        String strFlowUnit = tank.getFlowUnit();
        String str = FlowTag + strFlow;

        if(strFlow != "")
            str += strFlowUnit;

        Spannable wordtoSpan = new SpannableString(str);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, FlowTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

        if(strFlow != "") {
            if(tank.isWork()) {
                Pipe pipe = tank.getLinkPipe();
                if (pipe != null) {
                    int nAlarmType = pipe.getAlarmType();
                    if (nAlarmType == 1024 || nAlarmType == 2048)
                        wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 0, 0)), FlowTag.length(), str.length() - strFlowUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                    else
                        wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), FlowTag.length(), str.length() - strFlowUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
            }

            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), str.length() - strFlowUnit.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        }

        text.setText(wordtoSpan);

        // 유량 범위
        textRange.setTextColor(0xfff7c49a);

        if(tank.isWork()) {
            String range = tank.getFlowRangeString();
            if(range != "")
                textRange.setText(" (" + tank.getFlowRangeString() + ", " + tank.getFlowRangeTypeString() + ")");
        }
        else
            textRange.setText("");
    }

    // 유종
    private void setLiquidText(TextView text, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(1) == false) {
            text.setText("");
            return;
        }

        String strType = getLiquidType(tank.getLiquidType());
        String strLiquid = LiquidTag + strType;
        /*if(tank.getSulfuricObserve())
            strLiquid += " (감시중)";*/

        Spannable wordtoSpan = new SpannableString(strLiquid);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, LiquidTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(13, 255, 00)), LiquidTag.length(), strLiquid.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

        text.setText(wordtoSpan);
    }

    // 레벨
    private void setLevelText(TextView text, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(3)) {
            String strLevel = tank.getLevelString();
            String strUnit = "M";
            String str = LevelTag + strLevel;

            String strRange = "";

            if(strLevel != "") {
                str += strUnit;
                if (MenuActivity.Instance().getTankItems(4)) {
                    strRange = " " + tank.getHighLevelText();
                }
            }

            String strTotal = str + strRange;

            Spannable wordtoSpan = new SpannableString(strTotal);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, LevelTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            if(strLevel != "") {
                if (tank.getAlarmID1() != 0)
                    wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 0, 0)), LevelTag.length(), str.length() - strUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                else
                    wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), LevelTag.length(), str.length() - strUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), str.length() - strUnit.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                if (strRange.length() != 0) {
                    wordtoSpan.setSpan(new ForegroundColorSpan(0xfff7c49a), str.length(), strTotal.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                    DisplayMetrics dm = getResources().getDisplayMetrics();
                    int dp = Math.round(8 * dm.density);
                    wordtoSpan.setSpan(new AbsoluteSizeSpan(dp), str.length(), strTotal.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                }
            }

            text.setText(wordtoSpan);
        }
        else
            text.setText("");
    }

    // 온도
    private void setTempText(TextView text, TextView textRange, Tank tank)
    {
        if (MenuActivity.Instance().getTankItems(7)) {
            //String strStatus = tank.getTempStatus();
            String strTemp = tank.getTempString();
            String strUnit = tank.getTempUnit();
            String str = TemperatureTag + strTemp;
            if(strTemp != "")
                str += strUnit;

            Spannable wordtoSpan = new SpannableString(str);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, TemperatureTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            if(strTemp != "") {
                if (tank.getAlarmID2() != 0)
                    wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 0, 0)), TemperatureTag.length(), str.length() - strUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                else
                    wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), TemperatureTag.length(), str.length() - strUnit.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), str.length() - strUnit.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            }

            text.setText(wordtoSpan);
        }
        else
            text.setText("");

        if (MenuActivity.Instance().getTankItems(8)) {
            textRange.setTextColor(0xfff7c49a);

            String strTempRange = tank.getTempRangeText();

            if (strTempRange.length() <= 11)
                textRange.setText("          " + strTempRange);
            else
                textRange.setText("        " + strTempRange);
        }
        else
            textRange.setText("");
    }

    // 사용여부 (입출고 현황)
    private void setAlarmButton(ImageView btnWorkStatus, LayoutAlarmStatus layout, Tank tank)
    {
        ImageView btnAlarm = layout.ivStatus;
        TextView txtTime = layout.tvTime;

        //int nStatus = tank.getStatus();
        int nAlarmID1 = tank.getAlarmID1();
        int nAlarmID2 = tank.getAlarmID2();

        Pipe pipe = tank.getLinkPipe();
        int nPipeAlarm = 0;
        if(pipe != null)
        {
            nPipeAlarm = pipe.getAlarmID();
        }

        if (MenuActivity.Instance().getTankItems(9)) {
            if(nAlarmID1 != 0 || nAlarmID2 != 0 || nPipeAlarm != 0 || (MenuActivity.Instance().getTankItems(11) && tank.getSulfuricLeak()))
                setImage(btnAlarm, R.drawable.alarm_off);
            else {
                if(tank.isWork())
                    setImage(btnAlarm, R.drawable.pipe_normal_pressure);
                else
                    setImage(btnAlarm, R.drawable.tank_normal_status);
            }
        }

        // 작업시간
        if(tank.isWork()) {
            String time = tank.GetWorkTime();
            if (pipe != null) {
                time = pipe.GetWorkTime();
            }

            String[] times = time.split(",");
            if (times.length == 2) {
                int len1 = times[0].length();
                int len2 = times[1].length();

                String strTime = String.format("%s시간 %s분", times[0], times[1]);

                Spannable wordtoSpan = new SpannableString(strTime);
                wordtoSpan.setSpan(new RelativeSizeSpan(0.7f), len1, len1 + 3, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                wordtoSpan.setSpan(new RelativeSizeSpan(0.7f), len1 + 3 + len2, len1 + 3 + len2 + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                txtTime.setText(wordtoSpan);
            }
            else
                txtTime.setText("");
        }
        else
            txtTime.setText("");
        //

        if (tank.getFlow() > FlowStandard)
            setImage(btnWorkStatus, R.drawable.tank_in);
        else if (tank.getFlow() < -FlowStandard)
            setImage(btnWorkStatus, R.drawable.tank_out);
        else
            setImage(btnWorkStatus, R.drawable.tank_in_wait);
    }

    private void setLayerSize()
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View title = findViewById(R.id.imgTankTitle);
        View header = findViewById(R.id.layoutColumnHeader);
        View body = findViewById(R.id.scrollBody);
        //View body = findViewById(R.id.bodyLayout);
        View bottom = findViewById(R.id.bottomLayout);

        float titleHeight = title.getLayoutParams().height;
        float headerHeight = header.getLayoutParams().height;
        float bottomHeight = bottom.getLayoutParams().height;

        ViewGroup.LayoutParams param = body.getLayoutParams();
        int statusBarHeight = MainActivity.getStatusBarHeight(getResources());
        param.height = (int)(fScreenHeight - titleHeight - headerHeight - bottomHeight) - statusBarHeight;
        param.width = (int)(fScreenWidth);

        body.setLayoutParams(param);
    }

    private void initImage(int pipeID, int statusID)
    {
        setImage((ImageView)findViewById(pipeID), R.drawable.item_panel_dot_tank);
        setImage((ImageView)findViewById(statusID), R.drawable.status_wait);
    }

    private void initText(int liquidID, int tempID, int levelID, int gravityID, int remainID, int flowID)
    {
        TextView textLiquid = (TextView)findViewById(liquidID);
        TextView textTemp = (TextView)findViewById(tempID);
        TextView textLevel = (TextView)findViewById(levelID);
        TextView textGravity = (TextView)findViewById(gravityID);
        TextView textRemain = (TextView)findViewById(remainID);
        TextView textFlow = (TextView)findViewById(flowID);

        textLiquid.setText(LiquidTag);
        textTemp.setText(TemperatureTag);
        textLevel.setText(LevelTag);
        textGravity.setText(GravityTag);
        textRemain.setText(RemainTag);
        textFlow.setText(FlowTag);
    }

    private void setImage(ImageView view, int nImageID)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);
        m_mapViewImage.put(view.getId(), nImageID);
    }

    private Tank GetTankFromAlarmOff(View view)
    {
        ViewParent parent = view.getParent();

        if (parent == null)
            return null;

        if (parent instanceof LinearLayout) {
            LinearLayout parentLayout = (LinearLayout)parent;

            int nChildCount = parentLayout.getChildCount();

            for (int i=0;i<nChildCount;i++) {
                View childView = parentLayout.getChildAt(i);
                Tank tank = getTank(childView.getId());

                if (tank != null)
                    return tank;
            }
        }

        return null;
    }

    public void btnAlarmOffClick(View view)
    {
        View parent = (View)view.getParent();
        final Tank tank = GetTankFromAlarmOff(parent);
        //final Tank tank = getTank(parent.getId());
        if (tank == null)
            return;

        final Pipe pipe = tank.getLinkPipe();
        if(tank.getAlarmID1() == 0 && tank.getAlarmID2() == 0 && !tank.getSulfuricLeak() && (pipe == null || pipe.getAlarmID() == 0))
            return;

        // 알람 Off 권한이 있는가?
        if(!MenuActivity.Instance().getTankItems(9))
            return;
        if (MenuActivity.Instance().canAlarmOff() == false) {
            MainActivity.showAlert("알람해제 권한이 없는 사용자입니다.", "알림", this);
            return;
        }

        CreateDlgAlarmClear(tank);
    }

    private void CreateDlgAlarmClear(Tank tank)
    {
        if(m_dlgAlarmClear == null) {
            m_dlgAlarmClear = new DlgAlarmClear(this);
            m_dlgAlarmClear.getWindow().setGravity(Gravity.CENTER);

            m_dlgAlarmClear.setOnDismissListener(new DialogInterface.OnDismissListener() {
                @Override
                public void onDismiss(DialogInterface dialog) {
                    DlgAlarmClear dlg = (DlgAlarmClear)dialog;
                    OnAlarmClear(dlg);
                }
            });
        }

        m_dlgAlarmClear.SetTank(tank);

        m_dlgAlarmClear.show();
    }

    private void OnAlarmClear(DlgAlarmClear dlg)
    {
        int nType = dlg.GetOccurrenceType();
        String strComment = dlg.GetComment();
        Tank tank = dlg.GetTank();
        Pipe pipe = tank.getLinkPipe();

        if(dlg.Result()) {
            /*if(TextUtils.isEmpty(strComment))
            {
                MainActivity.showAlert("알람 해결 내용을 입력해 주세요.", "알림", this);
                return;
            }*/
            if (tank != null) {
                if(tank.getSulfuricLeak())
                    SulfuricLeakAlarmOff(tank, dlg.GetOccurrenceType(), dlg.GetComment());
                if(tank.getAlarmID1() == 0 && tank.getAlarmID2() == 0 && pipe != null)
                    MenuActivity.Instance().PipeAlarmOff(pipe, dlg.GetOccurrenceType(), dlg.GetComment());
                else if(tank.getAlarmID1() != 0 || tank.getAlarmID2() != 0)
                    MenuActivity.Instance().TankAlarmOff(tank, dlg.GetOccurrenceType(), dlg.GetComment());
            } else
                MainActivity.showAlert("배관과 연결되지 않은 탱크입니다.", "알림", this);

            dlg.dismiss();
        }
    }

    private void SulfuricLeakAlarmOff(Tank tank, int OccurType, String comment)
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.CLEAR_SULFURIC_ALARM);

        mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
        mgr.setParameter("TankID", String.valueOf(tank.getID()));
        mgr.setParameter("OccurType", String.valueOf(OccurType));
        mgr.setParameter("Comment", comment);
        mgr.start();
    }

    private Tank getTank(int viewID)
    {
        if (m_mapViewTankIndex.containsKey(viewID))
        {
            int nTankIndex = m_mapViewTankIndex.get(viewID) + 1;
            return MenuActivity.Instance().getTankStatus(nTankIndex);
        }
        /*switch (viewID)
        {
            case R.id.imgTank1WorkStatus:
            case R.id.layout_tankAlarmStatus1:
                return MenuActivity.Instance().getTankStatus(1);

            case R.id.imgTank2WorkStatus:
            case R.id.layout_tankAlarmStatus2:
                return MenuActivity.Instance().getTankStatus(2);

            case R.id.imgTank3WorkStatus:
            case R.id.layout_tankAlarmStatus3:
                return MenuActivity.Instance().getTankStatus(3);

            case R.id.imgTank4WorkStatus:
            case R.id.layout_tankAlarmStatus4:
                return MenuActivity.Instance().getTankStatus(4);

            case R.id.imgTank5WorkStatus:
            case R.id.layout_tankAlarmStatus5:
                return MenuActivity.Instance().getTankStatus(5);

            case R.id.imgTank6WorkStatus:
            case R.id.layout_tankAlarmStatus6:
                return MenuActivity.Instance().getTankStatus(6);

            case R.id.imgTank7WorkStatus:
            case R.id.layout_tankAlarmStatus7:
                return MenuActivity.Instance().getTankStatus(7);

            case R.id.imgTank8WorkStatus:
            case R.id.layout_tankAlarmStatus8:
                return MenuActivity.Instance().getTankStatus(8);

            case R.id.imgTank9WorkStatus:
            case R.id.layout_tankAlarmStatus9:
                return MenuActivity.Instance().getTankStatus(9);

            case R.id.imgTank10WorkStatus:
            case R.id.layout_tankAlarmStatus10:
                return MenuActivity.Instance().getTankStatus(10);

            case R.id.imgTank11WorkStatus:
            case R.id.layout_tankAlarmStatus11:
                return MenuActivity.Instance().getTankStatus(11);

            case R.id.imgTank12WorkStatus:
            case R.id.layout_tankAlarmStatus12:
                return MenuActivity.Instance().getTankStatus(12);

            case R.id.imgTank13WorkStatus:
            case R.id.layout_tankAlarmStatus13:
                return MenuActivity.Instance().getTankStatus(13);

            case R.id.imgTank14WorkStatus:
            case R.id.layout_tankAlarmStatus14:
                return MenuActivity.Instance().getTankStatus(14);

            case R.id.imgTank15WorkStatus:
            case R.id.layout_tankAlarmStatus15:
                return MenuActivity.Instance().getTankStatus(15);

            case R.id.imgTank16WorkStatus:
            case R.id.layout_tankAlarmStatus16:
                return MenuActivity.Instance().getTankStatus(16);

            case R.id.imgTank17WorkStatus:
            case R.id.layout_tankAlarmStatus17:
                return MenuActivity.Instance().getTankStatus(17);

            case R.id.imgTank18WorkStatus:
            case R.id.layout_tankAlarmStatus18:
                return MenuActivity.Instance().getTankStatus(18);

            case R.id.imgTank19WorkStatus:
            case R.id.layout_tankAlarmStatus19:
                return MenuActivity.Instance().getTankStatus(19);

            case R.id.imgTank20WorkStatus:
            case R.id.layout_tankAlarmStatus20:
                return MenuActivity.Instance().getTankStatus(20);

            case R.id.imgTank21WorkStatus:
            case R.id.layout_tankAlarmStatus21:
                return MenuActivity.Instance().getTankStatus(21);

            case R.id.imgTank22WorkStatus:
            case R.id.layout_tankAlarmStatus22:
                return MenuActivity.Instance().getTankStatus(22);
        }*/

        return null;
    }

    public void onNotify(String strTitle, String strMessage)
    {
        if (strTitle.equals("PipeAlarm"))
        {
            MenuActivity.Instance().setInitTag(strTitle);
            finish();
        }
    }

    private class TankTextSet
    {
        private TextView m_nameText = null;
        private TextView m_capacityText = null;
        private TextView m_linkText = null;
        private TextView m_linkText2 = null;
        private TextView m_liquidTypeText = null;
        private TextView m_levelText = null;
        //private TextView m_levelRangeText = null;
        private TextView m_tempText = null;
        private TextView m_tempRangeText = null;
        private TextView m_gravityText = null;
        private TextView m_massText = null;
        private TextView m_flowText = null;
        private TextView m_flowRangeText = null;
        private ImageView m_btnWorkStatus = null;
        //private ImageView m_btnAlarmStatus = null;
        private ImageView m_panel = null;

        public TextView getNameText()
        {
            return m_nameText;
        }
        public void setNameText(TextView view) { m_nameText = view; }

        public TextView getCapacityText() { return m_capacityText; }
        public void setCapacityText(TextView view) { m_capacityText = view; }

        public TextView getLinkText() { return m_linkText; }
        public void setLinkText(TextView view) { m_linkText = view; }

        public TextView getLinkText2() { return m_linkText2; }
        public void setLinkText2(TextView view) { m_linkText2 = view; }

        public TextView getLiquidTypeText()
        {
            return m_liquidTypeText;
        }
        public void setLiquidTypeText(TextView view) { m_liquidTypeText = view; }

        public TextView getLevelText()
        {
            return m_levelText;
        }
        public void setLevelText(TextView view) { m_levelText = view; }

        /*public TextView getLevelRangeText()
        {
            return m_levelRangeText;
        }*/

        public TextView getTempText()
        {
            return m_tempText;
        }
        public void setTempText(TextView view) { m_tempText = view; }

        public TextView getTempRangeText() { return m_tempRangeText; }
        public void setTempRangeText(TextView view) { m_tempRangeText = view; }

        public TextView getGravityText()
        {
            return m_gravityText;
        }
        public void setGravityText(TextView view) { m_gravityText = view; }

        public TextView getMassText()
        {
            return m_massText;
        }
        public void setMassText(TextView view) { m_massText = view; }

        public TextView getFlowText()
        {
            return m_flowText;
        }
        public void setFlowText(TextView view) { m_flowText = view; }

        public TextView getFlowRangeText()
        {
            return m_flowRangeText;
        }
        public void setFlowRangeText(TextView view) { m_flowRangeText = view; }

        public ImageView getWorkStatusButton()
        {
            return m_btnWorkStatus;
        }
        public void setWorkStatusButton(ImageView view) { m_btnWorkStatus = view; }

        /*public ImageView getAlarmStatusButton()
        {
            return m_btnAlarmStatus;
        }*/

        public ImageView getPanel()
        {
            return m_panel;
        }
        public void setPanel(ImageView view) { m_panel = view; }

        /*public TankTextSet(int index)
        {
            switch (index)
            {
                case 1:
                    m_nameText = (TextView)findViewById(R.id.textTank1Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank1Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank1Link);

                    m_liquidTypeText = (TextView)findViewById(R.id.textTank1Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank1Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank1LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank1Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank1TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank1Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank1Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank1Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank1FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank1WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank1AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank1);
                    break;

                case 2:
                    m_nameText = (TextView)findViewById(R.id.textTank2Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank2Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank2Link);

                    m_liquidTypeText = (TextView)findViewById(R.id.textTank2Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank2Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank2LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank2Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank2TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank2Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank2Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank2Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank2FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank2WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank2AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank2);
                    break;

                case 3:
                    m_nameText = (TextView)findViewById(R.id.textTank3Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank3Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank3Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank3Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank3Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank3LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank3Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank3TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank3Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank3Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank3Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank3FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank3WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank3AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank3);
                    break;

                case 4:
                    m_nameText = (TextView)findViewById(R.id.textTank4Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank4Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank4Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank4Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank4Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank4LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank4Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank4TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank4Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank4Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank4Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank4FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank4WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank4AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank4);
                    break;

                case 5:
                    m_nameText = (TextView)findViewById(R.id.textTank5Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank5Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank5Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank5Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank5Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank5LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank5Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank5TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank5Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank5Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank5Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank5FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank5WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank5AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank5);
                    break;

                case 6:
                    m_nameText = (TextView)findViewById(R.id.textTank6Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank6Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank6Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank6Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank6Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank6LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank6Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank6TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank6Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank6Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank6Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank6FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank6WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank6AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank6);
                    break;

                case 7:
                    m_nameText = (TextView)findViewById(R.id.textTank7Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank7Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank7Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank7Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank7Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank7LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank7Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank7TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank7Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank7Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank7Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank7FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank7WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank7AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank7);
                    break;

                case 8:
                    m_nameText = (TextView)findViewById(R.id.textTank8Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank8Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank8Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank8Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank8Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank8LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank8Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank8TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank8Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank8Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank8Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank8FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank8WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank8AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank8);
                    break;

                case 9:
                    m_nameText = (TextView)findViewById(R.id.textTank9Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank9Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank9Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank9Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank9Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank9LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank9Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank9TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank9Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank9Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank9Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank9FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank9WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank9AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank9);
                    break;

                case 10:
                    m_nameText = (TextView)findViewById(R.id.textTank10Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank10Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank10Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank10Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank10Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank10LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank10Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank10TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank10Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank10Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank10Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank10FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank10WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank10AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank10);
                    break;

                case 11:
                    m_nameText = (TextView)findViewById(R.id.textTank11Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank11Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank11Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank11Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank11Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank11LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank11Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank11TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank11Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank11Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank11Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank11FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank11WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank11AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank11);
                    break;

                case 12:
                    m_nameText = (TextView)findViewById(R.id.textTank12Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank12Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank12Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank12Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank12Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank12LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank12Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank12TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank12Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank12Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank12Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank12FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank12WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank12AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank12);
                    break;

                case 13:
                    m_nameText = (TextView)findViewById(R.id.textTank13Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank13Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank13Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank13Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank13Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank13LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank13Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank13TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank13Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank13Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank13Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank13FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank13WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank13AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank13);
                    break;

                case 14:
                    m_nameText = (TextView)findViewById(R.id.textTank14Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank14Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank14Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank14Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank14Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank14LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank14Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank14TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank14Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank14Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank14Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank14FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank14WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank14AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank14);
                    break;

                case 15:
                    m_nameText = (TextView)findViewById(R.id.textTank15Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank15Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank15Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank15Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank15Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank15LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank15Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank15TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank15Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank15Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank15Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank15FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank15WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank15AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank15);
                    break;

                case 16:
                    m_nameText = (TextView)findViewById(R.id.textTank16Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank16Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank16Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank16Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank16Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank16LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank16Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank16TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank16Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank16Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank16Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank16FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank16WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank16AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank16);
                    break;

                case 17:
                    m_nameText = (TextView)findViewById(R.id.textTank17Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank17Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank17Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank17Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank17Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank17LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank17Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank17TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank17Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank17Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank17Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank17FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank17WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank17AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank17);
                    break;

                case 18:
                    m_nameText = (TextView)findViewById(R.id.textTank18Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank18Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank18Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank18Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank18Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank18LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank18Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank18TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank18Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank18Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank18Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank18FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank18WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank18AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank18);
                    break;

                case 19:
                    m_nameText = (TextView)findViewById(R.id.textTank19Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank19Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank19Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank19Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank19Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank19LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank19Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank19TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank19Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank19Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank19Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank19FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank19WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank19AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank19);
                    break;

                case 20:
                    m_nameText = (TextView)findViewById(R.id.textTank20Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank20Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank20Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank20Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank20Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank20LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank20Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank20TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank20Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank20Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank20Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank20FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank20WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank20AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank20);
                    break;

                case 21:
                    m_nameText = (TextView)findViewById(R.id.textTank21Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank21Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank21Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank21Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank21Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank21LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank21Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank21TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank21Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank21Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank21Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank21FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank21WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank21AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank21);
                    break;

                case 22:
                    m_nameText = (TextView)findViewById(R.id.textTank22Name);
                    m_capacityText = (TextView)findViewById(R.id.textTank22Capacity);
                    m_linkText = (TextView)findViewById(R.id.textTank22Link);
                    m_liquidTypeText = (TextView)findViewById(R.id.textTank22Liquid);
                    m_levelText = (TextView)findViewById(R.id.textTank22Level);
                    //m_levelRangeText = (TextView)findViewById(R.id.textTank22LevelCapacity);
                    m_tempText = (TextView)findViewById(R.id.textTank22Temp);
                    m_tempRangeText = (TextView)findViewById(R.id.textTank22TempRange);
                    m_gravityText = (TextView)findViewById(R.id.textTank22Gravity);
                    m_massText = (TextView)findViewById(R.id.textTank22Remain);
                    m_flowText = (TextView)findViewById(R.id.textTank22Flow);
                    m_flowRangeText = (TextView)findViewById(R.id.textTank22FlowRange);
                    m_btnWorkStatus = (ImageView)findViewById(R.id.imgTank22WorkStatus);
                    //m_btnAlarmStatus = (ImageView)findViewById(R.id.imgTank22AlarmStatus);

                    m_panel = (ImageView)findViewById(R.id.imgTank22);
                    break;
            }
        }*/
    }

    public class LayoutAlarmStatus
    {
        public ImageView ivStatus = null;
        public TextView tvTime = null;

        public LayoutAlarmStatus(ImageView iv, TextView tv)
        {
            ivStatus = iv;
            tvTime = tv;
        }
    }
}
