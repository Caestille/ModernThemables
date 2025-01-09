namespace ModernThemables.Controls
{
    using CoreUtilities.HelperClasses.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourPicker : UserControl
    {
        private bool blockHtml;
        private bool blockColour;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public Action<Color>? colourChangedCallback;

        public Color Colour
        {
            get => (Color)this.GetValue(ColourProperty);
            set => this.SetValue(ColourProperty, value);
        }
        public static readonly DependencyProperty ColourProperty = DependencyProperty.Register(
            nameof(Colour),
            typeof(Color),
            typeof(ColourPicker),
            new UIPropertyMetadata(Colors.Black, OnColourSet));

        private static void OnColourSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ColourPicker self_)
            {
                if (self_.blockColour) return;
                self_.blockColour = true;
                self_.Html = self_.Colour.GetHexString();
                self_.blockColour = false;
            }
        }

        public string Html
        {
            get => (string)this.GetValue(HtmlProperty);
            set => this.SetValue(HtmlProperty, value);
        }
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register(
            nameof(Html),
            typeof(string),
            typeof(ColourPicker),
            new UIPropertyMetadata(string.Empty, OnHtmlSet));

        private static void OnHtmlSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ColourPicker self_)
            {
                try
                {
                    if (self_.blockHtml) return;
                    self_.blockHtml = true;
                    var colour = (Color)ColorConverter.ConvertFromString(self_.Html);
                    self_.Colour = colour;
                    self_.blockHtml = false;
                }
                catch (FormatException)
                {
                    self_.blockHtml = false;
                }
            }
        }

        private bool isMouseDown;
        private static readonly List<KeyValuePair<Color, double>> horizontalColourStops = new List<KeyValuePair<Color, double>>()
        {
            new KeyValuePair<Color, double>(Colors.Red, 0),
            new KeyValuePair<Color, double>(Colors.Magenta, 1d/6),
            new KeyValuePair<Color, double>(Colors.Blue, 2d/6),
            new KeyValuePair<Color, double>(Colors.Cyan, 3d/6),
            new KeyValuePair<Color, double>(Colors.Lime, 4d/6),
            new KeyValuePair<Color, double>(Colors.Yellow, 5d/6),
            new KeyValuePair<Color, double>(Colors.Red, 1)
        };
        private static readonly List<KeyValuePair<Color, double>> verticalColourStops = new List<KeyValuePair<Color, double>>()
        {
            new KeyValuePair<Color, double>(Colors.Black, 0),
            new KeyValuePair<Color, double>(Colors.Transparent, 0.48),
            new KeyValuePair<Color, double>(Colors.Transparent, 0.52),
            new KeyValuePair<Color, double>(Colors.White, 1)
        };

        public ColourPicker()
        {
            this.InitializeComponent();
            Loaded += this.ColourPicker_Loaded;
        }

        private async void ColourPicker_Loaded(object sender, RoutedEventArgs e)
        {
            var point = await this.GetPointAtColour(this.Colour);
            if (point.X != -1 && point.Y != -1)
            {
                this.AdjustSelectedColourCursor((int)point.X, (int)point.Y);
            }
            Loaded -= this.ColourPicker_Loaded;
        }

        public Color? GetColorAt(int x, int y)
        {
            x = (int)Math.Min(Math.Max(x, 1), this.ColourSelectionBorder.ActualWidth);
            y = (int)Math.Min(Math.Max(y, 1), this.ColourSelectionBorder.ActualHeight);

            var horizFrac = x / this.ColourSelectionBorder.ActualWidth;
            var vertFrac = (float)(((y / this.ColourSelectionBorder.ActualHeight) - 0.5) * 2);

            var leftColour = horizontalColourStops.Where(x => x.Value <= horizFrac)
                .DefaultIfEmpty(new KeyValuePair<Color, double>(Colors.Red, 0)).Last();
            var rightColour = horizontalColourStops.Where(x => x.Value >= horizFrac)
                .DefaultIfEmpty(new KeyValuePair<Color, double>(Colors.Red, 1)).First();

            var outputColour = leftColour.Key.Combine(rightColour.Key, (horizFrac - leftColour.Value) / (rightColour.Value - leftColour.Value));

            var output = outputColour.ChangeColourBrightness(-vertFrac);

            return output;
        }

        public async Task<Point> GetPointAtColour(Color colour)
        {
            return await Task.Run(() =>
            {
                this.cts.Cancel();
                this.cts = new CancellationTokenSource();
                var iter = 200;
                var threshold = 10;

                int width = (int)this.ColourSelectionBorder.ActualWidth;
                int height = (int)this.ColourSelectionBorder.ActualHeight;
                var xStep = width / iter;
                var yStep = height / iter;
                int xPos = 0;
                int yPos = 0;
                for (int i = 0; i < iter; i++)
                {
                    var doBreak = false;
                    for (int j = 0; j < iter; j++)
                    {
                        if (this.cts.IsCancellationRequested)
                        {
                            return new Point(-1, -1);
                        }
                        xPos = xStep * i;
                        yPos = yStep * j;
                        var sampled = this.GetColorAt(xPos, yPos);
                        if (sampled.HasValue && sampled.Value.ColoursAreClose(colour, threshold))
                        {
                            doBreak = true;
                            break;
                        }
                    }
                    if (doBreak)
                    {
                        break;
                    }
                }

                return new Point(xPos, yPos);
            });
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = true;
        }

        private void Border_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = false;
        }

        private async void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.isMouseDown) return;

            var point = await this.GetPointAtColour(this.Colour);
            if (point.X != -1 && point.Y != -1)
            {
                this.AdjustSelectedColourCursor((int)point.X, (int)point.Y);
            }
            if (this.colourChangedCallback != null) this.colourChangedCallback(this.Colour);
        }

        private void ColourSelectionBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var borderCursor = e.GetPosition(this.ColourSelectionBorder);
            borderCursor = new Point(
                Math.Max(Math.Min(borderCursor.X, this.ColourSelectionBorder.Width), 0),
                Math.Max(Math.Min(borderCursor.Y, this.ColourSelectionBorder.Height), 0));
            this.AdjustSelectedColourCursor((int)borderCursor.X, (int)borderCursor.Y);
            var c = this.GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
            this.Colour = c ?? this.Colour;
            if (this.colourChangedCallback != null) this.colourChangedCallback(this.Colour);
        }

        private void ColourSelectionBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (isMouseDown) isMouseDown = false;
        }

        private void AdjustSelectedColourCursor(int x, int y)
        {
            this.SelectedColourBorder.Margin = new Thickness(Math.Max(x - 5, -5), Math.Max(y - 5, -5), 0, 0);
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isMouseDown) return;

            var borderCursor = e.GetPosition(this.ColourSelectionBorder);
            borderCursor = new Point(
                Math.Max(Math.Min(borderCursor.X, this.ColourSelectionBorder.Width), 0),
                Math.Max(Math.Min(borderCursor.Y, this.ColourSelectionBorder.Height), 0));
            this.AdjustSelectedColourCursor((int)borderCursor.X, (int)borderCursor.Y);

            var cursor = this.PointToScreen(e.GetPosition(this));
            var c = this.GetColorAt((int)borderCursor.X, (int)borderCursor.Y);
            this.Colour = c ?? this.Colour;
            if (this.colourChangedCallback != null) this.colourChangedCallback(this.Colour);
        }
    }
}
