using System;

using Spring.Context;

using Spring.Context.Support;
using Spring.Core;
using Spring.Aop.Framework;
using Spring.Aop.Support;

using Spring.Objects;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Config;

namespace EventRegistry
{
	/// <summary>
	/// Small example application showing how objects can publish their events
	/// to an IApplicationContex.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The example then goes on to illustrate how subscribers can subscribe to
	/// any events by notifying an IApplicationContext instance. The context
	/// will only wire events and event handlers if they have compatible
	/// signatures.
	/// </p>
	/// </remarks>
	public sealed class Program
	{
		/// <summary>
		/// In this example, the subscriber is subscribing by publisher type. 
		/// </summary>
		[STAThread]
		public static void Main()
		{
			try
			{ 
				using (IApplicationContext ctx = ContextRegistry.GetContext())
				{
                    IObjectDefinitionFactory factory = new DefaultObjectDefinitionFactory();
                    ConstructorArgumentValues constructorArgs = new ConstructorArgumentValues();
                    //constructorArgs.AddNamedArgumentValue("PublisherName", "AAAA");

                    MutablePropertyValues properties = new MutablePropertyValues();
                    properties.Add("PublisherName", "AAAA");

                    AbstractObjectDefinition objDef = factory.CreateObjectDefinition("EventRegistry.MyEventPublisher, EventRegistry", null, AppDomain.CurrentDomain);
                    objDef.IsSingleton = false;    // set this to false for prototype definition
                    objDef.ConstructorArgumentValues = constructorArgs;
                    objDef.PropertyValues = properties;
                
                    IObjectFactory objectFactory = ((IConfigurableApplicationContext)ctx).ObjectFactory;
                    ((IObjectDefinitionRegistry)objectFactory).RegisterObjectDefinition("MyEventPublisher", objDef);

                  
					MyEventPublisher publisher2 = (MyEventPublisher) ctx.GetObject("MyEventPublisher");
                    ctx.PublishEvents(publisher2);

                    MyEventPublisher publisher3 = (MyEventPublisher)ctx.GetObject("MyEventPublisher");
                    publisher3.PublisherName = "BBBBB";
                    ctx.PublishEvents(publisher3);


					MyEventSubscriber subscriber = (MyEventSubscriber) ctx.GetObject("MyEventSubscriber");
					MyEventSubscriber subscriber2 = (MyEventSubscriber) ctx.GetObject("MyEventSubscriber");
					

					ctx.Subscribe(subscriber, typeof (MyEventPublisher));
                    //ctx.Subscribe(subscriber2, typeof(MyEventPublisher));

                    Console.WriteLine("Publisher name: " + publisher2.PublisherName);
					Console.WriteLine("Subscriber 1 Event Handled: " + subscriber.EventHandled);
					Console.WriteLine("Subscriber 2 Event Handled: " + subscriber2.EventHandled);

					// raises a publisher event...
                    publisher2.ClientMethodThatTriggersEvent1();
                    publisher3.ClientMethodThatTriggersEvent1();

					Console.WriteLine("Subscriber 1 Event Handled: " + subscriber.EventHandled);
					Console.WriteLine("Subscriber 2 Event Handled: " + subscriber2.EventHandled);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			finally
			{
                Console.WriteLine();
				Console.WriteLine("--- hit <return> to quit ---");
				Console.ReadLine();	
			}
		}
	}
}