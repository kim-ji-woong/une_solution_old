using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring.Aop.Framework;
using Spring.Aop.Support;
using WindowsFormsApplication10;

namespace PointCut
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                log4net.Config.DOMConfigurator.Configure();
            }
            catch (System.Exception)
            {
            }

            ProxyFactory factory = new ProxyFactory(new ServiceCommand());
            factory.AddAdvisor(new DefaultPointcutAdvisor(new SdkRegularExpressionMethodPointcut("Do"), new WindowsFormsApplication10.LoggingAdvice()));

            object command = factory.GetProxy();
            ICommand cmd = (ICommand)command;
            cmd.Execute("Hello.Spring.net");
            Console.Out.WriteLine("");
            cmd.DoExecute("DoExe");
            Console.In.ReadLine();    
        }
    }
}
