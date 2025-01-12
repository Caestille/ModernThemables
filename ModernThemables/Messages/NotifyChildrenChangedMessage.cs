namespace ModernThemables.Messages;

using ModernThemables.ViewModels;

public class NotifyChildrenChangedMessage
{
    public GenericViewModelBase Sender { get; private set; }

    public NotifyChildrenChangedMessage(GenericViewModelBase sender)
    {
        this.Sender = sender;
    }
}

public class NotifyChildrenChangedMessage<TChild> where TChild : GenericViewModelBase
{
    public ViewModelBase<TChild> Sender { get; private set; }

    public NotifyChildrenChangedMessage(ViewModelBase<TChild> sender)
    {
        this.Sender = sender;
    }
}