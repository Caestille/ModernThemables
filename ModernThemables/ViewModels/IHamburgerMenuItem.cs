namespace ModernThemables.ViewModels;

using System.Collections.Generic;

public interface IHamburgerMenuItem
{
    string Name { get; }

    List<object> GetChildren(bool recurse = false);
}
