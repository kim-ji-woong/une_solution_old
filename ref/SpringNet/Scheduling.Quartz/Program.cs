using System;
using System.Collections;
using System.Collections.Generic;

using Spring.Core;
using Spring.Context;
using Spring.Context.Support;

using Spring.Objects;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Config;



namespace Spring.Scheduling.Quartz.Example
{
    class Program
    {

        //static void RegisterObject(IApplicationContext ctx, IObjectDefinitionFactory factory, ConstructorArgumentValues constructorArgs, MutablePropertyValues properties, string szTypeName, string szTypeID)
        //{
        //    AbstractObjectDefinition objDef = factory.CreateObjectDefinition(szTypeName, null, AppDomain.CurrentDomain);
        //    objDef.IsSingleton = true;    // set this to false for prototype definition
        //    objDef.ConstructorArgumentValues = constructorArgs;
        //    objDef.PropertyValues = properties;

        //    IObjectFactory objectFactory = ((IConfigurableApplicationContext)ctx).ObjectFactory;
        //    ((IObjectDefinitionRegistry)objectFactory).RegisterObjectDefinition(szTypeID, objDef);
        //}

        static void Main()
        {
            try
            {
                IApplicationContext ctx = ContextRegistry.GetContext();    
				Console.WriteLine("Spring configuration succeeded, quartz jobs running.");
                
                AdminService service = new AdminService();
                service.UserName = "Admin";
                
                Dictionary<string, string> dic  = new Dictionary<string, string>();
                dic.Add("UserName","Alexandre");
                JobDetailObject exampleJob = new JobDetailObject()
                {
                    Name = "ExampleJob",
                    JobType = new Spring.Scheduling.Quartz.Example.ExampleJob().GetType(),
                    JobDataAsMap = dic
                };
                
                MethodInvokingJobDetailFactoryObject methodInvoker = new MethodInvokingJobDetailFactoryObject();
                methodInvoker.Name = "AdminSerivce";
                methodInvoker.TargetObject = service;
                methodInvoker.TargetMethod = "DoAdminWork";                
                methodInvoker.AfterPropertiesSet();

                CronTriggerObject trigger = new CronTriggerObject();
                trigger.Name = "cronTrigger";
                trigger.JobDetail = exampleJob;              
                trigger.CronExpressionString = "0/20 * * * * ?";
                trigger.AfterPropertiesSet();

                SimpleTriggerObject trigger2 = new SimpleTriggerObject();
                trigger2.Name = "simpleTrigger";
                trigger2.JobDetail = (global::Quartz.IJobDetail)methodInvoker.GetObject();
                trigger2.StartDelay = new TimeSpan(0, 0, 5);
                trigger2.RepeatInterval = new TimeSpan(0, 0, 5);
                trigger2.AfterPropertiesSet();

                Spring.Scheduling.Quartz.SchedulerFactoryObject sFactory = new SchedulerFactoryObject();
                sFactory.Triggers = new global::Quartz.ITrigger[] {trigger , trigger2};
                sFactory.SchedulerName = "Spring";
                //sFactory.QuartzProperties = 
                sFactory.AfterPropertiesSet();
                sFactory.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Out.WriteLine("--- Press <return> to quit ---");
                Console.ReadLine();
            }
            Console.Out.WriteLine("--- Press <return> to quit ---");
            Console.ReadLine();
        }
    }
}
