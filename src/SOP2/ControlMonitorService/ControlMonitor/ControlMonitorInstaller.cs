using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace ControlMonitoring
{
    [RunInstaller(true)]
    public partial class ControlMonitorInstaller : System.Configuration.Install.Installer
    {
        public ControlMonitorInstaller()
        {
            InitializeComponent();
        }
    }
}
