using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Assets
{
    public class UnityScene
    {
        // None : m_model을 단지 끄고 켜기만 한다.
        // OneChild : m_model을 화면에 나타낼 경우 m_model의 형제노드들은 모두 안보이도록 한다.
        public enum SceneOption { None = 0, OneChild };

        private GameObject m_model = null;
        private SceneOption m_option = SceneOption.None;
        private CameraData m_cameraData = null;
        private Vector3 m_vOrbitCenter;
        private float m_fPanningScale = 1.0f;
        private string m_strSceneName = "";

        // Key : Zone Name
        private Dictionary<string, GameObject> m_alarmZones = new Dictionary<string, GameObject>();
        private ConcurrentDictionary<GameObject, GameObject> m_dicActiveAlarmZones = null;

        public SceneOption Option
        {
            get { return m_option; }
            set { m_option = value; }
        }

        public GameObject Model
        {
            get { return m_model; }
            set { m_model = value; }
        }

        public CameraData CameraData
        {
            get { return m_cameraData; }
            set { m_cameraData = value; }
        }

        public Vector3 OrbitCenter
        {
            get { return m_vOrbitCenter; }
            set { m_vOrbitCenter = value; }
        }

        public float PanningScale
        {
            get { return m_fPanningScale; }
            set { m_fPanningScale = value; }
        }

        public string SceneName
        {
            get { return m_strSceneName; }
            set { m_strSceneName = value; }
        }

        public int AlarmZoneCount
        {
            get { return m_alarmZones.Count; }
        }

        public ConcurrentDictionary<GameObject, GameObject> ActiveAlarmZones
        {
            set { m_dicActiveAlarmZones = value; }
        }

        public GameObject GetAlarmZone(string strZoneName)
        {
            GameObject obj;

            if (m_alarmZones.TryGetValue(strZoneName, out obj))
                return obj;

            return null;
        }

        public void AddAlarmZone(GameObject obj, string strZoneName)
        {
            m_alarmZones[strZoneName] = obj;
        }

        public void ShowAlarmZone(string strZoneName, bool hideAllOthers = false)
        {
            GameObject temp;

            if (hideAllOthers)
            {
                foreach (KeyValuePair<string, GameObject> pair in m_alarmZones)
                {
                    if (pair.Key == strZoneName)
                    {
                        if (m_dicActiveAlarmZones != null)
                            m_dicActiveAlarmZones[pair.Value] = pair.Value;
                        else
                            pair.Value.SetActive(true);
                    }
                    else
                    {
                        pair.Value.SetActive(false);

                        if (m_dicActiveAlarmZones != null)
                            m_dicActiveAlarmZones.TryRemove(pair.Value, out temp);
                    }
                }
            }
            else
            {
                GameObject obj;

                if (m_alarmZones.TryGetValue(strZoneName, out obj))
                {
                    if (m_dicActiveAlarmZones != null)
                        m_dicActiveAlarmZones[obj] = obj;
                    else
                        obj.SetActive(true);
                }
            }
        }

        public void HideAlarmZone(string strZoneName)
        {
            GameObject obj, temp;

            if (m_alarmZones.TryGetValue(strZoneName, out obj))
            {
                obj.SetActive(false);

                if (m_dicActiveAlarmZones != null)
                    m_dicActiveAlarmZones.TryRemove(obj, out temp);
            }
        }

        public void HideAllAlarmZones()
        {
            GameObject temp;

            foreach (KeyValuePair<string, GameObject> pair in m_alarmZones)
            {
                pair.Value.SetActive(false);

                if (m_dicActiveAlarmZones != null)
                    m_dicActiveAlarmZones.TryRemove(pair.Value, out temp);
            }
        }
    }

    public class CameraData
    {
        private string m_strCameraName = "";
        private Vector3 m_pos, m_rotation, m_scale;

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public Vector3 LocalPosition
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        public Vector3 LocalEulerAngle
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        public Vector3 LocalScale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }
    }
}
