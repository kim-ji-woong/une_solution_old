using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;

namespace SOPManager.BLL.Resource
{
    public static class ID
    {
        // Key : Language
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, CategoryItem>> m_dicLanguageItems = new ConcurrentDictionary<string, ConcurrentDictionary<string, CategoryItem>>();
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> m_dicLanguageValues = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        private static string m_strTargetLanguage = "ko";

        public static string TargetLanguage
        {
            get { return m_strTargetLanguage; }
            set { m_strTargetLanguage = value; }
        }

        public static CategoryItem Get(string strCategoryName)
        {
            ConcurrentDictionary<string, CategoryItem> dicCategoryItems;

            if (m_dicLanguageItems.TryGetValue(m_strTargetLanguage, out dicCategoryItems) == false)
                return null;

            CategoryItem item;

            if (dicCategoryItems.TryGetValue(strCategoryName, out item))
                return item;

            return null;
        }

        public static string Value(string strKey)
        {
            ConcurrentDictionary<string, string> dicItemValues;

            if (m_dicLanguageValues.TryGetValue(m_strTargetLanguage, out dicItemValues) == false)
                return null;

            string strValue;

            if (dicItemValues.TryGetValue(strKey, out strValue) == false)
                return null;

            return strValue;
        }

        public static void Init()
        {
            string strExecutablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            int nIndex = strExecutablePath.LastIndexOf('\\');

            if (nIndex < 0)
                return;

            string strResourceFile = strExecutablePath.Substring(0, nIndex) + "\\Resource\\SopManager.language.json";
            StreamReader reader = new StreamReader(strResourceFile, System.Text.Encoding.UTF8);
            JsonTextReader jsonReader = new JsonTextReader(reader);

            int nDepth = 0;

            ConcurrentDictionary<string, CategoryItem> dicCategoryItems = new ConcurrentDictionary<string, CategoryItem>();
            ConcurrentDictionary<string, string> dicValues = new ConcurrentDictionary<string, string>();
            
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.StartObject)
                {
                    nDepth++;
                    ReadJson(jsonReader, dicValues, dicCategoryItems, ref nDepth);
                }
                else if (jsonReader.TokenType == JsonToken.EndObject)
                    nDepth--;
            }

            jsonReader.Close();

            string strTargetLanguage;
            
            if (dicValues.TryGetValue("targetLanguage", out strTargetLanguage))
            {
                m_strTargetLanguage = strTargetLanguage;
            }
        }

        private static void ReadJson(JsonTextReader jsonReader, ConcurrentDictionary<string, string> dicCategoryItemValues, ConcurrentDictionary<string, CategoryItem> dicCategoryItems, ref int nDepth)
        {
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndObject)
                {
                    nDepth--;
                    return;
                }
                else if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    if (jsonReader.Value == null)
                        continue;

                    string strPropertyName = jsonReader.Value.ToString().Trim();

                    if (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            nDepth++;

                            ConcurrentDictionary<string, string> dicSubCatgoryItemValues = new ConcurrentDictionary<string, string>();
                            ConcurrentDictionary<string, CategoryItem> dicSubCategoryItems = new ConcurrentDictionary<string, CategoryItem>();
                            CategoryItem item = new CategoryItem(dicSubCatgoryItemValues, dicSubCategoryItems);
                            dicCategoryItems[strPropertyName] = item;

                            if (nDepth == 2)
                            {
                                m_dicLanguageItems[strPropertyName] = dicSubCategoryItems;
                                m_dicLanguageValues[strPropertyName] = dicSubCatgoryItemValues;
                            }

                            ReadJson(jsonReader, dicSubCatgoryItemValues, dicSubCategoryItems, ref nDepth);
                        }
                        else if (jsonReader.TokenType == JsonToken.String)
                        {
                            string strValue = jsonReader.Value.ToString().Trim();
                            dicCategoryItemValues[strPropertyName] = strValue;
                        }
                    }
                }
                else
                    return;
            }
        }
    }

    public class CategoryItem
    {
        private ConcurrentDictionary<string, string> m_dicValues = null;
        private ConcurrentDictionary<string, CategoryItem> m_dicCategories = null;

        public CategoryItem(ConcurrentDictionary<string, string> dicValues, ConcurrentDictionary<string, CategoryItem> dicCategories)
        {
            m_dicValues = dicValues;
            m_dicCategories = dicCategories;
        }

        public CategoryItem Get(string strCategoryName)
        {
            CategoryItem item;

            if (m_dicCategories.TryGetValue(strCategoryName, out item))
                return item;

            return null;
        }

        public string Value(string strKey)
        {
            string strValue;

            if (m_dicValues.TryGetValue(strKey, out strValue))
                return strValue;

            return null;
        }
    }
}
