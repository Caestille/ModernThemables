namespace ModernThemables.Messages;

using CommunityToolkit.Mvvm.ComponentModel;

public class ViewModelRequestDeleteMessage
{
    public ObservableObject ViewModel { get; set; }

    public ViewModelRequestDeleteMessage(ObservableObject viewModelToDelete)
    {
        this.ViewModel = viewModelToDelete;
    }
}

public class ViewModelRequestDeleteMessage<T>
{
    public ObservableObject ViewModel { get; set; }

    public ViewModelRequestDeleteMessage(ObservableObject viewModelToDelete)
    {
        this.ViewModel = viewModelToDelete;
    }
}
