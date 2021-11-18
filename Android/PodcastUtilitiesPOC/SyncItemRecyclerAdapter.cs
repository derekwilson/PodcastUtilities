using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC
{
    class RecyclerSyncItem
    {
        public ISyncItem SyncItem { get; set; }
        public int ProgressPercentage { get; set; }
        public PodcastInfo Podcast { get; set; }
    }

    class SyncItemRecyclerAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private List<RecyclerSyncItem> Items = new List<RecyclerSyncItem>(20);

        public SyncItemRecyclerAdapter(Context context)
        {
            this.Context = context;
        }

        public void SetItems(List<RecyclerSyncItem> items)
        {
            this.Items = items;
        }

        public RecyclerSyncItem GetItemById(Guid id)
        {
            return this.Items.Find(item => item.SyncItem.Id == id);
        }

        public int GetItemPositionById(Guid id)
        {
             return this.Items.IndexOf(GetItemById(id));
        }

        public int SetItemProgress(Guid id, int progress)
        {
            var item = GetItemById(id);
            item.ProgressPercentage = progress;
            return this.Items.IndexOf(item);
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            vh.Label.Text = Items[position].SyncItem.EpisodeTitle;
            vh.SubLabel.Text = Items[position].SyncItem.Published.ToShortDateString();
            vh.Progress.Progress = Items[position].ProgressPercentage;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_syncitem, parent, false);
            return new RecyclerViewHolder(itemView);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public ProgressBar Progress { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Label = itemView.FindViewById<TextView>(Resource.Id.item_row_label);
                SubLabel = itemView.FindViewById<TextView>(Resource.Id.item_row_sub_label);
                Progress = itemView.FindViewById<ProgressBar>(Resource.Id.item_row_progress);
            }
        }
    }
}