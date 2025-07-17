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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        public Customer()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var customers = db.Customers.Select(c => new
                {
                    c.CustomerId,
                    c.FullName,
                    c.Phone,
                    c.Email,
                    c.Address,
                    Status = c.Status ? "Active" : "Inactive"
                }).ToList();
                CustomersDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {

            string fullName = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || (!isActive.HasValue && !isInactive.HasValue))
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Customers.Any(c => c.Email == email))
            {
                MessageBox.Show("Email already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Customers.Any(c => c.Phone == phone))
            {
                MessageBox.Show("Phone number already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var newCustomer = new Models.Customer
                {
                    CustomerId = IdGenerator.GenerateId("Customer"),
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Address = address,
                    Status = isActive.HasValue ? isActive.Value : false // Default to false if both are unchecked
                };
                db.Customers.Add(newCustomer);
                db.SaveChanges();
                MessageBox.Show("Customer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add customer: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedCustomer = CustomersDataGrid.SelectedItem as dynamic;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var idProperty = selectedCustomer.GetType().GetProperty("CustomerId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have a CustomerId property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string customerId = idProperty.GetValue(selectedCustomer)?.ToString();
            if (string.IsNullOrEmpty(customerId))
            {
                MessageBox.Show("Selected customer does not have a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IdTextBox.Text = customer.CustomerId;
            FullNameTextBox.Text = customer.FullName;
            EmailTextBox.Text = customer.Email;
            PhoneTextBox.Text = customer.Phone;
            AddressTextBox.Text = customer.Address;
            ActiveRadioButton.IsChecked = customer.Status;
            InactiveRadioButton.IsChecked = !customer.Status;

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string fullname = FullNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string phone = PhoneTextBox.Text.Trim();
            string address = AddressTextBox.Text.Trim();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;
            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || (!isActive.HasValue && !isInactive.HasValue))
            {
                MessageBox.Show("Please fill in all fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedCustomer = CustomersDataGrid.SelectedItem as dynamic;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to save changes.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var idProperty = selectedCustomer.GetType().GetProperty("CustomerId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have a CustomerId property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            string customerId = idProperty.GetValue(selectedCustomer)?.ToString();
            if (string.IsNullOrEmpty(customerId))
            {
                MessageBox.Show("Selected customer does not have a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                customer.FullName = fullname;
                customer.Email = email;
                customer.Phone = phone;
                customer.Address = address;
                customer.Status = isActive.HasValue ? isActive.Value : false; // Default to false if both are unchecked
                db.SaveChanges();
                MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update customer: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData(); 
            ClearFields_Click(sender, e);
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedCustomer = CustomersDataGrid.SelectedItem as dynamic;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to change status.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var idProperty = selectedCustomer.GetType().GetProperty("CustomerId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have a CustomerId property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string customerId = idProperty.GetValue(selectedCustomer)?.ToString();
            if (string.IsNullOrEmpty(customerId))
            {
                MessageBox.Show("Selected customer does not have a valid ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                customer.Status = !customer.Status; // Toggle status
                db.SaveChanges();
                MessageBox.Show("Customer status changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change customer status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var filteredCustomers = db.Customers
                    .Where(cu => searchTerms.All(term =>
                        cu.FullName.ToLower().Contains(term) ||
                        cu.Email.ToLower().Contains(term) ||
                        cu.CustomerId.ToLower().Contains(searchText)))
                    .Select(cu => new
                    {
                        cu.CustomerId,
                        cu.FullName,
                        cu.Phone,
                        cu.Email,
                        cu.Address,

                        Status = cu.Status ? "Active" : "Inactive"
                    })
                    .ToList();
                CustomersDataGrid.ItemsSource = filteredCustomers;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to search Category: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Call the search method here
                SearchTextBox_TextChanged(sender, null);
            }
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            Application.Current.MainWindow = adminWindow;
            adminWindow.Show();
            this.Close();
        }
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            IdTextBox.Text = string.Empty;
            FullNameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
            ActiveRadioButton.IsChecked = false;
            InactiveRadioButton.IsChecked = false;
        }
    }
}
