using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ModernThemables.Controls
{
	public class BlurHost : ContentControl
	{
		public Visual BlurBackground
		{
			get => (Visual)GetValue(BlurBackgroundProperty);
			set => SetValue(BlurBackgroundProperty, value);
		}

		public static readonly DependencyProperty BlurBackgroundProperty =
			DependencyProperty.Register(
			  "BlurBackground",
			  typeof(Visual),
			  typeof(BlurHost),
			  new PropertyMetadata(default(Visual), OnBlurBackgroundChanged));

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

		public BlurEffect BlurEffect
		{
			get => (BlurEffect)GetValue(BlurEffectProperty);
			set => SetValue(BlurEffectProperty, value);
		}

		public static readonly DependencyProperty BlurEffectProperty =
			DependencyProperty.Register(
			  "BlurEffect",
			  typeof(BlurEffect),
			  typeof(BlurHost),
			  new PropertyMetadata(
				new BlurEffect()
				{
					Radius = 30,
					KernelType = KernelType.Gaussian,
					RenderingBias = RenderingBias.Performance
				}));

		private Border PART_BlurDecorator { get; set; }
		private VisualBrush BlurDecoratorBrush { get; set; }

		static BlurHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurHost), new FrameworkPropertyMetadata(typeof(BlurHost)));
		}

		public BlurHost()
		{
			Loaded += OnLoaded;

			this.BlurDecoratorBrush = new VisualBrush()
			{
				ViewboxUnits = BrushMappingMode.Absolute,
				Opacity = this.BlurOpacity
			};
		}

		public void DrawBlurredElementBackground()
		{
			if (!BlurEnabled)
				return;

			if (!TryFindVisualRootContainer(this, out FrameworkElement rootContainer))
			{
				return;
			}

			this.BlurDecoratorBrush.Opacity = this.BlurOpacity;

			// Get the section of the image where the BlurHost element is located
			Rect elementBounds = TransformToVisual(rootContainer)
			  .TransformBounds(new Rect(this.RenderSize));

			elementBounds.Location = new Point(elementBounds.Left - OffsetX, elementBounds.Top - OffsetY);

			// Use the section bounds to actually "cut out" the image tile 
			this.BlurDecoratorBrush.Viewbox = elementBounds;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (TryFindVisualRootContainer(this, out FrameworkElement rootContainer))
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
			this.PART_BlurDecorator = GetTemplateChild("PART_BlurDecorator") as Border;
			this.PART_BlurDecorator.Effect = this.BlurEffect;
			this.PART_BlurDecorator.Background = this.BlurDecoratorBrush;
		}

		private static void OnBlurBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var this_ = d as BlurHost;

			if (!this_.BlurEnabled)
				return;

			Application.Current.Dispatcher.InvokeAsync(() => {
				this_.BlurDecoratorBrush.Visual = e.NewValue as Visual;
				this_.DrawBlurredElementBackground();
			});
		}

		private static void Draw(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var this_ = d as BlurHost;
			this_.DrawBlurredElementBackground();
		}

		private void OnRootContainerElementResized(object sender, SizeChangedEventArgs e)
		{
			if (!BlurEnabled)
				return;

			if (!PreventResample)
				DrawBlurredElementBackground();
		}

		private bool TryFindVisualRootContainer(DependencyObject child, out FrameworkElement rootContainerElement)
		{
			rootContainerElement = null;
			DependencyObject parent = VisualTreeHelper.GetParent(child);
			if (parent == null)
			{
				return false;
			}

			if (parent is not Window visualRoot)
			{
				return TryFindVisualRootContainer(parent, out rootContainerElement);
			}

			rootContainerElement = visualRoot.Content as FrameworkElement;
			return true;
		}
	}
}
