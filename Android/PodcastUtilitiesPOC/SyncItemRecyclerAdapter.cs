using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC
{
    class SyncItemRecyclerAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private List<ISyncItem> Items = new List<ISyncItem>(20);

        public SyncItemRecyclerAdapter(Context context)
        {
            this.Context = context;
        }

        public void SetItems(List<ISyncItem> items)
        {
            this.Items = items;
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            vh.Label.Text = Items[position].EpisodeTitle;
            vh.SubLabel.Text = Items[position].Published.ToShortDateString();
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