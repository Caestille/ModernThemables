namespace ModernThemables.Messages;

#pragma warning disable SA1402 // File may only contain a single type

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
