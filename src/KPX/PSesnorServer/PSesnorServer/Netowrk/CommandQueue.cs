using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSensorServer
{
    public class CommandQueue
    {
        private static CommandQueue m_Instance = null;

        public static CommandQueue Instance
        {
          get { 
              if( m_Instance == null)
                  m_Instance = new CommandQueue();
              return CommandQueue.m_Instance; 
          }
        }

        private CommandQueue()
        {

        }

        public void Add(JubixNetwork.JubixMessage msg)
        {
            m_Queue.Enqueue(msg);
        }

        public JubixNetwork.JubixMessage Get()
        {
            if( m_Queue.Count > 0)
            {
                return m_Queue.Dequeue();
            }
            return null;
        }

        public int Count
        {
            get { return m_Queue.Count; }
        }

        private Queue<JubixNetwork.JubixMessage> m_Queue = new Queue<JubixNetwork.JubixMessage>();
    }
}
