using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Response
{
    public class ResponseGltfDataList : MessageResult
    {
        private List<GltfModel> m_gltfModels = null;
        private GltfOption m_gltfOption = null;

        public List<GltfModel> Models
        {
            get { return m_gltfModels; }
            set { m_gltfModels = value; }
        }

        public GltfOption GltfOption
        {
            get { return m_gltfOption; }
            set { m_gltfOption = value; }
        }
    }

    public class GltfModel : SDMS.Model.GLTF.Model
    {
        private List<GltfModel> m_childModels = new List<GltfModel>();
        private List<SDMS.Model.GLTF.ModelData> m_modelDatas = new List<SDMS.Model.GLTF.ModelData>();
        private List<SDMS.Model.GLTF.ModelOrthoData> m_modelOrthoDatas = new List<SDMS.Model.GLTF.ModelOrthoData>();

        public List<GltfModel> ChildModels
        {
            get { return m_childModels; }
            set { m_childModels = value; }
        }

        public List<SDMS.Model.GLTF.ModelData> ModelDatas
        {
            get { return m_modelDatas; }
            set { m_modelDatas = value; }
        }

        public List<SDMS.Model.GLTF.ModelOrthoData> ModelOrthoDatas
        {
            get { return m_modelOrthoDatas; }
            set { m_modelOrthoDatas = value; }
        }
    }

    public class GltfOption
    {
        private string m_str3DModelBaseURL = null;
        private string m_str3DTextureBaseURL = null;
        private string m_str3DBackgroundImage = null;
        private bool m_indoorModelOnMemory = true;


        public string _3DModelBaseURL
        {
            get { return m_str3DModelBaseURL; }
            set { m_str3DModelBaseURL = value; }
        }

        public string _3DTextureBaseURL
        {
            get { return m_str3DTextureBaseURL; }
            set { m_str3DTextureBaseURL = value; }
        }

        public string _3DBackgroundImage
        {
            get { return m_str3DBackgroundImage; }
            set { m_str3DBackgroundImage = value; }
        }

        public bool IndoorModelOnMemory
        {
            get { return m_indoorModelOnMemory; }
            set { m_indoorModelOnMemory = value; }
        }
    }
}
