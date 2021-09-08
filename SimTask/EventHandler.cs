namespace SimTask
{
  public delegate void TaskAddedEventHandler(object sender, ITask task);
  public delegate void TaskRemovedEventHandler(object sender, ITask task);
  public delegate void TaskHandledEventHandler(object sender, ITask task);
}