package kr.co.unes.aqm.lbs.map.view;

import android.graphics.Color;
import android.support.v7.widget.RecyclerView;
import android.util.SparseBooleanArray;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout;
import android.widget.TextView;

import java.util.List;

import kr.co.unes.aqm.lbs.map.R;
import kr.co.unes.aqm.lbs.map.data.NodeData;
import kr.co.unes.aqm.lbs.map.fragment.AddressSearchResultFragment;

/**
 * Created by skkim on 2016-12-26.
 */

public class NodeItemRecyclerViewAdapter extends RecyclerView.Adapter<NodeItemRecyclerViewAdapter.ViewHolder> {

    private final List<NodeData> mValues;

    private final AddressSearchResultFragment.OnListFragmentInteractionListener mListener;

    private static int TYPE_HEADER = 0;


    public NodeItemRecyclerViewAdapter(List<NodeData> items, AddressSearchResultFragment.OnListFragmentInteractionListener listener) {
        mValues = items;
        mListener = listener;
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {

        int layout = 0;
        layout = R.layout.search_result_list_item;
        View view = LayoutInflater.from(parent.getContext()).inflate(layout, parent, false);
        return new ViewHolder(view, viewType);
    }

    @Override
    public void onBindViewHolder(final ViewHolder holder, int position) {


        holder.mItem = mValues.get(position);
        holder.mContentView.setText(mValues.get(position).content);
        holder.mValueView.setText(mValues.get(position).getAddress());

        holder.mView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (null != mListener) {

                    setSelection(holder.mItem);
                    mListener.onListFragmentInteraction(holder.mItem);
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
    public int getItemViewType(int position) {

            return position;

    }



    @Override
    public int getItemCount() {
        return (mValues.size() );
    }

    private SparseBooleanArray selectedItems;

    public void add(int position, NodeData item) {
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
    public void removeAllItems()
    {
        int size = this.mValues.size();
        if (size > 0) {
            for (int i = 0; i < size; i++) {
                this.mValues.remove(0);
            }

        }
        this.notifyDataSetChanged();
    }

    public void setSelection(NodeData item)
    {
        clearSelections();

        boolean bSelect = item.IsSelected();
        item.setSelected(true);
    }

    public void clearSelections() {
        for(int i = 0 ; i < mValues.size(); i++)
        {
            NodeData item = mValues.get(i);
            item.setSelected(false);
        }
        notifyDataSetChanged();
    }

    public class ViewHolder extends RecyclerView.ViewHolder {
        public final View mView;
        public final TextView mContentView;
        public final TextView mValueView;
        public NodeData mItem;

        public LinearLayout listItem;
        public ViewHolder(View view, int viewType) {
            super(view);

            if( viewType == TYPE_HEADER)
            {
                mView = view;

                mContentView = (TextView) view.findViewById(R.id.name);
                mValueView = (TextView)view.findViewById(R.id.value);
                listItem = (LinearLayout) view.findViewById(R.id.linear);
            }
            else
            {
                mView = view;
                mContentView = (TextView) view.findViewById(R.id.name);
                mValueView = (TextView)view.findViewById(R.id.value);
                listItem = (LinearLayout) view.findViewById(R.id.linear);
            }
        }

        @Override
        public String toString() {
            return super.toString() + " '" + mContentView.getText() + "'";
        }
    }
}
