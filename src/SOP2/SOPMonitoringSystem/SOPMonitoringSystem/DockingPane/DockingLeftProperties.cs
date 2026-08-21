using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class DockingLeftProperties : Form
    {
        //private DockingLeftPropertiesLevel m_propertiesLevel = new DockingLeftPropertiesLevel();
        private PropertiesProcess m_propertiesProcess = new PropertiesProcess();
        private PropertiesDecision m_propertiesDecision = new PropertiesDecision();
        private PropertiesAnnotation m_propertiesAnnotation = new PropertiesAnnotation();
        private PropertiesEndPoint m_propertiesEndPoint = new PropertiesEndPoint();
        private PropertiesLink m_propertiesLink = new PropertiesLink();
        private PropertiesTransSOP m_propertiesTransSOP = new PropertiesTransSOP();
        private PropertiesInternal m_propertiesInternal = new PropertiesInternal();
        private PropertiesExternal m_propertiesExternal = new PropertiesExternal();
        private PropertiesTransmission m_propertiesTransmission = new PropertiesTransmission();

        public DockingLeftProperties()
        {
            InitializeComponent();

            InitPropertiesProcess();
            InitPropertiesDicision();
            InitPropertiesAnnotation();
            InitPropertiesEndPoint();
            InitPropertiesLink();
            InitPropertiesTransSOP();
            InitPropertiesInternal();
            InitPropertiesExternal();
            InitPropertiesTransmission();
        }

        private void InitPropertiesProcess()
        {
            m_propertiesProcess.Location = new Point(0, 0);
            m_propertiesProcess.Dock = DockStyle.Fill;
            m_propertiesProcess.TopLevel = false;
            m_propertiesProcess.Parent = this;
            this.Controls.Add(m_propertiesProcess);
            m_propertiesProcess.Show();
        }

        private void InitPropertiesDicision()
        {
            m_propertiesDecision.Location = new Point(0, 0);
            m_propertiesDecision.Dock = DockStyle.Fill;
            m_propertiesDecision.TopLevel = false;
            m_propertiesDecision.Parent = this;
            this.Controls.Add(m_propertiesDecision);
            m_propertiesDecision.Show();
        }

        private void InitPropertiesAnnotation()
        {
            m_propertiesAnnotation.Location = new Point(0, 0);
            m_propertiesAnnotation.Dock = DockStyle.Fill;
            m_propertiesAnnotation.TopLevel = false;
            m_propertiesAnnotation.Parent = this;
            this.Controls.Add(m_propertiesAnnotation);
            m_propertiesAnnotation.Show();
        }

        private void InitPropertiesEndPoint()
        {
            m_propertiesEndPoint.Location = new Point(0, 0);
            m_propertiesEndPoint.Dock = DockStyle.Fill;
            m_propertiesEndPoint.TopLevel = false;
            m_propertiesEndPoint.Parent = this;
            this.Controls.Add(m_propertiesEndPoint);
            m_propertiesEndPoint.Show();
        }

        private void InitPropertiesLink()
        {
            m_propertiesLink.Location = new Point(0, 0);
            m_propertiesLink.Dock = DockStyle.Fill;
            m_propertiesLink.TopLevel = false;
            m_propertiesLink.Parent = this;
            this.Controls.Add(m_propertiesLink);
            m_propertiesLink.Show();
        }

        private void InitPropertiesTransSOP()
        {
            m_propertiesTransSOP.Location = new Point(0, 0);
            m_propertiesTransSOP.Dock = DockStyle.Fill;
            m_propertiesTransSOP.TopLevel = false;
            m_propertiesTransSOP.Parent = this;
            this.Controls.Add(m_propertiesTransSOP);
            m_propertiesTransSOP.Show();
        }

        private void InitPropertiesInternal()
        {
            m_propertiesInternal.Location = new Point(0, 0);
            m_propertiesInternal.Dock = DockStyle.Fill;
            m_propertiesInternal.TopLevel = false;
            m_propertiesInternal.Parent = this;
            this.Controls.Add(m_propertiesInternal);
            m_propertiesInternal.Show();
        }

        private void InitPropertiesExternal()
        {
            m_propertiesExternal.Location = new Point(0, 0);
            m_propertiesExternal.Dock = DockStyle.Fill;
            m_propertiesExternal.TopLevel = false;
            m_propertiesExternal.Parent = this;
            this.Controls.Add(m_propertiesExternal);
            m_propertiesExternal.Show();
        }

        private void InitPropertiesTransmission()
        {
            m_propertiesTransmission.Location = new Point(0, 0);
            m_propertiesTransmission.Dock = DockStyle.Fill;
            m_propertiesTransmission.TopLevel = false;
            m_propertiesTransmission.Parent = this;
            this.Controls.Add(m_propertiesTransmission);
            m_propertiesTransmission.Show();
        }

        public PropertiesProcess GetPropertiesProcess()
        {
            return m_propertiesProcess;
        }

        public PropertiesDecision GetPropertiesDecision()
        {
            return m_propertiesDecision;
        }

        public PropertiesAnnotation GetPropertiesAnnotation()
        {
            return m_propertiesAnnotation;
        }

        public PropertiesEndPoint GetPropertiesEndPoint()
        {
            return m_propertiesEndPoint;
        }

        public PropertiesLink GetPropertiesLink()
        {
            return m_propertiesLink;
        }

        public PropertiesTransSOP GetPropertiesTransSOP()
        {
            return m_propertiesTransSOP;
        }

        public PropertiesInternal GetPropertiesInternal()
        {
            return m_propertiesInternal;
        }

        public PropertiesExternal GetPropertiesExternal()
        {
            return m_propertiesExternal;
        }

        public PropertiesTransmission GetPropertiesTransmission()
        {
            return m_propertiesTransmission;
        }

        public void ShowProperties(int nIndex)
        {
            m_propertiesProcess.Hide();
            m_propertiesDecision.Hide();
            m_propertiesAnnotation.Hide();
            m_propertiesEndPoint.Hide();
            m_propertiesLink.Hide();
            m_propertiesTransSOP.Hide();
            m_propertiesInternal.Hide();
            m_propertiesExternal.Hide();
            m_propertiesTransmission.Hide();

            switch (nIndex)
            {
                case 1:
                    m_propertiesProcess.Show();
                    break;
                case 2:
                    m_propertiesDecision.Show();
                    break;
                case 3:
                    m_propertiesAnnotation.Show();
                    break;
                case 4:
                    m_propertiesEndPoint.Show();
                    break;
                case 5:
                    m_propertiesLink.Show();
                    break;
                case 6:
                    m_propertiesTransSOP.Show();
                    break;
                case 7:
                    m_propertiesInternal.Show();
                    break;
                case 8:
                    m_propertiesExternal.Show();
                    break;
                case 9:
                    m_propertiesTransmission.Show();
                    break;
            }
        }
    }
}
