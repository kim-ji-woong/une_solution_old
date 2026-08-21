package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AnimationUtils;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ViewFlipper;

import com.bumptech.glide.Glide;

import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

import kr.co.une.energyindustrye_sop.photoview.PhotoView;
import kr.co.une.energyindustrye_sop.photoview.PhotoViewOwner;
import kr.co.une.energyindustrye_sop.utility.ImageHelper;

public class ActionPrivateMission extends AppCompatActivity implements FCMReceiver, PhotoViewOwner {

    private int m_nSelectedTeam = -1;

    //private ImageHelper m_imgHelper = new ImageHelper();
    private int m_nImageWidth = -1;

    private int m_nImageIndex = -1;
    private int m_nImageCount = 0;
    private ViewFlipper mViewFlipper = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);
        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_private_mission);

        //initImageRatio();
        initLayout();
        initLeftImages();
        initBodyImageLayout();

        mViewFlipper = (ViewFlipper)findViewById(R.id.flipperSystem);

        onTeamClick(findViewById(R.id.center_boss));

        m_nSelectedTeam = R.id.center_boss;
        setMissionImage();
    }

    @Override
    protected void onDestroy() {
        clearFlipperImages();
        super.onDestroy();
    }

    private void initBodyImageLayout()
    {
        View panel = findViewById(R.id.actionPrivateMissionBody);
        //ImageView imgBody = (ImageView)findViewById(R.id.actionMissionImage);
        View title = findViewById(R.id.imgTitle);
        View imgPanel = findViewById(R.id.actionPrivateMissionBodyPanel);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);
        int titleHeight = title.getLayoutParams().height;

        // 표준모델
        int stTopPadding = 206, stLeftPadding = stTopPadding / 4;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = 0;
        int top = (int)(stTopPadding * fScreenHeight / stScreenHeight);
        int bottom = 0;

        ViewGroup.LayoutParams panelParams = imgPanel.getLayoutParams();
        int leftPanelWidth = findViewById(R.id.actionPrivateMissionLeftPanel).getLayoutParams().width;

        imgPanel.setPadding(left, top, right, bottom);
        panelParams.width = (int)fScreenWidth - leftPanelWidth;
        panelParams.height = (int)fScreenHeight - titleHeight - ((ViewGroup.MarginLayoutParams)panelParams).topMargin;// - menuHeight;

        m_nImageWidth = panelParams.width - left;
    }

    private void initLeftImages()
    {
        View leftPanal = findViewById(R.id.actionPrivateMissionLeftPanel);
        int layoutWidth = leftPanal.getLayoutParams().width;

        int paddingBottom = findViewById(R.id.center_boss).getPaddingBottom();
        leftPanal.setX(-paddingBottom);

        ImageHelper helper = new ImageHelper();
        long originalSize = helper.readImageOriginalSize(R.drawable.center_boss_normal, getResources());

        int nOriginalWidth = -1, nOriginalHeight = -1;

        if (originalSize > 0)
        {
            nOriginalWidth = (int)(originalSize >> 32);
            nOriginalHeight = (int)(originalSize & 0xffffffff);
        }

        setLeftimage(R.id.center_boss, R.drawable.center_boss_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.cs_team, R.drawable.cs_team_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.ms_team, R.drawable.ms_team_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.electric_team, R.drawable.electric_team_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
        setLeftimage(R.id.operating_team, R.drawable.operating_team_normal, layoutWidth, nOriginalWidth, nOriginalHeight);
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

        LinearLayout bodyLayout = (LinearLayout)findViewById(R.id.actionPrivateMissionBody);
        ViewGroup.LayoutParams bodyParams = bodyLayout.getLayoutParams();

        int nTitleHeight = imgTitle.getLayoutParams().height;
        //int nBottomMenuHeight = findViewById(R.id.actionSOPDetailBottomMenuPanel).getLayoutParams().height;

        bodyLayout.setX(0.0f);
        bodyLayout.setY(nTitleHeight);
        bodyParams.width = (int)fScreenWidth;
        bodyParams.height = (int)fScreenHeight - nTitleHeight;// - nBottomMenuHeight;
    }

    /*private void initImageRatio()
    {
        Resources res = getResources();

        m_imgHelper.readImageOriginalSize(R.drawable.center_boss_mission, res);

        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission01, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission02, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission03, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission04, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission05, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission06, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission07, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission08, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission09, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission10, res);
        m_imgHelper.readImageOriginalSize(R.drawable.cs_team_mission11, res);

        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission01, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission02, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission03, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission04, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission05, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission06, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission07, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission08, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission09, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission10, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission11, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission12, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission13, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission14, res);
        m_imgHelper.readImageOriginalSize(R.drawable.ms_team_mission15, res);

        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission01, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission02, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission03, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission04, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission05, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission06, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission07, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission08, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission09, res);
        m_imgHelper.readImageOriginalSize(R.drawable.electric_team_mission10, res);

        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission01, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission02, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission03, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission04, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission05, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission06, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission07, res);
        m_imgHelper.readImageOriginalSize(R.drawable.operating_team_mission08, res);
    }*/

    private void setMissionImage()
    {
        /*int nImageID = 0;

        if (m_nSelectedTeam == R.id.center_boss)
        {
            nImageID = R.drawable.center_boss_mission;
        }
        else if (m_nSelectedTeam == R.id.cs_team)
        {
            nImageID = R.drawable.cs_team_mission01;
        }
        else if (m_nSelectedTeam == R.id.ms_team)
        {
            nImageID = R.drawable.ms_team_mission01;
        }
        else if (m_nSelectedTeam == R.id.electric_team)
        {
            nImageID = R.drawable.electric_team_mission01;
        }
        else if (m_nSelectedTeam == R.id.operating_team)
        {
            nImageID = R.drawable.operating_team_mission01;
        }
        else
            return;*/

        setFlipperImage(m_nSelectedTeam);
        /*ImageView imgBody = (ImageView)findViewById(R.id.actionMissionImage);
        Glide.with(this).load(nImageID).into(imgBody);

        int originalWidth = m_imgHelper.getImageOriginalWidth(nImageID);
        int originalHeight = m_imgHelper.getImageOriginalHeight(nImageID);

        if (originalWidth > 0)
        {
            ViewGroup.LayoutParams imgParams = imgBody.getLayoutParams();
            imgParams.width = m_nImageWidth;
            imgParams.height = imgParams.width * originalHeight / originalWidth;
        }*/
    }

    private void setFlipperImage(int nButtonID)
    {
        clearFlipperImages();
        //initPointList();

        if (nButtonID == R.id.center_boss)
        {
            mViewFlipper.addView(getImageView(R.drawable.center_boss_mission));

            //addPoint(m_point3);
            //addPoint(m_point4);
        }
        else if (nButtonID == R.id.cs_team)
        {
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission01));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission02));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission03));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission04));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission05));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission06));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission07));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission08));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission09));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission10));
            mViewFlipper.addView(getImageView(R.drawable.cs_team_mission11));
        }
        else if (nButtonID == R.id.ms_team)
        {
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission01));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission02));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission03));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission04));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission05));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission06));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission07));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission08));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission09));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission10));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission11));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission12));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission13));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission14));
            mViewFlipper.addView(getImageView(R.drawable.ms_team_mission15));

            /*addPoint(m_point1);
            addPoint(m_point2);
            addPoint(m_point3);
            addPoint(m_point4);
            addPoint(m_point5);
            addPoint(m_point6);*/
        }
        else if (nButtonID == R.id.electric_team)
        {
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission01));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission02));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission03));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission04));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission05));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission06));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission07));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission08));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission09));
            mViewFlipper.addView(getImageView(R.drawable.electric_team_mission10));
        }
        else if (nButtonID == R.id.operating_team)
        {
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission01));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission02));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission03));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission04));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission05));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission06));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission07));
            mViewFlipper.addView(getImageView(R.drawable.operating_team_mission08));
        }
        else
            return;

        m_nImageCount = mViewFlipper.getChildCount();
        m_nImageIndex = 0;

        /*if (m_nImageCount > 1)
            selectPoint(0, -1);*/
    }

    private void clearFlipperImages()
    {
        mViewFlipper.removeAllViews();
    }

    private ImageView getImageView(int nID)
    {
        PhotoView image = new PhotoView(getApplicationContext());
        image.setOwner(this);

        Glide.with(this).load(nID).into(image);

        image.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
        image.setScaleType(ImageView.ScaleType.FIT_XY);
        return image;
    }

    public void onTeamClick(View v)
    {
        int nID = v.getId();

        if (nID == m_nSelectedTeam)
            return;

        if (m_nSelectedTeam >= 0)
            setSOPTeamImage(m_nSelectedTeam, false);

        m_nSelectedTeam = nID;
        setSOPTeamImage(m_nSelectedTeam, true);

        setMissionImage();
    }

    private void setSOPTeamImage(int nID, boolean selected)
    {
        ImageView view = (ImageView)findViewById(nID);

        if (nID == R.id.center_boss)
        {
            if (selected)
                Glide.with(this).load(R.drawable.center_boss_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.center_boss_normal).into(view);
        }
        else if (nID == R.id.cs_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.cs_team_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.cs_team_normal).into(view);
        }
        else if (nID == R.id.ms_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.ms_team_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.ms_team_normal).into(view);
        }
        else if (nID == R.id.electric_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.electric_team_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.electric_team_normal).into(view);
        }
        else if (nID == R.id.operating_team)
        {
            if (selected)
                Glide.with(this).load(R.drawable.operating_team_clicked).into(view);
            else
                Glide.with(this).load(R.drawable.operating_team_normal).into(view);
        }
        else
            return;
    }

    public void onNotify(String strTitle, String strBody)
    {
        ActionSOPActivity.getCurrentInstance().setInitTag(strTitle, strBody);
        finish();
    }

    public void leftSwipe()
    {
        showNextImage();
    }
    public void rightSwipe()
    {
        showPrevImage();
    }

    private void showPrevImage()
    {
        if (m_nImageIndex <= 0 || mViewFlipper == null)
            return;

        mViewFlipper.setInAnimation(AnimationUtils.loadAnimation(this, R.anim.right_in));
        mViewFlipper.setOutAnimation(AnimationUtils.loadAnimation(this, R.anim.right_out));

        mViewFlipper.showPrevious();
        m_nImageIndex--;

        //selectPoint(m_nImageIndex, m_nImageIndex + 1);
    }

    private void showNextImage()
    {
        if (m_nImageIndex >= m_nImageCount - 1 || mViewFlipper == null)
            return;

        mViewFlipper.setInAnimation(AnimationUtils.loadAnimation(this, R.anim.left_in));
        mViewFlipper.setOutAnimation(AnimationUtils.loadAnimation(this, R.anim.left_out));

        mViewFlipper.showNext();
        m_nImageIndex++;

        //selectPoint(m_nImageIndex, m_nImageIndex - 1);
    }
}
