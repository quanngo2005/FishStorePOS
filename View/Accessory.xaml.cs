using FishStore.Helper;
using FishStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Accessory.xaml
    /// </summary>
    public partial class Accessory : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        string role = Session.Role; // Lấy role từ Session
        public Accessory()
        {
            InitializeComponent();
            LoadData();
            if (role == "Staff")
            {
                Add.Visibility = Visibility.Collapsed;
                Edit.Visibility = Visibility.Collapsed;
                Save.Visibility = Visibility.Collapsed;
                TextBoxes.Visibility = Visibility.Collapsed;
            }

        }

        private void LoadData()
        {
            try
            {

                var accessories = db.AquariumAccessories
                            .Include(a => a.Category) // Chắc chắn Include hoạt động
                            .Select(a => new
                            {
                                a.AccessoryId,
                                a.AccessoryName,
                                CategoryName = a.Category != null ? a.Category.CategoryName : "No Category",
                                a.Description,
                                a.Price,
                                a.QuantityAvailable,
                                a.ImageUrl,
                                Status = a.Status ? "Available" : "Unavailable"
                            })
                            .ToList();

                AccessoriesDataGrid.ItemsSource = accessories; // Gán vào DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

            // Load ComboBox
            var categories = db.Categories.ToList();
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            CategoryComboBox.SelectedValuePath = "CategoryId";
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
            string categoryId = CategoryComboBox.SelectedValue?.ToString();
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantity = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            Boolean status = ActiveRadioButton.IsChecked == true ? true : false;

            // ✅ Validate các trường bằng Validator
             bool isValid = Validator.ValidateFields(
            (Validator.IsNullOrEmpty(accessoryName), "Accessory name is required."),
            (Validator.IsNullOrEmpty(description), "Description is required."),
            (Validator.IsNullOrEmpty(categoryId), "Category is required."),
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
                    AccessoryId = IdGenerator.GenerateId("Accessory"),
                    AccessoryName = accessoryName,
                    Description = description,
                    CategoryId = categoryId,
                    Price = price,
                    QuantityAvailable = quantity,
                    ImageUrl = imageUrl,
                    Status = status
                };
                db.AquariumAccessories.Add(newAccessory);
                db.SaveChanges();
                MessageBox.Show("New accessory created successfully.");
                LoadData();
                Helper.Helper.ClearFormFields(TextBoxes);
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
                MessageBox.Show("Please select an accessory to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var idProperty = selectedAccessory.GetType().GetProperty("AccessoryId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have an AccessoryId property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string accessoryId = idProperty.GetValue(selectedAccessory)?.ToString();
            if (string.IsNullOrEmpty(accessoryId))
            {
                MessageBox.Show("Accessory ID is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingAccessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accessoryId);
            if (existingAccessory == null)
            {
                MessageBox.Show("Accessory not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Gán dữ liệu lên form để người dùng chỉnh sửa
            AccessoryIdTextBox.Text = existingAccessory.AccessoryId;
            AccessoryNameTextBox.Text = existingAccessory.AccessoryName;
            DescriptionTextBox.Text = existingAccessory.Description;
            PriceTextBox.Text = existingAccessory.Price.ToString("0.##");
            QuantityTextBox.Text = existingAccessory.QuantityAvailable.ToString();
            ImageUrlTextBox.Text = existingAccessory.ImageUrl;

            // Gán giá trị cho ComboBox dựa trên CategoryId
            CategoryComboBox.SelectedValue = existingAccessory.CategoryId;

            // Set trạng thái radio button
            if (existingAccessory.Status)
                ActiveRadioButton.IsChecked = true;
            else
                InactiveRadioButton.IsChecked = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string accessoryName = AccessoryNameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            string categoryId = CategoryComboBox.SelectedValue?.ToString();
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantity = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            bool status = ActiveRadioButton.IsChecked == true;

            // ✅ Validate
            bool isValid = Validator.ValidateFields(
                (Validator.IsNullOrEmpty(accessoryName), "Accessory name is required."),
                (Validator.IsNullOrEmpty(description), "Description is required."),
                (Validator.IsNullOrEmpty(categoryId), "Category is required."),
                (price <= 0, "Price must be greater than 0."),
                (quantity < 0, "Quantity cannot be negative."),
                (Validator.IsNullOrEmpty(imageUrl), "Image URL is required.")
            );
            if (!isValid) return;

            var selectedAccessory = AccessoriesDataGrid.SelectedItem as dynamic;
            if (selectedAccessory == null)
            {
                MessageBox.Show("Please select an accessory to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var idProperty = selectedAccessory.GetType().GetProperty("AccessoryId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have an AccessoryId property.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string accessoryId = idProperty.GetValue(selectedAccessory)?.ToString();
            if (string.IsNullOrEmpty(accessoryId))
            {
                MessageBox.Show("Accessory ID is not valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var existingAccessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accessoryId);
            if (existingAccessory == null)
            {
                MessageBox.Show("Accessory not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ✅ Kiểm tra trùng tên trừ chính nó
            if (Validator.IsDuplicate(() =>
                    db.AquariumAccessories.Any(a => a.AccessoryName == accessoryName && a.AccessoryId != accessoryId),
                    "Accessory with this name already exists."))
                return;

            try
            {
                existingAccessory.AccessoryName = accessoryName;
                existingAccessory.Description = description;
                existingAccessory.CategoryId = categoryId;
                existingAccessory.Price = price;
                existingAccessory.QuantityAvailable = quantity;
                existingAccessory.ImageUrl = imageUrl;
                existingAccessory.Status = status;

                db.SaveChanges();
                MessageBox.Show("Accessory updated successfully.");
                LoadData();
                Helper.Helper.ClearFormFields(TextBoxes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating accessory: " + ex.Message);
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
