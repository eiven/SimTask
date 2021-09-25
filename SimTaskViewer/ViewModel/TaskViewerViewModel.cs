namespace SimTaskViewer.ViewModel
{
  using System.ComponentModel;
  using System.Collections.ObjectModel;
  using SimTaskViewer.Model;
  using SimTask;
  using System.Windows.Input;
  using System;

  public class TaskViewerViewModel : INotifyPropertyChanged
  {
    private ObservableCollection<TaskTreeListItem> taskTreeListItems;
    public event PropertyChangedEventHandler PropertyChanged;
    public TaskScheduler TaskScheduler { get; set; }

    public ObservableCollection<TaskTreeListItem> TaskTreeListItems
    {
      get
      {
        return this.taskTreeListItems;
      }

      set
      {
        this.taskTreeListItems = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaskViewerViewModel.TaskTreeListItems)));
      }
    }

    private ICommand clickCommand;

    public ICommand Tick
    {
      get {
        return this.clickCommand ?? (this.clickCommand = new TickCommand());
      }
      //this.TaskScheduler?.Tick(20);
    }

    private class TickCommand : ICommand
    {
      public event EventHandler CanExecuteChanged;

      public bool CanExecute(object parameter)
      {
        return parameter is TaskScheduler;
      }

      public void Execute(object parameter)
      {
        (parameter as TaskScheduler).Tick(200);
      }
    }
  }
}