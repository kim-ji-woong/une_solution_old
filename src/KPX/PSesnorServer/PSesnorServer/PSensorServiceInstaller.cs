using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace PSensorServer
{
	[RunInstaller(true)]
    public partial class PSensorServiceInstaller : System.Configuration.Install.Installer
	{
        public PSensorServiceInstaller()
        {
			InitializeComponent();
		}
	}
}
