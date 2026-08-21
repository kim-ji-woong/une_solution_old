package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;

public class FireReportActivity extends AppCompatActivity implements FCMReceiver {
    private int m_nActionStepID = -1;
    private int m_nTemporaryNormalTeamID = -1;
    private int m_nTemporaryEmergencyTeamID = -1;
    private String m_strDetectTime = "";
    private String m_strLocation = "";
    private String m_strTitle = "";
    private String m_strMaterial = "";
    private boolean isFireDetect = true;

    private String m_initTag1 = "", m_initTag2 = "";

    private static FireReportActivity m_instance = null;

    public static FireReportActivity getCurrentIntance()
    {
        return m_instance;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;
        MainActivity.getInstance().setCurrentInstance(this);

        Intent intent = getIntent();
        String strTitle = intent.getStringExtra("Title");
        String strMessage = intent.getStringExtra("Message");

        parseMessage(strTitle, strMessage);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_fire_report);

        initLayout();
        setData();

        /*setImage((ImageView)findViewById(R.id.btnLogo), R.drawable.top_logo);
        setImage((ImageView)findViewById(R.id.imgLocation), R.drawable.report_textbox);
        setImage((ImageView)findViewById(R.id.imgTime), R.drawable.report_textbox);
        setImage((ImageView)findViewById(R.id.btnSOP), R.drawable.report_sop_normal);
        setImage((ImageView)findViewById(R.id.btnMyMission), R.drawable.report_my_mission_clicked);
        setImage((ImageView)findViewById(R.id.btnSendSOS), R.drawable.report_send_sos_normal);
        setImage((ImageView)findViewById(R.id.btnMap), R.drawable.report_map_normal);*/
    }

    @Override
    protected void onStart() {
        super.onStart();
        getDelegate().onStart();
        MainActivity.getInstance().setCurrentInstance(this);

        String strTitle = m_initTag1;
        String strMessage = m_initTag2;
        m_initTag1 = "";
        m_initTag2 = "";

        if (strTitle.length() > 0 && strMessage.length() > 0)
        {
            parseMessage(strTitle, strMessage);
            setData();
        }
    }

    private void setData()
    {
        final TextView textTitle = (TextView)findViewById(R.id.textTitle);
        //textTitle.setText(m_strTitle);

        textTitle.post(new Runnable() {
            public void run() {
                textTitle.setText(m_strTitle);
            }
        });

        final TextView textLocation = (TextView)findViewById(R.id.textLocation);
        //textLocation.setText(m_strLocation);

        textLocation.post(new Runnable() {
            public void run() {
                textLocation.setText(m_strLocation);
            }
        });

        final TextView textTime = (TextView)findViewById(R.id.textTime);
        //textTime.setText(m_strDetectTime);

        textTime.post(new Runnable() {
            public void run() {
                textTime.setText(m_strDetectTime);
            }
        });
    }

    private void parseMessage(String strTitle, String strMessage)
    {
        m_strTitle = strTitle;

        if (strTitle.contains("화재"))
            isFireDetect = true;
        else
            isFireDetect = false;

        int nBeginIndex = 0;
        int nTargetCount = isFireDetect ? 2 : 3;

        for (int i=0;i<nTargetCount;i++)
        {
            int nIndex = strMessage.indexOf(',', nBeginIndex);

            if (nIndex >= 0)
            {
                String token = strMessage.substring(nBeginIndex, nIndex).trim();

                try {
                    if (i == 0) {
                        m_nActionStepID = Integer.parseInt(token);
                    } else if (i == 1) {
                        if (isFireDetect)
                            m_strDetectTime = token;
                        else
                            m_strMaterial = token;
                    } else if (i == 2) {
                        if (isFireDetect == false)
                            m_strDetectTime = token;
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }

            nBeginIndex = nIndex + 1;
        }

        m_strLocation = strMessage.substring(nBeginIndex).trim();
    }

    private void initLayout()
    {
        LinearLayout layoutRoot = (LinearLayout)findViewById(R.id.layoutRoot);
        ViewGroup.LayoutParams param = layoutRoot.getLayoutParams();

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        param.width = (int)fScreenWidth;
        param.height = (int)fScreenHeight;
    }

    private void setImage(ImageView view, Object image)
    {
        Glide.with(this).load(image).into(view);
    }

    private void toggleMenu(int menu)
    {
        if (menu == R.id.btnSOP)
        {
            ((ImageView)findViewById(R.id.btnSOP)).setImageResource(R.drawable.report_sop_clicked);
            ((ImageView)findViewById(R.id.btnMyMission)).setImageResource(R.drawable.report_my_mission_normal);
            ((ImageView)findViewById(R.id.btnSendSOS)).setImageResource(R.drawable.report_send_sos_normal);
            ((ImageView)findViewById(R.id.btnMap)).setImageResource(R.drawable.report_map_normal);
        }
        else if (menu == R.id.btnMyMission)
        {
            ((ImageView)findViewById(R.id.btnSOP)).setImageResource(R.drawable.report_sop_normal);
            ((ImageView)findViewById(R.id.btnMyMission)).setImageResource(R.drawable.report_my_mission_clicked);
            ((ImageView)findViewById(R.id.btnSendSOS)).setImageResource(R.drawable.report_send_sos_normal);
            ((ImageView)findViewById(R.id.btnMap)).setImageResource(R.drawable.report_map_normal);
        }
        else if (menu == R.id.btnSendSOS)
        {
            ((ImageView)findViewById(R.id.btnSOP)).setImageResource(R.drawable.report_sop_normal);
            ((ImageView)findViewById(R.id.btnMyMission)).setImageResource(R.drawable.report_my_mission_normal);
            ((ImageView)findViewById(R.id.btnSendSOS)).setImageResource(R.drawable.report_send_sos_clicked);
            ((ImageView)findViewById(R.id.btnMap)).setImageResource(R.drawable.report_map_normal);
        }
        else// if (menu == R.id.btnMap)
        {
            ((ImageView)findViewById(R.id.btnSOP)).setImageResource(R.drawable.report_sop_normal);
            ((ImageView)findViewById(R.id.btnMyMission)).setImageResource(R.drawable.report_my_mission_normal);
            ((ImageView)findViewById(R.id.btnSendSOS)).setImageResource(R.drawable.report_send_sos_normal);
            ((ImageView)findViewById(R.id.btnMap)).setImageResource(R.drawable.report_map_clicked);
        }
    }

    private void moveToActionRealTimeActivity(int btnID, String strMenu)
    {
        toggleMenu(btnID);

        Intent intent = new Intent(FireReportActivity.this, ActionRealTimeActivity.class);
        intent.putExtra("InitMenu", strMenu);
        intent.putExtra("ActionStepID", Integer.toString(m_nActionStepID));

        if (isFireDetect)
            intent.putExtra("QuickButtonID", "10000");
        else
            intent.putExtra("QuickButtonID", "10007");

        startActivity(intent);
    }

    public void btnSOPClick(View v)
    {
        moveToActionRealTimeActivity(R.id.btnSOP, "SOP");
    }

    public void btnMyMissionClick(View v)
    {
        moveToActionRealTimeActivity(R.id.btnMyMission, "MyMission");
    }

    public void btnSendSOSClick(View v)
    {
        moveToActionRealTimeActivity(R.id.btnSendSOS, "SendSOS");
    }

    public void btnMapClick(View v)
    {
        moveToActionRealTimeActivity(R.id.btnMap, "Map");
    }

    public void onNotify(String strTitle, String strBoday)
    {
        parseMessage(strTitle, strBoday);
        setData();
    }

    public void setInitTag(String initTag1, String initTag2)
    {
        m_initTag1 = initTag1;
        m_initTag2 = initTag2;
    }
}
