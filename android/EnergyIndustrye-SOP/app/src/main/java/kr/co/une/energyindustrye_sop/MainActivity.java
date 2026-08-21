package kr.co.une.energyindustrye_sop;

import android.app.Activity;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.os.Bundle;
import android.os.Handler;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.GridLayout;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.TableLayout;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.bumptech.glide.request.target.SizeReadyCallback;
import com.google.firebase.iid.FirebaseInstanceId;

import org.apache.http.HttpResponse;

import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import kr.co.une.energyindustrye_sop.task.ScheduleCheckAsync;

public class MainActivity extends AppCompatActivity implements FCMReceiver {

    private int m_nLogoHeight = -1;
    private int m_nMenuHeight = -1;
    private int m_nMenuButtonWidth = -1, m_nMenuButtonHeight = -1;
    private boolean m_scheduleVisible = false;

    private String m_initTag1 = "", m_initTag2 = "";

    private static int m_nStandardScreenWidth = 1440;
    private static int m_nStandardScreenHeight = 2560;

    public static int getStandardScreenWidth()
    {
        return m_nStandardScreenWidth;
    }

    public static int getStandardScreenHeight()
    {
        return m_nStandardScreenHeight;
    }

    private FCMReceiver m_currentReceiver = null;
    private static MainActivity m_instance = null;

    public static MainActivity getInstance()
    {
        return m_instance;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;
        setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_main);

        setImage(R.id.imgMainMenuLogo, R.drawable.logo);
        setImage(R.id.btnActionRule, R.drawable.action_rule_square_button);
        setImage(R.id.btnActionSOP, R.drawable.action_sop_square_button);
        setImage(R.id.btnPrivateMission, R.drawable.private_mission_square_button);
        setImage(R.id.btnMap, R.drawable.map_square_button);
        //setImageSizeNLocation((ImageView) findViewById(R.id.imgMainMenuLogo), R.drawable.logo, 810, 481);
        //setImageButtonSizeNLocation((ImageButton)findViewById(R.id.btnActionRule), 0, R.drawable.action_rule_button, 840, 289);
        //setImageButtonSizeNLocation((ImageButton)findViewById(R.id.btnActionSOP), 48, R.drawable.action_sop_button, 840, 289);
        //setImageButtonSizeNLocation((ImageButton)findViewById(R.id.btnMap), 48, R.drawable.map_button, 840, 289);

        initSchedule((ListView)findViewById(R.id.scheduleList));
        //findViewById(R.id.scheduleLayer).setVisibility(View.INVISIBLE);

        if (needRestart())
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage("알람 메시지 수신 등록중입니다.\r\n확인 버튼을 누른후 재시작하여 주세요.");
            builder.setTitle("확인");

            builder.setPositiveButton("확인", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    finish();
                }
            });

            builder.show();
        }

        Intent intent = getIntent();
        String strAlarm = intent.getStringExtra("Alarm");

        if (strAlarm.equals("1"))
        {
            String strTitle = intent.getStringExtra("Title");
            String strMessage = intent.getStringExtra("Message");

            moveToFireReportActivity(strTitle, strMessage);
        }

        /*int nParam = Integer.parseInt(strParam);

        if (nParam < 0)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage("알람 메시지 수신이 완료되었습니다.\r\n확인 버튼을 눌러 다시 시작하면 정상적인 서비스 이용이 가능합니다.");
            builder.setTitle("확인");

            builder.setPositiveButton("확인", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    finish();
                }
            });

            builder.show();
        }*/
    }

    private void setImage(int nViewID, int nImageID)
    {
        ImageView view = (ImageView)findViewById(nViewID);
        Glide.with(this).load(nImageID).into(view);
    }

    @Override
    protected void onStart() {
        super.onStart();
        getDelegate().onStart();
        setCurrentInstance(this);

        String strTitle = m_initTag1;
        String strMessage = m_initTag2;
        m_initTag1 = "";
        m_initTag2 = "";

        if (strTitle.length() > 0 && strMessage.length() > 0)
            moveToFireReportActivity(strTitle, strMessage);
    }

    private void moveToFireReportActivity(String strTitle, String strMessage)
    {
        Intent intent = new Intent(getApplication(), FireReportActivity.class);
        intent.putExtra("Title", strTitle);
        intent.putExtra("Message", strMessage);

        startActivity(intent);
    }

    private boolean needRestart()
    {
        // Device ID가 생성되지 않으면 App을 다시 실행한다.
        String deviceID = FirebaseInstanceId.getInstance().getToken();

        if (deviceID == null || deviceID.length() == 0)
            return true;

        return false;
    }

    private void initSchedule(ListView view)
    {
        String[] LIST_MENU = { getApplicationContext().getString(R.string.no_schedule) } ;
        ArrayAdapter adapter = new ArrayAdapter(this, android.R.layout.simple_list_item_1, LIST_MENU);
        view.setAdapter(adapter);
    }

    private void setImageButtonSizeNLocation(ImageButton btn, int paddingTop, int nImageID, int imgWidth, int imgHeight)
    {
        if (btn == null)
            return;

        Glide.with(this).load(nImageID).into(btn);

        float fScreenWidth = getScreenWidth(this);
        float fScreenHeight = getScreenHeight(this);
        //int imgWidth = btn.getDrawable().getIntrinsicWidth();
        //int imgHeight = btn.getDrawable().getIntrinsicHeight();

        // 표준모델
        int stLeftPadding = 300, stRightPadding = 300;
        int stScreenWidth = 1440, stScreenHeight = 2560;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = left;
        int top = paddingTop;
        int bottom = 0;

        btn.setPadding(left, top, right, bottom);
    }

    private void setImageSizeNLocation(ImageView view, int nImageID, int imgWidth, int imgHeight)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);

        float fScreenWidth = getScreenWidth(this);
        float fScreenHeight = getScreenHeight(this);
        //int imgWidth = view.getDrawable().getIntrinsicWidth();
        //int imgHeight = view.getDrawable().getIntrinsicHeight();

        // 표준모델
        int stTopPadding = 258, stLeftPadding = 315, stRightPadding = 315;
        int stScreenWidth = getStandardScreenWidth(), stScreenHeight = getStandardScreenHeight();

        float scale = 1.0f;//getResources().getDisplayMetrics().density;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth / scale);
        int right = (int)((fScreenWidth - stRightPadding * fScreenWidth / stScreenWidth) / scale);
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight / scale);
        int height = (right - left) * imgHeight / imgWidth;
        int bottom = top + height;

        //top += 100;
        bottom = 0;
        //bottom += 100;
        //left += 100;
        right = left;
        //right += 100;

        view.setPadding(left, top, right, bottom);
    }

    public static float getScreenWidth(Activity activity)
    {
        Display display = activity.getWindowManager().getDefaultDisplay();
        DisplayMetrics outMetrics = new DisplayMetrics();
        display.getMetrics(outMetrics);

        float pxWidth = outMetrics.widthPixels;
        return pxWidth;
    }

    public static float getScreenHeight(Activity activity)
    {
        Display display = activity.getWindowManager().getDefaultDisplay();
        DisplayMetrics outMetrics = new DisplayMetrics();
        display.getMetrics(outMetrics);

        float pxHeight = outMetrics.heightPixels;
        return pxHeight;
    }

    public void btnMainBottomClick(View v) {
        LinearLayout logoLayout = (LinearLayout) findViewById(R.id.layerLogo);
        TableLayout menuLayout = (TableLayout) findViewById(R.id.layerMenu);
        LinearLayout scheduleLayout = (LinearLayout) findViewById(R.id.scheduleLayout);

        ImageButton btnActionRule = (ImageButton)findViewById(R.id.btnActionRule);
        ImageButton btnActionSOP = (ImageButton)findViewById(R.id.btnActionSOP);
        ImageButton btnPrivateMission = (ImageButton)findViewById(R.id.btnPrivateMission);
        ImageButton btnMap = (ImageButton)findViewById(R.id.btnMap);

        if (m_nLogoHeight < 0 || m_nMenuHeight < 0)
        {
            m_nLogoHeight = logoLayout.getHeight();
            m_nMenuHeight = menuLayout.getHeight();

            m_nMenuButtonWidth = btnActionRule.getWidth() - (btnActionRule.getPaddingLeft() + btnActionRule.getPaddingRight());
            m_nMenuButtonHeight = btnActionRule.getHeight() - (btnActionRule.getPaddingTop() + btnActionRule.getPaddingBottom());
        }

        int scheduleHeight = m_scheduleVisible ? 0 : getScheduleHeight();

        ViewGroup.LayoutParams logoParams = logoLayout.getLayoutParams();
        ViewGroup.LayoutParams menuParams = menuLayout.getLayoutParams();
        ViewGroup.LayoutParams scheduleParams = scheduleLayout.getLayoutParams();

        logoParams.height = m_nLogoHeight;
        menuParams.height = m_nMenuHeight - scheduleHeight;
        scheduleParams.height = scheduleHeight;

        //setImageButtonSize(btnActionRule, R.drawable.action_rule_button, m_nMenuButtonWidth, m_nMenuButtonHeight);
        //setImageButtonSize(btnActionSOP, R.drawable.action_sop_button, m_nMenuButtonWidth, m_nMenuButtonHeight);
        //setImageButtonSize(btnMap, R.drawable.map_button, m_nMenuButtonWidth, m_nMenuButtonHeight);

        m_scheduleVisible = !m_scheduleVisible;

        if (m_scheduleVisible) {
            scheduleLayout.setVisibility(View.VISIBLE);

            if (menuParams.height <= 0)
                menuLayout.setVisibility(View.INVISIBLE);

            logoLayout.setBackgroundColor(Color.GRAY);
            menuLayout.setBackgroundColor(Color.GRAY);
        }
        else
        {
            scheduleLayout.setVisibility(View.INVISIBLE);
            menuLayout.setVisibility(View.VISIBLE);

            logoLayout.setBackgroundColor(Color.TRANSPARENT);
            menuLayout.setBackgroundColor(Color.TRANSPARENT);

            int mapHeight = btnMap.getHeight();

            btnActionRule.setVisibility(View.INVISIBLE);
            btnActionRule.setVisibility(View.VISIBLE);
            btnActionSOP.setVisibility(View.INVISIBLE);
            btnActionSOP.setVisibility(View.VISIBLE);
            btnPrivateMission.setVisibility(View.INVISIBLE);
            btnPrivateMission.setVisibility(View.VISIBLE);
            btnMap.setVisibility(View.INVISIBLE);
            btnMap.setVisibility(View.VISIBLE);
        }
    }

    private int getScheduleHeight()
    {
        java.text.DateFormat format = new java.text.SimpleDateFormat("M");
        Calendar cal = Calendar.getInstance();
        String thisMonth = format.format(cal.getTime()) + "월";

        TextView scheduleText = (TextView)findViewById(R.id.scheduleMonth);
        scheduleText.setText(thisMonth);

        ListView scheduleList = (ListView)findViewById(R.id.scheduleList);
        View listItem = scheduleList.getAdapter().getView(0, null, scheduleList);
        listItem.measure(0, 0);

        int itemHeight = listItem.getMeasuredHeight() + scheduleList.getDividerHeight();

        List<String> schedules = readSchedule();
        int nDataCount = schedules.size();
        int nScheduleCount = nDataCount / 2;

        if (nScheduleCount == 0) {
            initSchedule(scheduleList);
            return itemHeight;
        }

        String[] LIST_MENU = new String[nScheduleCount];

        for (int i=0;i<nScheduleCount;i++)
        {
            String strDay = schedules.get(i * 2).substring(8, 10);

            strDay = "[" + strDay + "일] ";
            String strItem = schedules.get(i * 2 + 1);
            LIST_MENU[i] = strDay + strItem;
        }

        ArrayAdapter adapter = new ArrayAdapter(this, android.R.layout.simple_list_item_1, LIST_MENU);
        scheduleList.setAdapter(adapter);

        int scheduleHeight = nScheduleCount * itemHeight;

        if (scheduleHeight > m_nMenuHeight)
            scheduleHeight = m_nMenuHeight;

        return scheduleHeight;
    }

    private List<String> readSchedule()
    {
        try
        {
            String url = Splash.getWebServerURL() + "/" + getApplicationContext().getString(R.string.schedule_url);
            //String url = getApplicationContext().getString(R.string.web_url) + "/" + getApplicationContext().getString(R.string.schedule_url);

            java.text.DateFormat format = new java.text.SimpleDateFormat("yyyy-MM");
            Calendar cal = Calendar.getInstance();

            String thisMonth = format.format(cal.getTime());

            HttpResponse response = new ScheduleCheckAsync().execute(url, "Month=" + thisMonth).get();

            BufferedReader bufreader = new BufferedReader(
                    new InputStreamReader(response.getEntity().getContent(),
                            "utf-8"));

            List<String> resultList = new ArrayList();
            String beginTag = "Begin Data", endTag = "End Data";
            boolean isBegin = false;
            String line = null;

            while ((line = bufreader.readLine()) != null) {
                line = line.trim();

                if (line.length() == 0)
                    continue;

                if (isBegin == false)
                {
                    if (line.equals(beginTag))
                    {
                        isBegin = true;
                    }
                }
                else
                {
                    if (line.equals(endTag))
                        break;

                    int a = line.indexOf('[');
                    int b = line.lastIndexOf(']');

                    if (a == -1 || b == -1)
                    {

                    }
                    else if (a + 1 == b)
                    {
                        line = "";
                    }
                    else
                        line = line.substring(a + 1, line.length() - 1);

                    resultList.add(line);
                }
            }

            return resultList;
        }
        catch (Exception e)
        {
            e.printStackTrace();

        }

        return null;
    }

    private void setImageButtonSize(ImageButton btn, int id, int width, int height)
    {
        Bitmap image = BitmapFactory.decodeResource(getResources(), id);
        Bitmap resized = Bitmap.createScaledBitmap(image, width, height, true);
        btn.setImageBitmap(resized);

        btn.setScaleType(ImageView.ScaleType.MATRIX);
    }

    public void btnActionRuleClick(View v) {
        Intent intent = new Intent(MainActivity.this, ActionRuleActivity.class);
        startActivity(intent);
    }

    public void btnActionSOPClick(View v) {
        Intent intent = new Intent(MainActivity.this, ActionSOPActivity.class);
        startActivity(intent);
    }

    public void btnPrivateMissionClick(View v) {
        Intent intent = new Intent(MainActivity.this, ActionPrivateMission.class);
        startActivity(intent);
    }

    public void btnMapClick(View v) {
        Intent intent = new Intent(MainActivity.this, ActionRealTimeActivity.class);
        intent.putExtra("InitMenu", "Map");
        startActivity(intent);
    }

    public static void showAlert(String message, String caption, Context context)
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(context);

        builder.setMessage(message);
        builder.setTitle(caption);
        builder.setCancelable(false);
        builder.setPositiveButton("확인", null);
        builder.show();
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
                MainActivity.super.onBackPressed();
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

    public FCMReceiver getCurrentInstance()
    {
        return m_currentReceiver;
    }

    public void setCurrentInstance(FCMReceiver instance)
    {
        m_currentReceiver = instance;
    }

    public void onNotify(String strTitle, String strBody)
    {
        moveToFireReportActivity(strTitle, strBody);
    }

    public void setInitTag(String initTag1, String initTag2)
    {
        m_initTag1 = initTag1;
        m_initTag2 = initTag2;
    }
}
