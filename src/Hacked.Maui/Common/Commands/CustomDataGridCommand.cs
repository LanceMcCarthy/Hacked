using System.Windows.Input;
using Telerik.XamarinForms.DataGrid.Commands;

namespace Hacked.Maui.Common.Commands;

public class CustomDataGridCommand : DataGridCommand
{
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(CustomDataGridCommand), null);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public override bool CanExecute(object parameter)
    {
        bool canExecuteDefaultCommand = Owner?.CommandService?.CanExecuteDefaultCommand(Id, parameter) ?? false;
        bool canExecuteCommand = Command?.CanExecute(parameter) ?? true;
        return canExecuteDefaultCommand && canExecuteCommand;
    }

    public override void Execute(object parameter)
    {
        Owner?.CommandService?.ExecuteDefaultCommand(Id, parameter);
        Command?.Execute(parameter);
    }
}
