using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PersonalFinanceTracker.Data
{
    /// <summary>
    /// Transaction model class
    /// </summary>
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Summary model class
    /// </summary>
    public class Summary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Category model class
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Income" or "Expense"
        public string Icon { get; set; } = string.Empty;
    }

    /// <summary>
    /// Handles database operations for the Personal Finance Tracker
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        private const string DatabaseFileName = "finance.db";

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName);
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        /// <summary>
        /// Initializes the database and creates the Transactions and Categories tables if they don't exist
        /// </summary>
        private void InitializeDatabase()
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                // Create Transactions table
                string createTransactionsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Transactions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        Notes TEXT
                    )";

                using (SqliteCommand command = new SqliteCommand(createTransactionsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Create Categories table
                string createCategoriesTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Type TEXT NOT NULL,
                        Icon TEXT
                    )";

                using (SqliteCommand command = new SqliteCommand(createCategoriesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Insert default categories if table is empty
                string checkCategoriesQuery = "SELECT COUNT(*) FROM Categories";
                using (SqliteCommand command = new SqliteCommand(checkCategoriesQuery, connection))
                {
                    int categoryCount = Convert.ToInt32(command.ExecuteScalar());
                    if (categoryCount == 0)
                    {
                        string insertDefaultCategoriesQuery = @"
                            INSERT INTO Categories (Name, Type, Icon) VALUES
                            ('Salary', 'Income', '💼'),
                            ('Bonus', 'Income', '🎁'),
                            ('Investment', 'Income', '📈'),
                            ('Gift', 'Income', '🎉'),
                            ('Other Income', 'Income', '💰'),
                            ('Food', 'Expense', '🍔'),
                            ('Rent', 'Expense', '🏠'),
                            ('Transport', 'Expense', '🚗'),
                            ('Entertainment', 'Expense', '🎬'),
                            ('Utilities', 'Expense', '💡'),
                            ('Healthcare', 'Expense', '🏥'),
                            ('Other', 'Expense', '📦')";

                        using (SqliteCommand insertCommand = new SqliteCommand(insertDefaultCategoriesQuery, connection))
                        {
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new transaction to the database
        /// </summary>
        public void AddTransaction(DateTime date, string category, decimal amount, string notes)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO Transactions (Date, Category, Amount, Notes)
                    VALUES (@date, @category, @amount, @notes)";

                using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@category", category);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@notes", notes ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves all transactions from the database
        /// </summary>
        public List<Transaction> GetTransactions()
        {
            var transactions = new List<Transaction>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Id, Date, Category, Amount, Notes FROM Transactions ORDER BY Date DESC";

                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new Transaction
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Date = DateTime.Parse(reader["Date"].ToString() ?? DateTime.Now.ToString("yyyy-MM-dd")),
                                Category = reader["Category"].ToString() ?? string.Empty,
                                Amount = decimal.Parse(reader["Amount"].ToString() ?? "0"),
                                Notes = reader["Notes"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }

            return transactions;
        }

        /// <summary>
        /// Gets a summary of income, expenses, and balance
        /// </summary>
        public Summary GetSummary()
        {
            decimal totalIncome = 0;
            decimal totalExpenses = 0;

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Category, Amount FROM Transactions";

                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string category = reader["Category"].ToString() ?? "";
                            decimal amount = decimal.Parse(reader["Amount"].ToString() ?? "0");

                            // Categorize as income or expense based on category
                            if (IsIncomeCategory(category))
                            {
                                totalIncome += amount;
                            }
                            else
                            {
                                totalExpenses += amount;
                            }
                        }
                    }
                }
            }

            return new Summary
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = totalIncome - totalExpenses
            };
        }

        /// <summary>
        /// Deletes a transaction from the database by ID
        /// </summary>
        public void DeleteTransaction(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Transactions WHERE Id = @id";

                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates an existing transaction in the database
        /// </summary>
        public void UpdateTransaction(int id, DateTime date, string category, decimal amount, string notes)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string updateQuery = @"
                    UPDATE Transactions 
                    SET Date = @date, Category = @category, Amount = @amount, Notes = @notes 
                    WHERE Id = @id";

                using (SqliteCommand command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@category", category);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@notes", notes ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves all categories from the database
        /// </summary>
        public List<Category> GetCategories()
        {
            var categories = new List<Category>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Id, Name, Type, Icon FROM Categories ORDER BY Name ASC";

                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString() ?? string.Empty,
                                Type = reader["Type"].ToString() ?? string.Empty,
                                Icon = reader["Icon"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Adds a new category to the database
        /// </summary>
        public void AddCategory(string name, string type, string icon = "")
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO Categories (Name, Type, Icon)
                    VALUES (@name, @type, @icon)";

                using (SqliteCommand command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@icon", icon ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deletes a category from the database by ID
        /// </summary>
        public void DeleteCategory(int id)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Categories WHERE Id = @id";

                using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates an existing category in the database
        /// </summary>
        public void UpdateCategory(int id, string name, string type, string icon = "")
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string updateQuery = @"
                    UPDATE Categories 
                    SET Name = @name, Type = @type, Icon = @icon 
                    WHERE Id = @id";

                using (SqliteCommand command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@icon", icon ?? string.Empty);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Determines if a category is an income category by looking it up in the database
        /// </summary>
        public bool IsIncomeCategory(string category)
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Type FROM Categories WHERE Name = @name";

                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", category);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString() == "Income";
                    }
                }
            }

            // Fallback to hardcoded list if category not found
            string[] incomeCategories = { "Salary", "Bonus", "Investment", "Gift", "Other Income" };
            return incomeCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
        }
    }
}
