using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Adapters
{
    public class PodcastFeedRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private List<PodcastFeedRecyclerItem> Items = new List<PodcastFeedRecyclerItem>(20);

        public PodcastFeedRecyclerItemAdapter(Context context)
        {
            Context = context;
        }

        public void SetItems(List<PodcastFeedRecyclerItem> items)
        {
            Items = items;
        }

        public override int ItemCount => Items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            RecyclerViewHolder vh = holder as RecyclerViewHolder;

            vh.Label.Text = Items[position].PodcastFeed.Folder;
            //vh.SubLabel.Text = Items[position].PodcastFeed.Feed.Address.ToString();
            var fmt = Context.GetString(Resource.String.feed_sublabel_fmt);
            vh.SubLabel.Text = string.Format(fmt,
                GetFormattedInt(Items[position].PodcastFeed.Feed.MaximumDaysOld.Value),
                GetFormattedInt(Items[position].PodcastFeed.Feed.DeleteDownloadsDaysOld.Value)
            );

            vh.Container.Tag = position.ToString();
        }

        private string GetFormattedInt(int value)
        {
            if (value == int.MaxValue)
            {
                return Context.GetString(Resource.String.max_int);
            }
            return value.ToString();
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

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = itemView.FindViewById<View>(Resource.Id.item_row_label_container);
                Label = itemView.FindViewById<TextView>(Resource.Id.item_row_label);
                SubLabel = itemView.FindViewById<TextView>(Resource.Id.item_row_sub_label);
            }
        }
    }
}