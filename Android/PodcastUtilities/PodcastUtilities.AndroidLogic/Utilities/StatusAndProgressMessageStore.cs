using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IStatusAndProgressMessageStore
    {
        void Reset();
        void Reset(Guid id);
        void StoreMessage(Guid id, string message);
        string GetMessage(Guid id);
        string GetAllMessages();
        int GetTotalNumberOfLines();
    }

    public class StatusAndProgressMessageStore : IStatusAndProgressMessageStore
    {
        private const string NEWLINE = "\n";

        // do not make this anything other than private
        private object SyncLock = new object();
        private Dictionary<Guid, StringBuilder> Store = new Dictionary<Guid, StringBuilder>(10);

        private ICrashReporter CrashReporter;

        public StatusAndProgressMessageStore(ICrashReporter crashReporter)
        {
            CrashReporter = crashReporter;
        }

        public string GetAllMessages()
        {
            lock (SyncLock)
            {
                StringBuilder result = new StringBuilder(100);
                foreach (var key in Store.Keys)
                {
                    result.Append(Store[key]);
                }
                return result.ToString();
            }
        }
        public int GetTotalNumberOfLines()
        {
            try
            {
                lock (SyncLock)
                {
                    int result = 0;
                    foreach (var key in Store.Keys)
                    {
                        result += GetMessageLineCount(key);
                    }
                    return result;
                }
            } catch (Exception e)
            {
                CrashReporter.LogNonFatalException(e);
                return -1;
            }
        }

        public string GetMessage(Guid id)
        {
            lock (SyncLock)
            {
                return Store[id].ToString();
            }
        }

        private int GetMessageLineCount(Guid id)
        {
            lock (SyncLock)
            {
                if (Store.ContainsKey(id))
                {
                    return Regex.Matches(GetMessage(id), NEWLINE).Count;
                }
                return 0;
            }
        }

        public void Reset()
        {
            Store.Clear();
        }

        public void Reset(Guid id)
        {
            Store[id].Clear();
        }

        public void StoreMessage(Guid id, string message)
        {
            lock (SyncLock)
            {
                if (Store.ContainsKey(id))
                {
                    Store[id].Append(message);
                }
                else
                {
                    Store.Add(id, new StringBuilder(message));
                }
                Store[id].Append(NEWLINE);
            }
        }
    }
}