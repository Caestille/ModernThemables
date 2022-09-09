using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace ModernThemables.Controls
{
	public partial class CircularProgressBar
	{
		private double indeterminateAngle1;
		private double indeterminateAngle2;
		private bool isIndeterminate;
		private const int lowSpeed = 15;
		private const int highSpeed = 24;
		private int indeterminateAngle1Speed = 15;
		private int indeterminateAngle2Speed = 24;
		private int sameSpeedCount;
		private bool setHigh;
		private bool breakAll = false;

		public CircularProgressBar()
		{
			InitializeComponent();
			var angle = (Percentage * 360) / 100;
			RenderArc(0, angle);
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			breakAll = true;
		}

		public double Radius
		{
			get => Math.Max(ActualWidth * 0.5 - StrokeThickness, 1);
		}

		public Brush SegmentColor
		{
			get => (Brush)GetValue(SegmentColorProperty);
			set => SetValue(SegmentColorProperty, value);
		}

		public double StrokeThickness
		{
			get => (double)GetValue(StrokeThicknessProperty);
			set => SetValue(StrokeThicknessProperty, value);
		}

		public double Percentage
		{
			get => (double)GetValue(PercentageProperty);
			set => SetValue(PercentageProperty, value);
		}

		public bool IsIndeterminate
		{
			get => (bool)GetValue(IsIndeterminateProperty);
			set => SetValue(IsIndeterminateProperty, value);
		}

		public static readonly DependencyProperty PercentageProperty =
			DependencyProperty.Register("Percentage", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(65d, OnPercentageChanged));

		public static readonly DependencyProperty StrokeThicknessProperty =
			DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircularProgressBar), new PropertyMetadata(5d, OnThicknessChanged));

		public static readonly DependencyProperty SegmentColorProperty =
			DependencyProperty.Register("SegmentColor", typeof(Brush), typeof(CircularProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnColorChanged));

		public static readonly DependencyProperty IsIndeterminateProperty =
			DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(CircularProgressBar), new PropertyMetadata(false, OnSetIndeterminate));

		private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			CircularProgressBar? this_ = sender as CircularProgressBar;
			this_?.set_Color((SolidColorBrush)args.NewValue);
		}

		private static void OnThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			CircularProgressBar? this_ = sender as CircularProgressBar;
			this_?.set_tick((double)args.NewValue);
		}

		private static void OnPercentageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var this_ = sender as CircularProgressBar;

			if (this_ == null || this_.IsIndeterminate)
				return;

			this_.PathRoot.Visibility = Visibility.Visible;

			if (this_?.Percentage > 100)
				this_.Percentage = 100;

			if (this_ != null)
			{
				var angle = (this_.Percentage * 360) / 100;
				this_?.RenderArc(0, angle);
			}
		}

		private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			CircularProgressBar? this_ = sender as CircularProgressBar;

			if (this_.IsIndeterminate)
				this_?.RenderArc(this_.indeterminateAngle1, this_.indeterminateAngle2);
			else
				this_?.RenderArc(0, (this_.Percentage * 360) / 100);
		}

		private static void OnSetIndeterminate(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var this_ = sender as CircularProgressBar;
			this_.isIndeterminate = this_.IsIndeterminate;
			if (this_.IsIndeterminate)
			{
				this_.PathRoot.Visibility = Visibility.Visible;
				this_.PathRoot.StrokeEndLineCap = PenLineCap.Round;
				this_.PathRoot.StrokeStartLineCap = PenLineCap.Round;
				this_.indeterminateAngle1 = 0;
				this_.indeterminateAngle2 = 45;

				Thread thread = new Thread(new ThreadStart(() =>
				{
					while (this_.isIndeterminate && !this_.breakAll)
					{
						var nextAngle2 = (this_.indeterminateAngle2 + this_.indeterminateAngle2Speed) % 360;
						var nextAngle1 = (this_.indeterminateAngle1 + this_.indeterminateAngle1Speed) % 360;
						var diff = 360 - Math.Abs((nextAngle2 < nextAngle1 ? nextAngle1 - (nextAngle2 + 360) : nextAngle1 - nextAngle2));
						var allowedDiff = 50;
						if (diff < allowedDiff)
						{
							this_.indeterminateAngle2Speed = lowSpeed;
							this_.setHigh = false;
						}

						if (diff > (360 - allowedDiff))
						{
							this_.indeterminateAngle1Speed = lowSpeed;
							this_.setHigh = true;
						}

						if (this_.indeterminateAngle1Speed == this_.indeterminateAngle2Speed)
						{
							this_.sameSpeedCount++;
						}

						if (this_.sameSpeedCount == 50)
						{
							this_.sameSpeedCount = 0;
							if (this_.setHigh)
								this_.indeterminateAngle2Speed = highSpeed;
							else
								this_.indeterminateAngle1Speed = highSpeed;
						}

						this_.indeterminateAngle1 += this_.indeterminateAngle1Speed;
						this_.indeterminateAngle2 += this_.indeterminateAngle2Speed;
						this_.indeterminateAngle1 %= 360;
						this_.indeterminateAngle2 %= 360;

						try 
						{ 
							Application.Current.Dispatcher.Invoke(() =>
							{
								if (!this_.breakAll)
								{
									this_?.RenderArc(this_.indeterminateAngle1, this_.indeterminateAngle2);
								}
							});
						}
						catch { break; }

						Thread.Sleep(1000 / 60);
					}
				}));
				thread.Start();
			}
			else
			{
				Thread thread = new Thread(new ThreadStart(() =>
				{
					while (true && !this_.breakAll)
					{
						var nextAngle2 = (this_.indeterminateAngle2 + lowSpeed) % 360;
						var nextAngle1 = (this_.indeterminateAngle1 + highSpeed) % 360;
						var diff = 360 - Math.Abs((nextAngle2 < nextAngle1 ? nextAngle1 - (nextAngle2 + 360) : nextAngle1 - nextAngle2));
						if (this_.indeterminateAngle1 > 360 - highSpeed && diff < 30)
						{
							try { Application.Current.Dispatcher.Invoke(() => this_.PathRoot.Visibility = Visibility.Collapsed); } catch { break; }
							break;
						}

						this_.indeterminateAngle1 += highSpeed;
						this_.indeterminateAngle2 += diff < 30 ? highSpeed : lowSpeed;
						if (this_.indeterminateAngle2 > 360 || this_.indeterminateAngle2 == highSpeed)
						{
							this_.indeterminateAngle2 = 360 - 1;
						}
						Debug.WriteLine($"Angle bois: {this_.indeterminateAngle1}, {this_.indeterminateAngle2}");
						try { Application.Current.Dispatcher.Invoke(() => this_?.RenderArc(this_.indeterminateAngle1, this_.indeterminateAngle2)); } catch { break; }
						Thread.Sleep(1000 / 30);
					}
				}));
				thread.Start();
			}
		}

		public void set_tick(double n)
		{
			PathRoot.StrokeThickness = n;
		}

		public void set_Color(SolidColorBrush n)
		{
			PathRoot.Stroke = n;
		}

		public void RenderArc(double angle1, double angle2)
		{
			//angle1 += 10;
			//angle2 -= 10;
			if (angle1 < 0)
				angle1 += 360;
			if (angle2 < 0)
				angle2 += 360;

			Point startPoint = ComputeCartesianCoordinate(angle1, Radius);
			Point endPoint = ComputeCartesianCoordinate(angle2, Radius);

			PathRoot.StrokeThickness = StrokeThickness;
			PathRoot.Width = Radius * 2 + StrokeThickness;
			PathRoot.Height = Radius * 2 + StrokeThickness;
			PathRoot.Margin = new Thickness(StrokeThickness, StrokeThickness, 0, 0);

			var diff = angle2 < angle1 ? (angle2 + 360) - angle1 : angle2 - angle1;
			bool largeArc = diff > 180.0;

			Size outerArcSize = new Size(Radius, Radius);

			pathFigure.StartPoint = startPoint;

			if (Math.Abs(startPoint.X - Math.Round(endPoint.X)) < 0.00001 && Math.Abs(startPoint.Y - Math.Round(endPoint.Y)) < 0.00001)
				endPoint.X -= 0.01;

			arcSegment.RotationAngle = angle1;
			arcSegment.Point = endPoint;
			arcSegment.Size = outerArcSize;
			arcSegment.IsLargeArc = largeArc;
		}

		private Point ComputeCartesianCoordinate(double angle, double radius)
		{
			// convert to radians and translate
			double angleRad = (Math.PI / 180.0) * (angle - 90);

			double x = radius * Math.Cos(angleRad);
			double y = radius * Math.Sin(angleRad);

			return new Point(x + radius, y + radius);
		}
	}
}