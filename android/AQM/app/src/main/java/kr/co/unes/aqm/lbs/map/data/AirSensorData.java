package kr.co.unes.aqm.lbs.map.data;

import android.graphics.Color;

/**
 * Created by skkim on 2016-08-29.
 */
public class AirSensorData {

    public String id;
    public String content;
    public String details;
    public String value = "0";


    public AirSensorData(String id, String content, String details) {
        this.id = id;
        this.content = content;
        this.details = details;
    }

    private boolean bSelected = false;

    public boolean IsSelected() {
        return bSelected;
    }

    public void setSelected(boolean bSelect)
    {
        bSelected = bSelect;
    }
    @Override
    public String toString() {
        return content;
    }


    private String lat ="";
    private String lon = "";

    public void setID(String id)
    {
        this.id = id;
    }

    public void setLatitude(String szLat)
    {
        lat = szLat;
    }

    public void setLongitutde(String szLon)
    {
        lon = szLon;
    }

    public void setValue(String val) { value = val; };

    public String getLatitude()
    {
        return lat;
    }

    public String getLongitutde()
    {
        return lon;
    }

    public String getValue() { return value; }


    public String getStatus()
    {
        Double val = Double.parseDouble(value);
        if( val > 30)
            return "아주 나쁨";
        else if(val > 20)
            return "나쁨";
        else if(val > 10)
            return "보통";
        else if(val > 0)
        {
            return "좋음";
        }
        else
            return "알수 없음";

    }

    public int getStatusColor()
    {
        Double val = Double.parseDouble(value);
        if( val > 30)
            return Color.RED;
        else if(val > 20)
            return Color.argb(255,255,128,0);
        else if(val > 10)
            return Color.CYAN;
        else if(val > 0)
        {
            return Color.GREEN;
        }
        else
            return Color.GRAY;
    }
}
