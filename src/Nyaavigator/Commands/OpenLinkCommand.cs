using System;
using System.Windows.Input;
using Nyaavigator.Builders;
using Nyaavigator.Enums;
using Nyaavigator.Utilities;

namespace Nyaavigator.Commands;

public class OpenLinkCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is not string { Length: > 0 } link)
        {
            Dialog.Create()
                .Type(DialogType.Error)
                .Content("Couldn't open the link because it's invalid.")
                .ShowAndForget();
            return;
        }

        Link.Open(link);
    }
}