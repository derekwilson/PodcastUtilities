using Android.Content;
using Android.Util;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using System.Diagnostics.CodeAnalysis;

namespace PodcastUtilities.AndroidLogic.CustomViews
{
    public class EmptyRecyclerView : RecyclerView
    {
        private View? emptyView = null;
        private AdapterDataObserver observer;

        public EmptyRecyclerView(Context context) : base(context)
        {
            Init();
        }

        public EmptyRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public EmptyRecyclerView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        // a bit shit but aparently the code analyser cannot work it out on its own
        // see https://stackoverflow.com/questions/72347014/understanding-why-im-still-getting-cs8618-error-in-constructor
        [MemberNotNull(nameof(observer))]
        private void Init()
        {
            observer = new AdapterObserver(this);
        }

        private void CheckIfEmpty()
        {
            if (emptyView != null)
            {
                // if there is no adapter then its empty by definition
                bool emptyViewVisible = GetAdapter() == null || GetAdapter()?.ItemCount == 0;
                // we either show the recycler or the empty message but not both
                emptyView.Visibility = emptyViewVisible ? ViewStates.Visible : ViewStates.Gone;
                this.Visibility = emptyViewVisible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        // TODO - set this via an XML attribute
        public void SetEmptyView(View view)
        {
            this.emptyView = view;
            CheckIfEmpty();
        }

        public override void SetAdapter(Adapter? adapter)
        {
            var oldAdapter = GetAdapter();
            oldAdapter?.UnregisterAdapterDataObserver(observer);
            base.SetAdapter(adapter);
            adapter?.RegisterAdapterDataObserver(observer);
            CheckIfEmpty();
        }

        private class AdapterObserver : AdapterDataObserver
        {
            private EmptyRecyclerView RecyclerView;

            public AdapterObserver(EmptyRecyclerView rv)
            {
                RecyclerView = rv;
            }
            public override void OnChanged()
            {
                base.OnChanged();
                RecyclerView.CheckIfEmpty();
            }
            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                base.OnItemRangeInserted(positionStart, itemCount);
                RecyclerView.CheckIfEmpty();
            }
            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                RecyclerView.CheckIfEmpty();
            }
        }
    }
}