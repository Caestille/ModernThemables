namespace ModernThemables.Charting.ViewModels.CartesianChart;

using System.Text;
using System.Windows;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using ModernThemables.Charting.Interfaces;

internal class InternalPathSeriesViewModel : ObservableObject
{
    private readonly string? pathStrokeData;
    private readonly string? pathFillData;
    private Point topLeft;
    private Point bottomRight;

    private double topMargin;
    private double bottomMargin;
    private double leftMargin;
    private double rightMargin;

    public InternalPathSeriesViewModel(
        string? name,
        Guid guid,
        IEnumerable<InternalChartEntity> data,
        IChartBrush? stroke,
        IChartBrush? fill)
    {
        this.Name = name;
        this.Identifier = guid;
        this.Data = data;
        this.Stroke = stroke;
        this.Fill = fill;

        if (!data.Any())
        {
            return;
        }

        this.pathStrokeData = this.ConvertDataToPath(data);
        this.pathFillData = this.ConvertPathForFill(this.pathStrokeData);
    }

    public IEnumerable<InternalChartEntity> Data { get; private set; }

    public string PathStrokeData => $"M{this.topLeft.X - this.leftMargin},{this.topLeft.Y - this.topMargin} {this.pathStrokeData} M{this.bottomRight.X + this.rightMargin},{this.bottomRight.Y + this.bottomMargin}";

    public string PathFillData => $"M{this.topLeft.X - this.leftMargin},{this.topLeft.Y - this.topMargin} {this.pathFillData} M{this.bottomRight.X + this.rightMargin},{this.bottomRight.Y + this.bottomMargin}";

    public IChartBrush? Stroke { get; }

    public IChartBrush? Fill { get; }

    public Guid Identifier { get; }

    public string? Name { get; }

    public void SetMargins(double topMargin, double bottomMargin, double leftMargin, double rightMargin)
    {
        this.topMargin = topMargin * (this.bottomRight.Y - this.topLeft.Y);
        this.bottomMargin = bottomMargin * (this.bottomRight.Y - this.topLeft.Y);
        this.leftMargin = leftMargin * (this.bottomRight.X - this.topLeft.X);
        this.rightMargin = rightMargin * (this.bottomRight.X - this.topLeft.X);
        this.OnPropertyChanged(nameof(this.PathStrokeData));
        this.OnPropertyChanged(nameof(this.PathFillData));
    }

    public InternalChartEntity? GetChartPointUnderTranslatedMouse(
        Point cursor,
        double xZoom,
        double yZoom,
        double xLeftOffset,
        double yTopOffset)
    {
        var translatedX = cursor.X / xZoom;
        var translatedY = cursor.Y / yZoom;

        var nearestPoint = this.Data.FirstOrDefault(
            x => Math.Abs(x.X - translatedX) == this.Data.Min(x => Math.Abs(x.X - translatedX)));
        if (nearestPoint == null)
        {
            return null;
        }

        var hoveredChartPoints = this.Data.Where(x => x.X == nearestPoint.X);
        var hoveredChartPoint = hoveredChartPoints.Count() > 1
            ? hoveredChartPoints.First(
                x => Math.Abs(x.Y - translatedY) == hoveredChartPoints.Min(x => Math.Abs(x.Y - translatedY)))
            : hoveredChartPoints.First();

        var x = (hoveredChartPoint.X * xZoom) - xLeftOffset;
        var y = (hoveredChartPoint.Y * yZoom) - yTopOffset;
        return new InternalChartEntity(x, y, hoveredChartPoint.BackingPoint);
    }

    public void UpdatePoints(IEnumerable<InternalChartEntity> data) => this.Data = data;

    public bool IsTranslatedMouseInBounds(double dataWidth, double mouseX, double zoomWidth)
    {
        var xZoom = zoomWidth / dataWidth;
        var translatedX = mouseX / xZoom;

        return translatedX <= this.Data.Max(x => x.X)
            && translatedX >= this.Data.Min(x => x.X);
    }

    private string ConvertDataToPath(IEnumerable<InternalChartEntity> data)
    {
        this.topLeft = new Point(data.Min(x => x.X), data.Min(x => x.Y));
        this.bottomRight = new Point(data.Max(x => x.X), data.Max(x => x.Y));

        var sb = new StringBuilder();
        var pointType = "M";
        foreach (var point in data)
        {
            sb.Append($" {pointType}{point.X} {point.Y}");
            pointType = "L";
        }

        return sb.ToString().Trim();
    }

    private string ConvertPathForFill(string strokePath)
    {
        var dataMin = this.Data.Min(x => x.BackingPoint.YValue);
        var dataMax = this.Data.Max(x => x.BackingPoint.YValue);
        var dataRange = dataMax - dataMin;
        var zero = Math.Min(Math.Max(0d, dataMin), dataMax);
        var ratio = (double)(1 - ((zero - dataMin) / dataRange));
        var min = this.Data.Min(x => x.Y);
        var max = this.Data.Max(x => x.Y);
        var zeroPoint = min + (ratio * (max - min));
        return $"M{this.Data.First().X} {zeroPoint} {strokePath.Replace("M", "L")} L{this.Data.Last().X} {zeroPoint}";
    }
}
