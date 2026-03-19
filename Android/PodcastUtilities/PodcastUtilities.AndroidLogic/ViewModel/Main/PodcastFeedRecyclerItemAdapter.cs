using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;
using System.Collections.Generic;
using static Android.Icu.Text.Transliterator;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
{
    public class PodcastFeedRecyclerItemAdapter : RecyclerView.Adapter
    {
        private Context Context;
        private MainViewModel ViewModel;
        private ILogger Logger;
        private List<PodcastFeedRecyclerItem> Items = new List<PodcastFeedRecyclerItem>(20);

        private bool PopupBeingDisplayed = false;
        private int PopupAnchorPosition = -1;

        public PodcastFeedRecyclerItemAdapter(Context context, MainViewModel viewModel, ILogger logger)
        {
            Context = context;
            ViewModel = viewModel;
            Logger = logger;
        }

        public void SetItems(List<PodcastFeedRecyclerItem> items)
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
            vh.OverflowMenuTextButton.Click -= OverflowMenuTextButton_Click;

            vh.Label.Text = Items[position].PodcastFeed.Folder;
            if (Items[position].PodcastFeed.Feed != null)
            {
                // feeds are optional
                vh.SubLabel.Visibility = ViewStates.Visible;
                vh.SubLabel2.Visibility = ViewStates.Visible;
                vh.OverflowMenuTextButton.Visibility = ViewStates.Visible;
                vh.SubLabel.Text = ViewModel.GetFeedItemSubLabel(Items[position].PodcastFeed);
                vh.SubLabel2.Text = ViewModel.GetFeedItemSubLabel2(Items[position].PodcastFeed);
            }
            else
            {
                // nothing to display as there is no feed
                vh.SubLabel.Visibility = ViewStates.Gone;
                vh.SubLabel2.Visibility = ViewStates.Gone;
                vh.OverflowMenuTextButton.Visibility = ViewStates.Gone;
            }

            vh.Container.Tag = position.ToString();
            vh.Container.Click += Container_Click;
            vh.OverflowMenuTextButton.Tag = position.ToString();
            vh.OverflowMenuTextButton.Click += OverflowMenuTextButton_Click;
        }

        private void OverflowMenuTextButton_Click(object? sender, EventArgs e)
        {
            var senderView = sender as View;
            if (sender == null || senderView == null)
            {
                throw new InvalidOperationException("no sender");
            }
            int position = Convert.ToInt32(senderView.Tag?.ToString());
            if (PopupBeingDisplayed)
            {
                Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:OverflowMenuTextButton_Click - already displaying popup: {position}");
                return;
            }
            Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:OverflowMenuTextButton_Click - position: {position}");
            var popup = new PopupMenu(this.Context, senderView);
            popup.Inflate(Resource.Menu.menu_main_item);
            // we want a separator on the menu
            if (popup.Menu != null)
            {
                MenuCompat.SetGroupDividerEnabled(popup.Menu, true);
            }
            popup.MenuItemClick += OverflowMenu_MenuItemClick;
            popup.DismissEvent += OverflowMenu_DismissEvent;
            PopupBeingDisplayed = true;
            PopupAnchorPosition = position;
            popup.Show();
            Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:OverflowMenuTextButton_Click - position: {position} - complete");
        }

        private void OverflowMenu_DismissEvent(object? sender, PopupMenu.DismissEventArgs e)
        {
            Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:OverflowMenu_DismissEvent");
            PopupBeingDisplayed = false;
        }

        private void OverflowMenu_MenuItemClick(object? sender, PopupMenu.MenuItemClickEventArgs e)
        {
            var senderView = sender as View;
            Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:OverflowMenu_MenuItemClick Position: {PopupAnchorPosition} ID: {e.Item?.ItemId}");
            if (PopupAnchorPosition < 0)
            {
                return;
            }
            if (e.Item?.ItemId == Resource.Id.action_download_single)
            {
                ViewModel.FeedItemSelected(Items[PopupAnchorPosition].Id, Items[PopupAnchorPosition].PodcastFeed);
                return;
            }
            if (e.Item?.ItemId == Resource.Id.action_share_feed)
            {
                ViewModel.ShareFeed(Items[PopupAnchorPosition].Id, Items[PopupAnchorPosition].PodcastFeed);
                return;
            }
            if (e.Item?.ItemId == Resource.Id.action_share_feed_episode)
            {
                ViewModel.ShareFeedEpisode(Items[PopupAnchorPosition].Id, Items[PopupAnchorPosition].PodcastFeed);
                return;
            }
        }

        private void Container_Click(object? sender, EventArgs e)
        {
            var senderView = sender as View;
            if (sender == null || senderView == null)
            {
                throw new InvalidOperationException("no sender");
            }
            int position = Convert.ToInt32(senderView.Tag?.ToString());
            Logger.Debug(() => $"PodcastFeedRecyclerItemAdapter:Container_Click - position: {position}");
            ViewModel.FeedItemSelected(Items[PopupAnchorPosition].Id, Items[position].PodcastFeed);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View? itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.list_item_feeditem, parent, false);
            return new RecyclerViewHolder(itemView!);
        }

        class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public View Container { get; private set; }
            public TextView Label { get; private set; }
            public TextView SubLabel { get; private set; }
            public TextView SubLabel2 { get; private set; }
            public TextView OverflowMenuTextButton { get; private set; }

            public RecyclerViewHolder(View itemView) : base(itemView)
            {
                Container = ViewHelper.FindViewByIdOrThrow<View>("container", itemView, Resource.Id.item_row_label_container);
                Label = ViewHelper.FindViewByIdOrThrow<TextView>("label", itemView, Resource.Id.item_row_label);
                SubLabel = ViewHelper.FindViewByIdOrThrow<TextView>("sub label", itemView, Resource.Id.item_row_sub_label);
                SubLabel2 = ViewHelper.FindViewByIdOrThrow<TextView>("sub label2", itemView, Resource.Id.item_row_sub_label2);
                OverflowMenuTextButton = ViewHelper.FindViewByIdOrThrow<TextView>("item_row_options", itemView, Resource.Id.item_row_options);
            }
        }
    }
}