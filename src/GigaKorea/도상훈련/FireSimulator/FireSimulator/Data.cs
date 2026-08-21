using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FireSimulator
{
    public class Project
    {
        private string m_strName = "";
        private List<Level> m_levels = new List<Level>();

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public List<Level> Levels
        {
            get { return m_levels; }
        }

        public Project(string strName)
        {
            m_strName = strName;
        }

        public static Project Read(XElement xml)
        {
            XElement xProject = FindElement(xml, "ProjectInfo");

            if (xProject == null)
                return null;

            XAttribute attr = xProject.Attribute("name");

            if (attr == null)
                return null;

            XElement xLevels = FindElement(xml, "Levels");

            if (xLevels == null)
                return null;

            Project building = new Project(attr.Value);

            foreach (XElement xLevel in xLevels.Elements())
            {
                Level level = Level.Read(xLevel);

                if (level == null)
                    return null;

                building.m_levels.Add(level);
            }

            return building;
        }

        public static XElement FindElement(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                XElement _element = FindElement(element, strNodeName);

                if (_element != null)
                    return _element;
            }

            return null;
        }

        public static XElement FindOneElement(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                if (element.Name == strNodeName)
                    return element;
            }

            return null;
        }
    }

    public class Level
    {
        private string m_strName = "";
        private string m_strID = "";
        private List<Space> m_spaces = new List<Space>();

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public List<Space> Spaces
        {
            get { return m_spaces; }
        }

        public static Level Read(XElement xLevel)
        {
            XAttribute attr = xLevel.Attribute("id");

            if (attr == null)
                return null;

            string strID = attr.Value;

            //XElement xName = Project.FindElement(xLevel, "Name");
            XElement xName = Project.FindOneElement(xLevel, "Name");

            if (xName == null)
                return null;

            Level level = new Level();

            level.Name = xName.Value;
            level.ID = strID;

            XElement xCollection = Project.FindElement(xLevel, "ElementCollection");

            if (xCollection == null)
                return null;

            foreach (XElement element in xCollection.Elements())
            {
                if (element.Name == "Space")
                {
                    Space space = Space.Read(element);

                    if (space == null)
                        return null;

                    level.Spaces.Add(space);
                }
            }

            return level;
        }
    }

    public class Space
    {
        private string m_strName = "";
        private string m_strID = "";

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public static Space Read(XElement xSpace)
        {
            XAttribute attrID = xSpace.Attribute("id");

            if (attrID == null)
                return null;

            string strID = attrID.Value;

            XAttribute attrName = xSpace.Attribute("name");

            if (attrName == null)
                return null;

            string strName = attrName.Value;

            Space space = new Space();
            space.Name = strName;
            space.ID = strID;

            return space;
        }
    }

    public class Alarm
    {
        private DateTime m_timeStamp = new DateTime();
        private FireSimulator.Level m_level = null;
        private FireSimulator.Space m_space = null;

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public FireSimulator.Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public FireSimulator.Space Space
        {
            get { return m_space; }
            set { m_space = value; }
        }
    }
}
