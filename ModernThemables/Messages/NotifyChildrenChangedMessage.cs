using ModernThemables.ViewModels;

namespace ModernThemables.Messages
{
	public class NotifyChildrenChangedMessage
	{
		public GenericViewModelBase Sender { get; private set; }

		public NotifyChildrenChangedMessage(GenericViewModelBase sender)
		{
			Sender = sender;
		}
	}

	public class NotifyChildrenChangedMessage<TChild> where TChild : GenericViewModelBase
	{
		public ViewModelBase<TChild> Sender { get; private set; }

		public NotifyChildrenChangedMessage(ViewModelBase<TChild> sender)
		{
			Sender = sender;
		}
	}
}