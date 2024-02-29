using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ModernThemables.Controls
{
	public class BlurHost : ContentControl
	{
		public FrameworkElement BlurBackground
		{
			get => (FrameworkElement)GetValue(BlurBackgroundProperty);
			set => SetValue(BlurBackgroundProperty, value);
		}

		public static readonly DependencyProperty BlurBackgroundProperty =
			DependencyProperty.Register(
			  "BlurBackground",
			  typeof(FrameworkElement),
			  typeof(BlurHost),
			  new PropertyMetadata(default(FrameworkElement), OnBlurBackgroundChanged));

		public double OffsetX
		{
			get => (double)GetValue(OffsetXProperty);
			set => SetValue(OffsetXProperty, value);
		}

		public static readonly DependencyProperty OffsetXProperty =
			DependencyProperty.Register(
			  "OffsetX",
			  typeof(double),
			  typeof(BlurHost),
			  new PropertyMetadata(0d));

		public double OffsetY
		{
			get => (double)GetValue(OffsetYProperty);
			set => SetValue(OffsetYProperty, value);
		}

		public static readonly DependencyProperty OffsetYProperty =
			DependencyProperty.Register(
			  "OffsetY",
			  typeof(double),
			  typeof(BlurHost),
			  new PropertyMetadata(0d));

		public double BlurRadius
		{
			get => (double)GetValue(BlurRadiusProperty);
			set => SetValue(BlurRadiusProperty, value);
		}

		public static readonly DependencyProperty BlurRadiusProperty =
			DependencyProperty.Register(
			  "BlurRadius",
			  typeof(double),
			  typeof(BlurHost),
			  new UIPropertyMetadata(30d, OnBlurBackgroundChanged));

		public double BlurOpacity
		{
			get => (double)GetValue(BlurOpacityProperty);
			set => SetValue(BlurOpacityProperty, value);
		}

		public static readonly DependencyProperty BlurOpacityProperty =
			DependencyProperty.Register(
			  "BlurOpacity",
			  typeof(double),
			  typeof(BlurHost),
			  new PropertyMetadata(1.0));

		public bool PreventResample
		{
			get => (bool)GetValue(PreventResampleProperty);
			set => SetValue(PreventResampleProperty, value);
		}

		public static readonly DependencyProperty PreventResampleProperty =
			DependencyProperty.Register(
			  "PreventResample",
			  typeof(bool),
			  typeof(BlurHost),
			  new PropertyMetadata(false));

		public bool BlurEnabled
		{
			get => (bool)GetValue(BlurEnabledProperty);
			set => SetValue(BlurEnabledProperty, value);
		}

		public static readonly DependencyProperty BlurEnabledProperty =
			DependencyProperty.Register(
			  "BlurEnabled",
			  typeof(bool),
			  typeof(BlurHost),
			  new PropertyMetadata(true));

		public object RedrawTrigger
		{
			get => GetValue(RedrawTriggerProperty);
			set => SetValue(RedrawTriggerProperty, value);
		}

		public static readonly DependencyProperty RedrawTriggerProperty =
			DependencyProperty.Register(
			  "RedrawTrigger",
			  typeof(object),
			  typeof(BlurHost),
			  new PropertyMetadata(new object(), Draw));

		private Border? PART_BlurDecorator { get; set; }
		private VisualBrush BlurDecoratorBrush { get; set; }

		static BlurHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurHost), new FrameworkPropertyMetadata(typeof(BlurHost)));
		}

		public BlurHost()
		{
			Loaded += OnLoaded;

			BlurDecoratorBrush = new VisualBrush()
			{
				ViewboxUnits = BrushMappingMode.Absolute,
				Opacity = BlurOpacity
			};
		}

		public void DrawBlurredElementBackground()
		{
			if (!BlurEnabled)
				return;

			if (!TryFindVisualRootContainer(this, out var blurHostContainer) 
				|| !TryFindVisualRootContainer(BlurBackground, out var backgroundContainer))
			{
				return;
			}

			Rect blurHostBounds = TransformToVisual(blurHostContainer)
				.TransformBounds(new Rect(RenderSize));
            Rect backgroundBounds = BlurBackground.TransformToVisual(backgroundContainer)
				.TransformBounds(new Rect(BlurBackground.RenderSize));

			var transform = backgroundContainer?.TransformToVisual(blurHostContainer).Transform(new Point(0, 0)) ?? new Point();

            var viewBox = new Rect(Math.Max(blurHostBounds.Left - transform.X, 0) + OffsetX, Math.Max(blurHostBounds.Top - transform.Y, 0) + OffsetY, blurHostBounds.Width, blurHostBounds.Height);

			BlurDecoratorBrush.Viewbox = viewBox;

            BlurDecoratorBrush.Opacity = BlurOpacity;
        }

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (TryFindVisualRootContainer(this, out var rootContainer) && rootContainer != null)
			{
				rootContainer.SizeChanged += OnRootContainerElementResized;
			}

			DrawBlurredElementBackground();
		}

		public override void OnApplyTemplate()
		{
			if (!BlurEnabled)
				return;

			base.OnApplyTemplate();
			if (GetTemplateChild("PART_BlurDecorator") is Border border)
			{
                PART_BlurDecorator = border;
            }

			if (PART_BlurDecorator != null)
			{
				PART_BlurDecorator.Effect = new BlurEffect()
				{
					Radius = BlurRadius,
					KernelType = KernelType.Gaussian,
					RenderingBias = RenderingBias.Performance
				};
				PART_BlurDecorator.Background = BlurDecoratorBrush;
			}
			else
			{
				throw new InvalidOperationException("Theme does not contain required UI elements");
			}

		}

		private static void OnBlurBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BlurHost this_ && this_.BlurEnabled)
			{
				Application.Current.Dispatcher.InvokeAsync(() =>
				{
					this_.BlurDecoratorBrush.Visual = e.NewValue as Visual;
					if (this_.PART_BlurDecorator != null)
					{
						this_.PART_BlurDecorator.Effect = new BlurEffect()
						{
							Radius = this_.BlurRadius,
							KernelType = KernelType.Gaussian,
							RenderingBias = RenderingBias.Performance
						};
					}
					this_.DrawBlurredElementBackground();
				});
			}
		}

		private static void Draw(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BlurHost this_)
			{
				this_.DrawBlurredElementBackground();
			}
		}

		private void OnRootContainerElementResized(object sender, SizeChangedEventArgs e)
		{
			if (!BlurEnabled)
				return;

			if (!PreventResample)
				DrawBlurredElementBackground();
		}

		private bool TryFindVisualRootContainer(DependencyObject child, out FrameworkElement? rootContainerElement)
		{
			if (child == null)
			{
				rootContainerElement = null;
				return false;
			}

			if ((child is ContentControl && child is not BlurHost) || child is Window)
			{
				rootContainerElement = child as FrameworkElement;
				return true;
			}

			rootContainerElement = null;
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			if (parent == null)
			{
				return false;
			}

			if (parent is not Window && parent is not ContentControl)
			{
				return TryFindVisualRootContainer(parent, out rootContainerElement);
			}

			rootContainerElement = parent as FrameworkElement;

			return true;
		}
	}
}
