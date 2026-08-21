package kr.co.unes.aqm.lbs.map.view;

import android.graphics.Color;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.AirSensorData;
import kr.co.unes.aqm.lbs.map.fragment.SensorListFragment;

import java.util.List;
import android.util.SparseBooleanArray;


public class SensorItemRecyclerViewAdapter extends RecyclerView.Adapter<SensorItemRecyclerViewAdapter.ViewHolder> {

    private final List<AirSensorData> mValues;
    private final SensorListFragment.OnListFragmentInteractionListener mListener;

    public SensorItemRecyclerViewAdapter(List<AirSensorData> items, SensorListFragment.OnListFragmentInteractionListener listener) {
        mValues = items;
        mListener = listener;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.fragment_item, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(final ViewHolder holder, int position) {
        holder.mItem = mValues.get(position);
        holder.mIdView.setText(mValues.get(position).id);
        holder.mContentView.setText(mValues.get(position).content);


        holder.mValueView.setText(mValues.get(position).value);

        String status = mValues.get(position).getStatus();
        int color = mValues.get(position).getStatusColor();
        holder.mStatusView.setTextColor(color);
        holder.mStatusView.setText(status);

        holder.mView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (null != mListener) {
                    // Notify the active callbacks interface (the activity, if the
                    // fragment is attached to one) that an item has been selected.
                    mListener.onListFragmentInteraction(holder.mItem);

                    setSelection(holder.mItem);

                }
            }
        });

        if (mValues.get(position).IsSelected()) {
            holder.listItem.setBackgroundColor(Color.parseColor("#d5d5d5"));
        } else {
            holder.listItem.setBackgroundColor(Color.TRANSPARENT);
        }
    }

    @Override
    public int getItemCount() {
        return mValues.size();
    }

    private SparseBooleanArray selectedItems;

    public void add(int position, AirSensorData item) {
        mValues.add(position, item);
        notifyItemInserted(position);
    }
    public void remove(String item) {
        int position = mValues.indexOf(item);
        if( position >= 0)
        {
            mValues.remove(position);
            notifyItemRemoved(position);
        }
    }

    public void setSelection(AirSensorData item)
    {
       clearSelections();

        boolean bSelect = item.IsSelected();
        item.setSelected(true);
    }

    public void clearSelections() {
        for(int i = 0 ; i < mValues.size(); i++)
        {
            AirSensorData item = mValues.get(i);
            item.setSelected(false);
        }
        notifyDataSetChanged();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        public final View mView;
        public final TextView mIdView;
        public final TextView mContentView;
        public final TextView mValueView;
        public final TextView mStatusView;
        public AirSensorData mItem;

        public LinearLayout listItem;
        public ViewHolder(View view) {
            super(view);
            mView = view;
            mIdView = (TextView) view.findViewById(R.id.id);
            mContentView = (TextView) view.findViewById(R.id.content);
            mValueView = (TextView)view.findViewById(R.id.value);
            mStatusView = (TextView)view.findViewById(R.id.status);
            listItem = (LinearLayout) view.findViewById(R.id.linear);
        }

        @Override
        public String toString() {
            return super.toString() + " '" + mContentView.getText() + "'";
        }
    }
}
