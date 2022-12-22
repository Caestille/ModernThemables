using CoreUtilities.HelperClasses.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ModernThemables.HelperClasses.Charting.Brushes;
using ModernThemables.Interfaces;
using System;

namespace ModernThemables.ViewModels.Charting.PieChart
{
    /// <summary>
    /// An internal representation of a chart point for rendering the actual series with.
    /// </summary>
    internal class InternalPieWedge : ObservableObject
	{
		private string name;
		/// <summary>
		/// The name of the pie wedge.
		/// </summary>
		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		private double percent;
		/// <double>
		/// The value of the pie wedge in percent of a full circle.
		/// </summary>
		public double Percent
		{
			get => percent;
			set => SetProperty(ref percent, value);
		}

		private double val;
		/// <double>
		/// The value of the pie wedge.
		/// </summary>
		public double Value
		{
			get => val;
			set => SetProperty(ref val, value);
		}

		private double startAngle;
		/// <summary>
		/// The start angle of the wedge.
		/// </summary>
		public double StartAngle
		{
			get => startAngle;
			set => SetProperty(ref startAngle, value);
		}

		private IChartBrush stroke;
		public IChartBrush Stroke
		{
			get => stroke;
			set => SetProperty(ref stroke, value);
		}

		private IChartBrush fill;
		public IChartBrush Fill
		{
			get => fill;
			set => SetProperty(ref fill, value);
		}

		private Guid identifier;
		public Guid Identifier
		{
			get => identifier;
			set => SetProperty(ref identifier, value);
		}

		private bool resizeTrigger;
		/// <summary>
		/// A <see cref="bool"/> property used to for the series to resize itself when desired by triggering a
		/// converter.
		/// </summary>
		public bool ResizeTrigger
		{
			get => resizeTrigger;
			set => SetProperty(ref resizeTrigger, value);
		}

		private bool isMouseOver;
		/// <summary>
		/// A <see cref="bool"/> indicating whether the mouse is over this wedge.
		/// </summary>
		public bool IsMouseOver
		{
			get => isMouseOver;
			set => SetProperty(ref isMouseOver, value);
		}

		/// <summary>
		/// Initialises a new <see cref="InternalPieWedge"/>.
		/// </summary>
		/// <param name="name">The wedge name.</param>
		/// <param name="percent">The wedge value in percent of a full circle.</param>
		/// <param name="value">The wedge value.</param>
		/// <param name="startAngle">The wedge start angle.</param>
		/// <param name="stroke">The wedge stroke.</param>
		/// <param name="fill">The wedge fill.</param>
		public InternalPieWedge(string name, Guid identifier, double percent, double value, double startAngle, IChartBrush stroke, IChartBrush fill)
        {
            Name = name;
            Identifier = identifier;
            Percent = percent;
			Value = value;
            StartAngle = startAngle;
            Stroke = stroke;
            Fill = fill;
        }
    }
}
