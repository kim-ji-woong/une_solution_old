package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TableLayout;

import com.bumptech.glide.Glide;

public class ActionSOPActivity extends AppCompatActivity implements FCMReceiver {
    private static ActionSOPActivity m_instance = null;
    private ImageButton m_currentSOP = null;

    private String m_initTag1 = "", m_initTag2 = "";

    public static ActionSOPActivity getCurrentInstance()
    {
        return m_instance;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_sop);

        initImageButtons();
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
            moveToMainActivity(strTitle, strMessage);
    }

    public void setInitTag(String initTag1, String initTag2)
    {
        m_initTag1 = initTag1;
        m_initTag2 = initTag2;
    }

    private void moveToMainActivity(String strTitle, String strBody)
    {
        MainActivity.getInstance().setInitTag(strTitle, strBody);
        finish();
    }

    private void initImageButtons()
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View imgTitle = findViewById(R.id.imgTitle);
        int nTitleHeight = imgTitle.getLayoutParams().height;//imgTitle.getDrawable().getIntrinsicHeight();

        TableLayout sopLayout = (TableLayout) findViewById(R.id.actionSOPLayout);
        ViewGroup.LayoutParams sopParams = sopLayout.getLayoutParams();

        sopParams.width = (int)fScreenWidth;
        sopParams.height = (int)(fScreenHeight - nTitleHeight);

        int sopWidth = sopParams.width;
        int sopHeight = sopParams.height;

        setImage(R.id.sop1, R.drawable.sop1_normal);
        setImage(R.id.sop2, R.drawable.sop2_normal);
        setImage(R.id.sop3, R.drawable.sop3_normal);
        setImage(R.id.sop4, R.drawable.sop4_normal);
        setImage(R.id.sop5, R.drawable.sop5_normal);
        setImage(R.id.sop6, R.drawable.sop6_normal);

        ImageButton btnSOP1 = (ImageButton)findViewById(R.id.sop1);
        ImageButton btnSOP5 = (ImageButton)findViewById(R.id.sop5);
        ImageButton btnSOP6 = (ImageButton)findViewById(R.id.sop6);

        // 표준모델
        int stTopPadding = 632, stLeftPadding = 125, stRightPadding = 125;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();
        int stHorSpacing = 70;

        float scale = 1.0f;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth / scale);
        int right = left;
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight / scale);
        int height = (right - left) * sopHeight / sopWidth;
        int bottom = 0;

        sopLayout.setPadding(left, top, right, bottom);

        int horSpacing = (int)(stHorSpacing * fScreenWidth / stScreenWidth);

        btnSOP1.getLayoutParams().width = (int)((fScreenWidth - left - right - horSpacing * 2) / 3);
        btnSOP5.getLayoutParams().width = btnSOP1.getLayoutParams().width;
        btnSOP6.getLayoutParams().width = btnSOP1.getLayoutParams().width;

        sopLayout.setX(0.0f);
        sopLayout.setY(nTitleHeight);
    }

    private void setImage(int nViewID, int nImageID)
    {
        ImageView view = (ImageView)findViewById(nViewID);
        Glide.with(this).load(nImageID).into(view);
    }

    public void btnSOPClick(View v) {
        int nID = v.getId();
        String strSOPName = "";

        if (nID == R.id.sop1)
        {
            Glide.with(this).load(R.drawable.sop1_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop1_name);
        }
        else if (nID == R.id.sop2)
        {
            Glide.with(this).load(R.drawable.sop2_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop2_name);
        }
        else if (nID == R.id.sop3)
        {
            Glide.with(this).load(R.drawable.sop3_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop3_name);
        }
        else if (nID == R.id.sop4)
        {
            Glide.with(this).load(R.drawable.sop4_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop4_name);
        }
        else if (nID == R.id.sop5)
        {
            Glide.with(this).load(R.drawable.sop5_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop5_name);
        }
        else if (nID == R.id.sop6)
        {
            Glide.with(this).load(R.drawable.sop6_clicked).into((ImageButton)v);
            strSOPName = getApplicationContext().getString(R.string.sop6_name);
        }
        else
            return;

        m_currentSOP = (ImageButton)v;
        m_instance = this;

        Intent intent = new Intent(ActionSOPActivity.this, ActionSOPDetailActivity.class);
        intent.putExtra("SOP", strSOPName);
        startActivity(intent);
    }

    public void onNotify(String strTitle, String strBoday)
    {
        moveToMainActivity(strTitle, strBoday);
    }
}
