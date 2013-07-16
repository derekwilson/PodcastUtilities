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