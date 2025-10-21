using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class ConfigPodcastFeedRecyclerItemAdapter : RecyclerView.Adapter
    {
        private ILogger Logger;
        private EditConfigViewModel ViewModel;
        private List<ConfigPodcastFeedRecyclerItem> Items = new List<ConfigPodcastFeedRecyclerItem>(20);

        public ConfigPodcastFeedRecyclerItemAdapter(ILogger logger, EditConfigViewModel viewModel)
        {
            Logger = logger;
            ViewModel = viewModel;
        }

        public void SetItems(List<ConfigPodcastFeedRecyclerItem> items)
        {
            Items = items;
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder? vh = holder as RecyclerViewHolder;
            if (vh == null)
            {
                return;
            }
            // unsubscribe if it was subscribed before
            vh.Container.Click -= Container_Click;
            vh.OptionButton.Click -= Option_Click;

            vh.Label.Text = Items[position].PodcastFeed.Folder;
            vh.SubLabel.Text = ViewModel.GetFeedSubLabel(Items[position].PodcastFeed);

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
            vh.OptionButton.Tag = position.ToString();
            vh.OptionButton.Click += Option_Click;
        }

        private void Option_Click(object? sender, EventArgs e)
        {
            Logger.Debug(() => $"Option_Click");
            var senderView = sender as View;
            if (sender == null || senderView == null)
            {
                throw new InvalidOperationException("no sender");
            }
            int position = Convert.ToInt32(senderView.Tag?.ToString());
            ViewModel.FeedItemOptionSelected(Items[position].Id, Items[position].PodcastFeed);
        }

        private void Container_Click(object? sender, EventArgs e)
        {
            Logger.Debug(() => $"Container_Click");
            var senderView = sender as View;
            if (sender == null || senderView == null)
            {
                throw new InvalidOperationException("no sender");
            }
            int position = Convert.ToInt32(senderView.Tag?.ToString());
            ViewModel.FeedItemSelected(Items[position].Id, Items[position].PodcastFeed);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View? itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.list_item_configfeeditem, parent, false);
            return new RecyclerViewHolder(itemView!);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public ImageView OptionButton { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = ViewHelper.FindViewByIdOrThrow<View>("container", itemView, Resource.Id.config_item_row_label_container);
                Label = ViewHelper.FindViewByIdOrThrow<TextView>("label", itemView, Resource.Id.config_item_row_label);
                SubLabel = ViewHelper.FindViewByIdOrThrow<TextView>("sub label", itemView, Resource.Id.config_item_row_sub_label);
                OptionButton = ViewHelper.FindViewByIdOrThrow<ImageView>("option", itemView, Resource.Id.config_item_row_option);
            }
        }
    }

}