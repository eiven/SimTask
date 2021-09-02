namespace SimTaskViewer.Model
{
  using SimTask;
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;

  public class TaskTreeListItem : INotifyPropertyChanged
  {
    private ITask task;

    public ITask Task => this.task;

    public TaskTreeListItem()
    {
    }

    public TaskTreeListItem(ITask task)
    {
      this.task = task;
      this.task.OnProgressChanged += TaskOnProgressChanged;
    }

    private void TaskOnProgressChanged(object sender, EventArgs e)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Progress)));
    }

    public string Name => this.task?.Name ?? string.Empty;

    public string HandlerName => this.task?.GetTaskHandler()?.Name ?? string.Empty;

    public float Progress => this.task?.GetProgress() ?? 0.0f;

    public ObservableCollection<TaskTreeListItem> SubListItems { get; set; } = new ObservableCollection<TaskTreeListItem>();

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
