using ModernThemables.ViewModels;

namespace ModernThemables.Messages
{
	public class ViewModelRequestShowMessage
	{
		public GenericViewModelBase ViewModel { get; protected set; }

		public ViewModelRequestShowMessage(GenericViewModelBase viewModelToShow)
		{
			ViewModel = viewModelToShow;
		}
	}

	public class ViewModelRequestShowMessage<T> where T : GenericViewModelBase
	{
		public T ViewModel { get; protected set; }

		public ViewModelRequestShowMessage(T viewModelToShow)
		{
			ViewModel = viewModelToShow;
		}
	}
}
