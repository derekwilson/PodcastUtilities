using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PodcastUtilities.App.Behaviours
{
	public class MouseDoubleClickBehaviour
	{
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClickBehaviour), new UIPropertyMetadata(null, OnCommandChanged));

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDoubleClickBehaviour), new UIPropertyMetadata(null));

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static object GetCommandParameter(DependencyObject obj)
		{
			return obj.GetValue(CommandParameterProperty);
		}

		public static void SetCommandParameter(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty, value);
		}

		private static void OnCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
		{
			var selector = target as Selector;

			if (selector == null)
			{
				return;
			}

			selector.MouseDoubleClick += (sender, eventArgs) => GetCommand(selector).Execute(GetCommandParameter(selector));
		}
	}
}