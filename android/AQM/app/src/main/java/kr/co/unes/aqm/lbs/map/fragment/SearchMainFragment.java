package kr.co.unes.aqm.lbs.map.fragment;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.ScaleDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;


import kr.co.unes.aqm.lbs.map.MainActivity;
import kr.co.unes.aqm.lbs.map.R;


/**
 * A simple {@link Fragment} subclass.
 * Activities that contain this fragment must implement the
 * {@link OnFragmentInteractionListener} interface
 * to handle interaction events.
 * Use the {@link SearchMainFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class SearchMainFragment extends Fragment {
    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    private OnFragmentInteractionListener mListener;

    public SearchMainFragment() {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment SearchMainFragment.
     */
    // TODO: Rename and change types and number of parameters
    public static SearchMainFragment newInstance(String param1, String param2) {
        SearchMainFragment fragment = new SearchMainFragment();
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
        View view = inflater.inflate(R.layout.fragment_search_main, container, false);

        MainActivity activity = (MainActivity)getActivity();
        ImageView btn1 = (ImageView)view.findViewById(R.id.imageView10);
        btn1.setOnClickListener(activity);

        //Drawable drawable = getResources().getDrawable(R.drawable.btn_local_s);
        //drawable.setBounds(0, 0, (int)(drawable.getIntrinsicWidth()*0.5),
        //        (int)(drawable.getIntrinsicHeight()*0.5));
        //ScaleDrawable sd = new ScaleDrawable(drawable, 0, 100, 100);
        //btn1.setCompoundDrawables(sd.getDrawable(), null, null, null);

        ImageView btn2 = (ImageView)view.findViewById(R.id.imageView11);
        btn2.setOnClickListener(activity);

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
}
