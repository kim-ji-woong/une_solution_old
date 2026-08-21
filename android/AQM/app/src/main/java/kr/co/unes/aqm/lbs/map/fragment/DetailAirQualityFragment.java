package kr.co.unes.aqm.lbs.map.fragment;

import android.content.Context;
import android.graphics.drawable.StateListDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;

import java.util.List;

import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.AirSensorManager;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.data.NodeManager;

/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link DetailAirQualityFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class DetailAirQualityFragment extends Fragment implements AdapterView.OnItemSelectedListener {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    public DetailAirQualityFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment DetailAirQualityFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static DetailAirQualityFragment newInstance(String param1, String param2) {
        DetailAirQualityFragment fragment = new DetailAirQualityFragment();
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

    private View contentView;
    private Spinner spinNodes;
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        contentView = inflater.inflate(R.layout.fragment_detail_air_quality, container, false);


        spinNodes = (Spinner)contentView.findViewById(R.id.spn_location);
        spinNodes.setOnItemSelectedListener(this);


        return contentView;
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

    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id)
    {

    }

    public void onNothingSelected(AdapterView<?> parent)
    {

    }

    // Process SensorCode
    // 36864 폼알데히드
    // 37120 라돈
    // 21248 이산화탄소
    String [] codes = { "36864","37120", "21248" };

    public void setNode(NodeData item)
    {
        String siteID = item.id;
        List<String> nodeList = NodeManager.getInstance().ReadSiteNode(siteID);

        if( nodeList.size() > 0)
        {
            String nodeID = nodeList.get(0);
            float value1 = AirSensorManager.getInstance().ReedSensorValue(nodeID, codes[0]);
            float value2 = AirSensorManager.getInstance().ReedSensorValue(nodeID, codes[1]);
            float value3 = AirSensorManager.getInstance().ReedSensorValue(nodeID, codes[2]);

            Log.d("포름알데히드", ""+value1);
            Log.d("라돈", ""+value2);
            Log.d("이산화탄소", ""+value3);

            int nStep = 1;

            int nDrawble = R.drawable.circle_orange;
            if( value1 < 90)
            {
                nDrawble = R.drawable.circle_blue;
                nStep = 1;
            }
            else if( value1 < 100 )
            {
                nDrawble = R.drawable.circle_green;
                nStep = 2;
            }
            else if( value1 < 110)
            {
                nDrawble = R.drawable.circle_orange;
                nStep = 3;
            }
            else if( value1 < 115)
            {
                nDrawble = R.drawable.circle_red;
                nStep = 4;
            }

            TextView tv1 =  (TextView)contentView.findViewById(R.id.sensor_value_2);
            tv1.setText(""+value1);
            ProgressBar pb1 = (ProgressBar)contentView.findViewById(R.id.sensor_value_graph_2);
            pb1.setMax(115);
            pb1.setProgress((int)value1);
            ImageView iv1 = (ImageView)contentView.findViewById(R.id.image_state_2);
            iv1.setImageResource(nDrawble);

            int nDrawble2 = R.drawable.circle_orange;
            if( value2 < 3.6)
            {
                nDrawble2 = R.drawable.circle_blue;

            }
            else if( value2 < 4.0)
            {
                nDrawble2 = R.drawable.circle_green;
                if(nStep <= 2)
                    nStep = 2;
            }
            else if( value2 < 4.4)
            {
                nDrawble2 = R.drawable.circle_orange;
                if(nStep <= 3)
                    nStep = 3;
            }
            else if( value2 < 4.6)
            {
                nDrawble2 = R.drawable.circle_red;
                if(nStep <= 4)
                    nStep = 4;
            }

            TextView tv2 =  (TextView)contentView.findViewById(R.id.sensor_value_1);
            tv2.setText(""+value2);
            ProgressBar pb2 = (ProgressBar)contentView.findViewById(R.id.sensor_value_graph_1);
            pb2.setMax(5);
            pb2.setProgress((int)value2);
            ImageView iv2 = (ImageView)contentView.findViewById(R.id.image_state_1);
            iv2.setImageResource(nDrawble2);


            TextView tv3 =  (TextView)contentView.findViewById(R.id.sensor_value_3);
            tv3.setText(""+value3);
            ProgressBar pb3 = (ProgressBar)contentView.findViewById(R.id.sensor_value_graph_3);
            pb3.setMax(1150);
            pb3.setProgress((int)value3);
            ImageView iv3 = (ImageView)contentView.findViewById(R.id.image_state_3);

            int nDrawble3 = R.drawable.circle_orange;
            if( value3 < 900)
            {
                nDrawble3 = R.drawable.circle_blue;
            }
            else if( value3 < 1000)
            {
                nDrawble3 = R.drawable.circle_green;
                if(nStep <= 2)
                    nStep = 2;
            }
            else if( value3 < 1100)
            {
                nDrawble3 = R.drawable.circle_orange;
                if(nStep <= 3)
                    nStep = 3;
            }
            else if( value3 < 1150)
            {
                nDrawble3 = R.drawable.circle_red;
                if(nStep <= 4)
                    nStep = 4;
            }
            iv3.setImageResource(nDrawble3);


            Log.d("SelectedStep", ""+ nStep);

            int tDrawable = R.drawable.circle_blue;
            String szStateText = "좋음";
            int pbValue = 100;
            if( nStep == 2)
            {
                nDrawble = R.drawable.circle_green;
                szStateText = "보통";
                pbValue = 75;
            }
            else if( nStep == 3)
            {
                nDrawble = R.drawable.circle_orange;
                szStateText = "주의";
                pbValue = 50;
            }
            else if( nStep == 4)
            {
                nDrawble = R.drawable.circle_red;
                szStateText = "나쁨";
                pbValue = 25;
            }

            TextView tv4 =  (TextView)contentView.findViewById(R.id.sensor_value_main);
            tv4.setText(szStateText);
            ProgressBar pb4 = (ProgressBar)contentView.findViewById(R.id.sensor_value_graph_main);
            pb4.setMax(100);
            pb4.setProgress(pbValue);
            ImageView iv4 = (ImageView)contentView.findViewById(R.id.image_state_main);
            iv4.setImageResource(nDrawble);
        }


    }

}
