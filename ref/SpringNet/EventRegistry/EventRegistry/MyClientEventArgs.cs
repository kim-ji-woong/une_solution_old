using System;

namespace EventRegistry
{
	public class MyClientEventArgs : EventArgs
	{
		private string _eventMessage;

		public MyClientEventArgs(string eventMessage)
		{
			_eventMessage = eventMessage;
		}

		public string EventMessage
		{
			get { return _eventMessage; }
		}
	}
}