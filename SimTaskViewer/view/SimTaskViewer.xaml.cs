using SimTaskViewer.ViewModel;
using System.Windows.Controls;

namespace SimTaskViewer.View
{

  /// <summary>
  /// Interaktionslogik für SimTaskViewer.xaml
  /// </summary>
  public partial class SimTaskViewer : Page
  {
    public TaskViewerViewModel TaskViewerViewModel { get; set; }

    public SimTaskViewer()
    {
      InitializeComponent();
    }
  }
}
