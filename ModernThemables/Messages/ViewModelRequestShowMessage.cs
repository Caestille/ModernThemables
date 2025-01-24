namespace ModernThemables.Messages;

#pragma warning disable SA1402 // File may only contain a single type

using ModernThemables.ViewModels;

public class ViewModelRequestShowMessage
{
    public ViewModelRequestShowMessage(GenericViewModelBase viewModelToShow, GenericViewModelBase sender)
    {
        this.ViewModel = viewModelToShow;
        this.Sender = sender;
    }

    public GenericViewModelBase Sender { get; protected set; }

    public GenericViewModelBase ViewModel { get; protected set; }
}

public class ViewModelRequestShowMessage<T>
    where T : GenericViewModelBase
{
    public ViewModelRequestShowMessage(T viewModelToShow, GenericViewModelBase sender)
    {
        this.ViewModel = viewModelToShow;
        this.Sender = sender;
    }

    public GenericViewModelBase Sender { get; protected set; }

    public T ViewModel { get; protected set; }
}
