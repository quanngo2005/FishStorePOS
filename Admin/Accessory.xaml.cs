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
using FishStore.Helper;


namespace FishStore.Admin
{
    /// <summary>
    /// Interaction logic for Accessory.xaml
    /// </summary>
    public partial class Accessory : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        public Accessory()
        {
            InitializeComponent();
            LoadData();
            LoadCategories();
        }

        private void LoadData()
        {
            try
            {
                var accessories = db.AquariumAccessories.ToList();
                AccessoriesDataGrid.ItemsSource = accessories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        private void LoadCategories()
        {
            var category = db.AquariumAccessories.Select(a => a.Category).Distinct().ToList();
            CategoryComboBox.ItemsSource = category;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Implement search functionality here
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

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filteredAccessories = db.AquariumAccessories
                .Where(f => f.AccessoryName.ToLower().Contains(searchText) || f.Description.ToLower().Contains(searchText))
                .ToList();
            AccessoriesDataGrid.ItemsSource = filteredAccessories;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string accessoryName = AccessoryNameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string category = CategoryComboBox.SelectedItem as string;
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantity = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            Boolean status = ActiveRadioButton.IsChecked == true ? true : false;

            // ✅ Validate các trường bằng Validator
             bool isValid = Validator.ValidateFields(
            (Validator.IsNullOrEmpty(accessoryName), "Accessory name is required."),
            (Validator.IsNullOrEmpty(description), "Description is required."),
            (Validator.IsNullOrEmpty(category), "Category is required."),
            (price <= 0, "Price must be greater than 0."),
            (quantity < 0, "Quantity cannot be negative."),
            (Validator.IsNullOrEmpty(imageUrl), "Image URL is required.")
            );
            if (!isValid) return;

            // ✅ Check trùng tên
            if (Validator.IsDuplicate(() => db.AquariumAccessories.Any(a => a.AccessoryName == accessoryName),
                                      "Accessory with this name already exists."))
            return;
            try
            {
                AquariumAccessory newAccessory = new AquariumAccessory
                {
                    AccessoryId = IdGenerator.GenerateId("AC"),
                    AccessoryName = accessoryName,
                    Description = description,
                    Category = category,
                    Price = price,
                    QuantityAvailable = quantity,
                    ImageUrl = imageUrl,
                    Status = status
                };
                db.AquariumAccessories.Add(newAccessory);
                db.SaveChanges();
                MessageBox.Show("New accessory created successfully.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating new accessory: " + ex.Message);
                return;
            }     
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccessory = AccessoriesDataGrid.SelectedItem as dynamic;
            if (selectedAccessory == null)
            {
                MessageBox.Show("Please select an accessory to edit.");
                return;
            }
            var idProperty =  selectedAccessory.GetType().GetProperty("AccessoryID");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have an AccessoryID property.");
                return;
            }
            string accessoryId = idProperty.GetValue(selectedAccessory)?.ToString();
            if (string.IsNullOrEmpty(accessoryId))
            {
                MessageBox.Show("Accessory ID is not valid.");
                return;
            }
            var existingAccessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accessoryId);
            if (existingAccessory == null)
            {
                MessageBox.Show("Accessory not found.");
                return;
            }
            existingAccessory.AccessoryId = AccessoryIdTextBox.Text.Trim();
            existingAccessory.AccessoryName = AccessoryNameTextBox.Text.Trim();
            existingAccessory.Description = DescriptionTextBox.Text.Trim();
            existingAccessory.Category = CategoryComboBox.SelectedItem as string;
            existingAccessory.Price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            existingAccessory.QuantityAvailable = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            existingAccessory.ImageUrl = ImageUrlTextBox.Text.Trim();
            ActiveRadioButton.IsChecked = existingAccessory.Status;
            InactiveRadioButton.IsChecked = !existingAccessory.Status;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            string accessoryName = AccessoryNameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string category = CategoryComboBox.SelectedItem as string;
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantity = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            Boolean status = ActiveRadioButton.IsChecked == true ? true : false;

            // ✅ Validate các trường bằng Validator
            bool isValid = Validator.ValidateFields(
           (Validator.IsNullOrEmpty(accessoryName), "Accessory name is required."),
           (Validator.IsNullOrEmpty(description), "Description is required."),
           (Validator.IsNullOrEmpty(category), "Category is required."),
           (price <= 0, "Price must be greater than 0."),
           (quantity < 0, "Quantity cannot be negative."),
           (Validator.IsNullOrEmpty(imageUrl), "Image URL is required.")
           );
            if (!isValid) return;

            // ✅ Check trùng tên
            if (Validator.IsDuplicate(() => db.AquariumAccessories.Any(a => a.AccessoryName == accessoryName),
                                      "Accessory with this name already exists."))
                return;
            var selectedAccessory = AccessoriesDataGrid.SelectedItem as dynamic;
            if (selectedAccessory == null)
            {
                MessageBox.Show("Please select an accessory to save changes.");
                return;
            }
            var idProperty = selectedAccessory.GetType().GetProperty("AccessoryID");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have an AccessoryID property.");
                return;
            }
            string accessoryId = idProperty.GetValue(selectedAccessory)?.ToString();
            try
            {
                var existingAccessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accessoryId);
                if (existingAccessory == null)
                {
                    MessageBox.Show("Accessory not found.");
                    return;
                }
                
                existingAccessory.AccessoryName = AccessoryNameTextBox.Text.Trim();
                existingAccessory.Description = DescriptionTextBox.Text.Trim();
                existingAccessory.Category = CategoryComboBox.SelectedItem as string;
                existingAccessory.Price = price;
                existingAccessory.QuantityAvailable = quantity;
                existingAccessory.ImageUrl = imageUrl;
                existingAccessory.Status = status;
                db.SaveChanges();
                MessageBox.Show("Accessory updated successfully.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating accessory: " + ex.Message);
                return;
            }

        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Helper.Helper.ClearFormFields(TextBoxes);
            LoadData();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccessory = AccessoriesDataGrid.SelectedItem as dynamic;
            if (selectedAccessory == null)
            {
                MessageBox.Show("Please select an accessory to change status.");
                return;
            }
            var idProperty = selectedAccessory.GetType().GetProperty("AccessoryID");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have an AccessoryID property.");
                return;
            }
            string accessoryId = idProperty.GetValue(selectedAccessory)?.ToString();
            if (string.IsNullOrEmpty(accessoryId))
            {
                MessageBox.Show("Accessory ID is not valid.");
                return;
            }
            var existingAccessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accessoryId);
            if (existingAccessory == null)
            {
                MessageBox.Show("Accessory not found.");
                return;
            }
            existingAccessory.Status = !existingAccessory.Status; // Toggle status
        }
    }
}
