using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Adapters
{
    public class DownloadRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private DownloadViewModel ViewModel;
        private List<DownloadRecyclerItem> Items = new List<DownloadRecyclerItem>(20);
        private bool ReadOnly = false;

        public DownloadRecyclerItemAdapter(Context context, DownloadViewModel viewModel)
        {
            Context = context;
            ViewModel = viewModel;
        }

        public void SetItems(List<DownloadRecyclerItem> items)
        {
            Items = items;
        }

        public DownloadRecyclerItem GetItemById(Guid id)
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

        public int SetItemStatus(Guid id, Status status, string message)
        {
            var item = GetItemById(id);
            item.DownloadStatus = status;
            var position = Items.IndexOf(item);
            if (status == Status.Complete)
            {
                item.Selected = false;
                ViewModel.SelectionChanged(position);
            }
            return position;
        }

        public void SetReadOnly(bool readOnly)
        {
            this.ReadOnly = readOnly;
            // everything needs to be redrawn
            NotifyDataSetChanged();
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            // unsubscribe if it was subscribed before
            vh.Container.Click -= Container_Click;

            vh.Label.Text = Items[position].SyncItem.EpisodeTitle;
            vh.SubLabel.Text = Items[position].SyncItem.Published.ToShortDateString();
            var fmt = Context.GetString(Resource.String.download_sublabel_fmt);
            vh.SubLabel.Text = string.Format(fmt,
                Items[position].SyncItem.Published.ToShortDateString(),
                GetStatusText(Items[position].DownloadStatus)
            );

            vh.Progress.Progress = Items[position].ProgressPercentage;

            vh.CheckBox.Checked = Items[position].Selected;
            vh.CheckBox.Enabled = !ReadOnly && Items[position].DownloadStatus != Status.Complete;

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
        }

        private object GetStatusText(Status downloadStatus)
        {
            switch (downloadStatus)
            {
                case Status.OK:
                    return "";
                case Status.Complete:
                    return Context.GetString(Resource.String.download_status_complete);
                case Status.Error:
                    return Context.GetString(Resource.String.download_status_error);
                case Status.Warning:
                    return Context.GetString(Resource.String.download_status_warning);
                case Status.Information:
                    return "";
            }
            return "";
        }

        private void Container_Click(object sender, EventArgs e)
        {
            if (!ReadOnly)
            {
                int position = Convert.ToInt32(((View)sender).Tag.ToString());
                Items[position].Selected = !Items[position].Selected;
                NotifyItemChanged(position);
                ViewModel.SelectionChanged(position);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_downloaditem, parent, false);
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