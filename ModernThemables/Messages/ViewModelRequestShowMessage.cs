using ModernThemables.ViewModels;

namespace ModernThemables.Messages
{
	public class ViewModelRequestShowMessage
    {
        public GenericViewModelBase Sender { get; protected set; }

        public GenericViewModelBase ViewModel { get; protected set; }

		public ViewModelRequestShowMessage(GenericViewModelBase viewModelToShow, GenericViewModelBase sender)
		{
			ViewModel = viewModelToShow;
            Sender = sender;
        }
	}

	public class ViewModelRequestShowMessage<T> where T : GenericViewModelBase
	{
        public GenericViewModelBase Sender { get; protected set; }

        public T ViewModel { get; protected set; }

		public ViewModelRequestShowMessage(T viewModelToShow, GenericViewModelBase sender)
		{
			ViewModel = viewModelToShow;
            Sender = sender;
        }
	}
}
