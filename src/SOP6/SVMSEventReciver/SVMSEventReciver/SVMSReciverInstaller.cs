using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace SVMSEventReciver
{
    [RunInstaller(true)]
    public partial class SVMSReciverInstaller : System.Configuration.Install.Installer
    {
        public SVMSReciverInstaller()
        {
            InitializeComponent();
        }
    }
}
