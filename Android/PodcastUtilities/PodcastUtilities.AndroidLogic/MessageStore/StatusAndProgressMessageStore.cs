using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PodcastUtilities.AndroidLogic.MessageStore
{
    public interface IStatusAndProgressMessageStore
    {
        void Reset();
        void Reset(Guid id);
        void StoreMessage(Guid id, string message);
        string GetMessage(Guid id);
        string GetAllMessages();
        string GetErrorMessages();
        int GetTotalNumberOfLines();
        string ConvertStatusUpdateLevelToString(StatusUpdateLevel level);
    }

    public class StatusAndProgressMessageStore : IStatusAndProgressMessageStore
    {
        private const string START_WARNING = "W,";
        private const string START_ERROR = "E,";
        private const string START_VERBOSE = "V,";
        private const string START_STATUS = " ,";

        private const string NEWLINE = "\n";
        private const string END_OF_LOGS = "\n--- end of logs ---\n";

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
                result.Append(END_OF_LOGS);
                return result.ToString();
            }
        }

        public string GetErrorMessages()
        {
            lock (SyncLock)
            {
                StringBuilder result = new StringBuilder(100);
                foreach (var key in Store.Keys)
                {
                    var multilineMessage = Store[key].ToString();
                    var lines = multilineMessage.Split(NEWLINE);
                    foreach (var line in lines)
                    {
                        // NOTE: this will only get the first line of multiline error messages
                        if (line.StartsWith(START_ERROR) || line.StartsWith(START_WARNING))
                        {
                            result.Append(line);
                            result.Append(NEWLINE);
                        }
                    }
                }
                result.Append(END_OF_LOGS);
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

        public string ConvertStatusUpdateLevelToString(StatusUpdateLevel level)
        {
            switch (level)
            {
                case StatusUpdateLevel.Status:
                    return START_STATUS;
                case StatusUpdateLevel.Warning:
                    return START_WARNING;
                case StatusUpdateLevel.Error:
                    return START_ERROR;
                case StatusUpdateLevel.Verbose:
                    return START_VERBOSE;
                default:
                    return START_STATUS;
            }
        }
    }
}