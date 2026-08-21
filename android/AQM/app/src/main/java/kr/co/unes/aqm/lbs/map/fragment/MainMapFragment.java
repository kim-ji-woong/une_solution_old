package kr.co.unes.aqm.lbs.map.fragment;

import android.app.FragmentManager;
import android.content.Context;
import android.graphics.Color;
import android.location.Location;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.MapFragment;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.model.BitmapDescriptorFactory;
import com.google.android.gms.maps.model.CircleOptions;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;
import com.google.android.gms.maps.model.MarkerOptions;

import java.util.ArrayList;
import java.util.HashMap;

import kr.co.unes.aqm.lbs.map.MainActivity;
import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.AirSensorData;
import kr.co.unes.aqm.lbs.map.data.AirSensorManager;

/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link MainMapFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class MainMapFragment extends Fragment implements OnMapReadyCallback, GoogleMap.OnMarkerClickListener, GoogleMap.OnInfoWindowClickListener
{
    // TODO: Rename parameter arguments, choose names t0.
    // hat match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    private GoogleMap map;

    private HashMap<String, MarkerOptions> mMakers = new HashMap<String, MarkerOptions>();
    private HashMap<String, CircleOptions> mCircles = new HashMap<String, CircleOptions>();
    private HashMap<Marker, AirSensorData> mMarkerMap  = new HashMap<Marker, AirSensorData>();

    public MainMapFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment MainMapFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static MainMapFragment newInstance(String param1, String param2) {
        MainMapFragment fragment = new MainMapFragment();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (getArguments() != null) {
            mParam1 = getArguments().getString(ARG_PARAM1);
            mParam2 = getArguments().getString(ARG_PARAM2);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_main_map, container, false);

        try
        {
            FragmentManager fm = this.getChildFragmentManager();
            if( fm != null)
            {
                int nID = R.id.map_main;
                MapFragment mapFragment = (MapFragment)fm.findFragmentById(nID);
                if(mapFragment != null  )
                    mapFragment.getMapAsync(this);
            }

        }
        catch(Exception ex)
        {
        }


        return view;
    }

    @Override
    public void onInfoWindowClick(Marker marker)
    {
        AirSensorData data = mMarkerMap.get(marker);
        if( data != null)
        {

            MainActivity activity = (MainActivity)getActivity();
            if( activity != null)
            {
                activity.onMainMapInfoWindowClick(marker, data );
            }

            //onListFragmentInteraction(data);
        }
        marker.hideInfoWindow();
    }

    @Override
    public boolean onMarkerClick(Marker marker)
    {
        MainActivity activity = (MainActivity)getActivity();
        if( activity != null)
        {
            return activity.onMainMapMarkerClick(marker);
        }
        return true;
    }


    @Override
    public void onMapReady(GoogleMap gmap)
    {
        map = gmap;
        //
        loadPoi();

        //startLocationService();

        // 지도 유형 설정. 지형도인 경우에는 GoogleMap.MAP_TYPE_TERRAIN, 위성 지도인 경우에는 GoogleMap.MAP_TYPE_SATELLITE
        map.setMapType(GoogleMap.MAP_TYPE_NORMAL);
        map.getUiSettings().setMapToolbarEnabled(true);
        CreatePOI();

        map.setOnMarkerClickListener(this);
        map.setOnInfoWindowClickListener(this);

        initCurrentLocation();
    }


    private void initCurrentLocation() {
        // 현재 위치를 이용해 LatLon 객체 생성
        //LatLng curPoint = new LatLng(latitude, longitude);
        LatLng curPoint = new LatLng(36.223562, 127.792266);
        map.animateCamera(CameraUpdateFactory.newLatLngZoom(curPoint, 7));
        // 현재 위치 주위에 아이콘을 표시하기 위해 정의한 메소드
        //showAllBankItems(latitude, longitude);
    }

    // TODO: Rename method, update argument and hook method into UI event
    public void onButtonPressed(Uri uri) {
        if (mListener != null) {
            mListener.onFragmentInteraction(uri);
        }
    }

    private void loadPoi()
    {
        ArrayList<AirSensorData> sensorList = AirSensorManager.getInstance().getSensorList();
        for(int i =0 ; i < sensorList.size(); i++)
        {
            AirSensorData data = sensorList.get(i);

            Double latitude = Double.parseDouble(data.getLatitude());
            Double longitude = Double.parseDouble(data.getLongitutde());

            MarkerOptions marker = new MarkerOptions();
            marker.position(new LatLng(latitude+0.001, longitude+0.001));
            marker.title(data.toString());
            marker.snippet("- 상태 : " + data.getStatus());
            marker.flat(true);
            marker.draggable(false);

            int color = data.getStatusColor();
            if( color == Color.RED)
            {
                marker.icon(BitmapDescriptorFactory.fromResource(R.drawable.icon_map_red));
            }
            else if(color == Color.CYAN)
            {
                marker.icon(BitmapDescriptorFactory.fromResource(R.drawable.icon_map_sky));
            }
            else if(color == Color.GREEN)
            {
                marker.icon(BitmapDescriptorFactory.fromResource(R.drawable.icon_map_green));
            }
            else
            {
                marker.icon(BitmapDescriptorFactory.fromResource(R.drawable.icon_map_yellow));
            }

            mMakers.put(data.id, marker);

            CircleOptions circle = new CircleOptions();
            circle.fillColor(Color.argb(128,0,0,192));
            circle.center(new LatLng(latitude+0.001, longitude+0.001));

            float[] d = new float[3];
            Location.distanceBetween(latitude,longitude, latitude+0.001, longitude+0.001, d);
            circle.radius(10.0);
            mCircles.put(data.id, circle);
        }
    }

    private void CreatePOI()
    {
        ArrayList<AirSensorData> sensorList = AirSensorManager.getInstance().getSensorList();
        for(int i = 0 ; i < sensorList.size(); i++)
        {
            AirSensorData data = sensorList.get(i);
            MarkerOptions marker = mMakers.get(data.id);
            CircleOptions circle = mCircles.get(data.id);

            Marker mk = map.addMarker(marker);
            mMarkerMap.put(mk, data);
            map.addCircle(circle);
        }
    }

    @Override
    public void onAttach(Context context) {
        super.onAttach(context);
        if (context instanceof OnFragmentInteractionListener) {
            mListener = (OnFragmentInteractionListener) context;
        } else {
            throw new RuntimeException(context.toString()
                    + " must implement OnFragmentInteractionListener");
        }
    }

    @Override
    public void onDetach() {
        super.onDetach();
        mListener = null;
    }


}
