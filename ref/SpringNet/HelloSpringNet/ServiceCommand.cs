using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AopAlliance.Intercept;
using Spring.Aop.Framework;
using Common.Logging;

namespace WindowsFormsApplication10
{
    public class ServiceCommand : ICommand
    {
        public object Execute(object ctx)
        {
            string szText = string.Format("Message : {0}", ctx);
            Console.Out.WriteLine(szText);

            return null;
        }
        
        public object DoExecute(object ctx)
        {           
            string szText = string.Format("Message : {0}", ctx);
            Console.Out.WriteLine(szText);
            return null;
        }
    }

    public class LoggingAdvice : AopAlliance.Intercept.IMethodInterceptor
    {
        private static ILog Log = null;
        public object Invoke(IMethodInvocation invocation)
        {
            if (Log == null)
                Log = LogManager.GetCurrentClassLogger();
            Log.Debug("Pre Advice Invoke");
            object result = invocation.Proceed();
            Log.Debug("Post Advice Invoke");
            return result;
        }
    }
}
