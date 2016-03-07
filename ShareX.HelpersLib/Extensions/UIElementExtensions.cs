using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HelpersLib
{
    public static class UIElementExtensions
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static void SetContextMenuOnMouseDown(this Button btn, RoutedEventArgs e)
        {
            ContextMenu cm = btn.ContextMenu;
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
            e.Handled = true;
        }
    }
}