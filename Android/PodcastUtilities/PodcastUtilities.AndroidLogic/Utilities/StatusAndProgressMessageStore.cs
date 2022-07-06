using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IStatusAndProgressMessageStore
    {
        void Reset();
        void Reset(Guid id);
        void StoreMessage(Guid id, string message);
        string GetMessage(Guid id);
        string GetAllMessages();
    }

    public class StatusAndProgressMessageStore : IStatusAndProgressMessageStore
    {
        // do not make this anything other than private
        private object SyncLock = new object();
        private Dictionary<Guid, StringBuilder> Store = new Dictionary<Guid, StringBuilder>(10);

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

        public string GetMessage(Guid id)
        {
            lock (SyncLock)
            {
                return Store[id].ToString();
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
                Store[id].Append("\n");
            }
        }
    }
}