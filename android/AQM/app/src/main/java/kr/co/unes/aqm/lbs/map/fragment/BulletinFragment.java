package kr.co.unes.aqm.lbs.map.fragment;

import android.app.FragmentManager;
import android.content.Context;
import android.location.Address;
import android.location.Geocoder;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.MapFragment;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;

import java.util.List;
import java.util.Locale;

import kr.co.unes.aqm.lbs.map.R;


public class BulletinFragment extends Fragment implements OnMapReadyCallback, GoogleMap.OnMarkerClickListener, GoogleMap.OnInfoWindowClickListener
{

    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    private GoogleMap map;

    public BulletinFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment BulletinFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static BulletinFragment newInstance(String param1, String param2) {
        BulletinFragment fragment = new BulletinFragment();
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

        View view = inflater.inflate(R.layout.fragment_bulletin, container, false);
        try
        {
            FragmentManager fm = this.getChildFragmentManager();
            if( fm != null)
            {
                int nID = R.id.map_search;
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

    // TODO: Rename method, update argument and hook method into UI event
    public void onButtonPressed(Uri uri) {
        if (mListener != null) {
            mListener.onFragmentInteraction(uri);
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

    public void onInfoWindowClick(Marker marker)
    {
        marker.hideInfoWindow();
    }

    public boolean onMarkerClick(Marker marker)
    {
        if(marker.isInfoWindowShown())
            marker.hideInfoWindow();
        else
            marker.showInfoWindow();

        return true;
    }

    @Override
    public void onMapReady(GoogleMap gmap)
    {
        map = gmap;

        // 지도 유형 설정. 지형도인 경우에는 GoogleMap.MAP_TYPE_TERRAIN, 위성 지도인 경우에는 GoogleMap.MAP_TYPE_SATELLITE
        map.setMapType(GoogleMap.MAP_TYPE_NORMAL);

        initCurrentLocation();

        moveMap("경기도");

        map.setOnMarkerClickListener(this);
        map.setOnInfoWindowClickListener(this);
    }

    private void initCurrentLocation() {
        // 현재 위치를 이용해 LatLon 객체 생성
        //LatLng curPoint = new LatLng(latitude, longitude);
        LatLng curPoint = new LatLng(36.223562, 127.792266);
        map.moveCamera(CameraUpdateFactory.newLatLngZoom(curPoint, 7));
        // 현재 위치 주위에 아이콘을 표시하기 위해 정의한 메소드
        //showAllBankItems(latitude, longitude);
    }

    private void setFragmentTitle(String szName)
    {
        TextView view = (TextView)getView().findViewById(R.id.titleView);
        if( view != null)
        {
            view.setText(szName);
        }
    }


    public void moveMap(String szSearchName)
    {
        String szTitile = szSearchName + "의 실내공기질 현황";
        setFragmentTitle(szTitile);

        Geocoder geoCoder = new Geocoder(this.getActivity(), Locale.getDefault());
        try
        {

            List<Address> addresses = geoCoder.getFromLocationName(szSearchName, 5);
            if (addresses.size() > 0)
            {
                Double lat = (double) (addresses.get(0).getLatitude());
                Double lon = (double) (addresses.get(0).getLongitude());
                LatLng user = new LatLng(lat, lon);
                map.animateCamera(CameraUpdateFactory.newLatLngZoom(user, 8));
            }
        }
        catch (Exception exn)
        {
        }
    }
}
