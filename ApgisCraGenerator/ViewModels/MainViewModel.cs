using CommunityToolkit.Mvvm.ComponentModel;

namespace ApgisCraGenerator.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedTabIndex;

    public TodoListViewModel TodoListViewModel { get; }
    public ProjetViewModel ProjetViewModel { get; }
    public CraViewModel CraViewModel { get; }

    public MainViewModel(
        TodoListViewModel todoListViewModel,
        ProjetViewModel projetViewModel,
        CraViewModel craViewModel)
    {
        TodoListViewModel = todoListViewModel;
        ProjetViewModel = projetViewModel;
        CraViewModel = craViewModel;
    }
}