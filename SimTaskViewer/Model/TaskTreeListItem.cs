namespace SimTaskViewer.Model
{
  using SimTask;
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Runtime.CompilerServices;

  public class TaskTreeListItem : INotifyPropertyChanged
  {
    private ITask task;
    private string progressToolTip = string.Empty;

    public ITask Task => this.task;

    public TaskTreeListItem()
    {
      this.UpdateToolTips();
    }

    public TaskTreeListItem(ITask task)
    {
      this.task = task;
      this.task.OnProgressChanged += TaskOnProgressChanged;
      this.UpdateToolTips();
    }

    private void TaskOnProgressChanged(object sender, EventArgs e)
    {
      this.NotifyPropertyChanged(nameof(this.Progress));
      this.UpdateToolTips();
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    private void UpdateToolTips()
    {
      this.ProgressToolTip = this.Name + ": " + ((this.task?.GetProgress() ?? 0.0f) * 100.0f).ToString() + " %";
    }

    public string Name => this.task?.Name ?? string.Empty;

    public string HandlerName => this.task?.GetTaskHandler()?.Name ?? string.Empty;

    public float Progress => this.task?.GetProgress() ?? 0.0f;

    public string ProgressToolTip 
    {
      get 
      {
        return this.progressToolTip;
      }

      set
      {
        this.progressToolTip = value;
        this.NotifyPropertyChanged();
      }
    }

    public ObservableCollection<TaskTreeListItem> SubListItems { get; set; } = new ObservableCollection<TaskTreeListItem>();

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
