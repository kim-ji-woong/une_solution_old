package kr.co.une.energyindustrye_sop;

import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.RectF;
import android.graphics.drawable.Drawable;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GestureDetectorCompat;
import android.support.v4.view.MotionEventCompat;
import android.support.v4.view.ViewCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AnimationUtils;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TableLayout;
import android.widget.ViewFlipper;

import com.bumptech.glide.Glide;
import com.bumptech.glide.RequestBuilder;
import com.bumptech.glide.request.target.BitmapImageViewTarget;

import java.util.ArrayList;
import java.util.List;

import kr.co.une.energyindustrye_sop.photoview.PhotoView;
import kr.co.une.energyindustrye_sop.photoview.PhotoViewAttacher;
import kr.co.une.energyindustrye_sop.photoview.PhotoViewOwner;

public class ActionRuleDetailActivity extends AppCompatActivity implements  FCMReceiver, PhotoViewOwner/*GestureDetector.OnGestureListener*/{
    private String m_strSOPName = "";
    //private ImageButton m_currentMenu = null;
    private View m_currentMenu = null;
    //private GestureDetectorCompat mDetector = null;

    private static final int SWIPE_MIN_DISTANCE = 120;
    private static final int SWIPE_MAX_OFF_PATH = 250;
    private static final int SWIPE_THRESHOLD_VELOCITY = 200;

    private int m_nImageIndex = -1;
    private int m_nImageCount = 0;
    private ViewFlipper mViewFlipper = null;

    private List<ImageView> m_pointList = new ArrayList();
    private ImageView m_point1 = null;
    private ImageView m_point2 = null;
    private ImageView m_point3 = null;
    private ImageView m_point4 = null;
    private ImageView m_point5 = null;
    private ImageView m_point6 = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        MainActivity.getInstance().setCurrentInstance(this);

        super.onCreate(savedInstanceState);

        // 타이틀바 감추기
        getSupportActionBar().hide();
        setContentView(R.layout.activity_action_rule_detail);

        // Instantiate the gesture detector with the
        // application context and an implementation of
        // GestureDetector.OnGestureListener
        //mDetector = new GestureDetectorCompat(this,this);

        Intent intent = getIntent();
        m_strSOPName = intent.getStringExtra("SOP");

        initLayout(R.id.layoutSystem);
        //initLayout(R.id.layoutProcedure);

        initPointLayout();

        mViewFlipper = (ViewFlipper)findViewById(R.id.flipperSystem);

        btnMenuClick(findViewById(R.id.btnSystem));
    }

    private void initLayout(int nLayoutID)
    {
        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        View imgTitle = findViewById(R.id.imgTitle);
        LinearLayout menuLayout = (LinearLayout)findViewById(R.id.actionRuleDetailMenu);

        int nTitleHeight = imgTitle.getLayoutParams().height;
        int nMenuHeight = menuLayout.getLayoutParams().height;

        menuLayout.setY(nTitleHeight);

        LinearLayout infoLayout = (LinearLayout) findViewById(nLayoutID);
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

        infoLayout.setPadding(left, top, right, bottom);

        //sopLayout.setX(0.0f);
        //sopLayout.setY(nTitleHeight);
    }

    @Override
    public void onBackPressed()
    {
        // 선택된 SOP 버튼을 초기화한다.
        ActionRuleActivity parent = ActionRuleActivity.getCurrentInstance();
        parent.initSOPButton();

        super.onBackPressed();
    }

    public void btnMenuClick(View v)
    {
        if (v == m_currentMenu)
            return;

        initMenuButton();
        m_currentMenu = v;

        int nID = m_currentMenu.getId();

        m_currentMenu.setBackgroundColor(ContextCompat.getColor(this, R.color.colorMenuSelected));
        setFlipperImage(nID);

        /*if (nID == R.id.btnSystem)
        {
            //m_currentMenu.setImageResource(R.drawable.system_clicked);
            //mViewFlipper = (ViewFlipper)findViewById(R.id.flipperSystem);
            //showLayout(R.id.layoutSystem);
            setFlipperImage(nID);
        }
        else if (nID == R.id.btnProcedure)
        {
            //m_currentMenu.setImageResource(R.drawable.procedure_clicked);
            //mViewFlipper = null;
            //showLayout(R.id.layoutProcedure);
            setFlipperImage(nID);
        }
        else if (nID == R.id.btnTogether)
        {
            //m_currentMenu.setImageResource(R.drawable.together_clicked);
        }
        else if (nID == R.id.btnSimilar)
        {
            //.setImageResource(R.drawable.similar_clicked);
        }
        else if (nID == R.id.btnEmergency)
        {
            //m_currentMenu.setImageResource(R.drawable.emergency_clicked);
        }*/
    }

    private void setFlipperImage(int nButtonID)
    {
        clearFlipperImages();
        //mViewFlipper.removeAllViews();
        initPointList();

        if (nButtonID == R.id.btnSystem)
        {
            mViewFlipper.addView(getImageView(R.drawable.system_image1));
            mViewFlipper.addView(getImageView(R.drawable.system_image2));

            addPoint(m_point3);
            addPoint(m_point4);
        }
        else if (nButtonID == R.id.btnProcedure)
        {
            mViewFlipper.addView(getImageView(R.drawable.procedure_image));
        }
        else if (nButtonID == R.id.btnTogether)
        {
            mViewFlipper.addView(getImageView(R.drawable.together_image01));
            mViewFlipper.addView(getImageView(R.drawable.together_image02));
            mViewFlipper.addView(getImageView(R.drawable.together_image03));
            mViewFlipper.addView(getImageView(R.drawable.together_image04));
            mViewFlipper.addView(getImageView(R.drawable.together_image05));
            mViewFlipper.addView(getImageView(R.drawable.together_image06));

            addPoint(m_point1);
            addPoint(m_point2);
            addPoint(m_point3);
            addPoint(m_point4);
            addPoint(m_point5);
            addPoint(m_point6);
        }
        else if (nButtonID == R.id.btnSimilar)
        {
            mViewFlipper.addView(getImageView(R.drawable.similar_image));
        }
        else if (nButtonID == R.id.btnEmergency)
        {
            mViewFlipper.addView(getImageView(R.drawable.emergency_image));
        }
        else
            return;

        m_nImageCount = mViewFlipper.getChildCount();
        m_nImageIndex = 0;

        if (m_nImageCount > 1)
            selectPoint(0, -1);
    }

    private void clearFlipperImages()
    {
        /*int nImageCount = mViewFlipper.getChildCount();

        for (int i=0;i<nImageCount;i++)
        {
            ImageView view = (ImageView)mViewFlipper.getChildAt(i);
            view.setImageBitmap(null);
        }*/

        /*for (Bitmap bmp : mBitmaps) {
            bmp.recycle();
            bmp = null;
        }

        mBitmaps.clear();*/
        mViewFlipper.removeAllViews();
    }
    //private List<Bitmap> mBitmaps = new ArrayList();
    private ImageView getImageView(int nID)
    {
        PhotoView image = new PhotoView(getApplicationContext());
        image.setOwner(this);
        //ImageView image = new ImageView(getApplicationContext());

        Glide.with(this).load(nID).into(image);
        /*Bitmap bmp = BitmapFactory.decodeResource(this.getResources(), nID);
        //mBitmaps.add(bmp);
        image.setImageBitmap(bmp);*/
        //image.setImageResource(nID);
        image.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
        image.setScaleType(ImageView.ScaleType.FIT_XY);
        return image;
    }

    private void showLayout(int nID)
    {
        if (nID != R.id.layoutSystem)
            findViewById(R.id.layoutSystem).setVisibility(View.INVISIBLE);

        //if (nID != R.id.layoutProcedure)
        //    findViewById(R.id.layoutProcedure).setVisibility(View.INVISIBLE);

        findViewById(nID).setVisibility(View.VISIBLE);
    }

    private void initMenuButton()
    {
        if (m_currentMenu == null)
            return;

        int nID = m_currentMenu.getId();

        m_currentMenu.setBackgroundColor(ContextCompat.getColor(this, R.color.colorTitleBack));
        /*if (nID == R.id.btnSystem)
        {
            m_currentMenu.setImageResource(R.drawable.system_normal);
        }
        else if (nID == R.id.btnProcedure)
        {
            m_currentMenu.setImageResource(R.drawable.procedure_normal);
        }
        else if (nID == R.id.btnTogether)
        {
            m_currentMenu.setImageResource(R.drawable.together_normal);
        }
        else if (nID == R.id.btnSimilar)
        {
            m_currentMenu.setImageResource(R.drawable.similar_normal);
        }
        else if (nID == R.id.btnEmergency)
        {
            m_currentMenu.setImageResource(R.drawable.emergency_normal);
        }*/

        m_currentMenu = null;
    }

    /*@Override
    public boolean onTouchEvent(MotionEvent event){
        this.mDetector.onTouchEvent(event);
        // Be sure to call the superclass implementation
        return super.onTouchEvent(event);
    }

    @Override
    public boolean onDown(MotionEvent event) {
        //Log.d(DEBUG_TAG,"onDown: " + event.toString());
        return true;
    }

    @Override
    public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
        try {
            if (Math.abs(e1.getY() - e2.getY()) > SWIPE_MAX_OFF_PATH)
                return false;

            // right to left swipe
            if (e1.getX() - e2.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
                showNextImage();
                //Toast.makeText(getApplicationContext(), "Left Swipe", Toast.LENGTH_SHORT).show();
            }
            // left to right swipe
            else if (e2.getX() - e1.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
                showPrevImage();
                //Toast.makeText(getApplicationContext(), "Right Swipe", Toast.LENGTH_SHORT).show();
            }
            // down to up swipe
            else if (e1.getY() - e2.getY() > SWIPE_MIN_DISTANCE && Math.abs(velocityY) > SWIPE_THRESHOLD_VELOCITY) {
                //Toast.makeText(getApplicationContext(), "Swipe up", Toast.LENGTH_SHORT).show();
            }
            // up to down swipe
            else if (e2.getY() - e1.getY() > SWIPE_MIN_DISTANCE && Math.abs(velocityY) > SWIPE_THRESHOLD_VELOCITY) {
                //Toast.makeText(getApplicationContext(), "Swipe down", Toast.LENGTH_SHORT).show();
            }
        } catch (Exception e) {

        }
        return true;

    }*/

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

        selectPoint(m_nImageIndex, m_nImageIndex + 1);
    }

    private void showNextImage()
    {
        if (m_nImageIndex >= m_nImageCount - 1 || mViewFlipper == null)
            return;

        mViewFlipper.setInAnimation(AnimationUtils.loadAnimation(this, R.anim.left_in));
        mViewFlipper.setOutAnimation(AnimationUtils.loadAnimation(this, R.anim.left_out));

        mViewFlipper.showNext();
        m_nImageIndex++;

        selectPoint(m_nImageIndex, m_nImageIndex - 1);
    }

    /*@Override
    public void onLongPress(MotionEvent event) {
        //Log.d(DEBUG_TAG, "onLongPress: " + event.toString());
    }

    @Override
    public boolean onScroll(MotionEvent e1, MotionEvent e2, float distanceX,
                            float distanceY) {
        return true;
    }

    @Override
    public void onShowPress(MotionEvent event) {
        //Log.d(DEBUG_TAG, "onShowPress: " + event.toString());
    }

    @Override
    public boolean onSingleTapUp(MotionEvent event) {
        //Log.d(DEBUG_TAG, "onSingleTapUp: " + event.toString());
        return true;
    }*/

    @Override
    protected void onDestroy() {
        clearFlipperImages();
        super.onDestroy();
    }

    private void initPointLayout() {
        View layout = findViewById(R.id.pointLayout);

        float fScreenWidth = MainActivity.getScreenWidth(this);
        float fScreenHeight = MainActivity.getScreenHeight(this);

        // 표준모델
        int stLeftPadding = 512, stRightPadding = 512;
        int stScreenWidth = MainActivity.getStandardScreenWidth(), stScreenHeight = MainActivity.getStandardScreenHeight();
        float stHorSpacingRatio = 0.57143f;

        float stImageWidth = (stScreenWidth - stLeftPadding - stRightPadding) / (6 + stHorSpacingRatio * 5);
        int imgWidth = (int)(stImageWidth * fScreenWidth / stScreenWidth);
        int imgHeight = imgWidth;
        int horSpacing = (int)(imgWidth * stHorSpacingRatio);

        int nLayoutHeight = layout.getLayoutParams().height;

        int left = (int)(stLeftPadding * fScreenWidth / stScreenWidth);
        int right = (int)(stRightPadding * fScreenWidth / stScreenWidth);;
        int top = 0, bottom = 0;

        layout.setPadding(left - 50, top, right - 50, bottom);
        layout.setX(0.0f);
        layout.setY(fScreenHeight - nLayoutHeight);

        m_point1 = setImage(R.id.actionRuleDetailPoint1, R.drawable.circle_no_selected);
        m_point2 = setImage(R.id.actionRuleDetailPoint2, R.drawable.circle_no_selected);
        m_point3 = setImage(R.id.actionRuleDetailPoint3, R.drawable.circle_no_selected);
        m_point4 = setImage(R.id.actionRuleDetailPoint4, R.drawable.circle_no_selected);
        m_point5 = setImage(R.id.actionRuleDetailPoint5, R.drawable.circle_no_selected);
        m_point6 = setImage(R.id.actionRuleDetailPoint6, R.drawable.circle_no_selected);

        float x = 0;

        setImageSize(R.id.actionRuleDetailPoint1, imgWidth, imgHeight, x);
        setImageSize(R.id.actionRuleDetailPoint2, imgWidth, imgHeight, x + horSpacing);
        setImageSize(R.id.actionRuleDetailPoint3, imgWidth, imgHeight, x + horSpacing * 2);
        setImageSize(R.id.actionRuleDetailPoint4, imgWidth, imgHeight, x + horSpacing * 3);
        setImageSize(R.id.actionRuleDetailPoint5, imgWidth, imgHeight, x + horSpacing * 4);
        setImageSize(R.id.actionRuleDetailPoint6, imgWidth, imgHeight, x + horSpacing * 5);

        /*View imgTitle = findViewById(R.id.imgTitle);
        LinearLayout menuLayout = (LinearLayout)findViewById(R.id.actionRuleDetailMenu);

        int nTitleHeight = imgTitle.getLayoutParams().height;
        int nMenuHeight = menuLayout.getLayoutParams().height;

        menuLayout.setY(nTitleHeight);

        LinearLayout infoLayout = (LinearLayout) findViewById(nLayoutID);
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

    private ImageView setImage(int nViewID, int nImageID)
    {
        ImageView view = (ImageView)findViewById(nViewID);
        Glide.with(this).load(nImageID).into(view);
        return view;
    }

    private void setImageSize(int nImageID, int width, int height, float x)
    {
        View view = findViewById(nImageID);

        ViewGroup.LayoutParams param = view.getLayoutParams();
        param.width = width;
        param.height = height;

        view.setX(x);
    }

    private void initPointList()
    {
        for (ImageView point : m_pointList)
        {
            Glide.with(this).load(R.drawable.circle_no_selected).into(point);
        }

        m_point1.setVisibility(View.INVISIBLE);
        m_point2.setVisibility(View.INVISIBLE);
        m_point3.setVisibility(View.INVISIBLE);
        m_point4.setVisibility(View.INVISIBLE);
        m_point5.setVisibility(View.INVISIBLE);
        m_point6.setVisibility(View.INVISIBLE);

        m_pointList.clear();
    }

    private void addPoint(ImageView point)
    {
        point.setVisibility(View.VISIBLE);
        m_pointList.add(point);
    }

    private void selectPoint(int nSelectedIndex, int nPrevIndex)
    {
        if (nSelectedIndex < m_nImageCount && nSelectedIndex >= 0)
            Glide.with(this).load(R.drawable.circle_selected).into(m_pointList.get(nSelectedIndex));

        if (nPrevIndex < m_nImageCount && nPrevIndex >= 0)
            Glide.with(this).load(R.drawable.circle_no_selected).into(m_pointList.get(nPrevIndex));
    }

    public void onNotify(String strTitle, String strBody)
    {
        ActionRuleActivity.getCurrentInstance().setInitTag(strTitle, strBody);
        finish();
    }
}
