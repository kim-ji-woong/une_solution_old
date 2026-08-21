using UnE.Geometry;

namespace SDMS.Model.GLTF
{
    public class ModelOrthoData : IIDObject
    {
        public enum Fields { ID, ModelID, ModelFile, CameraPositionX, CameraPositionY, CameraPositionZ, CameraQuaternionX, CameraQuaternionY, CameraQuaternionZ, CameraQuaternionW, CameraRotationX, CameraRotationY, CameraRotationZ, TargetX, TargetY, TargetZ, Zoom, ZoneID };

        private int m_nID = -1;
        private int m_nModelID = -1;
        private string m_strModelFile = "";
        private Vertex3D m_vCameraPosition = new Vertex3D();
        private Quaternion m_vCameraQuaternion = new Quaternion();
        private Vertex3D m_vCameraRotation = new Vertex3D();
        private Vertex3D m_vTarget = new Vertex3D();
        private float m_fZoom = 1.0f;
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

        public Vertex3D Target
        {
            get { return m_vTarget; }
        }

        public float TargetX
        {
            get { return (float)m_vTarget.x; }
            set { m_vTarget.x = value; }
        }

        public float TargetY
        {
            get { return (float)m_vTarget.y; }
            set { m_vTarget.y = value; }
        }

        public float TargetZ
        {
            get { return (float)m_vTarget.z; }
            set { m_vTarget.z = value; }
        }

        public float Zoom
        {
            get { return m_fZoom; }
            set { m_fZoom = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public static string TableName
        {
            get { return "SdmsGltfModelOrthoData"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ZoneID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
