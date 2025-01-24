namespace ModernThemables.Messages;

#pragma warning disable SA1402 // File may only contain a single type

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
