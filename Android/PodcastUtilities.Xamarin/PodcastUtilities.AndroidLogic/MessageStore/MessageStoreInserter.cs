using PodcastUtilities.AndroidLogic.Converter;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;

namespace PodcastUtilities.AndroidLogic.MessageStore
{
	public interface IMessageStoreInserter
	{
        Tuple<ISyncItem, Status, string> InsertStatus(StatusUpdateEventArgs statusUpdateEventArgs);
        Tuple<ISyncItem, int> InsertProgress(ProgressEventArgs progressEventArgs);
	}


    public class MessageStoreInserter : IMessageStoreInserter
    {
        private ILogger Logger;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IStatusAndProgressMessageStore MessageStore;
        private IByteConverter ByteConverter;

        // do not make this anything other than private
        private object MessageSyncLock = new object();

        public MessageStoreInserter(
            ILogger logger,
            IApplicationControlFileProvider applicationControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IStatusAndProgressMessageStore messageStore,
            IByteConverter byteConverter)
        {
            Logger = logger;
            ApplicationControlFileProvider = applicationControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            MessageStore = messageStore;
            ByteConverter = byteConverter;
        }

        private void AddMessageToStore(StatusUpdateLevel level, Guid id, string message)
        {
            MessageStore.StoreMessage(id, $"{MessageStore.ConvertStatusUpdateLevelToString(level)}{message}");
        }

        public Tuple<ISyncItem, Status, string> InsertStatus(StatusUpdateEventArgs statusUpdateEventArgs)
        {
            // the return value is used to signal anything that the UI needs to know about, null if there is nothing
            Tuple<ISyncItem, Status, string> retval = null;

            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            bool verbose = controlFile?.GetDiagnosticOutput() == DiagnosticOutputLevel.Verbose;
            ISyncItem item = null;
            string id = "NONE";
            if (statusUpdateEventArgs.UserState != null && statusUpdateEventArgs.UserState is ISyncItem)
            {
                item = statusUpdateEventArgs.UserState as ISyncItem;
                id = item.Id.ToString();
            }

            if (statusUpdateEventArgs.MessageLevel == StatusUpdateLevel.Verbose && !verbose)
            {
                // log the status update to the logger but not to the UI
                Logger.Verbose(() => $"MessageStoreInserter:StatusUpdate ID {id}, {statusUpdateEventArgs.Message}, Complete {statusUpdateEventArgs.IsTaskCompletedSuccessfully}");
                return retval;
            }

            // keep all the message together
            lock (MessageSyncLock)
            {
                if (statusUpdateEventArgs.Exception != null)
                {
                    Logger.LogException(() => $"MessageStoreInserter:StatusUpdate ID {id}, {statusUpdateEventArgs.Message} -> ", statusUpdateEventArgs.Exception);
                    CrashReporter.LogNonFatalException(statusUpdateEventArgs.Message, statusUpdateEventArgs.Exception);
                    if (item != null)
                    {
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, item.Id, statusUpdateEventArgs.Message);
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, item.Id, statusUpdateEventArgs.Exception.ToString());
                        retval = Tuple.Create(item, Status.Error, statusUpdateEventArgs.Message);
                    }
                    else
                    {
                        // its just a message - its not attached to a ISyncItem
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, Guid.Empty, statusUpdateEventArgs.Message);
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, Guid.Empty, statusUpdateEventArgs.Exception.ToString());
                    }
                }
                else
                {
                    Logger.Debug(() => $"MessageStoreInserter:StatusUpdate ID {id}, {statusUpdateEventArgs.Message}, Complete {statusUpdateEventArgs.IsTaskCompletedSuccessfully}");
                    Status status = (statusUpdateEventArgs.IsTaskCompletedSuccessfully ? Status.Complete : Status.Information);
                    if (status == Status.Complete)
                    {
                        AnalyticsEngine.DownloadEpisodeCompleteEvent();
                    }
                    if (item != null)
                    {
                        // we are updating the UI as we have a ISyncItem
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, item.Id, statusUpdateEventArgs.Message);
                        retval = Tuple.Create(item, status, statusUpdateEventArgs.Message);
                    }
                    else
                    {
                        // its just a message - its not attached to a ISyncItem
                        AddMessageToStore(statusUpdateEventArgs.MessageLevel, Guid.Empty, statusUpdateEventArgs.Message);
                    }
                }
            }
            return retval;
        }

        public Tuple<ISyncItem, int> InsertProgress(ProgressEventArgs progressEventArgs)
        {
            Tuple<ISyncItem, int> retval = null;
            lock (MessageSyncLock)
            {
                ISyncItem syncItem = progressEventArgs.UserState as ISyncItem;
                if (progressEventArgs.ProgressPercentage % 10 == 0)
                {
                    // only do every 10%
                    var line = string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                    DisplayFormatter.RenderFileSize(progressEventArgs.ItemsProcessed),
                                                    DisplayFormatter.RenderFileSize(progressEventArgs.TotalItemsToProcess),
                                                    progressEventArgs.ProgressPercentage);
                    Logger.Debug(() => line);
                    MessageStore.StoreMessage(syncItem.Id, line);
                    if (progressEventArgs.ProgressPercentage == 100)
                    {
                        AnalyticsEngine.DownloadEpisodeEvent(ByteConverter.BytesToMegabytes(progressEventArgs.TotalItemsToProcess));
                    }
                    retval = Tuple.Create(syncItem, progressEventArgs.ProgressPercentage);
                }
            }
            return retval;
        }
    }
}