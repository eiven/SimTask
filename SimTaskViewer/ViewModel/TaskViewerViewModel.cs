namespace SimTaskViewer.ViewModel
{
  using System.ComponentModel;
  using System.Collections.ObjectModel;
  using SimTaskViewer.Model;

  public class TaskViewerViewModel : INotifyPropertyChanged
  {
    private ObservableCollection<TaskTreeListItem> taskTreeListItems;
    public event PropertyChangedEventHandler PropertyChanged;

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
  }
}