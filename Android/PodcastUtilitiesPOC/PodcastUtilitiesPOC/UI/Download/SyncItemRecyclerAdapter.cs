using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilitiesPOC.UI.Download
{
    class SyncItemRecyclerAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private List<RecyclerSyncItem> Items = new List<RecyclerSyncItem>(20);
        private bool ReadOnly = false;

        public SyncItemRecyclerAdapter(Context context)
        {
            Context = context;
        }

        public void SetItems(List<RecyclerSyncItem> items)
        {
            Items = items;
        }

        public RecyclerSyncItem GetItemById(Guid id)
        {
            return Items.Find(item => item.SyncItem.Id == id);
        }

        public int GetItemPositionById(Guid id)
        {
            return Items.IndexOf(GetItemById(id));
        }

        public int SetItemProgress(Guid id, int progress)
        {
            var item = GetItemById(id);
            item.ProgressPercentage = progress;
            return Items.IndexOf(item);
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            // unsubscribe if it was subscribed before
            vh.Container.Click -= Container_Click;

            vh.Label.Text = Items[position].SyncItem.EpisodeTitle;
            vh.SubLabel.Text = Items[position].SyncItem.Published.ToShortDateString();

            vh.Progress.Progress = Items[position].ProgressPercentage;

            vh.CheckBox.Checked = Items[position].Selected;
            vh.CheckBox.Enabled = !ReadOnly;

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
        }

        private void Container_Click(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(((View)sender).Tag.ToString());
            Items[position].Selected = !Items[position].Selected;
            NotifyItemChanged(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_syncitem, parent, false);
            return new RecyclerViewHolder(itemView);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public ProgressBar Progress { get; private set; }
            public AppCompatCheckBox CheckBox { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<View>(Resource.Id.item_row_label_container);
                Label = itemView.FindViewById<TextView>(Resource.Id.item_row_label);
                SubLabel = itemView.FindViewById<TextView>(Resource.Id.item_row_sub_label);
                Progress = itemView.FindViewById<ProgressBar>(Resource.Id.item_row_progress);
                CheckBox = itemView.FindViewById<AppCompatCheckBox>(Resource.Id.item_row_check);
            }
        }
    }
}