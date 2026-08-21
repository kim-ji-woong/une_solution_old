using UnE.Geometry;

namespace SDMS.Model.GLTF
{
    public class ModelData : IIDObject
    {
        public enum Fields { ID, ModelID, ModelFile, ModelDisplayText, CameraPositionX, CameraPositionY, CameraPositionZ, CameraQuaternionX, CameraQuaternionY, CameraQuaternionZ, CameraQuaternionW, CameraRotationX, CameraRotationY, CameraRotationZ, CameraFov, CameraNear, CameraFar, OrbitTargetX, OrbitTargetY, OrbitTargetZ, FloorIndex, BuildingGroupID, BuildingID, ZoneID };

        private int m_nID = -1;
        private int m_nModelID = -1;
        private string m_strModelFile = "";
        private string m_strModelDisplayText = null;
        private Vertex3D m_vCameraPosition = new Vertex3D();
        private Quaternion m_vCameraQuaternion = new Quaternion();
        private Vertex3D m_vCameraRotation = new Vertex3D();
        private int m_nCameraFov = 0;
        private float m_fCameraNear = 0;
        private float m_fCameraFar = 0;
        private Vertex3D m_vOrbitTarget = new Vertex3D();
        private float? m_fFloorIndex = null;
        private int? m_nBuildingGroupID = null;
        private int? m_nBuildingID = null;
        private int? m_nZoneID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int ModelID
        {
            get { return m_nModelID; }
            set { m_nModelID = value; }
        }

        public string ModelFile
        {
            get { return m_strModelFile; }
            set { m_strModelFile = value; }
        }

        public string ModelDisplayText
        {
            get { return m_strModelDisplayText; }
            set { m_strModelDisplayText = value; }
        }

        public Vertex3D CameraPosition
        {
            get { return m_vCameraPosition; }
        }

        public float CameraPositionX
        {
            get { return (float)m_vCameraPosition.x; }
            set { m_vCameraPosition.x = value; }
        }

        public float CameraPositionY
        {
            get { return (float)m_vCameraPosition.y; }
            set { m_vCameraPosition.y = value; }
        }

        public float CameraPositionZ
        {
            get { return (float)m_vCameraPosition.z; }
            set { m_vCameraPosition.z = value; }
        }

        public Quaternion CameraQuaternion
        {
            get { return m_vCameraQuaternion; }
        }

        public float CameraQuaternionX
        {
            get { return (float)m_vCameraQuaternion.x; }
            set { m_vCameraQuaternion.x = value; }
        }

        public float CameraQuaternionY
        {
            get { return (float)m_vCameraQuaternion.y; }
            set { m_vCameraQuaternion.y = value; }
        }

        public float CameraQuaternionZ
        {
            get { return (float)m_vCameraQuaternion.z; }
            set { m_vCameraQuaternion.z = value; }
        }

        public float CameraQuaternionW
        {
            get { return (float)m_vCameraQuaternion.w; }
            set { m_vCameraQuaternion.w = value; }
        }

        public Vertex3D CameraRotation
        {
            get { return m_vCameraRotation; }
        }

        public float CameraRotationX
        {
            get { return (float)m_vCameraRotation.x; }
            set { m_vCameraRotation.x = value; }
        }

        public float CameraRotationY
        {
            get { return (float)m_vCameraRotation.y; }
            set { m_vCameraRotation.y = value; }
        }

        public float CameraRotationZ
        {
            get { return (float)m_vCameraRotation.z; }
            set { m_vCameraRotation.z = value; }
        }

        public int CameraFov
        {
            get { return m_nCameraFov; }
            set { m_nCameraFov = value; }
        }

        public float CameraNear
        {
            get { return m_fCameraNear; }
            set { m_fCameraNear = value; }
        }

        public float CameraFar
        {
            get { return m_fCameraFar; }
            set { m_fCameraFar = value; }
        }

        public Vertex3D OrbitTarget
        {
            get { return m_vOrbitTarget; }
        }

        public float OrbitTargetX
        {
            get { return (float)m_vOrbitTarget.x; }
            set { m_vOrbitTarget.x = value; }
        }

        public float OrbitTargetY
        {
            get { return (float)m_vOrbitTarget.y; }
            set { m_vOrbitTarget.y = value; }
        }

        public float OrbitTargetZ
        {
            get { return (float)m_vOrbitTarget.z; }
            set { m_vOrbitTarget.z = value; }
        }

        public float? FloorIndex
        {
            get { return m_fFloorIndex; }
            set { m_fFloorIndex = value; }
        }

        public int? BuildingGroupID
        {
            get { return m_nBuildingGroupID; }
            set { m_nBuildingGroupID = value; }
        }

        public int? BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public static string TableName
        {
            get { return "SdmsGltfModelData"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ModelDisplayText ||
                field == Fields.FloorIndex ||
                field == Fields.BuildingGroupID ||
                field == Fields.BuildingID ||
                field == Fields.ZoneID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
