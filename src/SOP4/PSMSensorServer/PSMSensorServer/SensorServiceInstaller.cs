using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace PSMSensorServer
{
	[RunInstaller(true)]
	public partial class PSMSensorServiceInstaller : System.Configuration.Install.Installer
	{
        public PSMSensorServiceInstaller()
		{
			InitializeComponent();
		}
	}
}
