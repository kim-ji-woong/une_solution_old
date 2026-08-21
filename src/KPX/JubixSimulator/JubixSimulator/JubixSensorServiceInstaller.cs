using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace S1SensorServer
{
	[RunInstaller(true)]
	public partial class JubixSensorServiceInstaller : System.Configuration.Install.Installer
	{
        public JubixSensorServiceInstaller()
		{
			InitializeComponent();
		}
	}
}
