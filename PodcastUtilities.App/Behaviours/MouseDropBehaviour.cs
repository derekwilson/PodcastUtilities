#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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