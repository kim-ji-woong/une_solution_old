package kr.co.une.energyindustrye_sop.utility;

import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.ViewGroup;

import java.util.HashMap;
import java.util.Map;

/**
 * Created by 김지웅 on 2017-05-28.
 */

public class ImageHelper {
    // Key : ImageView ID
    // Value : 이미지의 원래 크기 비율(상위 4바이트 : width, 하위 4바이트 : height)
    private Map<Integer, Long> m_mapImageSize = new HashMap<Integer, Long>();

    // 상위 4바이트 : width
    // 하위 4바이트 : height
    public long readImageOriginalSize(int nImageID, Resources res)
    {
        BitmapFactory.Options dimensions = new BitmapFactory.Options();
        dimensions.inJustDecodeBounds = true;

        Bitmap mBitmap = BitmapFactory.decodeResource(res, nImageID, dimensions);
        long height = dimensions.outHeight;
        long width =  dimensions.outWidth;

        long result = ((width << 32) | height);
        m_mapImageSize.put(nImageID, result);
        return result;
    }

    // Return 값 : (상위 4바이트 : width, 하위 4바이트 : height)
    //             이미지 크기를 얻어오지 못하면 0보다 작은 값을 리턴한다.
    public long getImageOriginalSize(int nImageID)
    {
        try {
            long imageSize = m_mapImageSize.get(nImageID);
            return imageSize;
        } catch (Exception e) {
        }

        return -1;
    }

    // Return 값 : 이미지 크기를 얻어오지 못하면 0보다 작은 값을 리턴한다.
    public int getImageOriginalWidth(int nImageID)
    {
        try {
            long imageSize = m_mapImageSize.get(nImageID);
            return (int) (imageSize >> 32);
        } catch (Exception e) {
        }

        return -1;
    }

    // Return 값 : 이미지 크기를 얻어오지 못하면 0보다 작은 값을 리턴한다.
    public int getImageOriginalHeight(int nImageID)
    {
        try {
            long imageSize = m_mapImageSize.get(nImageID);
            return (int) (imageSize & 0xffffffff);
        } catch (Exception e) {
        }

        return -1;
    }
}
