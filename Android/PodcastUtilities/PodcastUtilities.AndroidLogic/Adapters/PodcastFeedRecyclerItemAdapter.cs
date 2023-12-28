using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.Common.Configuration;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Adapters
{
    public class PodcastFeedRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private MainViewModel ViewModel;
        private List<PodcastFeedRecyclerItem> Items = new List<PodcastFeedRecyclerItem>(20);

        public PodcastFeedRecyclerItemAdapter(Context context, MainViewModel viewModel)
        {
            Context = context;
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

            vh.Label.Text = Items[position].PodcastFeed.Folder;
            if (Items[position].PodcastFeed.Feed != null)
            {
                // feeds are optional
                vh.SubLabel.Visibility = ViewStates.Visible;
                vh.SubLabel2.Visibility = ViewStates.Visible;
                var fmt = Context.GetString(Resource.String.feed_sublabel_fmt);
                vh.SubLabel.Text = string.Format(fmt,
                    GetSublabelPart(
                        Items[position].PodcastFeed.Feed.MaximumDaysOld.Value,
                        Resource.Plurals.feed_sublabel_download,
                        Resource.String.feed_sublabel_download_all),
                    GetSublabelPart(
                        Items[position].PodcastFeed.Feed.MaximumNumberOfDownloadedItems.Value,
                        Resource.Plurals.feed_sublabel_max,
                        Resource.String.feed_sublabel_no_max),
                    GetSublabelPart(
                        Items[position].PodcastFeed.Feed.DeleteDownloadsDaysOld.Value,
                        Resource.Plurals.feed_sublabel_delete,
                        Resource.String.feed_sublabel_delete_never)
                );
                var fmt2 = Context.GetString(Resource.String.feed_sublabel_fmt2);
                vh.SubLabel2.Text = string.Format(fmt2,
                    ViewModel.GetNamingStyleText(Items[position].PodcastFeed.Feed.NamingStyle.Value),
                    ViewModel.GetDownloadStratagyText(Items[position].PodcastFeed.Feed.DownloadStrategy.Value)
                );
            } else
            {
                // nothing to display as there is no feed
                vh.SubLabel.Visibility = ViewStates.Gone;
                vh.SubLabel2.Visibility = ViewStates.Gone;
            }

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
        }

        private void Container_Click(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(((View)sender).Tag.ToString());
            ViewModel.FeedItemSelected(Items[position].PodcastFeed);
        }

        private string GetSublabelPart(int value, int formattedId, int maxId)
        {
            if (value == int.MaxValue)
            {
                return Context.GetString(maxId);
            }
            return Context.Resources.GetQuantityString(formattedId, value, value);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_feeditem, parent, false);
            return new RecyclerViewHolder(itemView);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public TextView SubLabel2 { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<View>(Resource.Id.item_row_label_container);
                Label = itemView.FindViewById<TextView>(Resource.Id.item_row_label);
                SubLabel = itemView.FindViewById<TextView>(Resource.Id.item_row_sub_label);
                SubLabel2 = itemView.FindViewById<TextView>(Resource.Id.item_row_sub_label2);
            }
        }
    }
}