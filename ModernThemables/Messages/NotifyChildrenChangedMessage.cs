namespace ModernThemables.Messages;

using ModernThemables.ViewModels;

public class NotifyChildrenChangedMessage
{
    public NotifyChildrenChangedMessage(GenericViewModelBase sender)
    {
        this.Sender = sender;
    }

    public GenericViewModelBase Sender { get; private set; }
}

public class NotifyChildrenChangedMessage<TChild>
    where TChild : GenericViewModelBase
{
    public NotifyChildrenChangedMessage(ViewModelBase<TChild> sender)
    {
        this.Sender = sender;
    }

    public ViewModelBase<TChild> Sender { get; private set; }
}
