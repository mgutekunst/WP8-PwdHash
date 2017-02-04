using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace PwdHash.Utils
{
    class OpenFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var element = sender as FrameworkElement;

            var flyout = FlyoutBase.GetAttachedFlyout(element);

            if (parameter is RightTappedRoutedEventArgs)
                ((RightTappedRoutedEventArgs)parameter).Handled = true;

            if(flyout != null)
            {
                flyout.ShowAt(element);
            }
            else
            {
                throw new NullReferenceException();
            }

            return null;
        }
    }
}
