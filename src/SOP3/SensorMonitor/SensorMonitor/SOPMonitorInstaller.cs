using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace SensorMonitor
{
	[RunInstaller(true)]
	public partial class SOPMonitorInstaller : System.Configuration.Install.Installer
	{
		public SOPMonitorInstaller()
		{
			InitializeComponent();
		}
	}
}
