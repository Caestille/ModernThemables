namespace ModernThemables.Messages;

using CommunityToolkit.Mvvm.ComponentModel;

public class ViewModelRequestDeleteMessage
{
    public ViewModelRequestDeleteMessage(ObservableObject viewModelToDelete)
    {
        this.ViewModel = viewModelToDelete;
    }

    public ObservableObject ViewModel { get; set; }
}

public class ViewModelRequestDeleteMessage<T>
{
    public ViewModelRequestDeleteMessage(ObservableObject viewModelToDelete)
    {
        this.ViewModel = viewModelToDelete;
    }

    public ObservableObject ViewModel { get; set; }
}
