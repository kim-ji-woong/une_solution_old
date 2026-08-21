package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.LinearLayout;

import com.bumptech.glide.Glide;

import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

import kr.co.une.energyindustrye_sop.photoview.PhotoView;
import kr.co.une.energyindustrye_sop.utility.ImageHelper;

public class ActionSOPDetailActivity extends AppCompatActivity implements FCMReceiver {

    private int m_nSelectedSOPStep = -1;
    private int m_nSelectedTeam = -1;

    private ImageHelper m_imgHelper = new ImageHelper();
    private int m_nImageWidth = -1;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_sopdetail);

        initImageRatio();
        initLayout();
        initLeftImages();
        initBodyImageLayout();
        initSOPStepImages();

        onSOPTeamClick(findViewById(R.id.control_center));
        onStepClick(findViewById(R.id.btnActionSOPStep));
    }

    private void initBodyImageLayout()
    {
        View panel = findViewById(R.id.actionSOPDetailBody);
        ImageView imgBody = (ImageView)findViewById(R.id.actionSOPDetailImage);
        View bottomMenu = findViewById(R.id.actionSOPDetailBottomMenuPanel);
        View title = findViewById(R.id.imgTitle);
        View imgPanel = findViewById(R.id.actionSOPDetailBodyPanel);

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
        int leftPanelWidth = findViewById(R.id.actionSOPLeftPanel).getLayoutParams().width;

        imgPanel.setPadding(left, top, right, bottom);
        //panel.setPadding(left, top, right, bottom);
        panelParams.width = (int)fScreenWidth - leftPanelWidth;
        panelParams.height = (int)fScreenHeight - titleHeight - menuHeight;

        m_nImageWidth = panelParams.width - left;
        //ViewGroup.LayoutParams imgParams = imgBody.getLayoutParams();
        //imgParams.width =  panelParams.width - left;
        //imgParams.height = imgParams.width * 1189 / 1117;

        float bottomY = (int)(fScreenHeight - menuHeight * 1.4f);
        bottomMenu.setX(0.0f);
        bottomMenu.setY(bottomY);
    }

    private void initLeftImages()
    {
        View leftPanal = findViewById(R.id.actionSOPLeftPanel);
        int layoutWidth = leftPanal.getLayoutParams().width;

        int paddingBottom = findViewById(R.id.control_center).getPaddingBottom();
        leftPanal.setX(-paddingBottom);

        ImageHelper helper = new ImageHelper();
        long originalSize = helper.readImageOriginalSize(R.drawable.control_center_normal, getResources());

        int nOriginalWidth = -1, nOriginalHeight = -1;

        if (originalSize > 0)
        {
            nOriginalWidth = (int)(originalSize >> 32);
            nOriginalHeight = (int)(originalSize & 0xffffffff);
        }

        setLeftimage(R.id.control_center, R.drawable.control_center_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.facility_team, R.drawable.facility_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.local_boss, R.drawable.local_boss_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.safety, R.drawable.safety_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.cs_team, R.drawable.cs_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
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

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.actionSOPDetailBody);
        ViewGroup.LayoutParams bodyParams = bodyLayout.getLayoutParams();

        int nTitleHeight = imgTitle.getLayoutParams().height;
        int nBottomMenuHeight = findViewById(R.id.actionSOPDetailBottomMenuPanel).getLayoutParams().height;

        bodyLayout.setX(0.0f);
        bodyLayout.setY(nTitleHeight);
        bodyParams.width = (int)fScreenWidth;
        bodyParams.height = (int)fScreenHeight - nTitleHeight - nBottomMenuHeight;

        /*LinearLayout infoLayout = (LinearLayout) findViewById(nLayoutID);
        ViewGroup.LayoutParams infoParams = infoLayout.getLayoutParams();

        infoParams.width = (int)fScreenWidth;
        infoParams.height = (int)(fScreenHeight - nTitleHeight - nMenuHeight);

        int infoWidth = infoParams.width;
        int infoHeight = infoParams.height;

        // 표준모델
        int stTopPadding = 614, stLeftPadding = 104, stRightPadding = 104;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();
        int stHorSpacing = 70;

        float scale = 1.0f;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth / scale);
        int right = left;
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight / scale);
        int height = (right - left) * infoHeight / infoWidth;
        int bottom = 0;

        infoLayout.setPadding(left, top, right, bottom);*/

        //sopLayout.setX(0.0f);
        //sopLayout.setY(nTitleHeight);
    }

    private void initImageRatio()
    {
        Resources res = getResources();

        m_imgHelper.readImageOriginalSize(R.drawable.control_center_sop_step1, res);
        m_imgHelper.readImageOriginalSize(R.drawable.control_center_sop_step2, res);
        m_imgHelper.readImageOriginalSize(R.drawable.control_center_sop_step3, res);
        m_imgHelper.readImageOriginalSize(R.drawable.control_center_sop_step4, res);

        m_imgHelper.readImageOriginalSize(R.drawable.facility_sop_step1, res);
        m_imgHelper.readImageOriginalSize(R.drawable.facility_sop_step2, res);
        m_imgHelper.readImageOriginalSize(R.drawable.facility_sop_step3, res);
        m_imgHelper.readImageOriginalSize(R.drawable.facility_sop_step4, res);

        m_imgHelper.readImageOriginalSize(R.drawable.local_boss_sop_step1, res);
        m_imgHelper.readImageOriginalSize(R.drawable.local_boss_sop_step2, res);
        m_imgHelper.readImageOriginalSize(R.drawable.local_boss_sop_step3, res);
        m_imgHelper.readImageOriginalSize(R.drawable.local_boss_sop_step4, res);

        m_imgHelper.readImageOriginalSize(R.drawable.safety_sop_step1, res);
        m_imgHelper.readImageOriginalSize(R.drawable.safety_sop_step2, res);
        m_imgHelper.readImageOriginalSize(R.drawable.safety_sop_step3, res);
        m_imgHelper.readImageOriginalSize(R.drawable.safety_sop_step4, res);

        m_imgHelper.readImageOriginalSize(R.drawable.cs_sop_step1, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_sop_step2, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_sop_step3, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_sop_step4, res);
    }

    private void setSOPImage()
    {
        int nImageID = 0;

        if (m_nSelectedTeam == R.id.control_center)
        {
            if (m_nSelectedSOPStep == R.id.btnActionSOPStep)
                nImageID = R.drawable.control_center_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep2)
                nImageID = R.drawable.control_center_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep3)
                nImageID = R.drawable.control_center_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep4)
                nImageID = R.drawable.control_center_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.facility_team)
        {
            if (m_nSelectedSOPStep == R.id.btnActionSOPStep)
                nImageID = R.drawable.facility_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep2)
                nImageID = R.drawable.facility_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep3)
                nImageID = R.drawable.facility_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep4)
                nImageID = R.drawable.facility_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.local_boss)
        {
            if (m_nSelectedSOPStep == R.id.btnActionSOPStep)
                nImageID = R.drawable.local_boss_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep2)
                nImageID = R.drawable.local_boss_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep3)
                nImageID = R.drawable.local_boss_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep4)
                nImageID = R.drawable.local_boss_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.safety)
        {
            if (m_nSelectedSOPStep == R.id.btnActionSOPStep)
                nImageID = R.drawable.safety_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep2)
                nImageID = R.drawable.safety_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep3)
                nImageID = R.drawable.safety_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep4)
                nImageID = R.drawable.safety_sop_step4;
            else
                return;
        }
        else if (m_nSelectedTeam == R.id.cs_team)
        {
            if (m_nSelectedSOPStep == R.id.btnActionSOPStep)
                nImageID = R.drawable.cs_sop_step1;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep2)
                nImageID = R.drawable.cs_sop_step2;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep3)
                nImageID = R.drawable.cs_sop_step3;
            else if (m_nSelectedSOPStep == R.id.btnActionSOPStep4)
                nImageID = R.drawable.cs_sop_step4;
            else
                return;
        }
        else
            return;

        ImageView imgBody = (ImageView)findViewById(R.id.actionSOPDetailImage);
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

    public void onSOPTeamClick(View v)
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

    private void setSOPTeamImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.control_center)
        {
            if (selected)
                Glide.with(this).load(R.drawable.control_center_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.control_center_normal).into(view);
        }
        else if (nID == R.id.facility_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.facility_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.facility_normal).into(view);
        }
        else if (nID == R.id.local_boss)
        {
            if (selected)
                Glide.with(this).load(R.drawable.local_boss_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.local_boss_normal).into(view);
        }
        else if (nID == R.id.safety)
        {
            if (selected)
                Glide.with(this).load(R.drawable.safety_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.safety_normal).into(view);
        }
        else if (nID == R.id.cs_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.cs_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.cs_normal).into(view);
        }
        else
            return;
    }

    private void setSOPStepImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.btnActionSOPStep)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_sop_step1_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_sop_step1_normal).into(view);
        }
        else if (nID == R.id.btnActionSOPStep2)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_sop_step2_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_sop_step2_normal).into(view);
        }
        else if (nID == R.id.btnActionSOPStep3)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_sop_step3_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_sop_step3_normal).into(view);
        }
        else if (nID == R.id.btnActionSOPStep4)
        {
            if (selected)
                Glide.with(this).load(R.drawable.btn_sop_step4_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.btn_sop_step4_normal).into(view);
        }
        else
            return;
    }

    private void initSOPStepImages()
    {
        setSOPStepImage(R.id.btnActionSOPStep, false);
        setSOPStepImage(R.id.btnActionSOPStep2, false);
        setSOPStepImage(R.id.btnActionSOPStep3, false);
        setSOPStepImage(R.id.btnActionSOPStep4, false);
    }

    public void onNotify(String strTitle, String strBody)
    {
        ActionSOPActivity.getCurrentInstance().setInitTag(strTitle, strBody);
        finish();
    }
}
