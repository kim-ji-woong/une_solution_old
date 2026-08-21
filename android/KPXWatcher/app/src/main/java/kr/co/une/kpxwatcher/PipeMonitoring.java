package kr.co.une.kpxwatcher;

import android.app.ActionBar;
import android.content.DialogInterface;
import android.graphics.Color;
import android.graphics.Rect;
import android.support.v7.app.AlertDialog;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.TextUtils;
import android.text.style.ForegroundColorSpan;
import android.text.style.RelativeSizeSpan;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.Display;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.ScrollView;
import android.widget.TextView;

import com.bumptech.glide.Glide;
import com.google.firebase.iid.FirebaseInstanceId;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import kr.co.une.kpxwatcher.Data.Pipe;
import kr.co.une.kpxwatcher.Data.Tank;

public class PipeMonitoring extends AppCompatActivity{

    private final int PRESS_NORMAL = 0;
    private final int PRESS_INCREASE = 256;
    private final int PRESS_DECREASE = 512;
    private final int FLOW_INCREASE = 1024;
    private final int FLOW_DECREASE = 2048;

    private enum StatusMode { UNKNOWN, WAITING, IN_WORK, HI_ALARM, LOW_ALARM };

    private HashMap<View, StatusMode> mapWorkStatus = new HashMap<View, StatusMode>();
    private HashMap<View, StatusMode> mapAlarmStatus = new HashMap<View, StatusMode>();

    private final int STATUS = 0;
    private final int IGNORE_STATUS = 1;

    private String PressureTag = "·현재압력:";
    private String RangeTag = " 범위:";
    private String LiquidTag = "유종:";
    private String FlowTag = "·현재유량:";
    private String FlowRange = " 범위:";

    private Thread m_thread = null;
    private boolean m_runThread = false;

    private static PipeMonitoring m_instance = null;

    public static PipeMonitoring Instance()
    {
        return m_instance;
    }

    private int m_nPipeCount = 0;

    private Pipe m_selectPipe = null;
    private StatusMode m_statusMode = StatusMode.UNKNOWN;

    private DlgTankList m_dlgTankList = null;
    private DlgAlarmClear m_dlgAlarmClear = null;

    private List<RelativeLayout> m_listPipeLayout = new ArrayList<>();
    // Key : View ID
    // Value : View ID에 해당하는 Pipe Index
    private HashMap<Integer, Integer> m_mapViewPipeIndex = new HashMap<Integer, Integer>();

    List<LayoutPipeName> m_listPipeName = new ArrayList<>();
    List<LayoutPipeStatus> m_listPipeStatus = new ArrayList<>();
    List<LayoutWorkCommand> m_listWorkCommand = new ArrayList<>();
    private boolean m_bDlgVisible = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        m_instance = this;

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_pipe_monitoring);

        InitView();

        setImage((ImageView)findViewById(R.id.imgPipeTitle), R.drawable.pipe_title);

        initPipe();
        setImage((ImageView)findViewById(R.id.imgHeaderBackground), R.drawable.pipe_column_header);

        setLayerSize();
    }

    private void InitView()
    {
        // 사용하지 않는 리소스는 감춘다.
        InitVisible();

        int nPipeCount = m_listPipeLayout.size();

        for (int i=0;i<nPipeCount;i++)
        {
            RelativeLayout pipe = m_listPipeLayout.get(i);
            List<LinearLayout> layouts = GetPipeInfoLayout(pipe);

            if (layouts.size() == 3)
            {
                CreateName(layouts.get(0));
                CreateStatus(layouts.get(1));
                CreateWorkCommand(layouts.get(2));

                m_mapViewPipeIndex.put(layouts.get(2).getId(), i);
            }
        }

        // 배관 명
        /*LinearLayout layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName1);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName2);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName3);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName4);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName5);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName6);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName7);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName8);
        CreateName(layoutName);
        layoutName = (LinearLayout)findViewById(R.id.Layout_pipeName9);
        CreateName(layoutName);

        // Pipe status(배관 현황)
        LinearLayout layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus1);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus2);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus3);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus4);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus5);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus6);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus7);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus8);
        CreateStatus(layout);

        layout = (LinearLayout)findViewById(R.id.Layout_pipeStatus9);
        CreateStatus(layout);

        // 작업 관리
        layout = (LinearLayout)findViewById(R.id.layout_workcommand1);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand2);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand3);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand4);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand5);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand6);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand7);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand8);
        CreateWorkCommand(layout);
        layout = (LinearLayout)findViewById(R.id.layout_workcommand9);
        CreateWorkCommand(layout);*/
    }

    private void InitVisible()
    {
        m_nPipeCount = MenuActivity.Instance().getPipeCount();

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.bodyLayout);
        int nChildCount = bodyLayout.getChildCount();

        int nPipeIndex = 0;
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
                if (nPipeIndex++ < m_nPipeCount - 1) {
                    m_listPipeLayout.add((RelativeLayout) view);
                    view.setVisibility(View.VISIBLE);
                }
                else
                    break;
            }
        }

        m_listPipeLayout.add((RelativeLayout) lastView);
    }
    /*private void InitVisible()
    {
        m_nPipeCount = MenuActivity.Instance().getPipeCount();

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.bodyLayout);
        int nChildCount = bodyLayout.getChildCount();

        int nPipeIndex = 0;
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
                if (nPipeIndex++ < m_nPipeCount - 1)
                    m_listPipeLayout.add((RelativeLayout)view);
                else {
                    if (lastView != view)
                        view.setVisibility(View.GONE);
                    else
                        m_listPipeLayout.add((RelativeLayout)view);
                }
            }
        }
    }*/

    // Return 값 : 배관 명, 배관 현황, 작업 관리
    private List<LinearLayout> GetPipeInfoLayout(RelativeLayout layout)
    {
        List<LinearLayout> results = new ArrayList<>();

        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof LinearLayout)
            {
                LinearLayout parent = (LinearLayout)view ;
                int nChildCount2 = parent.getChildCount();

                int nLinearIndex = 0;

                for (int j=0;j<nChildCount2;j++)
                {
                    View view2 = parent.getChildAt(j);

                    if (view2 instanceof  LinearLayout)
                    {
                        if (nLinearIndex == 0)
                            results.add((LinearLayout)view2);
                        else if (nLinearIndex == 1)
                            results.add((LinearLayout)view2);
                        else if (nLinearIndex == 4) {
                            results.add((LinearLayout) view2);
                            break;
                        }

                        nLinearIndex++;
                    }
                }

                break;
            }
        }

        return results;
    }

    // Return 값 : 메인 이미지, 알람 현황, 작업 상태
    private List<ImageView> GetPipeInfo2Layout(RelativeLayout layout)
    {
        List<ImageView> results = new ArrayList<>();

        int nChildCount = layout.getChildCount();

        for (int i=0;i<nChildCount;i++)
        {
            View view = layout.getChildAt(i);

            if (view instanceof ImageView)
            {
                results.add((ImageView)view);
            }
            else if (view instanceof LinearLayout)
            {
                LinearLayout parent = (LinearLayout)view ;
                int nChildCount2 = parent.getChildCount();

                for (int j=0;j<nChildCount2;j++)
                {
                    View view2 = parent.getChildAt(j);

                    if (view2 instanceof ImageView)
                    {
                        results.add((ImageView)view2);
                    }
                }

                break;
            }
        }

        return results;
    }

    private void CreateName(LinearLayout parent)
    {
        LayoutPipeName pName = new LayoutPipeName();

        TextView view = CreateTextView();
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.gravity = Gravity.TOP | Gravity.LEFT;
        view.setLayoutParams(params);
        view.setPadding(0, 0, 0, -8);
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 30);
        parent.addView(view);
        pName.tvName = view;

        view = CreateTextView();
        params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.topMargin = -6;
        view.setLayoutParams(params);
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        parent.addView(view);
        pName.tvTank = view;
        //pName.tvLiquid = view;

        view = CreateTextView();
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        parent.addView(view);
        pName.tvLiquid = view;
        //pName.tvTank = view;

        m_listPipeName.add(pName);
    }

    private void CreateStatus(LinearLayout parent)
    {
        LinearLayout layout = new LinearLayout(this);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.WRAP_CONTENT, ActionBar.LayoutParams.MATCH_PARENT);
        params.weight = 60;
        layout.setOrientation(LinearLayout.VERTICAL);

        DisplayMetrics dm = getResources().getDisplayMetrics();
        int left = Math.round(20 * dm.density);
        int top = Math.round(5 * dm.density);
        layout.setPadding(left, top, 0, 0);

        LayoutPipeStatus pStatus = new LayoutPipeStatus();

        TextView view = CreateTextView();
        layout.addView(view);
        pStatus.tvPress = view;

        view = CreateTextView();
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        layout.addView(view);
        pStatus.tvPressRange = view;

        /*view = CreateTextView();
        layout.addView(view);
        pStatus.tvLiquid = view;*/

        view = CreateTextView();
        layout.addView(view);
        pStatus.tvFlow = view;

        view = CreateTextView();
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        layout.addView(view);
        pStatus.tvFlowRange = view;

        parent.addView(layout);

        m_listPipeStatus.add(pStatus);
    }

    private void CreateWorkCommand(LinearLayout parent)
    {
        DisplayMetrics dm = getResources().getDisplayMetrics();

        LayoutWorkCommand command = new LayoutWorkCommand();

        ImageView imgView = CreateImgaeView();
        int left = Math.round(20 * dm.density);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, Math.round(45 * dm.density));
        params.topMargin = Math.round(5 * dm.density);
        imgView.setLayoutParams(params);
        parent.addView(imgView);
        command.iv = imgView;

        TextView txtView = CreateTextView();
        params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, Math.round(27 * dm.density));
        params.topMargin = Math.round(-7 * dm.density);
        txtView.setLayoutParams(params);
        txtView.setTextAlignment(View.TEXT_ALIGNMENT_CENTER);
        txtView.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 9);
        txtView.setText("");
        parent.addView(txtView);
        command.tvDue = txtView;

        m_listWorkCommand.add(command);
    }

    private TextView CreateTextView()
    {
        TextView view = new TextView(this);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.gravity = Gravity.LEFT;
        view.setLayoutParams(params);
        view.setTextAlignment(View.TEXT_ALIGNMENT_INHERIT);
        view.setTextColor(getResources().getColor(R.color.colorItemNormal));
        view.setTextSize(TypedValue.COMPLEX_UNIT_DIP, 10);
        return view;
    }

    private ImageView CreateImgaeView()
    {
        ImageView view = new ImageView(this);
        LinearLayout.LayoutParams params = new LinearLayout.LayoutParams(ActionBar.LayoutParams.MATCH_PARENT, ActionBar.LayoutParams.WRAP_CONTENT);
        params.gravity = Gravity.CENTER_HORIZONTAL | Gravity.CENTER_VERTICAL;
        view.setLayoutParams(params);
        view.setAdjustViewBounds(true);
        view.setBackgroundColor(0x00000000);
        view.setOnClickListener(OnClickWorkCommand);

        return view;
    }

    View.OnClickListener OnClickWorkCommand = new View.OnClickListener(){
        @Override
        public void onClick(View v) {
            WorkCommandClick(v);
        }
    };

    private int GetWorkCommandIndex(View v)
    {
        int size = m_listWorkCommand.size();
        for(int i=0; i<size; ++i)
        {
            if(v == m_listWorkCommand.get(i).iv)
                return i;
        }
        return -1;
    }

    private void CreateDialog(View v)
    {
        if(m_dlgTankList == null) {
            m_dlgTankList = new DlgTankList(this);
            //dlg.getWindow().setGravity(Gravity.BOTTOM);
            WindowManager.LayoutParams params = m_dlgTankList.getWindow().getAttributes();

            Rect r = new Rect();
            v.getGlobalVisibleRect(r); //RootView 레이아웃을 기준으로한 좌표.

            DisplayMetrics dm = getResources().getDisplayMetrics();
            int x = Math.round(300 / dm.density);

            params.gravity = Gravity.BOTTOM;
            params.x = r.left - x;
            //params.x -= 300;
            //int width = r.right - r.left;
            /*DisplayMetrics dm = getResources().getDisplayMetrics();
            int x = Math.round(r.left / dm.density);
            params.x = r.left - dlg.GetWidth();*/

            m_dlgTankList.getWindow().setAttributes(params);

            m_dlgTankList.setOnDismissListener(new DialogInterface.OnDismissListener() {
                @Override
                public void onDismiss(DialogInterface dialog) {
                    DlgTankList dlg = (DlgTankList)dialog;
                    Tank tank = dlg.GetTank();
                    if(tank != null) {
                        WorkCommandClick(tank);
                        dlg.ReleaseTank();
                    }
                    m_bDlgVisible = false;
                }
            });
        }

        Display display = this.getWindowManager().getDefaultDisplay();
        WindowManager.LayoutParams params = m_dlgTankList.getWindow().getAttributes();
        Rect r = new Rect();
        v.getGlobalVisibleRect(r); //RootView 레이아웃을 기준으로한 좌표.

        //DisplayMetrics dm = getResources().getDisplayMetrics();
        //int x = Math.round(500 / dm.density);

        params.gravity = Gravity.BOTTOM;
        params.x = display.getWidth()/2 - r.width() - m_dlgTankList.GetWidth()/2;

        m_dlgTankList.getWindow().setAttributes(params);

        m_dlgTankList.show();
        m_bDlgVisible = true;
    }

    private void CreateDlgAlarmClear(Pipe pipe, Tank tank)
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

        m_dlgAlarmClear.SetPipe(pipe);
        m_dlgAlarmClear.SetTank(tank);

        m_dlgAlarmClear.show();
    }

    private void WorkCommandClick(final Tank tank)
    {
        if(m_statusMode != StatusMode.IN_WORK && tank.getLinkPipeSize() >= 2)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.setMessage("이미 다른 배관과 작업중입니다.\n최대 연결할 수 있는 배관의 수는 2개입니다.");
            builder.setTitle("확인");
            builder.setPositiveButton("확인", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    // Do nothing
                    dialog.dismiss();

                    m_selectPipe = null;
                    m_statusMode = StatusMode.UNKNOWN;
                }
            });
            builder.show();
            return;
        }
        if (m_selectPipe != null) {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            if (m_statusMode == StatusMode.IN_WORK)
                builder.setMessage(m_selectPipe.getName() + "의 작업을 종료하시겠습니까?");
            else
                builder.setMessage(m_selectPipe.getName() + "의 작업을 시작하시겠습니까?");

            builder.setTitle("확인");

            builder.setNegativeButton("네", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which)
                {
                    WebManager mgr = new WebManager(getApplicationContext());

                    if (m_statusMode == StatusMode.IN_WORK) {
                        mgr.setQueryType(WebManager.QueryType.END_WORK);
                        mgr.setParameter("WorkType", "5");
                    }
                    else {
                        mgr.setQueryType(WebManager.QueryType.BEGIN_WORK);
                        mgr.setParameter("WorkType", "4");
                    }
                    mgr.setParameter("DeviceID", MainActivity.getDeviceID());
                    mgr.setParameter("PipeID", String.valueOf(m_selectPipe.getID()));
                    if(tank != null)
                        mgr.setParameter("TankID", String.valueOf(tank.getID()));

                    mgr.start();

                    dialog.dismiss();

                    m_selectPipe = null;
                    m_statusMode = StatusMode.UNKNOWN;
                }
            });

            builder.setPositiveButton("아니오", new DialogInterface.OnClickListener()
            {
                public void onClick(DialogInterface dialog, int which)
                {
                    // Do nothing
                    dialog.dismiss();

                    m_selectPipe = null;
                    m_statusMode = StatusMode.UNKNOWN;
                }
            });

            builder.show();
        }
    }

    private void initPipe()
    {
        int nPipeCount = m_listPipeLayout.size();

        for (int i=0;i<nPipeCount;i++)
        {
            RelativeLayout pipe = m_listPipeLayout.get(i);
            List<ImageView> images = GetPipeInfo2Layout(pipe);

            if (images.size() == 3)
            {
                initImage(images.get(0).getId(), images.get(1).getId(), images.get(2).getId(), m_listWorkCommand.get(i).iv);
                m_mapViewPipeIndex.put(images.get(2).getId(), i);
            }
        }
        /*initImage(R.id.imgPipe1, R.id.imgPipe1WorkStatus, R.id.imgPipe1AlarmStatus, m_listWorkCommand.get(0).iv);
        initImage(R.id.imgPipe2, R.id.imgPipe2WorkStatus, R.id.imgPipe2AlarmStatus, m_listWorkCommand.get(1).iv);
        initImage(R.id.imgPipe3, R.id.imgPipe3WorkStatus, R.id.imgPipe3AlarmStatus, m_listWorkCommand.get(2).iv);
        initImage(R.id.imgPipe4, R.id.imgPipe4WorkStatus, R.id.imgPipe4AlarmStatus, m_listWorkCommand.get(3).iv);
        initImage(R.id.imgPipe5, R.id.imgPipe5WorkStatus, R.id.imgPipe5AlarmStatus, m_listWorkCommand.get(4).iv);
        initImage(R.id.imgPipe6, R.id.imgPipe6WorkStatus, R.id.imgPipe6AlarmStatus, m_listWorkCommand.get(5).iv);
        initImage(R.id.imgPipe7, R.id.imgPipe7WorkStatus, R.id.imgPipe7AlarmStatus, m_listWorkCommand.get(6).iv);
        initImage(R.id.imgPipe8, R.id.imgPipe8WorkStatus, R.id.imgPipe8AlarmStatus, m_listWorkCommand.get(7).iv);
        initImage(R.id.imgPipe9, R.id.imgPipe9WorkStatus, R.id.imgPipe9AlarmStatus, m_listWorkCommand.get(8).iv);*/

        initText(m_listPipeStatus);
    }

    @Override
    protected void onStart() {
        super.onStart();
        getDelegate().onStart();

        MenuActivity.Instance().setPipe(true);
        MainActivity.setCurrentActivity(MainActivity.ActivityType.PipeMonitor);

        m_runThread = true;

        m_thread = new Thread(new Runnable() {
            public void run() {
                while (m_runThread) {
                    final long currentMilli = System.currentTimeMillis();

                    for (int i=1;i<=m_nPipeCount;i++) {
                        if (i > m_listPipeLayout.size())
                            continue;

                        RelativeLayout pipeLayout = m_listPipeLayout.get(i - 1);
                        List<ImageView> images = GetPipeInfo2Layout(pipeLayout);

                        if (images.size() != 3)
                            continue;

                        final TextView textName = getPipeNameTextView(i);
                        final TextView textLiquid = getLiquidTextView(i);
                        final TextView textLink = getPipeLinkTextView(i);
                        final TextView textPressure = getPressureTextView(i);
                        final TextView textRange = getRangeTextView(i);
                        final TextView textFlow = getFlowTextView(i);
                        final TextView textFlowRange = getFlowRangeTextView(i);
                        //final ImageView btnStatus = getStatusImageView(i);
                        //final ImageView btnAlarm = getAlarmImageView(i);
                        final ImageView panel = images.get(0);
                        //final ImageView panel = getPipePanel(i);
                        final TextView textNotice = (TextView) findViewById(R.id.textNotice);
                        //final RelativeLayout layoutIgnoreAlarm = getIgnoreAlarmLayout(i);
                        //final TextView textIgnoreAlarm = getIgnoreAlarmText(i);
                        final ImageView btnWorkStatus = images.get(2);
                        final ImageView btnAlarmStatus = images.get(1);
                        /*final ImageView btnWorkStatus = getWorkStatus(i);
                        final ImageView btnAlarmStatus = getAlarmStatus(i);*/
                        final ImageView btnWorkCommand = getWorkCommand(i);
                        final TextView txtDue = getWorkCommandDue(i);

                        final Pipe pipe = MenuActivity.Instance().getPipeStatus(i);

                        if (pipe != null) {
                            textName.post(new Runnable() {
                                public void run() {
                                    setPipeName(textName, textLiquid, textLink, pipe);
                                    /*// 공간부족으로 배관명 앞의 "PT-"는 생략시킨다.
                                    textName.setText(pipe.getName().replace("PT-", ""));*/
                                }
                            });

                            textPressure.post(new Runnable() {
                                public void run() {
                                    setPressureText(textPressure, pipe);
                                    //textPressure.setText(PressureTag + pipe.getPressureString());
                                }
                            });

                            textRange.post(new Runnable() {
                                public void run() {
                                    setRangeText(textRange, pipe);
                                    //textRange.setText(RangeTag + pipe.getRangeString());
                                }
                            });

                            /*textLiquid.post(new Runnable() {
                                public void run() {
                                    setLiquidText(textLiquid, pipe);
                                }
                            });*/
                            textFlow.post(new Runnable() {
                                public void run() {
                                    setFlowText(textFlow, pipe);
                                }
                            });
                            textFlowRange.post(new Runnable() {
                                public void run() {
                                    setFlowRangeText(textFlowRange, pipe);
                                }
                            });

                            btnWorkStatus.post(new Runnable() {
                                public void run() {
                                    setStatus(btnWorkStatus, btnAlarmStatus, btnWorkCommand, txtDue, panel, pipe);
                                }
                            });

                            /*btnStatus.post(new Runnable() {
                                public void run() {
                                    setStatus(btnStatus, btnAlarm, panel, pipe, layoutIgnoreAlarm, textIgnoreAlarm);
                                }
                            });*/
                            if (i == m_nPipeCount)
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

                            /*btnAlarm.post(new Runnable() {
                                @Override
                                public void run() {
                                    if (pipe.getIgnoreAlarmMinute() > 0)
                                    {
                                        setImage(btnAlarm, R.drawable.ignore_alarm_run);
                                        textIgnoreAlarm.setText(pipe.getIgnoreAlarmTime());
                                    }
                                    else
                                    {
                                        setImage(btnAlarm, R.drawable.ignore_alarm_ready);
                                        textIgnoreAlarm.setText("");
                                    }
                                }
                            });*/
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

    private void setPipeName(TextView textName, TextView textLiquid, TextView textLink, Pipe pipe)
    {
        // 공간부족으로 배관명 앞의 "PT-"는 생략시킨다.
        String name = pipe.getName().replace("PT-", "");

        int nIndex = name.indexOf('(');

        if (nIndex >= 0)
            name = name.substring(0, nIndex);

        String type = pipe.getPipeType();
        //textName.setText(" " + name);
        //textType.setText("        " + type);

        String[] tokens = type.split("/");
        String strType = tokens[0].trim();
        String strInch = tokens[1].trim();

        name = " " + name;
        String strName = name + strType + strInch;//type;

        Spannable wordtoSpan = new SpannableString(strName);
        wordtoSpan.setSpan(new RelativeSizeSpan(0.8f), 0, name.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        wordtoSpan.setSpan(new RelativeSizeSpan(0.5f), name.length(), strName.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

        Tank tank = pipe.getLinkTank();
        textName.setText(wordtoSpan);

        //textLiquid.setText("   " + LiquidTag + pipe.getLiquid());
        if(tank == null)
            textLink.setText("");
        else {
            String strLiquidType = tank.getLiquidType().replace("메틸렌클로라이드", "MC");
            textLink.setText("   TK-" + tank.getName() + "(" + strLiquidType + ")");
            //if(pipe.getStatus() == 0)
                textLink.setTextColor(Color.GREEN);
            //else
            //    textLink.setTextColor(Color.RED);
        }
    }

    // 현재유량
    private void setFlowText(TextView text, Pipe pipe)
    {
        String strFlow = "";
        Tank tank = pipe.getLinkTank();
        if(tank != null) {
            strFlow = tank.getFlowString();
        }

        String str = FlowTag + strFlow;
        String total = str;
        if(strFlow != "")
            total += " KL/h";

        Spannable wordtoSpan = new SpannableString(total);
        wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, FlowTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        int nAlarmType = pipe.getAlarmType();
        if (strFlow != "" && (nAlarmType == 1024 || nAlarmType == 2048)) {
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 0, 0)), FlowTag.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        }
        else{
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(16, 253, 7)), FlowTag.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
        }

        text.setText(wordtoSpan);
    }

    // 유량 범위
    private void setFlowRangeText(TextView text, Pipe pipe)
    {
        boolean isInWork = pipe.getWork();
        if(isInWork) {
            Tank tank = pipe.getLinkTank();
            if(tank != null) {
                String strRange = tank.getFlowRangeString();
                String strRangeType = "";
                if(strRange != "")
                    strRangeType = "(" + tank.getFlowRangeTypeString() + ")";

                String str = FlowRange + strRange + strRangeType;
                Spannable wordtoSpan = new SpannableString(str);
                wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, FlowRange.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), FlowRange.length(), str.length() - strRangeType.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                wordtoSpan.setSpan(new RelativeSizeSpan(0.8f), str.length() - strRangeType.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                text.setText(wordtoSpan);
                return;
            }
        }

        text.setText(FlowRange);
    }

    // 압력범위
    private void setRangeText(TextView text, Pipe pipe)
    {
        boolean isInWork = pipe.getWork();

        if(isInWork) {
            String strRange = pipe.getPressureRangeString();
            String str = RangeTag + pipe.getRangeString() + strRange;

            Spannable wordtoSpan = new SpannableString(str);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, RangeTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(252, 157, 71)), RangeTag.length(), str.length() - strRange.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            wordtoSpan.setSpan(new RelativeSizeSpan(0.8f), str.length() - strRange.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            text.setText(wordtoSpan);
        }
        else
            text.setText(RangeTag);
    }

    // 현재압력
    private void setPressureText(TextView text, Pipe pipe)
    {
        int nAlarmType = pipe.getAlarmType();
        String str = PressureTag + pipe.getPressureString();
        String total = str;
        if(str != "")
            total = str + " Kg/cm²";

        if (nAlarmType == 256 || nAlarmType == 512) {
            Spannable wordtoSpan = new SpannableString(total);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, PressureTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(255, 0, 0)), PressureTag.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            text.setText(wordtoSpan);
        }
        else/* if (strStatus.endsWith("작동중"))*/{
            int colorGoodWork = 0x10FD07;

            Spannable wordtoSpan = new SpannableString(total);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.rgb(16, 253, 7)), PressureTag.length(), str.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            wordtoSpan.setSpan(new ForegroundColorSpan(Color.WHITE), 0, PressureTag.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            text.setText(wordtoSpan);
        }
        /*else
            text.setText(str);*/
    }

    // 작업관리
    private void setStatus(ImageView btnWorkStatus, ImageView btnAlarmStatus, ImageView btnWorkCommand, TextView txtDue, ImageView panel, Pipe pipe)
    {
        int nStatus = pipe.getAlarmID();
        boolean isInWork = pipe.getWork();
        boolean alarm = false;

        Tank tank = pipe.getLinkTank();
        int nAlarmID1 = 0;
        int nAlarmID2 = 0;
        if(tank != null)
        {
            nAlarmID1 = tank.getAlarmID1();
            nAlarmID2 = tank.getAlarmID2();
        }

        if (nStatus != 0 || nAlarmID1 != 0 || nAlarmID2 != 0) {
            alarm = true;
            setImage(panel, R.drawable.item_panel_alarm_dot);

            if (MenuActivity.Instance().getPipeItems(1)) {
                /*if (nStatus == PRESS_DECREASE) { // 온도 하강
                    mapAlarmStatus.put(btnAlarmStatus, StatusMode.LOW_ALARM);
                } else {    // 온도 상승
                    mapAlarmStatus.put(btnAlarmStatus, StatusMode.HI_ALARM);
                }*/
                setImage(btnAlarmStatus, R.drawable.alarm_off);
            }
        }
        else {
            setImage(panel, R.drawable.item_panel_dot);
            if (MenuActivity.Instance().getPipeItems(1)) {
                if(isInWork)
                    setImage(btnAlarmStatus, R.drawable.pipe_normal_pressure);
                else
                    setImage(btnAlarmStatus, R.drawable.pipe_normal_status);
                //setImage(btnAlarmStatus, R.drawable.pipe_no_alarm);
            }
        }

        if(isInWork)
        {
            setImage(btnWorkStatus, R.drawable.pipe_in_work);
            mapWorkStatus.put(btnWorkCommand, StatusMode.IN_WORK);
            setImage(btnWorkCommand, R.drawable.pipe_end_work);

            String time = pipe.GetWorkTime();
            String [] times = time.split(",");

            if(times.length == 2) {
                int len1 = times[0].length();
                int len2 = times[1].length();

                String strTime = String.format("%s시간 %s분", times[0], times[1]);

                Spannable wordtoSpan = new SpannableString(strTime);
                wordtoSpan.setSpan(new RelativeSizeSpan(0.7f), len1, len1 + 3, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                wordtoSpan.setSpan(new RelativeSizeSpan(0.7f), len1 + 3 + len2, len1 + 3 + len2 + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                txtDue.setText(wordtoSpan);
            }
        }
        else
        {
            setImage(btnWorkStatus, R.drawable.pipe_in_wait);
            mapWorkStatus.put(btnWorkCommand, StatusMode.WAITING);
            setImage(btnWorkCommand, R.drawable.pipe_begin_work);

            txtDue.setText("");
        }
    }

    /*private ImageView getWorkStatus(int index)
    {
        switch (index)
        {
            case 1:
                return (ImageView) findViewById(R.id.imgPipe1WorkStatus);

            case 2:
                return (ImageView) findViewById(R.id.imgPipe2WorkStatus);

            case 3:
                return (ImageView) findViewById(R.id.imgPipe3WorkStatus);

            case 4:
                return (ImageView) findViewById(R.id.imgPipe4WorkStatus);

            case 5:
                return (ImageView) findViewById(R.id.imgPipe5WorkStatus);

            case 6:
                return (ImageView) findViewById(R.id.imgPipe6WorkStatus);

            case 7:
                return (ImageView) findViewById(R.id.imgPipe7WorkStatus);

            case 8:
                return (ImageView) findViewById(R.id.imgPipe8WorkStatus);

            case 9:
                return (ImageView) findViewById(R.id.imgPipe9WorkStatus);
        }

        return null;
    }

    private ImageView getAlarmStatus(int index)
    {
        switch (index)
        {
            case 1:
                return (ImageView) findViewById(R.id.imgPipe1AlarmStatus);

            case 2:
                return (ImageView) findViewById(R.id.imgPipe2AlarmStatus);

            case 3:
                return (ImageView) findViewById(R.id.imgPipe3AlarmStatus);

            case 4:
                return (ImageView) findViewById(R.id.imgPipe4AlarmStatus);

            case 5:
                return (ImageView) findViewById(R.id.imgPipe5AlarmStatus);

            case 6:
                return (ImageView) findViewById(R.id.imgPipe6AlarmStatus);

            case 7:
                return (ImageView) findViewById(R.id.imgPipe7AlarmStatus);

            case 8:
                return (ImageView) findViewById(R.id.imgPipe8AlarmStatus);

            case 9:
                return (ImageView) findViewById(R.id.imgPipe9AlarmStatus);
        }

        return null;
    }*/

    private ImageView getWorkCommand(int index)
    {
        if(m_listWorkCommand.size() < index)
            return null;

        return m_listWorkCommand.get(index-1).iv;
    }

    private TextView getWorkCommandDue(int index)
    {
        if(m_listWorkCommand.size() < index)
            return null;

        return m_listWorkCommand.get(index-1).tvDue;
    }

    /*private ImageView getPipePanel(int index)
    {
        switch (index)
        {
            case 1:
                return (ImageView)findViewById(R.id.imgPipe1);

            case 2:
                return (ImageView)findViewById(R.id.imgPipe2);

            case 3:
                return (ImageView)findViewById(R.id.imgPipe3);

            case 4:
                return (ImageView)findViewById(R.id.imgPipe4);

            case 5:
                return (ImageView)findViewById(R.id.imgPipe5);

            case 6:
                return (ImageView)findViewById(R.id.imgPipe6);

            case 7:
                return (ImageView)findViewById(R.id.imgPipe7);

            case 8:
                return (ImageView)findViewById(R.id.imgPipe8);

            case 9:
                return (ImageView)findViewById(R.id.imgPipe9);
        }

        return null;
    }*/

    private TextView getPipeNameTextView(int index)
    {
        if(m_listPipeName.size() < index)
            return null;

        return (TextView)m_listPipeName.get(index-1).tvName;
    }

    private TextView getLiquidTextView(int index)
    {
        if(m_listPipeStatus.size() < index)
            return null;

        return (TextView)m_listPipeName.get(index-1).tvLiquid;
    }

    /*private TextView getPipeTypeTextView(int index)
    {
        if(m_listPipeName.size() < index)
            return null;

        return (TextView)m_listPipeName.get(index-1).tvLiquid;
    }*/

    private TextView getPipeLinkTextView(int index)
    {
        if(m_listPipeName.size() < index)
            return null;

        return (TextView)m_listPipeName.get(index-1).tvTank;
    }

    private TextView getPressureTextView(int index)
    {
        if(m_listPipeStatus.size() < index)
            return null;

        return (TextView)m_listPipeStatus.get(index-1).tvPress;
    }

    private TextView getFlowTextView(int index)
    {
        if(m_listPipeStatus.size() < index)
            return null;

        return (TextView)m_listPipeStatus.get(index-1).tvFlow;
    }

    private TextView getFlowRangeTextView(int index)
    {
        if(m_listPipeStatus.size() < index)
            return null;

        return (TextView)m_listPipeStatus.get(index-1).tvFlowRange;
    }

    private TextView getRangeTextView(int index)
    {
        if(m_listPipeStatus.size() < index)
            return null;

        return (TextView)m_listPipeStatus.get(index-1).tvPressRange;
    }

    private void setLayerSize()
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View title = findViewById(R.id.imgPipeTitle);
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

    private void initImage(int pipeID, int workStatusID, int alarmStatusID, ImageView workCommand)
    {
        setImage((ImageView)findViewById(pipeID), R.drawable.item_panel_dot);
        setImage((ImageView)findViewById(workStatusID), R.drawable.pipe_no_alarm);
        setImage((ImageView)findViewById(alarmStatusID), R.drawable.pipe_no_alarm);
        setImage(workCommand, R.drawable.pipe_no_alarm);
    }

    private void initText(int pressureID, int rangeID, int liquidID)
    {
        TextView textPressure = (TextView)findViewById(pressureID);
        TextView textRange = (TextView)findViewById(rangeID);
        TextView textLiquid = (TextView)findViewById(liquidID);

        textPressure.setText(PressureTag);
        //textRange.setText(RangeTag);
        textLiquid.setText(LiquidTag);
    }

    private void initText(List<LayoutPipeStatus> list)
    {
        int cnt = list.size();
        for(int i=0; i<cnt; ++i) {
            LayoutPipeStatus pStatus = list.get(i);
            pStatus.tvPress.setText(PressureTag);
            //pStatus.tvPressRange.setText(RangeTag);
            //pStatus.tvLiquid.setText(LiquidTag);
            pStatus.tvFlow.setText(FlowTag);
            pStatus.tvFlowRange.setText(FlowRange);
        }
    }

    private void setImage(ImageView view, int nImageID)
    {
        if (view == null)
            return;

        Glide.with(this).load(nImageID).into(view);
    }

    public void btnIgnoreAlarmClick(View view)
    {
        // 알람 Off 권한이 있는가?
        if (MenuActivity.Instance().canAlarmOff() == false) {
            MainActivity.showAlert("알람해제 권한이 없는 사용자입니다.", "알림", this);
            return;
        }

        //long ignoreBegin = mapIgnoreAlarmTime.get(view);

        final View view2 = view;
        final Pipe pipe = getPipe(view.getId());

        if (pipe != null) {
            String strMessage = "";
            String strMessageTag = "";
            String strCommand = "";

            if (pipe.getIgnoreAlarmMinute() == 0) {
                String ignoreAlarmMinute = MenuActivity.readIgnoreAlarmMinute(getApplicationContext());
                strMessage = pipe.getName() + "의 알람이 " + ignoreAlarmMinute + this.getString(R.string.ignore_alarm_message);
                strMessageTag = this.getString(R.string.ignore_alarm_message_tag);
                strCommand = "0";
            }
            else {
                strMessage = pipe.getName() + "의 " + this.getString(R.string.cancel_ignore_alarm_message);
                strMessageTag = this.getString(R.string.cancel_ignore_alarm_message_tag);
                strCommand = "1";
            }

            final String ignoreCommand = strCommand;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage(strMessage);
            builder.setTitle(strMessageTag);

            builder.setNegativeButton("네", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which)
                {
                    //mapIgnoreAlarmTime.put(view2, System.currentTimeMillis());
                    //setImage((ImageView)view2, R.drawable.ignore_alarm_run);
                    WebManager mgr = new WebManager(getApplicationContext());
                    mgr.setQueryType(WebManager.QueryType.IGNORE_ALARM);

                    mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
                    mgr.setParameter("PipeID", String.valueOf(pipe.getID()));
                    mgr.setParameter("IgnoreType", ignoreCommand);
                    mgr.start();

                    int nStatus = pipe.getStatus();

                    if (nStatus != 0) {
                        mgr = new WebManager(getApplicationContext());
                        mgr.setQueryType(WebManager.QueryType.CLEAR_PIPE_ALARM);

                        mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
                        mgr.setParameter("PipeID", String.valueOf(pipe.getID()));
                        mgr.start();
                    }

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
    }

    public void WorkCommandClick(View view)
    {
        // Dlg가 떠 있으면 닫음.
        if(m_dlgTankList != null && m_bDlgVisible)
        {
            m_dlgTankList.dismiss();
            return;
        }

        m_statusMode = mapWorkStatus.get(view);

        if (m_statusMode != StatusMode.WAITING && m_statusMode != StatusMode.IN_WORK)
            return;

        // 작업 시작/종료 권한이 있는가?
        if (MenuActivity.Instance().canAlarmOff() == false) {
            MainActivity.showAlert("작업 시작/종료를 수행할수 없는 사용자입니다.", "알림", this);
            return;
        }

        View v = (View)view.getParent();
        m_selectPipe = getPipe(v.getId());

        if (m_statusMode != StatusMode.IN_WORK) {
            CreateDialog(view);
        }
        else {
            WorkCommandClick(m_selectPipe.getLinkTank());
        }
    }

    private Pipe GetPipeFromAlarmOff(View view)
    {
        ViewParent parent = view.getParent();

        if (parent == null)
            return null;

        if (parent instanceof LinearLayout) {
            LinearLayout parentLayout = (LinearLayout)parent;

            int nChildCount = parentLayout.getChildCount();

            for (int i=0;i<nChildCount;i++) {
                View childView = parentLayout.getChildAt(i);
                Pipe pipe = getPipe(childView.getId());

                if (pipe != null)
                    return pipe;
            }
        }

        return null;
    }

    public void btnAlarmOffClick(View view)
    {
        //StatusMode mode = mapAlarmStatus.get(view);

        /*if (mode != StatusMode.HI_ALARM && mode != StatusMode.LOW_ALARM)
            return;*/

        final Pipe pipe = GetPipeFromAlarmOff(view);
        //final Pipe pipe = getPipe(view.getId());
        if(pipe == null)
            return;

        final Tank tank = pipe.getLinkTank();
        if(tank == null)
            return;

        if(pipe.getAlarmID() == 0 && tank.getAlarmID1() == 0 && tank.getAlarmID2() == 0)
            return;

        // 알람 Off 권한이 있는가?
        if (MenuActivity.Instance().canAlarmOff() == false) {
            MainActivity.showAlert("알람해제 권한이 없는 사용자입니다.", "알림", this);
            return;
        }

        if (pipe != null) {
            CreateDlgAlarmClear(pipe, tank);
            /*AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.setMessage(pipe.getName() + "의 알람을 해제하시겠습니까?");
            builder.setTitle("확인");

            builder.setNegativeButton("네", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which)
                {
                    if(tank != null) {
                        if(pipe.getAlarmID() == 0)
                            MenuActivity.Instance().TankAlarmOff(tank);
                        else {
                            WebManager mgr = new WebManager(getApplicationContext());
                            mgr.setQueryType(WebManager.QueryType.CLEAR_PIPE_ALARM);

                            mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
                            mgr.setParameter("PipeID", String.valueOf(pipe.getID()));
                            mgr.setParameter("TankID", String.valueOf(tank.getID()));
                            mgr.start();
                        }
                    }
                    else
                        MainActivity.showAlert("알람해제 권한이 없는 사용자입니다.", "알림", null);

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

            builder.show();*/
        }
    }

    private void OnAlarmClear(DlgAlarmClear dlg)
    {
        int nType = dlg.GetOccurrenceType();
        String strComment = dlg.GetComment();
        Tank tank = dlg.GetTank();
        Pipe pipe = dlg.GetPipe();

        if(dlg.Result()) {
            if(TextUtils.isEmpty(strComment))
            {
                MainActivity.showAlert("알람 해결 내용을 입력해 주세요.", "알림", this);
                return;
            }
            if (tank != null && pipe != null) {
                if (pipe.getAlarmID() == 0)
                    MenuActivity.Instance().TankAlarmOff(tank, dlg.GetOccurrenceType(), dlg.GetComment());
                else {
                    MenuActivity.Instance().PipeAlarmOff(pipe, dlg.GetOccurrenceType(), dlg.GetComment());
                    /*WebManager mgr = new WebManager(getApplicationContext());
                    mgr.setQueryType(WebManager.QueryType.CLEAR_PIPE_ALARM);

                    mgr.setParameter("DeviceID", FirebaseInstanceId.getInstance().getToken());
                    mgr.setParameter("PipeID", String.valueOf(pipe.getID()));
                    mgr.setParameter("TankID", String.valueOf(tank.getID()));
                    mgr.setParameter("OccurType", String.valueOf(dlg.GetOccurrenceType()));
                    mgr.setParameter("Comment", dlg.GetComment());
                    mgr.start();*/
                }
            } else
                MainActivity.showAlert("탱크와 연결되지 않은 배관입니다.", "알림", this);

            dlg.dismiss();
        }
    }

    private Pipe getPipe(int viewID)
    {
        if (m_mapViewPipeIndex.containsKey(viewID))
        {
            int nPipeIndex = m_mapViewPipeIndex.get(viewID) + 1;
            return MenuActivity.Instance().getPipeStatus(nPipeIndex);
        }
        /*switch (viewID)
        {
            case R.id.layout_workcommand1:
            case R.id.imgPipe1AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(1);

            case R.id.layout_workcommand2:
            case R.id.imgPipe2AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(2);

            case R.id.layout_workcommand3:
            case R.id.imgPipe3AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(3);

            case R.id.layout_workcommand4:
            case R.id.imgPipe4AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(4);

            case R.id.layout_workcommand5:
            case R.id.imgPipe5AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(5);

            case R.id.layout_workcommand6:
            case R.id.imgPipe6AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(6);

            case R.id.layout_workcommand7:
            case R.id.imgPipe7AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(7);

            case R.id.layout_workcommand8:
            case R.id.imgPipe8AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(8);

            case R.id.layout_workcommand9:
            case R.id.imgPipe9AlarmStatus:
                return MenuActivity.Instance().getPipeStatus(9);
        }*/

        return null;
    }

    public void onNotify(String strTitle, String strMessage)
    {
        if (strTitle.equals("TankAlarm"))
        {
            MenuActivity.Instance().setInitTag(strTitle);
            finish();
        }
    }

    public class LayoutPipeName
    {
        public TextView tvName = null;
        public TextView tvLiquid = null; // 유종
        public TextView tvTank = null;
    }

    public class LayoutPipeStatus
    {
        public TextView tvPress = null;
        public TextView tvPressRange = null;
        public TextView tvFlow = null;
        public TextView tvFlowRange = null;
    }

    public class LayoutWorkCommand
    {
        public ImageView iv = null;
        public TextView tvDue = null;
    }
}
