using System;
using System.Collections.Generic;
using System.Threading;

namespace UBMLViewer
{
  
    public class PythonLogger
    {
        private Mutex _mutex = new Mutex(false);
        private UInt32 _entryCount = 0;
        public class Entry
        {
            public enum EntryType
            {
                Info,
                Warning,
                Error,
                Fault
            }

            private EntryType _entryType;
            private DateTime _timestamp;
            private String _msg;
            private UInt32 _index;

            private object _tag;

            private Entry()
            {
                
            }

            public Entry(EntryType entryType, String msg, UInt32 index)
            {
                _tag = null;
                _msg = msg;
                _timestamp = DateTime.Now;
                _entryType = entryType;
                _index = index;
            }

            public object Tag { get { return _tag; } set { _tag = value; } }
            public String msg { get { return _msg; } }
            public DateTime timestamp { get { return _timestamp; } }
            public EntryType entryType { get { return _entryType; } }
            public UInt32 index { get { return _index; } }

            public override string ToString()
            {
                return String.Format("[{0}][{1}][{2}][{3}]", _timestamp, _index, _entryType, _msg);
            }
        }

        private List<Entry> _entries = new List<Entry>();

        public void Reset()
        {
            try
            {
                _mutex.WaitOne();
                _entries = new List<Entry>();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public Int32 Count
        {
            get
            {
                _mutex.WaitOne();
                Int32 result = _entries.Count; _mutex.ReleaseMutex(); return result;
            }
        }

        /// <summary>
        /// Gets the first entry in log and removes it from the log.
        /// Returns null if the log is empty.
        /// </summary>
        /// <returns></returns>
        public Entry GetFirst()
        {
            Entry result = null;
            try
            {
                _mutex.WaitOne();
                if (_entries.Count > 0)
                {
                    result = _entries[0];
                    _entries.RemoveAt(0);
                }

            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return result;
        }

        /// <summary>
        /// Retrives all the entries from the log.  The log will be 
        /// empty after the operation has been executed.
        /// </summary>
        /// <returns></returns>
        public List<Entry> GetAll()
        {
            List<Entry> result = null;
            try
            {
                _mutex.WaitOne();
                result = _entries;
                _entries = new List<Entry>();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return result;
        }

        public Entry AddInfo(String msg)
        {
            Entry newEntry = null;
            try
            {                
                _mutex.WaitOne();
                newEntry = new Entry(Entry.EntryType.Info, msg, _entryCount++);
                _entries.Add(newEntry);                
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return newEntry;
        }

        public Entry AddWarning(String msg)
        {
            Entry newEntry = null;
            try
            {               
                _mutex.WaitOne();
                 newEntry = new Entry(Entry.EntryType.Warning, msg, _entryCount++);
                _entries.Add(newEntry);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return newEntry;
        }

        public Entry AddError(String msg)
        {
            Entry newEntry = null;
            try
            {
                
                _mutex.WaitOne();
                newEntry = new Entry(Entry.EntryType.Error, msg, _entryCount++);
                _entries.Add(newEntry);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return newEntry;
        }

        public Entry AddFault(String msg)
        {
            Entry newEntry = null;
            try
            {
                _mutex.WaitOne();
                newEntry = new Entry(Entry.EntryType.Fault, msg, _entryCount++);
                _entries.Add(newEntry);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return newEntry;
        }

        public Entry AddFault(Exception ex)
        {
            Entry newEntry = null;
            try
            {
                _mutex.WaitOne();
                String msg = ex.Message;
                if (ex.InnerException != null)
                    msg += " (+INNER): " + ex.InnerException.Message;

                newEntry = new Entry(Entry.EntryType.Fault, msg, _entryCount++);
                _entries.Add(newEntry);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
            return newEntry;
        }
    }
}
