using SimTask;
using SimTaskViewer.Model;
using SimTaskViewer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimTaskViewer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public ObservableCollection<TaskTreeListItem> TaskTreeListItems { get; set; }
    public TaskScheduler scheduler = new TaskScheduler();
    public Timer Timer = new Timer();
    private TaskHandler cleanerOne;
    private TaskHandler cleanerTwo;
    private TaskHandler cleanerThree;

    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = this;
      //this.TestSideBySide();
      this.TestComplexTaskScenario();
    }

    /// <summary>
    /// This method creates a side by side task scenario.
    /// </summary>
    public void TestSideBySide()
    {
      var taskViewerViewModel = new TaskViewerViewModel();
      this.TaskTreeListItems = new ObservableCollection<TaskTreeListItem>();
      taskViewerViewModel.TaskTreeListItems = this.TaskTreeListItems;
      this.TaskTreeListItems.Add(new TaskTreeListItem());
      this.taskViewer.DataContext = taskViewerViewModel;
      scheduler.OnTaskAdded += SchedulerOnTaskAdded;
      scheduler.SimulationTime = 100;
      this.cleanerOne = new TaskHandler();
      cleanerOne.Name = "CLEANER ONE";
      this.cleanerTwo = new TaskHandler();
      cleanerTwo.Name = "CLEANER TWO";
      this.cleanerThree = new TaskHandler();
      cleanerThree.Name = "CLEANER THREE";

      var sideBySideCleanApartment = new Task();
      sideBySideCleanApartment.ChildMode = TaskChildMode.SideBySide;
      sideBySideCleanApartment.Name = "clean apartment";
      sideBySideCleanApartment.OnTaskFinished += SideBySideCleanApartment_OnTaskFinished;

      var taskCleanKitchen = new Task();
      taskCleanKitchen.SetTimeCosts(1500);
      taskCleanKitchen.Name = "clean kitchen";

      var taskCleanBathroom = new Task();
      taskCleanBathroom.SetTimeCosts(1000);
      taskCleanBathroom.Name = "clean bathroom";

      var taskCleanLivingRoom = new Task();
      taskCleanLivingRoom.ChildMode = TaskChildMode.Sequentiell;
      taskCleanLivingRoom.Name = "clean living room";

      var taskMoveTable = new Task();
      taskMoveTable.Name = "move table";
      taskMoveTable.ChildMode = TaskChildMode.Simultaneously;

      var taskMoveTablePartner1 = new Task();
      taskMoveTablePartner1.Name = "carry table together 1";

      var taskMoveTablePartner2 = new Task();
      taskMoveTablePartner2.Name = "carry table together 2";
      taskMoveTablePartner2.SetTimeCosts(750);
      taskMoveTablePartner1.SetTimeCosts(750);

      var taskWashLivingRoomFloor = new Task();
      taskWashLivingRoomFloor.SetTimeCosts(1250);
      taskWashLivingRoomFloor.Name = "wash living room floor";

      cleanerOne.AddTask(sideBySideCleanApartment);
      cleanerOne.AddTask(taskCleanKitchen);
      cleanerTwo.AddTask(taskCleanLivingRoom);
      cleanerTwo.AddTask(taskMoveTable);
      cleanerTwo.AddTask(taskMoveTablePartner1);
      cleanerTwo.AddTask(taskWashLivingRoomFloor);
      cleanerThree.AddTask(taskCleanBathroom);
      cleanerThree.AddTask(taskMoveTablePartner2);
      sideBySideCleanApartment.AddChildTask(taskCleanKitchen);
      sideBySideCleanApartment.AddChildTask(taskCleanLivingRoom);
      sideBySideCleanApartment.AddChildTask(taskCleanBathroom);
      taskCleanLivingRoom.AddChildTask(taskMoveTable);
      taskMoveTable.AddChildTask(taskMoveTablePartner1);
      taskMoveTable.AddChildTask(taskMoveTablePartner2);
      taskCleanLivingRoom.AddChildTask(taskWashLivingRoomFloor);

      scheduler.AddTask(taskCleanKitchen);
      scheduler.AddTask(taskCleanLivingRoom);
      scheduler.AddTask(taskCleanBathroom);
      scheduler.AddTask(taskMoveTable);
      scheduler.AddTask(taskMoveTablePartner1);
      scheduler.AddTask(taskMoveTablePartner2);
      scheduler.AddTask(taskWashLivingRoomFloor);
      scheduler.AddTask(sideBySideCleanApartment);

      //Timer.Interval = 1;
      //Timer.Elapsed += Timer_Elapsed;
      //Timer.Start();
    }

    public void TestComplexTaskScenario()
    {
      var taskViewerViewModel = new TaskViewerViewModel();
      this.TaskTreeListItems = new ObservableCollection<TaskTreeListItem>();
      taskViewerViewModel.TaskTreeListItems = this.TaskTreeListItems;
      taskViewerViewModel.TaskScheduler = scheduler;
      this.TaskTreeListItems.Add(new TaskTreeListItem());
      this.taskViewer.DataContext = taskViewerViewModel;
      scheduler.OnTaskAdded += SchedulerOnTaskAdded;
      scheduler.SimulationTime = 100;

      var handleOrderTask = new Task() { Name = "Handle order 100 article A", ChildMode = TaskChildMode.Sequentiell };
      handleOrderTask.SetTimeCosts(1000);
      var taskForProductionDepartment = new Task { Name = "Department produce 100 article A", ChildMode = TaskChildMode.Sequentiell };
      taskForProductionDepartment.SetTimeCosts(1000);
      var taskForFactory = new Task { Name = "Factory produce 100 article A" };
      taskForFactory.SetTimeCosts(1000);
      var salesDepartment = new TaskHandler { Name = "Sales department" };
      var productionDepartment = new TaskHandler{Name = "Production department"};
      var factory = new TaskHandler { Name = "Factory" };
      salesDepartment.AddTask(handleOrderTask);

      handleOrderTask.OnTaskStarted += (s, e) => {
        this.Timer.Stop();
        handleOrderTask.AddChildTask(taskForProductionDepartment);
        productionDepartment.AddTask(taskForProductionDepartment);
        scheduler.AddTask(taskForProductionDepartment);
        this.Timer.Start();
      };

      taskForProductionDepartment.OnTaskStarted += (s, e) => {
        this.Timer.Stop();
        taskForProductionDepartment.AddChildTask(taskForFactory);
        factory.AddTask(taskForFactory);
        scheduler.AddTask(taskForFactory);
        this.Timer.Start();
      };

      taskForFactory.OnTaskStarted += (s, e) =>
      {
        taskForFactory.ChildMode = TaskChildMode.Sequentiell;
        var setProductionLine = new Task { Name = "set production line article A" };
        setProductionLine.SetTimeCosts(500);
        factory.AddTask(setProductionLine);
        taskForFactory.AddChildTask(setProductionLine);
        var prepareProductionLine = new Task { Name = "prepare production line article A" };
        prepareProductionLine.SetTimeCosts(700);
        factory.AddTask(prepareProductionLine);
        taskForFactory.AddChildTask(prepareProductionLine);
        var produce = new Task { Name = "produce 100 article A" };
        produce.SetTimeCosts(2000);
        taskForFactory.AddChildTask(produce);
        factory.AddTask(produce);
        scheduler.AddTask(setProductionLine);
        scheduler.AddTask(prepareProductionLine);
        scheduler.AddTask(produce);
      };

      scheduler.AddTask(handleOrderTask);

      //Timer.Interval = 1;
      //Timer.Elapsed += Timer_Elapsed;
      //Timer.Start();
    }

    private void SideBySideCleanApartment_OnTaskFinished(object sender, EventArgs e)
    {
      App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
      {
        var goHome1 = new SimTask.Task { Name = "go home" };
        goHome1.SetTimeCosts(2000);
        goHome1.SetTaskHandler(cleanerOne);
        scheduler.AddTask(goHome1);
        var goHome2 = new Task { Name = "go home"};
        goHome2.SetTimeCosts(2000);
        goHome2.SetTaskHandler(cleanerTwo);
        scheduler.AddTask(goHome2);
        var goHome3 = new Task { Name = "go home" };
        goHome3.SetTimeCosts(2000);
        goHome3.SetTaskHandler(cleanerThree);
        scheduler.AddTask(goHome3);
      });
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      scheduler.Tick(10);
    }

    private void SchedulerOnTaskAdded(object sender, ITask task)
    {
      task.OnParentTaskChanged += Task_OnParentTaskChanged;
      this.InsertTreeListItem(task);
    }

    private void InsertTreeListItem(ITask task)
    {
      if (this.Find(task, this.TaskTreeListItems, false) != null)
      {
        return;
      }

      var parentTreeListItem = this.Find(task.GetParentTask(), this.TaskTreeListItems, false);
      if (parentTreeListItem != null)
      {
        parentTreeListItem.SubListItems.Add(new TaskTreeListItem(task));
      }
      else
      {
        if (task.GetParentTask() != null)
        {
          var parentTask = task.GetParentTask();
          this.InsertTreeListItem(parentTask);
          this.InsertTreeListItem(task);
        }
      }
    }

    private void Task_OnParentTaskChanged(object sender, EventArgs e)
    {
      var task = (ITask)sender;
      var parentTask = string.Empty;
      this.Find(task, this.TaskTreeListItems, true);
      this.InsertTreeListItem(task);
    }

    TaskTreeListItem Find(ITask task, ObservableCollection<TaskTreeListItem> items, bool remove)
    {
      foreach (var item in items.ToArray())
      {
        if (item.Task == task)
        {
          if (remove && items.Contains(item))
          {
            items.Remove(item);
          }

          return item;
        }
        else
        {
          var result = this.Find(task, item.SubListItems, remove);
          if (result != null)
          {
            return result;
          }
        }
      }

      return null;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.scheduler.Tick(20);
    }
  }
}
