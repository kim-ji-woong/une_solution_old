package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.Drawable;
import android.support.v4.app.ActivityManagerCompat;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Layout;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.GridLayout;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.ScrollView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.load.DecodeFormat;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import kr.co.une.energyindustrye_sop.photoview.PhotoView;
import kr.co.une.energyindustrye_sop.utility.ImageHelper;

public class ActionRealTimeActivity extends AppCompatActivity implements FCMReceiver {

    private int m_nSelectedSOPStep = -1;
    private int m_nSelectedTeam = -1;
    private int m_nSelectedTopMenu = -1;
    private int m_nSelectedMission = -1;

    private int m_nImageWidth = -1;
    private ImageHelper m_imgHelper = new ImageHelper();

    private HashMap<RelativeLayout, Boolean> m_mapMissionSelected = new HashMap<RelativeLayout, Boolean>();
    private List<RelativeLayout> m_myMissionList = new ArrayList();
    private List<RelativeLayout> m_completeMissionList = new ArrayList();

    private boolean parentIsMainActivity = true;

    private boolean m_disasterEquipVisible = false;
    private int m_nGridRowHeight = -1;
    private int m_nGridHeaderHeight = -1;
    private int m_nGridMinHeight = -1;

    private class DisasterEquipment
    {
        public String equipmentName = "";
        public String equipmentUse = "";
        public String equipmentCount = "";
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_real_time);

        initImageRatio();
        initTopMenu();
        initLayout();
        initLeftImages();
        initBodyImageLayout();
        initSOPStepImages();
        initPageLayout();

        initMyMissionPage();
        initSendSOSPage();
        initMapPage();

        onRealTimeTeamClick(findViewById(R.id.realTimeLeftControlCenter));
        onStepClick(findViewById(R.id.btnRealTimeSOPStep1));

        Intent intent = getIntent();
        String menuName = intent.getStringExtra("InitMenu");

        if (menuName.equals("SOP"))
            onTopMenuClick(findViewById(R.id.btnRealTimeSOP));
        else if (menuName.equals("MyMission"))
            onTopMenuClick(findViewById(R.id.btnMyMission));
        else if (menuName.equals("SendSOS"))
            onTopMenuClick(findViewById(R.id.btnSendSOS));
        else if (menuName.equals("Map"))
            onTopMenuClick(findViewById(R.id.btnMap));

        String strActionStepID = intent.getStringExtra("ActionStepID");
        String strQuickButtonID = intent.getStringExtra("QuickButtonID");

        if (strActionStepID != null && strQuickButtonID != null) {
            parentIsMainActivity = false;
            readSOPData(strActionStepID, strQuickButtonID);
        }
        else
            parentIsMainActivity = true;
    }

    private void readSOPData(String strActionStepID, String strQuickButtonID)
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.REQUEST_SOP_DATA);
        mgr.setParameter("ActionStepID", strActionStepID);
        mgr.setParameter("QuickButtonID", strQuickButtonID);
        mgr.setParameter("SiteID", getApplicationContext().getString(R.string.site_id));
        mgr.setParameter("SerialNumber", WebManager.getDeviceSerialNumber());
        mgr.start();

        int nTimeOut = 5000, delay = 500, sum = 0;

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
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS) {
            List<String> results = mgr.getResultSet();

            LinearLayout layoutMissions = (LinearLayout)findViewById(R.id.layoutMissions);

            if (results.size() > 1)
            {
                layoutMissions.setVisibility(View.VISIBLE);

                int nSOPDataCount = results.size() - 1;
                int nChildCount = layoutMissions.getChildCount();

                if (nSOPDataCount > nChildCount)
                {
                    for (int i=0;i<nChildCount;i++)
                    {
                        RelativeLayout layout = (RelativeLayout)layoutMissions.getChildAt(i);
                        layout.setVisibility(View.VISIBLE);

                        selectLayout(layout, false);

                        String strProcessID = "", strMissionText = "";
                        String strData = results.get(i + 1);

                        int nIndex = strData.indexOf('_');

                        if (nIndex < 0)
                            continue;

                        strProcessID = strData.substring(0, nIndex);
                        strMissionText = strData.substring(nIndex + 1);

                        TextView text = (TextView)layout.getChildAt(1);
                        text.setText(strMissionText);
                        text.setTag(strProcessID);
                        //text.setText(results.get(i + 1));
                    }

                    /*for (int i=nChildCount;i<nSOPDataCount;i++) {
                        Drawable newDrawable = dr.getConstantState().newDrawable().mutate();

                        ImageView image = new ImageView(imgMission.getContext());
                        image.setId(R.id.btnMission + i);
                        image.setImageDrawable(newDrawable);
                        Glide.with(this).load(R.drawable.mission_bar_normal).into(image);

                        layoutMissions.addView(image);
                        image.setVisibility(View.VISIBLE);
                    }*/
                }
                else if (nSOPDataCount < nChildCount)
                {
                    for (int i=0;i<nSOPDataCount;i++)
                    {
                        RelativeLayout layout = (RelativeLayout)layoutMissions.getChildAt(i);
                        layout.setVisibility(View.VISIBLE);

                        selectLayout(layout, false);

                        String strProcessID = "", strMissionText = "";
                        String strData = results.get(i + 1);

                        int nIndex = strData.indexOf('_');

                        if (nIndex < 0)
                            continue;

                        strProcessID = strData.substring(0, nIndex);
                        strMissionText = strData.substring(nIndex + 1);

                        TextView text = (TextView)layout.getChildAt(1);
                        text.setText(strMissionText);
                        text.setTag(strProcessID);
                        //text.setText(results.get(i + 1));
                    }

                    for (int i=nSOPDataCount;i<nChildCount;i++)
                    {
                        layoutMissions.removeViewAt(nSOPDataCount);
                    }
                }
            }
            else
                layoutMissions.setVisibility(View.INVISIBLE);
        }
    }

    private void selectLayout(RelativeLayout layout, boolean isSelected)
    {
        ImageView img = (ImageView)layout.getChildAt(0);
        TextView text = (TextView)layout.getChildAt(1);

        text.setTypeface(null, Typeface.BOLD);

        if (isSelected) {
            Glide.with(this).load(R.drawable.mission_bar_selected).into(img);
            text.setTextColor(0xffffffff);
        }
        else {
            Glide.with(this).load(R.drawable.mission_bar_normal).into(img);
            text.setTextColor(0xff871919);
        }

        m_mapMissionSelected.put(layout, isSelected);
    }

    private boolean isSelectedLayout(RelativeLayout layout)
    {
        Boolean isSelected = m_mapMissionSelected.get(layout);

        if (isSelected == null)
            return false;

        return isSelected.booleanValue();
    }

    private void initSendSOSPage()
    {
        ImageView logo = (ImageView)findViewById(R.id.realTimeSendSOSLogo);

        int stScreenWidth = MainActivity.getStandardScreenWidth();
        int stScreenHeight = MainActivity.getStandardScreenHeight();
        int stTopPadding = 152, stLeftPadding = 375, stRightPadding = 375;

        float screenWidth = MainActivity.getScreenWidth(this);
        float screenHeight = MainActivity.getScreenHeight(this);

        int left = (int)(stLeftPadding * screenWidth / stScreenWidth);
        int right = (int)(stRightPadding * screenWidth / stScreenWidth);
        int top = (int)(stTopPadding * screenHeight / stScreenHeight);
        int bottom = 0;

        logo.setPadding(left, top, right, bottom);
        Glide.with(this).load(R.drawable.logo).into(logo);

        GridLayout editLayout = (GridLayout)findViewById(R.id.editSOSLayout);
        ImageView type = (ImageView)findViewById(R.id.sos_type);
        ImageView count = (ImageView)findViewById(R.id.request_count);

        stTopPadding = 235;
        stLeftPadding = 179;
        stRightPadding = 179;

        left = (int)(stLeftPadding * screenWidth / stScreenWidth);
        right = (int)(stRightPadding * screenWidth / stScreenWidth);
        top = (int)(stTopPadding * screenHeight / stScreenHeight);

        editLayout.setPadding(left, top, right, bottom);

        Glide.with(this).load(R.drawable.sos_type).into(type);
        Glide.with(this).load(R.drawable.request_count).into(count);

        /*ImageHelper helper = new ImageHelper();
        long originalSize = helper.readImageOriginalSize(R.drawable.request_normal, getResources());
        int btnRequestOriginWidth = (int)(originalSize >> 32);
        int btnRequestOriginHeight = (int)(originalSize & 0xffffffff);

        int titleHeight =  findViewById(R.id.realTimeMapTitle).getLayoutParams().height;
        int btnHeight = titleHeight / 2;
        int btnWidth = btnHeight * btnRequestOriginWidth / btnRequestOriginHeight;*/

        stTopPadding = 235;
        stLeftPadding = 540;
        stRightPadding = 540;

        top = (int)(stTopPadding * screenHeight / stScreenHeight);
        left = 0;//(int)(stLeftPadding * screenWidth / stScreenWidth);
        right = 0;//(int)(stRightPadding * screenWidth / stScreenWidth);

        ImageButton btnRequest = (ImageButton)findViewById(R.id.btnRequest);
        findViewById(R.id.requestButtonLayout).setPadding(left, top, right, 0);
        //findViewById(R.id.requestButtonLayout).setY(screenHeight - btnHeight * 2);

        //btnRequest.getLayoutParams().width = (int)(screenWidth - left - right);
        Glide.with(this).load(R.drawable.request_normal).into(btnRequest);
    }

    private void initMapPage()
    {
        PhotoView map = (PhotoView)findViewById(R.id.realTimeMapImage);
        map.setMaximumScale(map.getMaximumScale() * 3);
        Glide.with(this).load(R.drawable.map_image).into(map);
    }

    private void initMyMissionPage()
    {
        ImageHelper helper = new ImageHelper();
        long originalSize = helper.readImageOriginalSize(R.drawable.mm_my_mission_normal, getResources());

        int originalImageWidth = (int)(originalSize >> 32);
        int originalImageHeight = (int)(originalSize & 0xffffffff);

        int titleHeight =  findViewById(R.id.realTimeMapTitle).getLayoutParams().height;
        int imageWidth = titleHeight * originalImageWidth / originalImageHeight;
        int imageHeight = titleHeight;
        int spaceHor = 3;

        ImageView myMission = (ImageView)findViewById(R.id.realTimeMMMyMission);
        ImageView completeMission = (ImageView)findViewById(R.id.realTimeMMCompleteMission);

        ViewGroup.LayoutParams myMissionParams = myMission.getLayoutParams();
        ViewGroup.LayoutParams completeMissionParams = completeMission.getLayoutParams();

        float screenWidth = MainActivity.getScreenWidth(this);
        float screenHeight = MainActivity.getScreenHeight(this);

        float x1 = (screenWidth - imageWidth - imageWidth - spaceHor) / 2;
        float x2 = x1 + spaceHor + imageWidth;
        float y = titleHeight / 2;

        myMissionParams.width = imageWidth;
        myMissionParams.height = imageHeight;
        myMission.setX(x1);
        myMission.setY(y);

        completeMissionParams.width = imageWidth;
        completeMissionParams.height = imageHeight;
        completeMission.setX(x1 + spaceHor);
        completeMission.setY(y);

        onMissionTypeClick(myMission);
    }

    private void initImageRatio()
    {
        Resources res = getResources();

        m_imgHelper.readImageOriginalSize(R.drawable.realtime_sop_body, res);
    }

    private void initSOPStepImages()
    {
        setSOPStepImage(R.id.btnRealTimeSOPStep1, false);
        setSOPStepImage(R.id.btnRealTimeSOPStep2, false);
        setSOPStepImage(R.id.btnRealTimeSOPStep3, false);
        setSOPStepImage(R.id.btnRealTimeSOPStep4, false);
    }

    private void setSOPStepImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.btnRealTimeSOPStep1)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_realtime_sop_step1_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_realtime_sop_step1_normal).into(view);
        }
        else if (nID == R.id.btnRealTimeSOPStep2)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_realtime_sop_step2_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_realtime_sop_step2_normal).into(view);
        }
        else if (nID == R.id.btnRealTimeSOPStep3)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_realtime_sop_step3_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_realtime_sop_step3_normal).into(view);
        }
        else if (nID == R.id.btnRealTimeSOPStep4)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_realtime_sop_step4_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_realtime_sop_step4_normal).into(view);
        }
        else
            return;
    }

    private void setSOPTeamImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.realTimeLeftControlCenter)
        {
            if (selected)
                Glide.with(this).load(R.drawable.red_control_center_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.red_control_center_normal).into(view);
        }
        else if (nID == R.id.realTimeLeftFacility)
        {
            if (selected)
                Glide.with(this).load(R.drawable.red_facility_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.red_facility_normal).into(view);
        }
        else if (nID == R.id.realTimeLeftLocalBoss)
        {
            if (selected)
                Glide.with(this).load(R.drawable.red_local_boss_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.red_local_boss_normal).into(view);
        }
        else if (nID == R.id.realTimeLeftSafety)
        {
            if (selected)
                Glide.with(this).load(R.drawable.red_safety_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.red_safety_normal).into(view);
        }
        else if (nID == R.id.realTimeLeftCS)
        {
            if (selected)
                Glide.with(this).load(R.drawable.red_cs_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.red_cs_normal).into(view);
        }
    }

    private void setTopMenuImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.btnRealTimeSOP)
        {
            if (selected) {
                Glide.with(this).load(R.drawable.sop_clicked).into(view);
                findViewById(R.id.realTimePageSOP).setVisibility(View.VISIBLE);
            }
            else {
                Glide.with(this).load(R.drawable.sop_normal).into(view);
                findViewById(R.id.realTimePageSOP).setVisibility(View.INVISIBLE);
            }
        }
        else if (nID == R.id.btnMyMission)
        {
            if (selected) {
                Glide.with(this).load(R.drawable.my_mission_clicked).into(view);
                findViewById(R.id.realTimePageMyMission).setVisibility(View.VISIBLE);
            }
            else {
                Glide.with(this).load(R.drawable.my_mission_normal).into(view);
                findViewById(R.id.realTimePageMyMission).setVisibility(View.INVISIBLE);
            }
        }
        else if (nID == R.id.btnSendSOS)
        {
            if (selected) {
                Glide.with(this).load(R.drawable.send_sos_clicked).into(view);
                findViewById(R.id.realTimePageSendSOS).setVisibility(View.VISIBLE);
            }
            else {
                Glide.with(this).load(R.drawable.send_sos_normal).into(view);
                findViewById(R.id.realTimePageSendSOS).setVisibility(View.INVISIBLE);
            }
        }
        else if (nID == R.id.btnMap)
        {
            if (selected) {
                Glide.with(this).load(R.drawable.map_clicked).into(view);
                findViewById(R.id.realTimePageMap).setVisibility(View.VISIBLE);
            }
            else {
                Glide.with(this).load(R.drawable.map_normal).into(view);
                findViewById(R.id.realTimePageMap).setVisibility(View.INVISIBLE);
            }

            showDisasterEquipment();
        }
    }

    private void showDisasterEquipment()
    {
        if (m_nGridRowHeight < 0) {
            TextView textName = (TextView) findViewById(R.id.columnEquipName);
            m_nGridRowHeight = textName.getLayoutParams().height;

            LinearLayout header = (LinearLayout)findViewById(R.id.disasterEquipGridHead);
            m_nGridHeaderHeight = header.getLayoutParams().height;

            LinearLayout disasterEquipLayout = (LinearLayout)findViewById(R.id.disasterEquipLayout);
            m_nGridMinHeight = disasterEquipLayout.getLayoutParams().height;
        }

        TextView btnBell = (TextView)findViewById(R.id.btnDisasterEquip);
        GridLayout grid = (GridLayout)findViewById(R.id.disasterEquipGrid);

        grid.removeAllViews();

        if (m_disasterEquipVisible) {
            // DB로부터 장비 리스트를 얻어온다.
            List<DisasterEquipment> equipmentList = getDisasterEquipmentList();

            if (equipmentList == null) {
                btnBell.setText("▲");

                LinearLayout disasterEquipLayout = (LinearLayout) findViewById(R.id.disasterEquipLayout);
                m_disasterEquipVisible = false;
                disasterEquipLayout.setVisibility(View.INVISIBLE);
                disasterEquipLayout.getLayoutParams().height = m_nGridMinHeight;

                return;
            }

            if (m_disasterEquipVisible)
                btnBell.setText("▼");
            else
                btnBell.setText("▲");

            int nEquipCount = equipmentList.size();

            grid.setRowCount(nEquipCount + 1);

            for (int i = 0; i < nEquipCount; i++) {
                DisasterEquipment equipment = equipmentList.get(i);
                addDiasterEquipment(grid, equipment.equipmentName, equipment.equipmentUse, equipment.equipmentCount);
            }

            addDiasterEquipment(grid, "", "", "");
        }
        else
            btnBell.setText("▲");
    }

    private List<DisasterEquipment> getDisasterEquipmentList()
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.REQUEST_DISASTER_EQUIPMENT_LIST);
        mgr.start();

        int nTimeOut = 5000, delay = 500, sum = 0;

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
            }
        }

        if (mgr.getResult() == WebManager.ResultType.SUCCESS) {
            List<DisasterEquipment> equipmentList = new ArrayList();
            List<String> results = mgr.getResultSet();

            if (results.size() >= 2) {
                for (int i = 0; i < results.size() - 1; i += 2) {
                    DisasterEquipment equip = new DisasterEquipment();

                    equip.equipmentName = results.get(i);
                    equip.equipmentCount = results.get(i + 1);

                    if (equip.equipmentName.equals("방독면") || equip.equipmentName.equals("정화통"))
                        equip.equipmentUse = "호흡기보호";
                    else if (equip.equipmentName.contains("방제"))
                        equip.equipmentUse = "방재장비";
                    else
                        equip.equipmentUse = "인체보호";

                    equipmentList.add(equip);
                }
            }

            return equipmentList;
        }

        return null;
    }

    private void addDiasterEquipment(GridLayout grid, String strName, String strUse, String strCount)
    {
        TextView text1 = (TextView)findViewById(R.id.equipName);
        TextView text2 = (TextView)findViewById(R.id.equipUse);
        TextView text3 = (TextView)findViewById(R.id.equipCount);

        int nWidth1 = text1.getWidth();
        int nWidth2 = text2.getWidth();
        int nWidth3 = text3.getWidth();

        addDisasterEquipmentText(grid, strName, nWidth1);
        addDisasterEquipmentText(grid, strUse, nWidth2);
        addDisasterEquipmentText(grid, strCount, nWidth3);
    }

    private void addDisasterEquipmentText(GridLayout grid, String str, int nWidth)
    {
        TextView text = new TextView(this);

        ViewGroup.LayoutParams param = new ViewGroup.LayoutParams(nWidth, m_nGridRowHeight);

        text.setLayoutParams(param);
        text.setGravity(Gravity.CENTER);
        text.setText(str);
        text.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 20);
        text.setTextColor(getResources().getColor(R.color.colorGridFore));
        text.setBackgroundColor(getResources().getColor(R.color.colorGridBack));

        grid.addView(text);
    }

    private void initBodyImageLayout()
    {
        ImageView imgBody = (ImageView)findViewById(R.id.actionRealTimeImage);
        View bottomMenu = findViewById(R.id.actionRealTimeBottomMenuPanel);
        View title = findViewById(R.id.imgTitle);
        View imgPanel = findViewById(R.id.actionRealTimeBodyPanel);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);
        int menuHeight = bottomMenu.getLayoutParams().height;
        int titleHeight = title.getLayoutParams().height;

        // 표준모델
        int stTopPadding = 206, stLeftPadding = stTopPadding / 4;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = 0;
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight);
        int bottom = 0;

        ViewGroup.LayoutParams panelParams = imgPanel.getLayoutParams();
        int leftPanelWidth = findViewById(R.id.actionRealTimeLeftPanel).getLayoutParams().width;

        imgPanel.setPadding(left, top, right, bottom);

        panelParams.width = (int)fScreenWidth - leftPanelWidth;
        panelParams.height = (int)fScreenHeight - titleHeight - menuHeight;

        m_nImageWidth = panelParams.width - left;
        //ViewGroup.LayoutParams imgParams = imgBody.getLayoutParams();
        //imgParams.width =  panelParams.width - left;
        //imgParams.height = imgParams.width * 1189 / 1117;

        bottomMenu.getLayoutParams().width = (int)fScreenWidth;

        float bottomY = menuHeight * (-0.4f);
        //float bottomY = (int)(fScreenHeight - menuHeight * 1.4f);
        bottomMenu.setX(0.0f);
        bottomMenu.setY(bottomY);
    }

    private void initLeftImages()
    {
        View leftPanal = findViewById(R.id.actionRealTimeLeftPanel);
        int layoutWidth = leftPanal.getLayoutParams().width;

        int paddingBottom = findViewById(R.id.realTimeLeftControlCenter).getPaddingBottom();
        leftPanal.setX(-paddingBottom);

        ImageHelper helper = new ImageHelper();
        long originalSize = helper.readImageOriginalSize(R.drawable.saf_pol_normal, getResources());

        int nOriginalWidth = -1, nOriginalHeight = -1;

        if (originalSize > 0)
        {
            nOriginalWidth = (int)(originalSize >> 32);
            nOriginalHeight = (int)(originalSize & 0xffffffff);
        }

        setLeftimage(R.id.realTimeLeftControlCenter, R.drawable.red_control_center_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.realTimeLeftFacility, R.drawable.red_facility_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.realTimeLeftLocalBoss, R.drawable.red_local_boss_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.realTimeLeftSafety, R.drawable.red_safety_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.realTimeLeftCS, R.drawable.red_cs_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
    }

    private void setLeftimage(int nViewID, int nImageID, int width, int nOriginalWidth, int nOriginalHeight)
    {
        ImageView image = (ImageView)findViewById(nViewID);
        Glide.with(this).load(nImageID).into(image);

        ViewGroup.LayoutParams param = image.getLayoutParams();

        param.width = width;

        if (nOriginalWidth > 0 && nOriginalHeight > 0)
            param.height = width * nOriginalHeight / nOriginalWidth;
    }

    private void initLayout()
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View imgTitle = findViewById(R.id.imgTitle);

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.actionRealTimeBody);
        ViewGroup.LayoutParams bodyParams = bodyLayout.getLayoutParams();

        int nTitleHeight = imgTitle.getLayoutParams().height;
        int nBottomMenuHeight = findViewById(R.id.actionRealTimeBottomMenuPanel).getLayoutParams().height;

        bodyLayout.setX(0.0f);
        bodyLayout.setY(0.0f);
        bodyParams.width = (int)fScreenWidth;
        bodyParams.height = (int)fScreenHeight - nTitleHeight - nBottomMenuHeight;
    }

    private void initPageLayout()
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View imgTitle = findViewById(R.id.imgTitle);
        int nTitleHeight = imgTitle.getLayoutParams().height;

        initPageLayout(R.id.realTimePageSOP, fScreenWidth, fScreenHeight, nTitleHeight);
        initPageLayout(R.id.realTimePageMyMission, fScreenWidth, fScreenHeight, nTitleHeight);
        initPageLayout(R.id.realTimePageSendSOS, fScreenWidth, fScreenHeight, nTitleHeight);
        initPageLayout(R.id.realTimePageMap, fScreenWidth, fScreenHeight, nTitleHeight);
    }

    private void initPageLayout(int nPageID, float fScreenWidth, float fScreenHeight, int nTitleHeight)
    {
        LinearLayout page = (LinearLayout)findViewById(nPageID);
        ViewGroup.LayoutParams pageParams = page.getLayoutParams();

        page.setVisibility(View.INVISIBLE);
        page.setX(0.0f);
        page.setY(nTitleHeight);
        pageParams.width = (int)fScreenWidth;
        pageParams.height = (int)fScreenHeight - nTitleHeight;
    }

    private void initTopMenu()
    {
        View title = findViewById(R.id.imgTitle);
        View topMenu = findViewById(R.id.actionRealTimeTopMenuPanel);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);
        int titleHeight = title.getLayoutParams().height;

        topMenu.setX(0.0f);
        topMenu.setY(titleHeight / 2);
    }

    public void onStepClick(View v)
    {
        int nID = v.getId();

        if (nID == m_nSelectedSOPStep)
            return;

        if (m_nSelectedSOPStep >= 0)
            setSOPStepImage(m_nSelectedSOPStep, false);

        m_nSelectedSOPStep = nID;
        setSOPStepImage(m_nSelectedSOPStep, true);

        setSOPImage();
    }

    public void onRealTimeTeamClick(View v)
    {
        int nID = v.getId();

        if (nID == m_nSelectedTeam)
            return;

        if (m_nSelectedTeam >= 0)
            setSOPTeamImage(m_nSelectedTeam, false);

        m_nSelectedTeam = nID;
        setSOPTeamImage(m_nSelectedTeam, true);

        setSOPImage();
    }

    private void setSOPImage()
    {
        int nImageID = 0;

        if (m_nSelectedTeam == R.id.realTimeLeftControlCenter)
        {
            if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep1)
                nImageID = R.drawable.red_control_center_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep2)
                nImageID = R.drawable.red_control_center_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep3)
                nImageID = R.drawable.red_control_center_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep4)
                nImageID = R.drawable.red_control_center_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.realTimeLeftFacility)
        {
            if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep1)
                nImageID = R.drawable.red_facility_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep2)
                nImageID = R.drawable.red_facility_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep3)
                nImageID = R.drawable.red_facility_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep4)
                nImageID = R.drawable.red_facility_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.realTimeLeftLocalBoss)
        {
            if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep1)
                nImageID = R.drawable.red_local_boss_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep2)
                nImageID = R.drawable.red_local_boss_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep3)
                nImageID = R.drawable.red_local_boss_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep4)
                nImageID = R.drawable.red_local_boss_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.realTimeLeftSafety)
        {
            if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep1)
                nImageID = R.drawable.red_safety_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep2)
                nImageID = R.drawable.red_safety_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep3)
                nImageID = R.drawable.red_safety_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep4)
                nImageID = R.drawable.red_safety_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.realTimeLeftCS)
        {
            if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep1)
                nImageID = R.drawable.red_cs_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep2)
                nImageID = R.drawable.red_cs_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep3)
                nImageID = R.drawable.red_cs_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnRealTimeSOPStep4)
                nImageID = R.drawable.red_cs_sop_step4;
            else
                return;
        }
        else
            return;

        ImageView imgBody = (ImageView)findViewById(R.id.actionRealTimeImage);
        Glide.with(this).load(nImageID).into(imgBody);

        int originalWidth = m_imgHelper.getImageOriginalWidth(nImageID);
        int originalHeight = m_imgHelper.getImageOriginalHeight(nImageID);

        if (originalWidth > 0)
        {
            ViewGroup.LayoutParams imgParams = imgBody.getLayoutParams();
            imgParams.width = m_nImageWidth;
            imgParams.height = imgParams.width * originalHeight / originalWidth;
        }
    }

    public void onTopMenuClick(View v)
    {
        int nID = v.getId();

        if (nID == m_nSelectedTopMenu)
            return;

        if (m_nSelectedTopMenu >= 0)
            setTopMenuImage(m_nSelectedTopMenu, false);

        m_nSelectedTopMenu = nID;
        setTopMenuImage(m_nSelectedTopMenu, true);
    }

    public void onMissionTypeClick(View v)
    {
        int nID = v.getId();

        if (nID == m_nSelectedMission)
            return;

        ImageView myMission = (ImageView)findViewById(R.id.realTimeMMMyMission);
        ImageView completeMission = (ImageView)findViewById(R.id.realTimeMMCompleteMission);

        if (nID == R.id.realTimeMMMyMission)
        {
            Glide.with(this).load(R.drawable.mm_my_mission_clicked).into(myMission);
            Glide.with(this).load(R.drawable.mm_complete_mission_normal).into(completeMission);

            if (m_myMissionList.size() > 0)
            {
                LinearLayout layoutParent = (LinearLayout)findViewById(R.id.layoutMissions);
                layoutParent.removeAllViews();

                for (RelativeLayout layout : m_myMissionList)
                {
                    layoutParent.addView(layout);
                }
            }
        }
        else
        {
            Glide.with(this).load(R.drawable.mm_my_mission_normal).into(myMission);
            Glide.with(this).load(R.drawable.mm_complete_mission_clicked).into(completeMission);

            m_myMissionList.clear();
            m_completeMissionList.clear();

            LinearLayout layoutParent = (LinearLayout)findViewById(R.id.layoutMissions);
            int nChildCount = layoutParent.getChildCount();

            for (int i=0;i<nChildCount;i++)
            {
                RelativeLayout layout = (RelativeLayout)layoutParent.getChildAt(i);
                m_myMissionList.add(layout);

                if (isSelectedLayout(layout))
                    m_completeMissionList.add(layout);
            }

            layoutParent.removeAllViews();

            for (RelativeLayout layout : m_completeMissionList)
            {
                layoutParent.addView(layout);
            }
        }
    }

    public void onMissionClick(View v)
    {
        ImageView image = (ImageView)v;
        RelativeLayout layout = (RelativeLayout)image.getParent();

        boolean isSelected = isSelectedLayout(layout);
        selectLayout(layout, !isSelected);

        TextView text = (TextView)layout.getChildAt(1);
        String strID = text.getTag().toString();
        sendSOPCommand(strID, !isSelected);
    }

    private void sendSOPCommand(String strProcessID, boolean isChecked)
    {
        WebManager mgr = new WebManager(getApplicationContext());
        mgr.setQueryType(WebManager.QueryType.SOP_COMMAND);
        mgr.setParameter("ProcessID", strProcessID);

        if (isChecked)
            mgr.setParameter("Checked", "1");
        else
            mgr.setParameter("Checked", "0");
        mgr.start();
    }

    public void onRequestClick(View v)
    {
        EditText textRequestType = (EditText)findViewById(R.id.realTimeEditRequestType);
        EditText textRequestCount = (EditText)findViewById(R.id.realTimeEditRequestCount);

        String strRequestType = getEditText(textRequestType);

        if (strRequestType == null)
            return;

        String strRequestCount = getEditText(textRequestCount);

        if (strRequestCount == null)
            return;

        int requestCount = 0;

        try
        {
            requestCount = Integer.parseInt(strRequestCount);
        }
        catch (Exception e)
        {
            showAlert("수량은 0보다 큰 정수를 입력해야 합니다.", "오류");
            return;
        }

        if (requestCount <= 0) {
            showAlert("수량은 0보다 큰 정수를 입력해야 합니다.", "오류");
            return;
        }

        Intent intent = new Intent(ActionRealTimeActivity.this, PopupWindowActivity.class);
        intent.putExtra("Message", "정상적으로 물품 및\n장비 요청이 되었습니다.");
        startActivity(intent);
    }

    private String getEditText(EditText textCtrl)
    {
        String text = textCtrl.getText().toString().trim();

        if (text.length() == 0)
        {
            if (textCtrl.getId() == R.id.realTimeEditRequestType)
                showAlert("지원종류를 입력하세요.", "오류");
            else
                showAlert("수량을 입력하세요.", "오류");

            return null;
        }

        return text;
    }

    private void showAlert(String message, String caption)
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(this);

        builder.setMessage(message);
        builder.setTitle(caption);
        builder.setCancelable(false);
        builder.setPositiveButton("확인", null);
        builder.show();
    }

    public void onNotify(String strTitle, String strBody)
    {
        if (parentIsMainActivity == false)
            FireReportActivity.getCurrentIntance().setInitTag(strTitle, strBody);
        else
            MainActivity.getInstance().setInitTag(strTitle, strBody);

        finish();
    }

    public void btnDisasterEquipClick(View v) {
        ImageView imgTitle = (ImageView) findViewById(R.id.imgTitle);
        TextView textMapTitle = (TextView) findViewById(R.id.realTimeMapTitle);
        View mapView = findViewById(R.id.realTimeMapImage);

        int nTitleImageHeight = imgTitle.getHeight();
        int nTitleTextHeight = textMapTitle.getHeight();
        int nMapHeight = mapView.getHeight();

        int nEquipListHeight = m_disasterEquipVisible ? 0 : getDisasterEquipHeight();

        LinearLayout disasterEquipLayout = (LinearLayout)findViewById(R.id.disasterEquipLayout);
        //ViewGroup.LayoutParams params = disasterEquipLayout.getLayoutParams();
        //params.height = nEquipListHeight;

        m_disasterEquipVisible = !m_disasterEquipVisible;

        if (m_disasterEquipVisible) {
            disasterEquipLayout.setVisibility(View.VISIBLE);
            disasterEquipLayout.getLayoutParams().height = m_nGridRowHeight * 3 + m_nGridHeaderHeight + m_nGridMinHeight;
        }
        else
        {
            disasterEquipLayout.setVisibility(View.INVISIBLE);
            disasterEquipLayout.getLayoutParams().height = m_nGridMinHeight;
        }

        showDisasterEquipment();
    }

    private int getDisasterEquipHeight()
    {
        LinearLayout head = (LinearLayout)findViewById(R.id.disasterEquipGridHead);
        ScrollView body = (ScrollView)findViewById(R.id.disasterEquipGridBody);

        return head.getHeight() + body.getHeight();
    }
}
