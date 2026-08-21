package kr.co.unes.aqm.lbs.map;
import android.app.ActionBar;
import android.app.Activity;

import android.content.Context;
import android.content.pm.PackageManager;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.util.Log;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.InputMethodManager;
import android.widget.ImageButton;
import android.widget.Toast;

import android.app.Fragment;
import android.app.FragmentManager;
import android.content.res.Configuration;
import android.support.v4.app.ActionBarDrawerToggle;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;


import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.model.Marker;

import kr.co.unes.aqm.lbs.map.data.AirSensorData;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.data.NodeManager;
import kr.co.unes.aqm.lbs.map.fragment.AddressSearchResultFragment;
import kr.co.unes.aqm.lbs.map.fragment.BulletinFragment;
import kr.co.unes.aqm.lbs.map.fragment.DetailAirQualityFragment;
import kr.co.unes.aqm.lbs.map.fragment.MainMapFragment;
import kr.co.unes.aqm.lbs.map.fragment.OnFragmentInteractionListener;
import kr.co.unes.aqm.lbs.map.fragment.SearchAddressFragment;
import kr.co.unes.aqm.lbs.map.fragment.SearchMainFragment;
import kr.co.unes.aqm.lbs.map.fragment.SearchNameFragment;
import kr.co.unes.aqm.lbs.map.fragment.SensorDetailFragment;
import kr.co.unes.aqm.lbs.map.fragment.SensorListFragment;


public class MainActivity extends Activity
        implements
         SensorListFragment.OnListFragmentInteractionListener ,OnFragmentInteractionListener, View.OnClickListener ,  AddressSearchResultFragment.OnListFragmentInteractionListener
{

    private DrawerLayout mDrawerLayout;

    private ListView mDrawerList;

    private MainMapFragment mMainFragment;

    private SensorListFragment mItemFragment;
    private SensorDetailFragment mDetailFragment;

    private BulletinFragment mBulletinFragment;
    private SearchAddressFragment mSearchAddressFragment;
    private SearchMainFragment mSearchMainFregment;
    private AddressSearchResultFragment mAddressSearchResultFragment;
    private DetailAirQualityFragment mDetailAirQualityFragment;

    private Fragment[] mFragments;


    private ActionBarDrawerToggle mDrawerToggle;

    private CharSequence mDrawerTitle;
    private CharSequence mTitle;
    private String[] mFragmentTitles;

    private GoogleMap map;

    private SensorManager mSensorManager;
    private boolean mCompassEnabled;



    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        ActionBar action = getActionBar();
        if( action != null) {
            action.setLogo(R.drawable.ic_launcher);
            action.setDisplayHomeAsUpEnabled(true);
            action.setDisplayUseLogoEnabled(true);
            action.show();
        }

        setContentView(R.layout.activity_main);

        mTitle = mDrawerTitle = getTitle();

        mFragmentTitles = getResources().getStringArray(R.array.navi_menu_arrays);

        mDrawerLayout = (DrawerLayout) findViewById(R.id.mainLayout);
        mDrawerList = (ListView) findViewById(R.id.left_drawer);

        // set a custom shadow that overlays the main content when the drawer opens
        //mDrawerLayout.setDrawerShadow(R.drawable.drawer_shadow, GravityCompat.START);


        // set up the drawer's list view with items and click listener
        mDrawerList.setAdapter(new ArrayAdapter<String>(this,
                R.layout.drawer_list_item, mFragmentTitles));


        mDrawerList.setOnItemClickListener(new DrawerItemClickListener());
        int width = getResources().getDisplayMetrics().widthPixels/2;
        DrawerLayout.LayoutParams params = (android.support.v4.widget.DrawerLayout.LayoutParams) mDrawerList.getLayoutParams();
        params.width = width;
        mDrawerList.setLayoutParams(params);


        mFragments = new Fragment[9];

        mMainFragment = (MainMapFragment)getFragmentManager().findFragmentById(R.id.main_map);
        mMainFragment.onAttach(this);
        mFragments[0] = mMainFragment;
        //MapFragment mapFragment = (MapFragment) getFragmentManager()
        //        .findFragmentById(R.id.map);
        //mapFragment.getMapAsync(this);
        //mFragments[0] = mapFragment;


        // 센서 관리자 객체 참조
        mSensorManager = (SensorManager)getSystemService(Context.SENSOR_SERVICE);

        // ActionBarDrawerToggle ties together the the proper interactions
        // between the sliding drawer and the action bar app icon
        mDrawerToggle = new ActionBarDrawerToggle(
                this,                  /* host Activity */
                mDrawerLayout,         /* DrawerLayout object */
                R.drawable.ic_drawer,  /* nav drawer image to replace 'Up' caret */
                R.string.drawer_open,  /* "open drawer" description for accessibility */
                R.string.drawer_close  /* "close drawer" description for accessibility */
        ) {
            public void onDrawerClosed(View view) {
                //getActionBar().setTitle(mTitle);
                invalidateOptionsMenu(); // creates call to onPrepareOptionsMenu()
            }

            public void onDrawerOpened(View drawerView) {
               // getActionBar().setTitle(mDrawerTitle);
                invalidateOptionsMenu(); // creates call to onPrepareOptionsMenu()
            }
        };
        mDrawerLayout.setDrawerListener(mDrawerToggle);



        // Fragment Init
        FragmentManager fm = getFragmentManager();
        mItemFragment = (SensorListFragment)fm.findFragmentById(R.id.main_list);
        mItemFragment.onAttach(this);
        mFragments[1] = mItemFragment;


        mDetailFragment = (SensorDetailFragment)fm.findFragmentById(R.id.sensor_detail);
        mDetailFragment.onAttach(this);
        mFragments[2] = mDetailFragment;

        mBulletinFragment = (BulletinFragment)fm.findFragmentById(R.id.map_local);
        mBulletinFragment.onAttach(this);
        mFragments[3] = mBulletinFragment;

        mSearchAddressFragment = (SearchAddressFragment)fm.findFragmentById(R.id.search_address);
        mSearchAddressFragment.onAttach(this);
        mFragments[4] = mSearchAddressFragment;

        mSearchMainFregment = (SearchMainFragment)fm.findFragmentById(R.id.search_main);
        mSearchMainFregment.onAttach(this);
        mFragments[5] = mSearchMainFregment;

        SearchNameFragment frg = (SearchNameFragment)fm.findFragmentById(R.id.search_name);
        frg.onAttach(this);
        mFragments[6] = frg;

        mAddressSearchResultFragment =  (AddressSearchResultFragment)fm.findFragmentById(R.id.address_search_result);
        mAddressSearchResultFragment.onAttach(this);
        mFragments[7] = mAddressSearchResultFragment;


        mDetailAirQualityFragment = (DetailAirQualityFragment)fm.findFragmentById(R.id.sensor_detial_view);
        mDetailAirQualityFragment.onAttach(this);
        mFragments[8] = mDetailAirQualityFragment;

        ImageButton btnHome =(ImageButton)findViewById(R.id.imageButton);
        btnHome.setOnClickListener(this);

        ImageButton btnAlarm =(ImageButton)findViewById(R.id.imageButton3);
        btnAlarm.setOnClickListener(this);

        ImageButton btnSearch =(ImageButton)findViewById(R.id.imageButton2);
        btnSearch.setOnClickListener(this);


        checkDangerousPermissions();

        if (savedInstanceState == null) {
            selectMenuItem(0);
        }
    }


    public void onListFragmentInteraction(NodeData item)
    {
        String szID = item.id;
        showFragment(R.id.sensor_detial_view);
        mDetailAirQualityFragment.setNode(item);
    }

    private int m_nPrevType = 1;
    public void onListFragmentInteraction(AirSensorData item) {
        // Toast.makeText(this, item.content, Toast.LENGTH_LONG).show();

        m_nPrevType = findPosition();
        Log.d("Detail", "Prev Fragment index : " + m_nPrevType);
        showFragment(R.id.sensor_detail);
        mDetailFragment.setSensor(item);
    }

    public void onFragmentInteraction(Uri uri)
    {
        if( uri != null)
        {
            String szFragName = uri.getFragment();
            if( szFragName != null && szFragName.compareTo("AddressSearchResultFragment" ) == 0 )
            {
                hideAllFragment();
                mAddressSearchResultFragment.setData(uri, NodeManager.getInstance().getNodeList());
                showFragment(7, true);
            }
            else
            {

                Log.d("Detail", "Set Fragment index : " + m_nPrevType);
                hideAllFragment();
                showFragment(m_nPrevType, true);
            }
        }

        if(uri == null)
        {

            Log.d("Detail", "Set Fragment index : " + m_nPrevType);
            hideAllFragment();
            showFragment(m_nPrevType, true);
        }
    }

    private void hideKeyboard(){

        InputMethodManager inputManager = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);

        inputManager.hideSoftInputFromWindow(this.getCurrentFocus().getWindowToken(), InputMethodManager.HIDE_NOT_ALWAYS);

    }

    private final long FINISH_INTERVAL_TIME = 2000;
    private long backPressedTime = 0;

    @Override
    public void onBackPressed() {
        long tempTime = System.currentTimeMillis();
        long intervalTime = tempTime - backPressedTime;

        if (0 <= intervalTime && FINISH_INTERVAL_TIME >= intervalTime)
        {
            super.onBackPressed();
        }
        else
        {
            backPressedTime = tempTime;
            Toast.makeText(getApplicationContext(), "한번 더 누르면 종료됩니다.", Toast.LENGTH_SHORT).show();
        }
    }

    public void onMainMapInfoWindowClick(Marker marker, AirSensorData data)
    {
        if(data != null)
        {
            BulletinFragment fragment = (BulletinFragment)getFragmentManager().findFragmentById(R.id.map_local);
            fragment.moveMap(data.toString());
            selectMenuItem(1);
        }
    }

    public boolean onMainMapMarkerClick(Marker marker)
    {
        if(marker.isInfoWindowShown())
            marker.hideInfoWindow();
        else
            marker.showInfoWindow();

        return true;
    }

    @Override
    public void onClick(View v) {
        switch (v.getId()) {
            case R.id.imageView10:
                selectMenuItem(4); // 학교검색
                break;
            case R.id.imageView11:
                selectMenuItem(3); // 지역검색
                break;


            case R.id.imageButton:
                selectMenuItem(0);
                break;

            case R.id.imageButton2:
                selectMenuItem(2);
                break;
        }


    }

    /**
     * 현재 위치 확인을 위해 정의한 메소드
     */
    private void startLocationService() {
        // 위치 관리자 객체 참조
        LocationManager manager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);

        // 리스너 객체 생성
        GPSListener gpsListener = new GPSListener();
        long minTime = 10000;
        float minDistance = 0;

        try {
            // GPS 기반 위치 요청
            manager.requestLocationUpdates(
                    LocationManager.GPS_PROVIDER,
                    minTime,
                    minDistance,
                    gpsListener);

            // 네트워크 기반 위치 요청
            manager.requestLocationUpdates(
                    LocationManager.NETWORK_PROVIDER,
                    minTime,
                    minDistance,
                    gpsListener);

        } catch(SecurityException e) {
            e.printStackTrace();
        }

        //Toast.makeText(getApplicationContext(), "위치 확인 시작함. 로그를 확인하세요.", Toast.LENGTH_SHORT).show();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.main, menu);
        return super.onCreateOptionsMenu(menu);
    }

    /* Called whenever we call invalidateOptionsMenu() */
    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        // If the nav drawer is open, hide action items related to the content view
        boolean drawerOpen = mDrawerLayout.isDrawerOpen(mDrawerList);
        menu.findItem(R.id.action_websearch).setVisible(!drawerOpen);
        return super.onPrepareOptionsMenu(menu);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // The action bar home/up action should open or close the drawer.
        // ActionBarDrawerToggle will take care of this.
        if (mDrawerToggle.onOptionsItemSelected(item)) {
            return true;
        }
        // Handle action buttons
        switch(item.getItemId()) {
            case R.id.action_websearch:
                selectMenuItem(2);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    /* The click listner for ListView in the navigation drawer */
    private class DrawerItemClickListener implements ListView.OnItemClickListener {
        @Override
        public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
            selectMenuItem(position);
        }
    }

    private void selectSensorItem(int position)
    {

    }

    private void showFragment(Fragment fragment)
    {
        if(fragment == null)
            return;

        FragmentManager fragmentManager = getFragmentManager();
        for(int i = 0 ; i < mFragments.length ; i++)
        {
            Fragment f = mFragments[i];
            if( f != fragment )
            {
                fragmentManager.beginTransaction().hide(f).commit();
            }
            else
            {
                fragmentManager.beginTransaction().show(f).commit();
            }
        }
    }

    private void showFragment(int nFragmentID)
    {
        FragmentManager fragmentManager = getFragmentManager();
        Fragment fragment = fragmentManager.findFragmentById(nFragmentID);
        showFragment(fragment);
    }

    private void hideAllFragment()
    {
        FragmentManager fragmentManager = getFragmentManager();
        for(int i = 0 ; i < mFragments.length ; i++)
        {
            Fragment f = mFragments[i];
            fragmentManager.beginTransaction().hide(f).commit();
        }
    }

    private void showFragment(int index, boolean bVisible)
    {
        FragmentManager fragmentManager = getFragmentManager();
       // for(int i = 0 ; i < mFragments.length ; i++)
        {
            Fragment f = mFragments[index];
            if( bVisible == false )
            {
                fragmentManager.beginTransaction().hide(f).commit();
            }
            else
            {
                fragmentManager.beginTransaction().show(f).commit();
            }
        }
    }

    private int findPosition()
    {
        int position = -1;

        FragmentManager fragmentManager = getFragmentManager();
        for(int i = 0 ; i < mFragments.length ; i++)
        {
            Fragment f = mFragments[i];
            if( f.isVisible() == true)
            {
                position = i;
                break;
            }
        }
        return position;
    }

    private void selectMenuItem(int position) {

        if( position < 0 || mFragmentTitles.length <= position)
            return;

        String szFragName = mFragmentTitles[position];

        //<item>전국</item>
        //<item>지역별</item>
        //<item>검색</item>
        //<item>지역검색</item>
       // <item>학교검색</item>
       // <item>검색결과</item>
       // <item>세부정보</item>

        if( position == 0 )
        {
            showFragment(R.id.main_map);
        }
        else if( position == 1) {
            showFragment(R.id.map_local);
        }
        else if( position == 2)
        {
            showFragment(R.id.search_main);
        }
        else if( position == 3)
        {
            showFragment(R.id.search_address);
        }
        else if(position == 4)
        {
            showFragment(R.id.search_name);
        }
        else if(position == 5)
        {
            showFragment(R.id.address_search_result);
        }
        else if(position == 6)
        {
            showFragment(R.id.sensor_detial_view);
        }

        // update selected item and title, then close the drawer
        mDrawerList.setItemChecked(position, true);
        setTitle(szFragName);
        mDrawerLayout.closeDrawer(mDrawerList);
    }

    @Override
    public void setTitle(CharSequence title) {
        mTitle = title;
        //getActionBar().setTitle(mTitle);
    }

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        mDrawerToggle.syncState();
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        // Pass any configuration change to the drawer toggls
        mDrawerToggle.onConfigurationChanged(newConfig);
    }

     private void checkDangerousPermissions() {
        String[] permissions = {
                android.Manifest.permission.INTERNET,
                android.Manifest.permission.READ_EXTERNAL_STORAGE,
                android.Manifest.permission.WRITE_EXTERNAL_STORAGE,
                android.Manifest.permission.ACCESS_NETWORK_STATE,
                android.Manifest.permission.ACCESS_COARSE_LOCATION,
                android.Manifest.permission.ACCESS_FINE_LOCATION
        };

        int permissionCheck = PackageManager.PERMISSION_GRANTED;
        for (int i = 0; i < permissions.length; i++) {
            permissionCheck = ContextCompat.checkSelfPermission(this, permissions[i]);
            if (permissionCheck == PackageManager.PERMISSION_DENIED) {
                break;
            }
        }

        if (permissionCheck == PackageManager.PERMISSION_GRANTED) {
            Toast.makeText(this, "권한 있음", Toast.LENGTH_LONG).show();
        } else {
            Toast.makeText(this, "권한 없음", Toast.LENGTH_LONG).show();

            if (ActivityCompat.shouldShowRequestPermissionRationale(this, permissions[0])) {
                Toast.makeText(this, "권한 설명 필요함.", Toast.LENGTH_LONG).show();
            } else {
                ActivityCompat.requestPermissions(this, permissions, 1);
            }
        }
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        if (requestCode == 1) {
            for (int i = 0; i < permissions.length; i++) {
                if (grantResults[i] == PackageManager.PERMISSION_GRANTED) {
                    Toast.makeText(this, permissions[i] + " 권한이 승인됨.", Toast.LENGTH_LONG).show();
                } else {
                    Toast.makeText(this, permissions[i] + " 권한이 승인되지 않음.", Toast.LENGTH_LONG).show();
                }
            }
        }
    }


    @Override
    public void onResume() {
        super.onResume();

        try {
            // 내 위치 자동 표시 enable

            //if( map != null)
            //    map.setMyLocationEnabled(true);
        } catch(SecurityException e) {
            e.printStackTrace();
        }

        if(mCompassEnabled) {
            mSensorManager.registerListener(mListener, mSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION), SensorManager.SENSOR_DELAY_UI);
        }
    }

    @Override
    public void onPause() {
        super.onPause();

        try {
            // 내 위치 자동 표시 disable
            //if( map != null)
            //    map.setMyLocationEnabled(false);
        } catch(SecurityException e) {
            e.printStackTrace();
        }

        if(mCompassEnabled) {
            mSensorManager.unregisterListener(mListener);
        }
    }

    /**
     * 센서의 정보를 받기 위한 리스너 객체 생성
     */
    private final SensorEventListener mListener = new SensorEventListener() {
        private int iOrientation = -1;

        public void onAccuracyChanged(Sensor sensor, int accuracy) {

        }

        // 센서의 값을 받을 수 있도록 호출되는 메소드
        public void onSensorChanged(SensorEvent event) {
            if (iOrientation < 0) {
                iOrientation = ((WindowManager) getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay().getRotation();
            }

            //mCompassView.setAzimuth(event.values[0] + 90 * iOrientation);
            //mCompassView.invalidate();
        }
    };

    private class GPSListener implements LocationListener {
        private boolean m_First = true;
        /**
         * 위치 정보가 확인되었을 때 호출되는 메소드
         */
        public void onLocationChanged(Location location) {
            Double latitude = location.getLatitude();
            Double longitude = location.getLongitude();

            String msg = "Latitude : "+ latitude + "\nLongitude:"+ longitude;
            Log.i("GPSLocationService", msg);

            // 현재 위치의 지도를 보여주기 위해 정의한 메소드 호출
            if(m_First == true)
            {
                m_First = false;

            }
        }

        public void onProviderDisabled(String provider) {
        }

        public void onProviderEnabled(String provider) {
        }

        public void onStatusChanged(String provider, int status, Bundle extras) {
        }
    }
}