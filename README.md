# Personal Finance Tracker - V2 Development

> **🚀 Version 2.0 In Progress** - Active development branch with new features and enhancements
>
> This is the V2 development repository, based on the stable V1.0.0 release.
> For the stable version, see [V1.0.0 Release](https://github.com/chinmay005/Personal-Finance-Tracker-WPF/releases/tag/v1.0.0)

A powerful and intuitive WPF desktop application to manage personal expenses and income with advanced visualizations. Built with modern C# and .NET technologies, featuring SQLite database, interactive charts, and comprehensive search/filter capabilities.

## Features

🔐 **User Authentication & Profile Management**

- ✅ Secure user registration with email validation
- ✅ Password hashing using SHA256 encryption
- ✅ User login with username and password
- ✅ Unique username and email constraints
- ✅ Per-user transaction and category isolation
- ✅ User profile display in application window title
- ✅ Password strength validation (minimum 6 characters)

✨ **Core Transaction Management**

- ✅ Add income and expense transactions with ease
- ✅ Edit and delete existing transactions with confirmation dialogs
- ✅ View all transactions in a searchable, sortable data grid
- ✅ Real-time summary dashboard (Total Income, Total Expenses, Balance)
- ✅ Automatic categorization of transactions
- ✅ Notes field for transaction details
- ✅ Transaction activity log with timestamps and action indicators

📊 **Advanced Visualizations**

- ✅ **Monthly Income vs Expenses Line Chart** - Track income and expense trends over time
- ✅ **Monthly Expense Distribution Pie Chart** - Visualize expense breakdown by category
- ✅ Color-coded charts for easy interpretation
- ✅ Interactive chart legends and axis labels

🔍 **Search & Filter**

- ✅ **Keyword Search** - Filter transactions by category or notes
- ✅ **Date Range Filters** - View transactions within specific date ranges
- ✅ **Clear All Filters** - Quickly reset filters to view all transactions
- ✅ Real-time filtered results display

📁 **Category Management**

- ✅ Dynamic category management - Add, edit, and delete custom categories
- ✅ Income and Expense category separation
- ✅ Category icons/emojis for better UX
- ✅ Categories stored in database for persistence
- ✅ Income/Expense dropdown with visual separator

🛡️ **Reliability**

- ✅ Input validation for all fields
- ✅ Comprehensive error handling
- ✅ Error logging to `logs.txt` for debugging
- ✅ SQLite database persistence

💅 **User Interface**

- ✅ Tab-based layout (Transactions, Categories, Graphs)
- ✅ Modern Material Design with professional styling
- ✅ Blue borders for active tabs
- ✅ Alternating row colors in data grids for readability
- ✅ Currency formatting with rupee symbol (₹)
- ✅ Color-coded summary (Income: Green, Expenses: Red, Balance: Blue)
- ✅ Responsive design with proper spacing and visual hierarchy
- ✅ Emoji support with proper Unicode encoding

## Tech Stack

- **Language**: C# 12
- **Framework**: .NET 10.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Database**: SQLite with Microsoft.Data.Sqlite v8.0.0
- **Charting**: OxyPlot.Wpf for professional data visualization
- **IDE**: Visual Studio Code / Visual Studio

## Project Structure

```
PersonalFinanceTracker/
├── Data/
│   └── DatabaseHelper.cs       # Database operations & models
├── LoginWindow.xaml            # Login/Register UI
├── LoginWindow.xaml.cs         # Login/Register logic
├── MainWindow.xaml             # Main application UI layout
├── MainWindow.xaml.cs          # Event handlers & business logic
├── App.xaml                    # Application configuration
├── App.xaml.cs                 # Application startup & window management
└── PersonalFinanceTracker.csproj
```

## Installation & Setup

### Prerequisites

- .NET 10.0 SDK or higher
- Windows OS (for WPF)

### Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/chinmay005/Personal-Finance-Tracker-WPF.git
   cd chinmay005-Personal-Finance-Tracker-WPF/PersonalFinanceTracker
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Build the project**

   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

## Usage

### Getting Started - Login & Registration

#### First Time Users (Creating Account)

1. **Launch the application** - Login window will appear
2. Click on the **"Sign Up"** tab
3. Enter your details:
   - **Username**: Must be at least 3 characters long (unique)
   - **Email**: Valid email address (unique)
   - **Password**: Minimum 6 characters
   - **Confirm Password**: Must match the password field
4. Click **"Create Account"** button
5. Success message will appear - you can now log in

#### Returning Users (Login)

1. **Launch the application** - Login window will appear
2. **Login tab** is selected by default
3. Enter your credentials:
   - **Username**: Your registered username
   - **Password**: Your account password
4. Click **"Login"** button or press **Enter** in the password field
5. **MainWindow** will open with your username displayed in the title bar

#### About Your Account

- Each user has their **own separate data** (transactions, categories)
- Password is **securely hashed** using SHA256 encryption
- Username and email are **unique** across all users
- Your data is stored in the SQLite database with user isolation

### Adding a Transaction

1. Go to the **💳 Transactions** tab
2. Select a **date** using the date picker
3. Choose a **category** from the dropdown
   - **Income**: Salary, Bonus, Investment, Gift, Other Income
   - **Expense**: Food, Rent, Transport, Entertainment, Utilities, Healthcare, Other
4. Enter the **amount** (positive number only)
5. _(Optional)_ Add **notes** for transaction details
6. Click **"➕ Add Transaction"** button

### Searching & Filtering Transactions

1. Go to the **💳 Transactions** tab
2. Enter **keyword** to search by category or notes
3. Select **From Date** to filter transactions from a specific date
4. Select **To Date** to filter transactions until a specific date
5. Click **"🔎 Filter"** to apply filters
6. Click **"✖️ Clear"** to reset all filters and view all transactions

### Viewing Transaction Analytics

1. Go to the **📊 Graphs** tab
2. View **Monthly Income vs Expenses Trend** line chart:
   - Green line represents income
   - Red line represents expenses
   - Hover over data points for exact values
3. View **Monthly Expense Distribution** pie chart:
   - Shows expense breakdown by category for current month
   - Click on slices to see category names and amounts

### Managing Categories

1. Go to the **🏷️ Categories** tab
2. **Add a New Category**
   - Enter category **name** (e.g., "Shopping")
   - Select **type**: Income or Expense
   - _(Optional)_ Add an **icon** (emoji, e.g., 🛍️)
   - Click **"➕ Add Category"** button
3. **View Existing Categories**
   - All categories are displayed in the data grid
   - Shows: ID, Icon, Name, Type, Actions
4. **Edit or Delete Categories**
   - Click **"✏️ Edit"** to edit a category (feature coming soon)
   - Click **"🗑️ Delete"** to remove a category with confirmation

### Summary Dashboard

The bottom section displays real-time summaries:

- **💚 Total Income**: Sum of all income transactions (Green)
- **❤️ Total Expenses**: Sum of all expense transactions (Red)
- **💙 Balance**: Income minus Expenses (Blue)

## Key Features Explained

### Input Validation

- Amount must be a positive decimal number
- Date must be selected
- Category must be chosen
- Shows validation messages if any field is invalid

### Error Handling

- All database operations are wrapped in try-catch blocks
- Errors are logged to `logs.txt` in the application directory
- User-friendly error messages are displayed
- Application continues to function even if an error occurs

### Data Persistence

- Uses SQLite database (`finance.db`)
- Automatically creates the database on first run
- Transactions are stored permanently

## Database Schema

### Users Table

```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt TEXT NOT NULL
)
```

### Transactions Table

```sql
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    Category TEXT NOT NULL,
    Amount REAL NOT NULL,
    Notes TEXT
)
```

### Categories Table

```sql
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Type TEXT NOT NULL,           -- "Income" or "Expense"
    Icon TEXT                     -- Emoji or icon representation
)
```

**Default Categories:**

- **Income**: Salary 💼, Bonus 🎁, Investment 📈, Gift 🎉, Other Income 💰
- **Expense**: Food 🍔, Rent 🏠, Transport 🚗, Entertainment 🎬, Utilities 💡, Healthcare 🏥, Other 📦

## Screenshots

_(To be added - Take screenshots using Windows Snipping Tool or ShareX and save to `docs/screenshots/`)_

- Main Window with Transaction List
- Category Management Section
- Transaction Log and Summary Dashboard

## Future Enhancements

### V2.0 Roadmap (Priority)

- [ ] Budget planning and spending limits per category
- [ ] Spending alerts when budget threshold exceeded
- [ ] Monthly/yearly financial summaries and reports
- [ ] Recurring transaction templates
- [ ] Multi-currency support with conversion
- [ ] Data import from CSV/Excel files
- [ ] Data export to CSV, Excel, and PDF reports
- [ ] Dark mode theme option
- [ ] Database backup and restore functionality

### Future Releases (V2.1+)

- [x] Edit and delete transactions
- [x] Category management and custom categories
- [x] Charts and graphs for expense visualization (Line chart, Pie chart)
- [x] Search and filter transactions by keyword
- [x] Date range filters
- [ ] Multi-user support with authentication
- [ ] Cloud synchronization with backup
- [ ] Unit tests and integration tests
- [ ] Performance optimizations for large datasets
- [ ] Mobile companion app

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Chinmay** - [GitHub Profile](https://github.com/chinmay005)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

If you encounter any issues, please:

1. Check the `logs.txt` file for error details
2. Open an issue on GitHub with a detailed description
3. Include steps to reproduce the problem

## Changelog

### Version 2.0.0 (Development) 🔄

**Status**: Active Development
**Base**: Built from v1.0.0 stable release

🎯 **Planned Features for V2.0**

- ✅ User authentication and registration (COMPLETED)
- ✅ Secure password hashing with SHA256 (COMPLETED)
- ✅ Login window with tab-based Sign Up (COMPLETED)
- 🚀 Per-user data isolation
- 🚀 Enhanced dashboard with quick statistics
- 🎯 Budget planning and spending alerts
- 📊 Advanced reporting with monthly/yearly summaries
- 🏷️ Recurring transaction support
- 💱 Multi-currency support
- 📁 Data import/export (CSV, Excel, PDF)
- 🌙 Dark mode theme
- ⚡ Performance optimizations
- 🔐 Data backup and restore functionality
- 📱 Responsive layout improvements

### Version 1.0.0 (Release) 🎉

**Release Date**: February 17, 2026

✨ **New Features**

- ✨ **Graphs Tab** with interactive charts:
  - 📈 Monthly Income vs Expenses Line Chart - Track financial trends over time
  - 🥧 Monthly Expense Distribution Pie Chart - Visualize spending by category
- ✨ **Advanced Search & Filter**:
  - Keyword-based search (filter by category or notes)
  - Date range filters (From Date and To Date)
  - Clear filters button for quick reset
- ✨ **Dynamic Category Management**:
  - Add, edit, and delete custom categories
  - Category icons/emojis for better identification
  - Income/Expense category separation with visual separator in dropdown
- ✨ **Transaction Editing** - Full edit functionality for existing transactions
- ✨ **Professional Charting** - OxyPlot.Wpf integration for data visualization
- ✨ **Enhanced UI**:
  - Tab-based layout (Transactions, Categories, Graphs)
  - Material Design styling with blue active tab borders
  - Improved visual hierarchy and spacing
  - Emoji support with proper Unicode encoding

🐛 **Bug Fixes**

- Fixed emoji corruption issues with proper Unicode escape sequences
- Resolved tab styling and visibility issues
- Fixed category dropdown display issues

💅 **Improvements**

- Responsive UI with Material Design colors
- Better error messages and validation feedback
- Comprehensive activity logging with timestamps
- Clean, intuitive user interface
- Professional styling across all components

### Version 0.1.0 (Initial Release)

- Initial project setup
- Core transaction management functionality
- SQLite database integration
- Basic input validation and error handling
- WPF UI with simple layout

---

Made with ❤️ for personal finance management
