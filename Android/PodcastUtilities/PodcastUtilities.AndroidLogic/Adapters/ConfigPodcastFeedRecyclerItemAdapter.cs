using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.ViewModel.Edit;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Adapters
{
    public class ConfigPodcastFeedRecyclerItemAdapter : RecyclerView.Adapter
    {
        private ILogger Logger;
        private EditConfigViewModel ViewModel;
        private List<PodcastFeedRecyclerItem> Items = new List<PodcastFeedRecyclerItem>(20);

        public ConfigPodcastFeedRecyclerItemAdapter(ILogger logger, EditConfigViewModel viewModel)
        {
            Logger = logger;
            ViewModel = viewModel;
        }

        public void SetItems(List<PodcastFeedRecyclerItem> items)
        {
            Items = items;
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            // unsubscribe if it was subscribed before
            vh.Container.Click -= Container_Click;
            vh.OptionButton.Click -= Option_Click;

            vh.Label.Text = Items[position].PodcastFeed.Folder;

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
            vh.OptionButton.Tag = position.ToString();
            vh.OptionButton.Click += Option_Click;
        }

        private void Option_Click(object sender, EventArgs e)
        {
            Logger.Debug(() => $"Option_Click");
            int position = Convert.ToInt32(((View)sender).Tag.ToString());
            ViewModel.FeedItemOptionSelected(Items[position].Id, Items[position].PodcastFeed);
        }

        private void Container_Click(object sender, EventArgs e)
        {
            Logger.Debug(() => $"Container_Click");
            int position = Convert.ToInt32(((View)sender).Tag.ToString());
            ViewModel.FeedItemSelected(Items[position].Id, Items[position].PodcastFeed);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_configfeeditem, parent, false);
            return new RecyclerViewHolder(itemView);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public ImageView OptionButton { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<View>(Resource.Id.config_item_row_label_container);
                Label = itemView.FindViewById<TextView>(Resource.Id.config_item_row_label);
                SubLabel = itemView.FindViewById<TextView>(Resource.Id.config_item_row_sub_label);
                OptionButton = itemView.FindViewById<ImageView>(Resource.Id.config_item_row_option);
            }
        }
    }

}