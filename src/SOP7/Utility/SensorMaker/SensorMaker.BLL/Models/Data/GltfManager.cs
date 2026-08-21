using System.Collections.Generic;
using SDMS.IDAL;
using Common.Model.Option;

namespace SensorMaker.BLL.Models.Data
{
    using Response;
    using Request;

    public static class GltfManager
    {
        public static ICollection<GltfModel> LoadGltfModels(IDataManager dataManager, List<int> siteIDs, out string strErrorMessage)
        {
            string strAdditionalConditions = null;

            if (siteIDs != null && siteIDs.Count > 0)
            {
                foreach (int nSiteID in siteIDs)
                {
                    if (strAdditionalConditions == null)
                        strAdditionalConditions = nSiteID.ToString();
                    else
                        strAdditionalConditions += "," + nSiteID.ToString();
                }

                bool isNullable;
                strAdditionalConditions = string.Format("{0} in ({1})", GltfModel.GetFieldName(SDMS.Model.GLTF.Model.Fields.SiteID, out isNullable), strAdditionalConditions);
            }

            strErrorMessage = null;
            List<SDMS.Model.GLTF.Model> models = dataManager.GetSelectManager().SelectGltfModels(null, strAdditionalConditions, out strErrorMessage);

            if (models == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadGltfModels error : " + strErrorMessage);
                return null;
            }

            Dictionary<int, GltfModel> dicModels = new Dictionary<int, GltfModel>();
            Dictionary<int, int> dicParentIDs = new Dictionary<int, int>();

            foreach (SDMS.Model.GLTF.Model model in models)
            {
                GltfModel gltf = new GltfModel();

                gltf.ID = model.ID;
                gltf.ModelName = model.ModelName;
                gltf.ParentID = model.ParentID;
                gltf.SiteID = model.SiteID;

                dicModels[gltf.ID] = gltf;

                if (gltf.ParentID != null)
                {
                    dicParentIDs[gltf.ID] = (int)gltf.ParentID;
                }
            }

            foreach (KeyValuePair<int, int> pair in dicParentIDs)
            {
                GltfModel model, parent;

                if (dicModels.TryGetValue(pair.Key, out model) && dicModels.TryGetValue(pair.Value, out parent))
                {
                    parent.ChildModels.Add(model);
                }
            }

            List<SDMS.Model.GLTF.ModelData> modelDatas = dataManager.GetSelectManager().SelectGltfModelDatas(null, null, out strErrorMessage);

            if (modelDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadGltfModels error : " + strErrorMessage);
                return null;
            }

            foreach (SDMS.Model.GLTF.ModelData modelData in modelDatas)
            {
                GltfModel model;

                if (dicModels.TryGetValue(modelData.ModelID, out model))
                {
                    model.ModelDatas.Add(modelData);
                }
            }

            List<SDMS.Model.GLTF.ModelOrthoData> modelOrthoDatas = dataManager.GetSelectManager().SelectGltfModelOrthoDatas(null, null, out strErrorMessage);

            if (modelOrthoDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadGltfModels error : " + strErrorMessage);
                return null;
            }

            foreach (SDMS.Model.GLTF.ModelOrthoData modelOrthoData in modelOrthoDatas)
            {
                GltfModel model;

                if (dicModels.TryGetValue(modelOrthoData.ModelID, out model))
                {
                    model.ModelOrthoDatas.Add(modelOrthoData);
                }
            }

            return dicModels.Values;
        }

        public static GltfOption LoadGltfOption(Common.IDAL.IDataManager dataManager, string str3DHighVer, out string strErrorMessage)
        {
            GltfOption gltfOption = new GltfOption();
            List<Options> options = dataManager.GetSelectManager().SelectOptions(Options.OptionTarget.GLTF, out strErrorMessage);

            if (options == null)
                return null;

            if (str3DHighVer == "false")
            {
                Options option = FindGlftOption(options, "3DLightModelBaseURL");

                if (option != null)
                    gltfOption._3DModelBaseURL = option.PropertyValue;
            }
            else
            {
                Options option = FindGlftOption(options, "3DModelBaseURL");

                if (option != null)
                    gltfOption._3DModelBaseURL = option.PropertyValue;
            }

            Options optionTextureBaseURL = FindGlftOption(options, "3DTextureBaseURL");

            if (optionTextureBaseURL != null)
                gltfOption._3DTextureBaseURL = optionTextureBaseURL.PropertyValue;

            Options optionBackgroundImage = FindGlftOption(options, "3DBackgroundImage");

            if (optionBackgroundImage != null)
                gltfOption._3DBackgroundImage = optionBackgroundImage.PropertyValue;

            Options optionIndoorModelOnMemory = FindGlftOption(options, "indoorModelOnMemory");

            if (optionIndoorModelOnMemory != null)
            {
                bool success;
                bool onMomery = GetBooleanValue(optionIndoorModelOnMemory, out success);

                if (success)
                    gltfOption.IndoorModelOnMemory = onMomery;
            }

            return gltfOption;
        }

        private static bool GetBooleanValue(Options option, out bool success)
        {
            success = false;

            string strValue = option.PropertyValue;

            if (strValue == null)
                return false;

            strValue = strValue.ToLower();

            if (strValue == "1" || strValue == "true")
            {
                success = true;
                return true;
            }
            else if (strValue == "0" || strValue == "false")
            {
                success = true;
                return false;
            }

            return false;
        }

        private static Options FindGlftOption(List<Options> options, string strOptionName)
        {
            foreach (Options option in options)
            {
                if (string.Compare(option.PropertyName, strOptionName, true) == 0)
                    return option;
            }

            return null;
        }
    }
}
