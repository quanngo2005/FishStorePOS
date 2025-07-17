using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace FishStore.Admin
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : Window

    {
        ShopBanCaContext db = new ShopBanCaContext();
        Account selectedAccount = null;
        public Accounts()
        {
            InitializeComponent();         
            LoadData();
            LoadRole();
        }

        private void LoadData()
        {
            try
            {
                // Assuming you have a DataGrid named AccountsDataGrid in your XAML
                var accounts = db.Accounts
                    .Select(a => new
                    {
                        a.UserId,
                        a.Username,
                        a.FullName,
                        a.PasswordHash,
                        a.PhoneNumber,
                        a.Role,
                        Status = a.Status ? "Active" : "Inactive"
                    })
                    .ToList();

                AccountsDataGrid.ItemsSource = accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load accounts: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadRole()
        {
            try 
            {              
                var roles = db.Accounts.Select(a => a.Role).Distinct().ToList();
                RoleComboBox.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load roles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadData(); // Reload all data if search text is empty
                return;
            }
            string[] searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            try
            {
                var filteredAccounts = db.Accounts
                    .Where(a => searchTerms.All(term =>
                        a.Username.ToLower().Contains(term) ||
                        a.FullName.ToLower().Contains(term) ||
                        a.UserId.ToLower().Contains(searchText)))
                    .Select(a => new
                    {
                        a.UserId,
                        a.Username,
                        a.FullName,
                        a.PasswordHash,
                        a.PhoneNumber,
                        a.Role,
                        Status = a.Status ? "Active" : "Inactive"
                    })
                    .ToList();
                AccountsDataGrid.ItemsSource = filteredAccounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to search accounts: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
    
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Trigger search when Enter key is pressed
                SearchTextBox_TextChanged(sender, null);
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            string fullName = FullNameTextBox.Text.Trim();
            string phoneNumber = PhoneTextBox.Text.Trim();
            string role = RoleComboBox.SelectedItem?.ToString();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || !isActive.HasValue && !isInactive.HasValue)
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (username.Equals("Username") || fullName.Equals("Full Name")|| password.Equals("Password") || phoneNumber.Equals("Phone Number"))
            {
                MessageBox.Show("Please enter valid values for Username, Password, Full Name, and Phone Number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Accounts.Any(a => a.Username == username))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Duplicate Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Accounts.Any(a => a.PhoneNumber == phoneNumber))
            {
                MessageBox.Show("Phone number already exists. Please choose a different phone number.", "Duplicate Phone Number", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newAccount = new Account
            {
                UserId = IdGenerator.GenerateId("User"), // Generate a new UserId
                Username = username,
                PasswordHash = password, // Consider hashing the password before storing it
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Role = role,
                Status = isActive.HasValue ? isActive.Value : false // Default to false if neither is checked
            };
            try
            {
                db.Accounts.Add(newAccount);
                db.SaveChanges();
                MessageBox.Show("Account added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields_Click(sender, e); // Clear input fields after adding
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add account: " + ex.Message +
                    (ex.InnerException != null ? "\nInner: " + ex.InnerException.Message : ""),
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            var selected = AccountsDataGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Please select an account to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Get the UserId property from the selected item (anonymous type)
            var userIdProperty = selected.GetType().GetProperty("UserId");
            if (userIdProperty == null)
            {
                MessageBox.Show("Unable to determine selected account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string userId = userIdProperty.GetValue(selected)?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Unable to determine selected account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Find the account in the database
            var account = db.Accounts.FirstOrDefault(a => a.UserId == userId);
            if (account == null)
            {
                MessageBox.Show("Account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Populate the input fields with the selected account's data
            UserIdTextBox.Text = account.UserId;
            UsernameTextBox.Text = account.Username;
            PasswordTextBox.Text = account.PasswordHash;
            FullNameTextBox.Text = account.FullName ?? string.Empty;
            PhoneTextBox.Text = account.PhoneNumber;
            RoleComboBox.SelectedItem = account.Role;
            ActiveRadioButton.IsChecked = account.Status;
            InactiveRadioButton.IsChecked = !account.Status;

            
        }
        private void SaveAccount_Click(object sender, RoutedEventArgs e)
        {

            // Validate input
            string userId = UserIdTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            string fullName = FullNameTextBox.Text.Trim();
            string phoneNumber = PhoneTextBox.Text.Trim();
            string role = RoleComboBox.SelectedItem?.ToString();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("No account selected for update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role) || (!isActive.HasValue && !isInactive.HasValue))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (username.Equals("Username") || fullName.Equals("Full Name") || password.Equals("Password") || phoneNumber.Equals("Phone Number"))
            {
                MessageBox.Show("Please enter valid values for Username, Password, Full Name, and Phone Number.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Find the account in the database
            var account = db.Accounts.FirstOrDefault(a => a.UserId == userId);
            if (account == null)
            {
                MessageBox.Show("Account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check for duplicate username or phone number (excluding the current account)
            if (db.Accounts.Any(a => a.Username == username && a.UserId != userId))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Duplicate Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Accounts.Any(a => a.PhoneNumber == phoneNumber && a.UserId != userId))
            {
                MessageBox.Show("Phone number already exists. Please choose a different phone number.", "Duplicate Phone Number", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update account properties
            account.Username = username;
            account.PasswordHash = password; // In production, hash the password
            account.FullName = fullName;
            account.PhoneNumber = phoneNumber;
            account.Role = role;
            account.Status = isActive.HasValue ? isActive.Value : false;

            try
            {
                db.SaveChanges();
                MessageBox.Show("Account updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update account: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            ClearFields_Click(sender, e);
            LoadData();
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            Application.Current.MainWindow = adminWindow;
            adminWindow.Show();
            this.Close();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected account from the DataGrid
            var selected = AccountsDataGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Please select an account to change status.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show("Are you sure you want to change the status of this account?", "Confirm Change", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No, MessageBoxOptions.None);
            // Get the UserId property from the selected item (anonymous type)
            var userIdProperty = selected.GetType().GetProperty("UserId");
            if (userIdProperty == null)
            {
                MessageBox.Show("Unable to determine selected account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string userId = userIdProperty.GetValue(selected)?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Unable to determine selected account.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Find the account in the database
            var account = db.Accounts.FirstOrDefault(a => a.UserId == userId);
            if (account == null)
            {
                MessageBox.Show("Account not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Toggle the status
            account.Status = !account.Status;

            try
            {
                db.SaveChanges();
                MessageBox.Show($"Account status changed to {(account.Status ? "Active" : "Inactive")}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {   
            UserIdTextBox.Text = string.Empty;
            UsernameTextBox.Text = string.Empty;
            PasswordTextBox.Text = string.Empty;
            FullNameTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            RoleComboBox.SelectedIndex = -1;
            ActiveRadioButton.IsChecked = false;
            InactiveRadioButton.IsChecked = false;
        }
        

        
    }
}
