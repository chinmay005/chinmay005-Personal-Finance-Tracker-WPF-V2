using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PersonalFinanceTracker.Data;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace PersonalFinanceTracker
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper? db;
        private const string LogFile = "logs.txt";
        private int? editingTransactionId = null;
        private List<Transaction>? allTransactions = null;

        public PlotModel IncomeExpenseModel { get; set; } = new PlotModel();
        public PlotModel ExpenseDistributionModel { get; set; } = new PlotModel();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                // Set DataContext for binding
                this.DataContext = this;

                // Initialize DatePicker
                DateInput.SelectedDate = DateTime.Now;
                
                db = new DatabaseHelper();
                
                // Load categories dynamically
                LoadCategories();
                
                // Load transaction data
                LoadData();
                AddLog("✅ Application started successfully");
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Error initializing application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (db == null)
                {
                    AddLog("❌ Error: Database not initialized");
                    return;
                }

                // Validate inputs
                if (!ValidateInputs())
                    return;

                var date = DateInput.SelectedDate ?? DateTime.Now;
                var category = (CategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Other";
                var amount = decimal.Parse(AmountInput.Text.Trim());
                var notes = NotesInput.Text.Trim();

                if (editingTransactionId.HasValue)
                {
                    // Update existing transaction
                    db.UpdateTransaction(editingTransactionId.Value, date, category, amount, notes);
                    AddLog($"✏️ Updated [{editingTransactionId.Value}] {date:yyyy-MM-dd} | {category} | ₹{amount:F2} | {notes}");
                    editingTransactionId = null;
                }
                else
                {
                    // Add new transaction
                    db.AddTransaction(date, category, amount, notes);
                    AddLog($"✅ Added | {date:yyyy-MM-dd} | {category} | ₹{amount:F2} | {notes}");
                }
                
                // Clear inputs on success
                DateInput.SelectedDate = DateTime.Now;
                CategoryInput.SelectedIndex = 0;
                AmountInput.Text = string.Empty;
                NotesInput.Text = string.Empty;
                AddButton.Content = "➕ Add Transaction";

                LoadData();
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error: {ex.Message}");
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Transaction transaction)
                {
                    // Populate form with transaction data
                    DateInput.SelectedDate = transaction.Date;
                    CategoryInput.SelectedItem = CategoryInput.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => item.Content.ToString() == transaction.Category) ?? CategoryInput.Items[0];
                    AmountInput.Text = transaction.Amount.ToString();
                    NotesInput.Text = transaction.Notes;
                    
                    editingTransactionId = transaction.Id;
                    AddButton.Content = "💾 Update Transaction";
                    AddLog($"📝 Editing transaction [{transaction.Id}] - Make changes and click Update");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error editing transaction: {ex.Message}");
            }
        }

        private void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Transaction transaction && db != null)
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"Are you sure you want to delete this transaction?\n\nDate: {transaction.Date:yyyy-MM-dd}\nCategory: {transaction.Category}\nAmount: ₹{transaction.Amount:F2}",
                        "Delete Transaction",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.DeleteTransaction(transaction.Id);
                        AddLog($"🗑️ Deleted [{transaction.Id}] {transaction.Date:yyyy-MM-dd} | {transaction.Category} | ₹{transaction.Amount:F2}");
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error deleting transaction: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            // Validate date
            if (DateInput.SelectedDate == null)
            {
                AddLog("⚠️ Validation: Please select a valid date");
                DateInput.Focus();
                return false;
            }

            // Validate category
            if (CategoryInput.SelectedItem == null)
            {
                AddLog("⚠️ Validation: Please select a category");
                CategoryInput.Focus();
                return false;
            }

            // Validate amount
            var amountText = AmountInput.Text.Trim();
            if (string.IsNullOrEmpty(amountText))
            {
                AddLog("⚠️ Validation: Please enter an amount");
                AmountInput.Focus();
                return false;
            }

            if (!decimal.TryParse(amountText, out decimal amount))
            {
                AddLog("⚠️ Validation: Please enter a valid number for amount");
                AmountInput.Focus();
                return false;
            }

            if (amount <= 0)
            {
                AddLog("⚠️ Validation: Amount must be greater than 0");
                AmountInput.Focus();
                return false;
            }

            return true;
        }

        private void LoadData()
        {
            try
            {
                if (db == null)
                    return;

                allTransactions = db.GetTransactions();
                TransactionsGrid.ItemsSource = allTransactions;
                
                var summary = db.GetSummary();
                IncomeLabel.Text = $"Income: ₹{summary.TotalIncome:F2}";
                ExpenseLabel.Text = $"Expenses: ₹{summary.TotalExpenses:F2}";
                BalanceLabel.Text = $"Balance: ₹{summary.Balance:F2}";

                // Build charts
                BuildIncomeExpenseChart();
                BuildExpenseDistributionChart();

                // Update DataContext for charts
                IncomeExpenseChart.Model = IncomeExpenseModel;
                ExpenseDistributionChart.Model = ExpenseDistributionModel;
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error loading transactions: {ex.Message}");
            }
        }

        private void AddLog(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
                LogTextBox.AppendText(logEntry);
                LogTextBox.ScrollToEnd();
            }
            catch
            {
                // Silently fail if logging fails
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n";
                File.AppendAllText(LogFile, logMessage);
            }
            catch
            {
                // Silently fail if logging fails
            }
        }

        private void LoadCategories()
        {
            try
            {
                if (db == null)
                    return;

                var categories = db.GetCategories();
                
                // Populate CategoryInput ComboBox with categories separated by Income/Expense
                CategoryInput.Items.Clear();
                
                // Add Income categories first
                var incomeCategories = categories.Where(c => c.Type == "Income").OrderBy(c => c.Name).ToList();
                foreach (var category in incomeCategories)
                {
                    CategoryInput.Items.Add(new ComboBoxItem { Content = category.Name });
                }
                
                // Add separator
                CategoryInput.Items.Add(new Separator());
                
                // Add Expense categories
                var expenseCategories = categories.Where(c => c.Type == "Expense").OrderBy(c => c.Name).ToList();
                foreach (var category in expenseCategories)
                {
                    CategoryInput.Items.Add(new ComboBoxItem { Content = category.Name });
                }
                
                if (CategoryInput.Items.Count > 0)
                {
                    // Find first ComboBoxItem (skip Separator)
                    var firstItem = CategoryInput.Items.OfType<ComboBoxItem>().FirstOrDefault();
                    if (firstItem != null)
                        CategoryInput.SelectedItem = firstItem;
                }

                // Populate Categories DataGrid
                CategoriesGrid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error loading categories: {ex.Message}");
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (db == null)
                    return;

                // Validate inputs
                var categoryName = NewCategoryNameInput.Text.Trim();
                var categoryType = (NewCategoryTypeInput.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Expense";
                var categoryIcon = NewCategoryIconInput.Text.Trim();

                if (string.IsNullOrEmpty(categoryName))
                {
                    AddLog("⚠️ Validation: Please enter a category name");
                    return;
                }

                // Add category to database
                db.AddCategory(categoryName, categoryType, categoryIcon);
                AddLog($"✅ Added category | {categoryIcon} {categoryName} ({categoryType})");

                // Clear inputs
                NewCategoryNameInput.Text = string.Empty;
                NewCategoryTypeInput.SelectedIndex = 0;
                NewCategoryIconInput.Text = string.Empty;

                // Reload categories
                LoadCategories();
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error adding category: {ex.Message}");
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Category category && db != null)
                {
                    NewCategoryNameInput.Text = category.Name;
                    NewCategoryTypeInput.SelectedItem = NewCategoryTypeInput.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => item.Content.ToString() == category.Type) ?? NewCategoryTypeInput.Items[0];
                    NewCategoryIconInput.Text = category.Icon;

                    AddLog($"📝 Editing category [{category.Id}] - Make changes and click 'Update' (feature coming soon)");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error editing category: {ex.Message}");
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Category category && db != null)
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"Are you sure you want to delete this category?\n\nName: {category.Name}\nType: {category.Type}",
                        "Delete Category",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        db.DeleteCategory(category.Id);
                        AddLog($"🗑️ Deleted category [{category.Id}] {category.Icon} {category.Name}");
                        LoadCategories();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error deleting category: {ex.Message}");
            }
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allTransactions == null || allTransactions.Count == 0)
                {
                    AddLog("⚠️ No transactions to filter");
                    return;
                }

                var keyword = SearchKeywordInput.Text.Trim().ToLower();
                var fromDate = FilterFromDate.SelectedDate;
                var toDate = FilterToDate.SelectedDate;

                var filtered = allTransactions.AsEnumerable();

                // Apply keyword filter
                if (!string.IsNullOrEmpty(keyword))
                {
                    filtered = filtered.Where(t => 
                        t.Category.ToLower().Contains(keyword) || 
                        t.Notes.ToLower().Contains(keyword));
                }

                // Apply date filters
                if (fromDate.HasValue)
                {
                    filtered = filtered.Where(t => t.Date >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filtered = filtered.Where(t => t.Date <= toDate.Value);
                }

                var filteredList = filtered.ToList();
                TransactionsGrid.ItemsSource = filteredList;

                AddLog($"🔍 Filtered {filteredList.Count} transactions (out of {allTransactions.Count})");
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error applying filter: {ex.Message}");
            }
        }

        private void BuildIncomeExpenseChart()
        {
            try
            {
                IncomeExpenseModel = new PlotModel
                {
                    Title = "Monthly Income vs Expenses",
                    Background = OxyColor.FromArgb(255, 255, 255, 255),
                    TitleFontSize = 14,
                    TitleFontWeight = 700
                };

                if (allTransactions == null || allTransactions.Count == 0)
                {
                    IncomeExpenseModel.Annotations.Add(new OxyPlot.Annotations.TextAnnotation 
                    { 
                        Text = "No data available",
                        TextPosition = new DataPoint(0.5, 0.5),
                        FontSize = 12
                    });
                    return;
                }

                // Group by month and calculate income/expense
                var monthlyData = allTransactions
                    .GroupBy(t => new { t.Date.Year, t.Date.Month })
                    .OrderBy(g => g.Key.Year)
                    .ThenBy(g => g.Key.Month)
                    .ToList();

                var incomeValues = new List<DataPoint>();
                var expenseValues = new List<DataPoint>();
                var months = new List<string>();

                int pointIndex = 0;
                foreach (var monthGroup in monthlyData)
                {
                    var monthDate = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1);
                    var monthStr = monthDate.ToString("MMM yyyy");
                    months.Add(monthStr);

                    var income = monthGroup
                        .Where(t => db != null && db.IsIncomeCategory(t.Category))
                        .Sum(t => (double)t.Amount);

                    var expense = monthGroup
                        .Where(t => db != null && !db.IsIncomeCategory(t.Category))
                        .Sum(t => (double)t.Amount);

                    incomeValues.Add(new DataPoint(pointIndex, income));
                    expenseValues.Add(new DataPoint(pointIndex, expense));
                    pointIndex++;
                }

                // Create line series for income
                var incomeSeries = new LineSeries
                {
                    Title = "📈 Income",
                    Color = OxyColor.FromArgb(255, 76, 175, 80),
                    StrokeThickness = 2,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4
                };
                foreach (var point in incomeValues)
                    incomeSeries.Points.Add(point);

                // Create line series for expenses
                var expenseSeries = new LineSeries
                {
                    Title = "📉 Expenses",
                    Color = OxyColor.FromArgb(255, 244, 67, 54),
                    StrokeThickness = 2,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4
                };
                foreach (var point in expenseValues)
                    expenseSeries.Points.Add(point);

                IncomeExpenseModel.Series.Add(incomeSeries);
                IncomeExpenseModel.Series.Add(expenseSeries);

                // Configure axes
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Month",
                    StringFormat = "0"
                };
                IncomeExpenseModel.Axes.Add(xAxis);

                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Amount (₹)",
                    StringFormat = "0.00"
                };
                IncomeExpenseModel.Axes.Add(yAxis);

                // Add legend
                IncomeExpenseModel.IsLegendVisible = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error building income/expense chart: {ex.Message}");
            }
        }

        private void BuildExpenseDistributionChart()
        {
            try
            {
                ExpenseDistributionModel = new PlotModel
                {
                    Title = "Monthly Expense Distribution by Category",
                    Background = OxyColor.FromArgb(255, 255, 255, 255),
                    TitleFontSize = 14,
                    TitleFontWeight = 700
                };

                if (allTransactions == null || allTransactions.Count == 0)
                {
                    ExpenseDistributionModel.Annotations.Add(new OxyPlot.Annotations.TextAnnotation 
                    { 
                        Text = "No data available",
                        TextPosition = new DataPoint(0.5, 0.5),
                        FontSize = 12
                    });
                    return;
                }

                // Get current month or latest month with data
                var now = DateTime.Now;
                var currentMonthTransactions = allTransactions
                    .Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month)
                    .ToList();

                // If current month has no data, get the latest month with data
                if (currentMonthTransactions.Count == 0)
                {
                    var latestMonth = allTransactions
                        .OrderByDescending(t => t.Date)
                        .FirstOrDefault();

                    if (latestMonth != null)
                    {
                        currentMonthTransactions = allTransactions
                            .Where(t => t.Date.Year == latestMonth.Date.Year && t.Date.Month == latestMonth.Date.Month)
                            .ToList();
                    }
                }

                // Group expenses by category
                var expensesByCategory = currentMonthTransactions
                    .Where(t => db != null && !db.IsIncomeCategory(t.Category))
                    .GroupBy(t => t.Category)
                    .OrderByDescending(g => g.Sum(t => t.Amount))
                    .ToList();

                if (expensesByCategory.Count == 0)
                {
                    ExpenseDistributionModel.Annotations.Add(new OxyPlot.Annotations.TextAnnotation 
                    { 
                        Text = "No expenses found",
                        TextPosition = new DataPoint(0.5, 0.5),
                        FontSize = 12
                    });
                    return;
                }

                // Create pie series
                var pieSeries = new PieSeries
                {
                    InsideLabelPosition = 0.5,
                    AngleSpan = 360,
                    StartAngle = 0,
                    InnerDiameter = 0,
                    Stroke = OxyColors.White,
                    StrokeThickness = 2
                };

                // Define colors for categories
                var colors = new[]
                {
                    OxyColor.FromArgb(255, 244, 67, 54),    // Red
                    OxyColor.FromArgb(255, 33, 150, 243),   // Blue
                    OxyColor.FromArgb(255, 76, 175, 80),    // Green
                    OxyColor.FromArgb(255, 255, 152, 0),    // Orange
                    OxyColor.FromArgb(255, 156, 39, 176),   // Purple
                    OxyColor.FromArgb(255, 0, 188, 212),    // Cyan
                    OxyColor.FromArgb(255, 255, 87, 34),    // Deep Orange
                    OxyColor.FromArgb(255, 63, 81, 181)     // Indigo
                };

                int colorIndex = 0;
                foreach (var categoryGroup in expensesByCategory)
                {
                    var amount = categoryGroup.Sum(t => (double)t.Amount);
                    pieSeries.Slices.Add(new PieSlice(
                        $"{categoryGroup.Key}\n₹{amount:F0}",
                        amount
                    )
                    {
                        Fill = colors[colorIndex % colors.Length]
                    });
                    colorIndex++;
                }

                ExpenseDistributionModel.Series.Add(pieSeries);
                ExpenseDistributionModel.IsLegendVisible = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error building expense distribution chart: {ex.Message}");
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchKeywordInput.Text = string.Empty;
                FilterFromDate.SelectedDate = null;
                FilterToDate.SelectedDate = null;

                if (allTransactions != null)
                {
                    TransactionsGrid.ItemsSource = allTransactions;
                }

                AddLog("✅ Filters cleared");
            }
            catch (Exception ex)
            {
                LogError(ex);
                AddLog($"❌ Error clearing filters: {ex.Message}");
            }
        }
    }
}