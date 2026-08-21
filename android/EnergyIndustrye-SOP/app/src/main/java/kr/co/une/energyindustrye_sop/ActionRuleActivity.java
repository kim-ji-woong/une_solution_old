package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.GridLayout;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TableLayout;

import com.bumptech.glide.Glide;

public class ActionRuleActivity extends AppCompatActivity implements FCMReceiver {

    private static ActionRuleActivity m_instance = null;
    private ImageButton m_currentSOP = null;

    private String m_initTag1 = "", m_initTag2 = "";

    public static ActionRuleActivity getCurrentInstance()
    {
        return m_instance;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_rule);

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

        TableLayout sopLayout = (TableLayout) findViewById(R.id.sopLayout);
        ViewGroup.LayoutParams sopParams = sopLayout.getLayoutParams();

        sopParams.width = (int)fScreenWidth;
        sopParams.height = (int)(fScreenHeight - nTitleHeight);

        int sopWidth = sopParams.width;
        int sopHeight = sopParams.height;

        setImage(R.id.fireSOP, R.drawable.fire_normal);
        setImage(R.id.typhoonSOP, R.drawable.typhoon_normal);
        setImage(R.id.earthquakeSOP, R.drawable.earthquake_normal);
        setImage(R.id.pollutionSOP, R.drawable.pollution_normal);
        setImage(R.id.heavySnowSOP, R.drawable.heavy_snow_normal);
        setImage(R.id.floodSOP, R.drawable.flood_normal);
        setImage(R.id.destructionSOP, R.drawable.destruction_normal);
        setImage(R.id.explosionSOP, R.drawable.explosion_normal);

        ImageButton btnFire = (ImageButton)findViewById(R.id.fireSOP);
        ImageButton btnEarthquake = (ImageButton)findViewById(R.id.earthquakeSOP);
        ImageButton btnTyphoon = (ImageButton)findViewById(R.id.typhoonSOP);

        // 표준모델
        int stTopPadding = 330, stLeftPadding = 125, stRightPadding = 125;
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

        btnFire.getLayoutParams().width = (int)((fScreenWidth - left - right - horSpacing * 2) / 3);
        btnEarthquake.getLayoutParams().width = btnFire.getLayoutParams().width;
        btnTyphoon.getLayoutParams().width = btnFire.getLayoutParams().width;

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

        if (nID == R.id.fireSOP)
        {
            Glide.with(this).load(R.drawable.fire_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.fire_clicked);
            strSOPName = getApplicationContext().getString(R.string.fire);
        }
        else if (nID == R.id.typhoonSOP)
        {
            Glide.with(this).load(R.drawable.typhoon_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.typhoon_clicked);
            strSOPName = getApplicationContext().getString(R.string.typhoon);
        }
        else if (nID == R.id.earthquakeSOP)
        {
            Glide.with(this).load(R.drawable.earthquake_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.earthquake_clicked);
            strSOPName = getApplicationContext().getString(R.string.earthquake);
        }
        else if (nID == R.id.pollutionSOP)
        {
            Glide.with(this).load(R.drawable.pollution_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.pollution_clicked);
            strSOPName = getApplicationContext().getString(R.string.pollution);
        }
        else if (nID == R.id.heavySnowSOP)
        {
            Glide.with(this).load(R.drawable.heavy_snow_clicked).into((ImageButton)v);
            //(ImageButton)v).setImageResource(R.drawable.heavy_snow_clicked);
            strSOPName = getApplicationContext().getString(R.string.heavy_snow);
        }
        else if (nID == R.id.floodSOP)
        {
            Glide.with(this).load(R.drawable.flood_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.flood_clicked);
            strSOPName = getApplicationContext().getString(R.string.flood);
        }
        else if (nID == R.id.destructionSOP)
        {
            Glide.with(this).load(R.drawable.destruction_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.destruction_clicked);
            strSOPName = getApplicationContext().getString(R.string.destruction);
        }
        else if (nID == R.id.explosionSOP)
        {
            Glide.with(this).load(R.drawable.explosion_clicked).into((ImageButton)v);
            //((ImageButton)v).setImageResource(R.drawable.explosion_clicked);
            strSOPName = getApplicationContext().getString(R.string.explosion);
        }
        else
            return;

        m_currentSOP = (ImageButton)v;
        m_instance = this;

        Intent intent = new Intent(ActionRuleActivity.this, ActionRuleDetailActivity.class);
        intent.putExtra("SOP", strSOPName);
        startActivity(intent);
    }

    // 선택된 SOP 버튼을 초기화 시킨다.
    public void initSOPButton()
    {
        if (m_currentSOP == null)
            return;

        int nID = m_currentSOP.getId();

        if (nID == R.id.fireSOP)
        {
            Glide.with(this).load(R.drawable.fire_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.fire_normal);
        }
        else if (nID == R.id.typhoonSOP)
        {
            Glide.with(this).load(R.drawable.typhoon_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.typhoon_normal);
        }
        else if (nID == R.id.earthquakeSOP)
        {
            Glide.with(this).load(R.drawable.earthquake_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.earthquake_normal);
        }
        else if (nID == R.id.pollutionSOP)
        {
            Glide.with(this).load(R.drawable.pollution_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.pollution_normal);
        }
        else if (nID == R.id.heavySnowSOP)
        {
            Glide.with(this).load(R.drawable.heavy_snow_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.heavy_snow_normal);
        }
        else if (nID == R.id.floodSOP)
        {
            Glide.with(this).load(R.drawable.flood_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.flood_normal);
        }
        else if (nID == R.id.destructionSOP)
        {
            Glide.with(this).load(R.drawable.destruction_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.destruction_normal);
        }
        else if (nID == R.id.explosionSOP)
        {
            Glide.with(this).load(R.drawable.explosion_normal).into(m_currentSOP);
            //m_currentSOP.setImageResource(R.drawable.explosion_normal);
        }
    }

    public void onNotify(String strTitle, String strBody)
    {
        moveToMainActivity(strTitle, strBody);
    }
}
