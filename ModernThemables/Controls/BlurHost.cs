namespace ModernThemables.Controls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

public class BlurHost : ContentControl
{
    public FrameworkElement BlurBackground
    {
        get => (FrameworkElement)this.GetValue(BlurBackgroundProperty);
        set => this.SetValue(BlurBackgroundProperty, value);
    }

    public static readonly DependencyProperty BlurBackgroundProperty =
        DependencyProperty.Register(
          nameof(BlurBackground),
          typeof(FrameworkElement),
          typeof(BlurHost),
          new PropertyMetadata(default(FrameworkElement), OnBlurBackgroundChanged));

    public double OffsetX
    {
        get => (double)this.GetValue(OffsetXProperty);
        set => this.SetValue(OffsetXProperty, value);
    }

    public static readonly DependencyProperty OffsetXProperty =
        DependencyProperty.Register(
          nameof(OffsetX),
          typeof(double),
          typeof(BlurHost),
          new PropertyMetadata(0d));

    public double OffsetY
    {
        get => (double)this.GetValue(OffsetYProperty);
        set => this.SetValue(OffsetYProperty, value);
    }

    public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register(
          nameof(OffsetY),
          typeof(double),
          typeof(BlurHost),
          new PropertyMetadata(0d));

    public double BlurRadius
    {
        get => (double)this.GetValue(BlurRadiusProperty);
        set => this.SetValue(BlurRadiusProperty, value);
    }

    public static readonly DependencyProperty BlurRadiusProperty =
        DependencyProperty.Register(
          nameof(BlurRadius),
          typeof(double),
          typeof(BlurHost),
          new UIPropertyMetadata(30d, OnBlurBackgroundChanged));

    public double BlurOpacity
    {
        get => (double)this.GetValue(BlurOpacityProperty);
        set => this.SetValue(BlurOpacityProperty, value);
    }

    public static readonly DependencyProperty BlurOpacityProperty =
        DependencyProperty.Register(
          nameof(BlurOpacity),
          typeof(double),
          typeof(BlurHost),
          new PropertyMetadata(1.0));

    public bool PreventResample
    {
        get => (bool)this.GetValue(PreventResampleProperty);
        set => this.SetValue(PreventResampleProperty, value);
    }

    public static readonly DependencyProperty PreventResampleProperty =
        DependencyProperty.Register(
          nameof(PreventResample),
          typeof(bool),
          typeof(BlurHost),
          new PropertyMetadata(false));

    public bool BlurEnabled
    {
        get => (bool)this.GetValue(BlurEnabledProperty);
        set => this.SetValue(BlurEnabledProperty, value);
    }

    public static readonly DependencyProperty BlurEnabledProperty =
        DependencyProperty.Register(
          nameof(BlurEnabled),
          typeof(bool),
          typeof(BlurHost),
          new PropertyMetadata(true));

    public object RedrawTrigger
    {
        get => this.GetValue(RedrawTriggerProperty);
        set => this.SetValue(RedrawTriggerProperty, value);
    }

    public static readonly DependencyProperty RedrawTriggerProperty =
        DependencyProperty.Register(
          nameof(RedrawTrigger),
          typeof(object),
          typeof(BlurHost),
          new PropertyMetadata(new object(), Draw));

    private Border? PART_BlurDecorator { get; set; }
    private VisualBrush BlurDecoratorBrush { get; set; }

    public BlurHost()
    {
        Loaded += this.OnLoaded;

        this.BlurDecoratorBrush = new VisualBrush()
        {
            ViewboxUnits = BrushMappingMode.Absolute,
            Opacity = this.BlurOpacity
        };
    }

    public void DrawBlurredElementBackground()
    {
        if (!this.BlurEnabled)
        {
            return;
        }

        if (!this.TryFindVisualRootContainer(this, out var blurHostContainer)
            || !this.TryFindVisualRootContainer(this.BlurBackground, out var backgroundContainer))
        {
            return;
        }

        Rect blurHostBounds = this.TransformToVisual(blurHostContainer)
            .TransformBounds(new Rect(this.RenderSize));
        Rect backgroundBounds = this.BlurBackground.TransformToVisual(backgroundContainer)
            .TransformBounds(new Rect(this.BlurBackground.RenderSize));

        var transform = backgroundContainer?.TransformToVisual(blurHostContainer).Transform(new Point(0, 0)) ?? new Point();

        var viewBox = new Rect(Math.Max(blurHostBounds.Left - transform.X, 0) + this.OffsetX, Math.Max(blurHostBounds.Top - transform.Y, 0) + this.OffsetY, blurHostBounds.Width, blurHostBounds.Height);

        this.BlurDecoratorBrush.Viewbox = viewBox;

        this.BlurDecoratorBrush.Opacity = this.BlurOpacity;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (this.TryFindVisualRootContainer(this, out var rootContainer) && rootContainer != null)
        {
            rootContainer.SizeChanged += this.OnRootContainerElementResized;
        }

        this.DrawBlurredElementBackground();
    }

    public override void OnApplyTemplate()
    {
        if (!this.BlurEnabled)
        {
            return;
        }

        base.OnApplyTemplate();
        if (this.GetTemplateChild("PART_BlurDecorator") is Border border)
        {
            this.PART_BlurDecorator = border;
        }

        if (this.PART_BlurDecorator != null)
        {
            this.PART_BlurDecorator.Effect = new BlurEffect()
            {
                Radius = this.BlurRadius,
                KernelType = KernelType.Gaussian,
                RenderingBias = RenderingBias.Performance
            };
            this.PART_BlurDecorator.Background = this.BlurDecoratorBrush;
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
        if (!this.BlurEnabled)
        {
            return;
        }

        if (!this.PreventResample)
        {
            this.DrawBlurredElementBackground();
        }
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
            return this.TryFindVisualRootContainer(parent, out rootContainerElement);
        }

        rootContainerElement = parent as FrameworkElement;

        return true;
    }
}
