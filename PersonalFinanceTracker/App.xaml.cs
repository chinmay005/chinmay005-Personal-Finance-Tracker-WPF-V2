using System.Configuration;
using System.Data;
using System.Windows;
using PersonalFinanceTracker.Data;

namespace PersonalFinanceTracker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    // Prevent automatic shutdown - we'll handle it manually
    this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

    try
    {
      // Show login window as a regular window (not modal dialog)
      LoginWindow loginWindow = new LoginWindow();
      loginWindow.Show();
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error in login: {ex.Message}\n\n{ex.StackTrace}",
                     "Application Error",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error);
      this.Shutdown();
    }
  }
}
