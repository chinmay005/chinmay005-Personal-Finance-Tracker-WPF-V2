using System;
using System.Text.RegularExpressions;
using System.Windows;
using PersonalFinanceTracker.Data;

namespace PersonalFinanceTracker
{
  /// <summary>
  /// Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginWindow : Window
  {
    private DatabaseHelper? _database;
    public User? LoggedInUser { get; private set; }

    public LoginWindow()
    {
      InitializeComponent();
      _database = new DatabaseHelper();
      LoginSignupTabs.SelectedIndex = 0; // Start with Login tab
    }

    /// <summary>
    /// Handles login button click
    /// </summary>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
      PerformLogin();
    }

    /// <summary>
    /// Handles signup button click
    /// </summary>
    private void SignupButton_Click(object sender, RoutedEventArgs e)
    {
      string username = SignupUsername.Text.Trim();
      string email = SignupEmail.Text.Trim();
      string password = SignupPassword.Password;
      string confirmPassword = SignupConfirmPassword.Password;

      SignupErrorMessage.Visibility = Visibility.Collapsed;

      // Validation
      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
          string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
      {
        ShowSignupError("Please fill in all fields.");
        return;
      }

      if (username.Length < 3)
      {
        ShowSignupError("Username must be at least 3 characters long.");
        return;
      }

      if (password.Length < 6)
      {
        ShowSignupError("Password must be at least 6 characters long.");
        return;
      }

      if (password != confirmPassword)
      {
        ShowSignupError("Passwords do not match.");
        return;
      }

      if (!IsValidEmail(email))
      {
        ShowSignupError("Please enter a valid email address.");
        return;
      }

      if (_database == null)
      {
        ShowSignupError("Database error. Please try again.");
        return;
      }

      // Check if username already exists
      if (_database.UsernameExists(username))
      {
        ShowSignupError("Username already exists. Please choose a different one.");
        return;
      }

      // Check if email already exists
      if (_database.EmailExists(email))
      {
        ShowSignupError("Email already registered. Please use a different email or log in.");
        return;
      }

      // Register user
      if (_database.RegisterUser(username, email, password))
      {
        MessageBox.Show("Account created successfully! You can now log in.",
                       "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        ClearSignupFields();
        LoginSignupTabs.SelectedIndex = 0; // Switch to Login tab
        LoginUsername.Text = username;
        LoginPassword.Focus();
      }
      else
      {
        ShowSignupError("Error creating account. Please try again.");
      }
    }

    /// <summary>
    /// Validates email format
    /// </summary>
    private bool IsValidEmail(string email)
    {
      try
      {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    /// Shows login error message
    /// </summary>
    private void ShowLoginError(string message)
    {
      LoginErrorMessage.Text = message;
      LoginErrorMessage.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Shows signup error message
    /// </summary>
    private void ShowSignupError(string message)
    {
      SignupErrorMessage.Text = message;
      SignupErrorMessage.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Clears all signup fields
    /// </summary>
    private void ClearSignupFields()
    {
      SignupUsername.Clear();
      SignupEmail.Clear();
      SignupPassword.Clear();
      SignupConfirmPassword.Clear();
    }

    /// <summary>
    /// Switches to signup tab
    /// </summary>
    private void SwitchToSignup(object sender, RoutedEventArgs e)
    {
      LoginSignupTabs.SelectedIndex = 1;
      ClearSignupFields();
      SignupErrorMessage.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Switches to login tab
    /// </summary>
    private void SwitchToLogin(object sender, RoutedEventArgs e)
    {
      LoginSignupTabs.SelectedIndex = 0;
      LoginUsername.Clear();
      LoginPassword.Clear();
      LoginErrorMessage.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Handles Enter key press in login password field
    /// </summary>
    private void LoginPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Return)
      {
        e.Handled = true;
        PerformLogin();
      }
    }

    /// <summary>
    /// Performs the login action
    /// </summary>
    private void PerformLogin()
    {
      string username = LoginUsername.Text.Trim();
      string password = LoginPassword.Password;

      LoginErrorMessage.Visibility = Visibility.Collapsed;

      // Validation
      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
      {
        ShowLoginError("Please enter both username and password.");
        return;
      }

      if (_database == null)
      {
        ShowLoginError("Database error. Please try again.");
        return;
      }

      // Authenticate user
      User? user = _database.AuthenticateUser(username, password);

      if (user != null)
      {
        LoggedInUser = user;
        
        try
        {
          // Create and show MainWindow
          MainWindow mainWindow = new MainWindow(user);
          Application.Current.MainWindow = mainWindow;
          mainWindow.Show();
          
          // Close login window
          this.Close();
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Error loading main window: {ex.Message}\n\n{ex.StackTrace}", 
                         "Application Error", 
                         MessageBoxButton.OK, 
                         MessageBoxImage.Error);
          // Keep login window open on error
        }
      }
      else
      {
        ShowLoginError("Invalid username or password.");
        LoginPassword.Clear();
      }
    }
  }
}
