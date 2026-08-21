using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XMLWebServiceManager.BIM
{
    public class Project
    {
        public enum UnitOfLength { MM = 0, CM, M };

        private int m_nProjectID = 0;
        private string m_strProjectName = "";
        private UnitOfLength m_unit = UnitOfLength.CM;
        private DateTime m_timeStamp = new DateTime();
        private string m_strAuthor = null;

        private List<Level> m_levels = new List<Level>();
        private Dictionary<int, Component> m_dicComponents = new Dictionary<int, Component>();

        private string m_strLocalFilePath = null;
        private List<Property> m_properties = new List<Property>();

        private AnchorNode m_anchorNode = new AnchorNode();

        public int ID
        {
            get { return m_nProjectID; }
            set { m_nProjectID = value; }
        }

        public string Name
        {
            get { return m_strProjectName; }
            set { m_strProjectName = value; }
        }

        public UnitOfLength Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public string Author
        {
            get { return m_strAuthor; }
            set { m_strAuthor = value; }
        }

        public List<Level> Levels
        {
            get { return m_levels; }
        }

        public string LocalFilePath
        {
            get { return m_strLocalFilePath; }
            set { m_strLocalFilePath = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public List<Component> Components
        {
            get { return m_dicComponents.Values.ToList(); }
        }

        public AnchorNode AnchorNode
        {
            get { return m_anchorNode; }
            set { m_anchorNode = value; }
        }

        public Level GetFirstLevel()
        {
            foreach (Level level in m_levels)
            {
                if (level.FloorIndex == 0)
                    return level;
            }

            if (m_levels.Count > 0)
                return m_levels[0];

            return null;
        }

        public void AddComponent(Component component)
        {
            m_dicComponents[component.ID] = component;
        }

        public Component FindComponent(int nComponentID)
        {
            Component component = null;
            m_dicComponents.TryGetValue(nComponentID, out component);
            return component;
        }

        public void RemoveComponent(Component component)
        {
            m_dicComponents.Remove(component.ID);
        }

        public Component FindComponentFromCode(string strCode)
        {
            string strComponentName = Component.GetComponentNameFromCode(strCode);

            foreach (KeyValuePair<int, Component> pair in m_dicComponents)
            {
                if (pair.Value.ComponentName == strComponentName)
                    return pair.Value;
            }

            return null;
        }

        public void SortLevel()
        {
            m_levels.Sort();

            int nLevelCount = m_levels.Count;
            int nFloorIndex = 0;
            int nFirstIndex = 0;

            for (int i = 0; i < nLevelCount; i++)
            {
                Level level = m_levels[i];

                if (level.Elevation >= 0.0f)
                {
                    if (nFloorIndex == 0)
                        nFirstIndex = i;

                    level.FloorIndex = nFloorIndex++;
                }

                string strName = level.Name.ToLower();

                if (strName.Contains("roof") || strName.Contains("지붕"))
                    level.FloorIndex = Level.RoofFloorIndex;
            }

            nFloorIndex = -1;

            for (int i = nFirstIndex - 1; i >= 0; i--)
            {
                Level level = m_levels[i];
                level.FloorIndex = nFloorIndex--;
            }
        }

        public static UnitOfLength GetUnit(string strUnit)
        {
            if (strUnit == "mm")
                return UnitOfLength.MM;
            else if (strUnit == "cm")
                return UnitOfLength.CM;
            else if (strUnit == "meter")
                return UnitOfLength.M;

            return UnitOfLength.MM;
        }

        public string GetUnitString()
        {
            if (m_unit == UnitOfLength.MM)
                return "mm";
            else if (m_unit == UnitOfLength.CM)
                return "cm";
            else if (m_unit == UnitOfLength.M)
                return "meter";

            return "";
        }
    }
}
