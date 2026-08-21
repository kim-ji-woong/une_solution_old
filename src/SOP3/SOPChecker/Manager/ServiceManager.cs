using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;
using System.Management;


namespace SOPChecker
{
	public class ServiceManager
	{
        public static string GetServiceStartMode(string serviceName)
        {
            string filter = String.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", serviceName);
            ManagementObjectSearcher query = new ManagementObjectSearcher(filter);
            if (query == null)
                return "<null>";
            try
            {
                ManagementObjectCollection services = query.Get();
                foreach (ManagementObject service in services)
                {
                    return service.GetPropertyValue("StartMode").ToString() == "Auto" ? "Automatic" : "Manual";
                }
            }
            catch (Exception)
            {
                return "<null>";
            }
            return "<null>";
        }

        public static bool IsRunningSerivce(string szSerivceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == szSerivceName)
                    if (service.Status == ServiceControllerStatus.Running)
                        return true;
            }
            return false;
        }

		public static string FindServiceName(string regionString)
		{
			ServiceController[] services = ServiceController.GetServices();
			foreach (ServiceController service in services)
			{
				Debug.WriteLine(service.ServiceName);
				if (service.ServiceName.ToLower().IndexOf(regionString.ToLower()) != -1)
					return service.ServiceName;
			}
			return "";
		}

		private static ServiceController GetService(string szServiceName)
		{
			ServiceController[] services = ServiceController.GetServices();
			foreach (ServiceController service in services)
			{
				if (service.ServiceName == szServiceName)
					return service;
			}
			return null;
		}
		
		public static bool IsServiceInstalled(string serviceName)
		{
			// get list of Windows services
			ServiceController[] services = ServiceController.GetServices();

			// try to find service name
			foreach (ServiceController service in services)
			{
				if (service.ServiceName == serviceName)
					return true;
			}
			return false;
		}

		public static void RestartService(string serviceName, int timeoutMilliseconds)
		{
			ServiceController service = GetService(serviceName);
			if (service == null)
				return;
			try
			{
				int millisec1 = Environment.TickCount;
				TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

				service.Stop();
				service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

				// count the rest of the timeout
				int millisec2 = Environment.TickCount;
				timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

				service.Start();
				service.WaitForStatus(ServiceControllerStatus.Running, timeout);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}

		public static void StartService(string serviceName, int timeoutMilliseconds)
		{
			ServiceController service = GetService(serviceName);
			if (service == null)
				return;
			try
			{
				TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
				service.Start();
				service.WaitForStatus(ServiceControllerStatus.Running, timeout);
			}
			catch(Exception)
			{
			}
		}

		public static void StopService(string serviceName, int timeoutMilliseconds)
		{
			ServiceController service = GetService(serviceName);
			if (service == null)
				return;
			try
			{
				TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
				service.Stop();
				service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
		}
	}
}
