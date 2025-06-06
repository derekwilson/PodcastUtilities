﻿using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
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
                vh.SubLabel.Text = ViewModel.GetFeedItemSubLabel(Items[position].PodcastFeed);
                vh.SubLabel2.Text = ViewModel.GetFeedItemSubLabel2(Items[position].PodcastFeed);
            }
            else
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