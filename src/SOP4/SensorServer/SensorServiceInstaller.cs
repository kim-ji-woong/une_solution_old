using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace SensorServer
{
	[RunInstaller(true)]
	public partial class SensorServiceInstaller : System.Configuration.Install.Installer
	{
		public SensorServiceInstaller()
		{
			InitializeComponent();
		}
	}
}
