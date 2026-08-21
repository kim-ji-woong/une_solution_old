using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace RestoreService
{
	[RunInstaller(true)]
	public partial class SOPRestoreInstaller : System.Configuration.Install.Installer
	{
		public SOPRestoreInstaller()
		{
			InitializeComponent();
		}
	}
}
