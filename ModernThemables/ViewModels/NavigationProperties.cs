namespace ModernThemables.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ModernThemables.Messages;

public class NavigationProperties
{
    public NavigationProperties(bool canDelete, SelectionMode selectionMode)
    {
        this.CanDelete = canDelete;
        this.SelectionMode = selectionMode;
    }

    private NavigationProperties()
    {
    }

    public bool CanDelete { get; set; } = false;

    public SelectionMode SelectionMode { get; set; } = SelectionMode.Automatic;

    public static NavigationProperties Default => new NavigationProperties();
}
