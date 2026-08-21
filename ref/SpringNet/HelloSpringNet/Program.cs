using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Spring.Aop.Framework;

namespace WindowsFormsApplication10
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                log4net.Config.DOMConfigurator.Configure();
            }
            catch (System.Exception)
            {
            }

            ProxyFactory factory = new ProxyFactory(new ServiceCommand());
            factory.AddAdvice(new LoggingAdvice());

            object command = factory.GetProxy();
            ICommand cmd = (ICommand)command;
            cmd.Execute("Hello.Spring.net");

            Console.In.ReadLine();            
        }
    }
}
