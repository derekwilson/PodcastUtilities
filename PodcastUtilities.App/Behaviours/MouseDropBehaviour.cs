using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace PodcastUtilities.App.Behaviours
{
    public class MouseDropBehaviour
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDropBehaviour), new UIPropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var element = target as UIElement;

            if (element == null)
            {
                return;
            }

            element.Drop += (sender, eventArgs) =>
                                {
                                    var command = GetCommand(element);
                                    if (command.CanExecute(eventArgs.Data))
                                    {
                                        GetCommand(element).Execute(eventArgs.Data);
                                        eventArgs.Effects = GetDragDropEffect(eventArgs.AllowedEffects);
                                        eventArgs.Handled = true;
                                    }
                                };

            element.DragOver += (sender, eventArgs) =>
                                    {
                                        eventArgs.Effects = GetCommand(element).CanExecute(eventArgs.Data)
                                                                ? GetDragDropEffect(eventArgs.AllowedEffects)
                                                                : DragDropEffects.None;
                                        eventArgs.Handled = true;
                                    };
        }

        private static DragDropEffects GetDragDropEffect(DragDropEffects allowedEffects)
        {
            if ((allowedEffects & DragDropEffects.Link) == DragDropEffects.Link)
            {
                return DragDropEffects.Link;
            }

            return DragDropEffects.Copy;
        }
    }
}