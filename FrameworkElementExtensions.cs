using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Win10Themables
{
    public static class FrameworkElementExtensions
    {
		public static List<FrameworkElement> GetLogicalElements(this object parent)
		{
			var list = new List<FrameworkElement>();
			if (parent == null)
				return list;

			if (parent.GetType().IsSubclassOf(typeof(FrameworkElement)))
				list.Add((FrameworkElement)parent);

			var doParent = parent as DependencyObject;
			if (doParent == null)
				return list;

			foreach (object child in LogicalTreeHelper.GetChildren(doParent))
			{
				list.AddRange(GetLogicalElements(child));
			}

			return list;
		}
	}
}
