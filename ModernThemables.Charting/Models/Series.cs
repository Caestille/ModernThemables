namespace ModernThemables.Charting.Models;

using ModernThemables.Charting.Interfaces;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Generic series.
/// </summary>
public class Series : ISeries
{
    /// <inheritdoc />
    public event EventHandler<PropertyChangedEventArgs>? PropertyChanged;

    /// <inheritdoc />
    public event EventHandler<NotifyCollectionChangedEventArgs>? CollectionChanged;

    /// <inheritdoc />
    public Func<IChartEntity, string>? ValueFormatter { get; set; }

    /// <inheritdoc />
    public IChartBrush? Stroke { get; set; }

    /// <inheritdoc />
    public IChartBrush? Fill { get; set; }

    /// <inheritdoc />
    public Guid Identifier { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public string? Name { get; set; }

    private ObservableCollection<IChartEntity> values = new();
    /// <inheritdoc />
    public ObservableCollection<IChartEntity> Values
    {
        get => this.values;
        set
        {
            if (this.values != null)
            {
                this.values.CollectionChanged -= this.Values_CollectionChanged;
            }
            this.values = value;
            this.values.CollectionChanged += this.Values_CollectionChanged;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Values)));
        }
    }

    private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }
}
