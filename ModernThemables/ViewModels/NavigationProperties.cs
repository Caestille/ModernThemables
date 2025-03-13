namespace ModernThemables.ViewModels;
public class NavigationProperties
{
    public NavigationProperties(bool canDelete, SelectionMode selectionMode)
    {
        this.CanDelete = canDelete;
        this.SelectionMode = selectionMode;
    }

    private NavigationProperties()
    {
    }

    public bool CanDelete { get; set; } = false;

    public SelectionMode SelectionMode { get; set; } = SelectionMode.Automatic;

    public static NavigationProperties Default => new NavigationProperties();
}
