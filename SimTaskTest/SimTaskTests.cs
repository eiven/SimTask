using NUnit.Framework;
using System;
using System.Diagnostics;
using SimTask;
using TaskingFrameworkNUnitTest.Tasking3.TestData;

namespace TaskingFrameworkNUnitTest.Tasking3
{
  public class SimTaskTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test(Description = "Test unload task")]
    public void TestSimultaneously()
    {
      var scheduler = new TaskScheduler();
      scheduler.SimulationTime = 100;
      TaskHandler driver = new TaskHandler();
      driver.Name = "Driver";
      TaskHandler warehouse = new TaskHandler();
      warehouse.Name = "Warehouse";

      var sync = new Task();
      sync.ChildMode = TaskChildMode.Simultaneously;
      sync.Name = "Sync unload tasks";
      var taskUnloadDriver = new TaskUnload();
      taskUnloadDriver.SetTimeCosts(1000);
      taskUnloadDriver.Name = "taskUnloadDriver";
      var taskUnloadWarehouse = new TaskUnload();
      taskUnloadWarehouse.SetTimeCosts(1000);
      taskUnloadWarehouse.Name = "taskUnloadWarehouse";
      var taskCleanWarehouse = new Task();
      taskCleanWarehouse.SetTimeCosts(2000);
      taskCleanWarehouse.Name = "taskCleanWarehouse";

      driver.AddTask(sync);
      driver.AddTask(taskUnloadDriver);
      warehouse.AddTask(taskCleanWarehouse);
      warehouse.AddTask(taskUnloadWarehouse);
      // Reihenfolge ist aktuell noch wichtig erst AddChildTask dann addtask
      // andernfalls wird keine treenode für den childtask an der root oder previous node erstellt
      sync.AddChildTask(taskUnloadDriver);
      sync.AddChildTask(taskUnloadWarehouse);

      taskUnloadDriver.OnProgressChanged += this.ShowProgress;
      taskUnloadWarehouse.OnProgressChanged += this.ShowProgress;
      taskCleanWarehouse.OnProgressChanged += this.ShowProgress;
      sync.OnProgressChanged += this.ShowProgress;

      for (var i = 0; i < 100; i++)
      {
        scheduler.Tick(100);
      }
    }


    [Test(Description = "Test unload task")]
    public void TestSequentiell()
    {
      var scheduler = new TaskScheduler();
      scheduler.SimulationTime = 100;
      TaskHandler driver = new TaskHandler();
      driver.Name = "Driver";
      TaskHandler warehouse = new TaskHandler();
      warehouse.Name = "Warehouse";

      var sequentiellUnloading = new Task();
      sequentiellUnloading.ChildMode = TaskChildMode.Sequentiell;
      sequentiellUnloading.Name = "Unload stock sequentiell";
      var taskUnloadDriver = new TaskUnload();
      taskUnloadDriver.SetTimeCosts(1000);
      taskUnloadDriver.Name = "taskUnloadDriver";
      var taskUnloadWarehouse = new TaskUnload();
      taskUnloadWarehouse.SetTimeCosts(1000);
      taskUnloadWarehouse.Name = "taskUnloadWarehouse";
      var taskCleanWarehouse = new Task();
      taskCleanWarehouse.SetTimeCosts(2000);
      taskCleanWarehouse.Name = "taskCleanWarehouse";

      driver.AddTask(sequentiellUnloading);
      driver.AddTask(taskUnloadDriver);
      warehouse.AddTask(taskCleanWarehouse);
      warehouse.AddTask(taskUnloadWarehouse);
      sequentiellUnloading.AddChildTask(taskUnloadDriver);
      sequentiellUnloading.AddChildTask(taskUnloadWarehouse);

      taskUnloadDriver.OnProgressChanged += this.ShowProgress;
      taskUnloadWarehouse.OnProgressChanged += this.ShowProgress;
      taskCleanWarehouse.OnProgressChanged += this.ShowProgress;
      sequentiellUnloading.OnProgressChanged += this.ShowProgress;

      for (var i = 0; i < 100; i++)
      {
        scheduler.Tick(100);
      }

      Assert.That(true);
    }

    [Test(Description = "Test unload task")]
    public void TestSideBySide()
    {
      var scheduler = new TaskScheduler();
      scheduler.SimulationTime = 100;
      TaskHandler cleanerOne = new TaskHandler();
      cleanerOne.Name = "Cleaner one";
      TaskHandler cleanerTwo = new TaskHandler();
      cleanerTwo.Name = "Cleaner two";
      TaskHandler cleanerThree = new TaskHandler();
      cleanerThree.Name = "Cleaner three";

      var sideBySideCleanApartment = new Task();
      sideBySideCleanApartment.ChildMode = TaskChildMode.SideBySide;
      sideBySideCleanApartment.Name = "Cleaning apartment.";
      var taskCleanKitchen = new TaskUnload();
      taskCleanKitchen.SetTimeCosts(1500);
      taskCleanKitchen.Name = "taskCleanKitchen";
      var taskCleanBathroom = new TaskUnload();
      taskCleanBathroom.SetTimeCosts(1000);
      taskCleanBathroom.Name = "taskCleanBathroom";
      var taskCleanLivingRoom = new Task();
      taskCleanLivingRoom.SetTimeCosts(2000);
      taskCleanLivingRoom.Name = "taskCleanLivingRoom";

      cleanerOne.AddTask(sideBySideCleanApartment);
      cleanerOne.AddTask(taskCleanKitchen);
      cleanerTwo.AddTask(taskCleanLivingRoom);
      cleanerThree.AddTask(taskCleanBathroom);
      sideBySideCleanApartment.AddChildTask(taskCleanKitchen);
      sideBySideCleanApartment.AddChildTask(taskCleanLivingRoom);
      sideBySideCleanApartment.AddChildTask(taskCleanBathroom);

      taskCleanKitchen.OnProgressChanged += this.ShowProgress;
      taskCleanBathroom.OnProgressChanged += this.ShowProgress;
      taskCleanLivingRoom.OnProgressChanged += this.ShowProgress;
      sideBySideCleanApartment.OnProgressChanged += this.ShowProgress;

      for (var i = 0; i < 100; i++)
      {
        scheduler.Tick(100);
      }

      Assert.That(true);
    }

    private void ShowProgress(object sender, EventArgs eventArgs)
    {
      var task = (ITask)sender;
      Debug.WriteLine("TASK: " + task.Name + " PROGRESS: " + task.GetProgress());
    }
  }
}
