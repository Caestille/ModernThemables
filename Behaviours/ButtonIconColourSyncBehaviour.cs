using Microsoft.Xaml.Behaviors;
using ModernThemables.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ModernThemables.Behaviours
{
	public class ButtonIconColourSyncBehaviour : Behavior<Control>
	{
		private Control parent;

		protected override void OnAttached()
		{
			int limit = 10;
			int current = 0;
			object currentObject = AssociatedObject;
			while (parent == null && current < limit && currentObject != null)
			{
				current++;
				if (currentObject is ExtendedButton || currentObject is ExtendedToggleButton)
				{
					parent = currentObject as Control;
				}
				currentObject = VisualTreeHelper.GetParent(currentObject as DependencyObject);
			}

			if (parent != null)
			{
				if (parent.IsLoaded && AssociatedObject.IsLoaded)
				{
					SetBindings();
				}
				else
				{
					if (!parent.IsLoaded)
					{
						parent.Loaded += ButtonIconColourSyncBehaviour_Loaded;
					}
					if (!AssociatedObject.IsLoaded)
					{
						AssociatedObject.Loaded += AssociatedObject_Loaded;
					}
				}
			}
		}

		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			if (parent.IsLoaded && AssociatedObject.IsLoaded)
			{
				SetBindings();
			}
		}

		private void ButtonIconColourSyncBehaviour_Loaded(object sender, RoutedEventArgs e)
		{
			parent.Loaded -= ButtonIconColourSyncBehaviour_Loaded;
			if (parent.IsLoaded && AssociatedObject.IsLoaded)
			{
				SetBindings();
			}
		}

		private void SetBindings()
		{
			if (parent is ExtendedButton extendedButton)
			{
				if (AssociatedObject.ReadLocalValue(Control.BackgroundProperty) == DependencyProperty.UnsetValue)
				{
					BindingOperations.SetBinding(AssociatedObject, Control.BackgroundProperty, new Binding { Path = new PropertyPath(ExtendedButton.CurrentBackgroundProperty), Source = extendedButton });
				}
				if (AssociatedObject.ReadLocalValue(Control.ForegroundProperty) == DependencyProperty.UnsetValue)
				{
					BindingOperations.SetBinding(AssociatedObject, Control.ForegroundProperty, new Binding { Path = new PropertyPath(ExtendedButton.CurrentForegroundProperty), Source = extendedButton });
				}
			}
			else if (parent is ExtendedToggleButton extendedToggleButton)
			{
				if (AssociatedObject.ReadLocalValue(Control.BackgroundProperty) == DependencyProperty.UnsetValue)
				{
					BindingOperations.SetBinding(AssociatedObject, Control.BackgroundProperty, new Binding { Path = new PropertyPath(ExtendedToggleButton.CurrentBackgroundProperty), Source = extendedToggleButton });
				}
				if (AssociatedObject.ReadLocalValue(Control.ForegroundProperty) == DependencyProperty.UnsetValue)
				{
					BindingOperations.SetBinding(AssociatedObject, Control.ForegroundProperty, new Binding { Path = new PropertyPath(ExtendedToggleButton.CurrentForegroundProperty), Source = extendedToggleButton });
				}
			}
		}
	}
}
