using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SOPManager
{
    public class SOPCategory
    {
        private static List<Bitmap> m_subCategoryImageList = null;
        private static List<int> m_subCategoryImageIndex = null;

        private const int Typhoon = 0;
        private const int Earthquake = 1;
        private const int Snowfall = 2;
        private const int Flooding = 3;
        private const int EtcUser = 4;
        private const int Fire = 5;
        private const int Spill = 6;
        private const int Terror = 7;
        private const int Volcano = 8;
        private const int _119 = 9;
        private const int Security = 10;

        private static Bitmap GetBitmap(int nIndex)
        {
            switch (nIndex)
            {
                case Typhoon:
                    return global::SOPManager.Properties.Resources.btn_sub_typoon;

                case Earthquake:
                    return global::SOPManager.Properties.Resources.btn_sub_earthquake;

                case Snowfall:
                    return global::SOPManager.Properties.Resources.btn_sub_snowfall;

                case Flooding:
                    return global::SOPManager.Properties.Resources.btn_sub_flooding;

                case EtcUser:
                    return global::SOPManager.Properties.Resources.btnEtc_User;

                case Fire:
                    return global::SOPManager.Properties.Resources.btn_sub_fire;

                case Spill:
                    return global::SOPManager.Properties.Resources.btn_sub_spill;

                case Terror:
                    return global::SOPManager.Properties.Resources.btn_sub_terror;

                case Volcano:
                    return global::SOPManager.Properties.Resources.btn_sub_volcano;

                case _119:
                    return global::SOPManager.Properties.Resources.btn_sub_119;

                case Security:
                    return global::SOPManager.Properties.Resources.btn_sub_security;
            }

            return global::SOPManager.Properties.Resources.btnEtc_User;
        }

        // 첫번째 요소 : DisasterCategory ID
        private static object[] m_arSubCategorys =
		{
			"자연재해", "태풍", Typhoon,
			"자연재해", "지진", Earthquake,
			"자연재해", "폭설", Snowfall,
			"자연재해", "침수", Flooding,
			"기타", "일반재해", EtcUser,
			"화재", "화재", Fire,
			"화재", "산불", Fire,
			"유출사고", "오염", Spill,
			"유출사고", "누출", Spill,
			"유출사고", "유출", Spill,
			"유출사고", "암모니아", Spill,
			"테러", "테러", Terror,
			"폭발", "폭발", Volcano,
			"인명구조 및 의료지원", "119상황", _119,
			"인명구조 및 의료지원", "SOP상황", EtcUser,
			"테러", "무장", Terror,
			"테러", "괴선박", Terror,
			"테러", "폭탄", Terror,
			"테러", "침입", Terror,
			"테러", "폭약", Terror,
            "방범", "침입", Security,
            "방범", "배회", Security,
            "방범", "쓰러짐", Security,
            "방범", "도난", Security,
            "방범", "방치", Security,
            "방범", "가상펜스", Security,
            "방범", "내부비상벨", Security,
            "방범", "외부비상벨", Security,
            "방범", "기타", Security,
            "방범", "방범", Security
		};

        public static Image GetDefaultSubCategoryImage()
        {
            return new Bitmap(1, 1); // global::SOPManager.Properties.Resources.btnEtc_User;
        }

        public static Image GetSubCategoryImage(string strDiasterCategoryName, string strSubDisasterCategoryName)
        {
            return GetDefaultSubCategoryImage();

            //for (int i = 0; i < m_arSubCategorys.Length; i += 3)
            //{
            //    if (strDiasterCategoryName == (string)m_arSubCategorys[i])
            //    {
            //        if (strSubDisasterCategoryName == (string)m_arSubCategorys[i + 1] || strSubDisasterCategoryName.Contains((string)m_arSubCategorys[i + 1]))
            //            return GetBitmap((int)m_arSubCategorys[i + 2]);
            //    }
            //}
            //return GetDefaultSubCategoryImage();
        }

        private static Data_DisasterCategory GetDisasterCategory(int nDisasterCategoryID)
        {
            Data_DisasterCategory category = null;

            foreach (Data_DisasterCategory disasterCategory in FormMain.Instance.DisasterCategory)
            {
                if (disasterCategory.ID == nDisasterCategoryID)
                {
                    category = disasterCategory;
                    break;
                }
            }

            return category;
        }

        public static Image GetSubCategoryImage(int nDisasterCategoryID, string strSubDisasterCategoryName)
        {
            return GetDefaultSubCategoryImage();

            //Data_DisasterCategory category = GetDisasterCategory(nDisasterCategoryID);

            //if (category != null)
            //    return GetSubCategoryImage(category.CategoryName, strSubDisasterCategoryName);

            //return GetDefaultSubCategoryImage();
        }

        private static void MakeSubCategoryImageList()
        {
            if (m_subCategoryImageList != null)
                return;

            m_subCategoryImageList = new List<Bitmap>();
            m_subCategoryImageIndex = new List<int>();

            Dictionary<int, int> dicImageList = new Dictionary<int, int>();

            for (int i = 0; i < m_arSubCategorys.Length; i += 3)
            {
                dicImageList[(int)m_arSubCategorys[i + 2]] = (int)m_arSubCategorys[i + 2];
                //m_subCategoryImageList.Add((Bitmap)m_arSubCategorys[i + 2]);
            }

            foreach (KeyValuePair<int, int> pair in dicImageList)
            {
                m_subCategoryImageIndex.Add(pair.Key);
                m_subCategoryImageList.Add(GetBitmap(pair.Key));
            }
        }

        public static List<Bitmap> GetSubCategoryIamgeList()
        {
            if (m_subCategoryImageList == null)
                MakeSubCategoryImageList();

            return m_subCategoryImageList;
        }

        public static int GetSubCategoryImageIndex(string strDisasterCategoryName, string strSubDisasterCategoryName)
        {
            int nImageIndex = -1;

            for (int i = 0; i < m_arSubCategorys.Length; i += 3)
            {
                if (strDisasterCategoryName == (string)m_arSubCategorys[i])
                {
                    if (strSubDisasterCategoryName == (string)m_arSubCategorys[i + 1] || strSubDisasterCategoryName.Contains((string)m_arSubCategorys[i + 1]))
                    {
                        nImageIndex = (int)m_arSubCategorys[i + 2];
                        break;
                    }
                }
            }

            if (nImageIndex >= 0)
            {
                int nIndexCount = m_subCategoryImageIndex.Count;

                for (int i=0;i<nIndexCount;i++)
                {
                    if (nImageIndex == m_subCategoryImageIndex[i])
                        return i;
                }
            }

            return 0;
        }

        public static int GetSubCategoryImageIndex(int nDisasterCategoryID, string strSubDisasterCategoryName)
        {
            Data_DisasterCategory category = GetDisasterCategory(nDisasterCategoryID);

            if (category != null)
                return GetSubCategoryImageIndex(category.CategoryName, strSubDisasterCategoryName);

            return 0;
        }
    }
}
