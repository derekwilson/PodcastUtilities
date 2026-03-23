using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Main;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidLogic.ViewModel.Share
{
    public class ShareEpisodeRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private ShareEpisodeViewModel ViewModel;
        private ILogger Logger;

        private List<ShareEpisodeRecyclerItem> Items = new List<ShareEpisodeRecyclerItem>(20);

        public ShareEpisodeRecyclerItemAdapter(Context context, ShareEpisodeViewModel viewModel, ILogger logger)
        {
            Context = context;
            ViewModel = viewModel;
            Logger = logger;
        }

        public void SetItems(List<ShareEpisodeRecyclerItem> items)
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
            vh.Label.Text = ViewModel.GetEpisodeItemLabel(Items[position].Episode);
            vh.SubLabel.Text = ViewModel.GetEpisodeItemSubLabel(Items[position].Episode);

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
        }

        private void Container_Click(object? sender, EventArgs e)
        {
            var senderView = sender as View;
            if (sender == null || senderView == null)
            {
                throw new InvalidOperationException("no sender");
            }
            int position = Convert.ToInt32(senderView.Tag?.ToString());
            Logger.Debug(() => $"ShareEpisodeRecyclerItemAdapter:Container_Click - position: {position}");
            ViewModel.EpisodeItemSelected(Items[position].Episode);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View? itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.list_item_shareepisodeitem, parent, false);
            return new RecyclerViewHolder(itemView!);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = ViewHelper.FindViewByIdOrThrow<View>("container", itemView, Resource.Id.item_row_label_container);
                Label = ViewHelper.FindViewByIdOrThrow<TextView>("label", itemView, Resource.Id.item_row_label);
                SubLabel = ViewHelper.FindViewByIdOrThrow<TextView>("sub label", itemView, Resource.Id.item_row_sub_label);
            }
        }
    }
}
