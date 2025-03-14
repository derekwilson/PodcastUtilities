﻿using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class SelectableStringArrayAdapter : BaseAdapter
    {
        private LayoutInflater inflater;
        private List<SelectableString> itemList;

        public SelectableStringArrayAdapter(Context context, List<SelectableString> items)
        {
            inflater = LayoutInflater.From(context);
            itemList = items;
        }

        public override int Count => itemList.Count;

        public override Object GetItem(int position) => itemList[position];

        public override long GetItemId(int position) => itemList[position].Id;

        public override bool HasStableIds => true;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var itemView = convertView;
            if (itemView == null)
            {
                itemView = inflater.Inflate(Resource.Layout.list_item_selectable_string, parent, false);
            }
            var holder = new ViewHolder(itemView);
            holder.Bind(itemList[position]);
            return itemView;
        }

        class ViewHolder
        {
            private TextView txtLabel;
            private ImageView btnCheck;

            public ViewHolder(View view)
            {
                txtLabel = view.FindViewById<TextView>(Resource.Id.selectable_string_text);
                btnCheck = view.FindViewById<ImageView>(Resource.Id.selectable_string_check);
            }

            internal void Bind(SelectableString item)
            {
                txtLabel.Text = item.Name;
                btnCheck.Visibility = item.Selected ? ViewStates.Visible : ViewStates.Gone;
            }
        }
    }
}