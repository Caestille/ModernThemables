namespace ModernThemables.Charting.ViewModels.PieChart
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A view model for an internal representation of a series used by the <see cref="CartesianChart"/>.
    /// </summary>
    internal class InternalPieSeriesViewModel : ObservableObject
    {
        private ObservableCollection<InternalPieWedgeViewModel> wedges;

        /// <summary>
        /// The pie wedges.
        /// </summary>
        public ObservableCollection<InternalPieWedgeViewModel> Wedges
        {
            get => this.wedges;
            set => this.SetProperty(ref this.wedges, value);
        }

        /// <summary>
        /// The series name for the legend.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initialises a new <see cref="InternalPieSeriesViewModel"/>.
        /// </summary>
        /// <param name="name">The series name.</param>
        /// <param name="wedges">The data this series represents.</param>
        public InternalPieSeriesViewModel(
            string name,
            ObservableCollection<InternalPieWedgeViewModel> wedges)
        {
            this.Name = name;
            this.Wedges = wedges;
        }
    }
}
