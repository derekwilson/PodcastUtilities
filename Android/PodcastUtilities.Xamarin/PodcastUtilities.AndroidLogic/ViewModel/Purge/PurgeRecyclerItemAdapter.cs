using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.ViewModel.Purge
{
    public class PurgeRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private PurgeViewModel ViewModel;

        private List<PurgeRecyclerItem> Items = new List<PurgeRecyclerItem>(20);
        private bool ReadOnly = false;

        public PurgeRecyclerItemAdapter(Context context, PurgeViewModel viewModel)
        {
            Context = context;
            ViewModel = viewModel;
        }

        public void SetItems(List<PurgeRecyclerItem> items)
        {
            Items = items;
        }

        public void SetReadOnly(bool readOnly)
        {
            ReadOnly = readOnly;
            // everything needs to be redrawn
            NotifyDataSetChanged();
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;
            // unsubscribe if it was subscribed before
            vh.Container.Click -= Container_Click;

            vh.Label.Text = ViewModel.GetLabelForList(Items[position].FileOrDirectoryItem);
            vh.CheckBox.Checked = Items[position].Selected;
            vh.CheckBox.Enabled = !ReadOnly;

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_purgeitem, parent, false);
            return new RecyclerViewHolder(itemView);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public AppCompatCheckBox CheckBox { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<View>(Resource.Id.purge_row_label_container);
                Label = itemView.FindViewById<TextView>(Resource.Id.purge_row_label);
                CheckBox = itemView.FindViewById<AppCompatCheckBox>(Resource.Id.purge_row_check);
            }
        }
    }
}