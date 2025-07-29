using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        Categories selectedCategory = null;
        string role = Session.Role; // Lấy role từ Session
        public Categories()
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
                var categories = db.Categories
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.CategoryName,
                        c.Description,
                        Status = c.Status ? "Active" : "Inactive"
                    })
                    .ToList();
                CategoriesDataGrid.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load categories: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }           
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText) )
            {
                LoadData(); // Reload all data if search text is empty
                return;
            }
            string[] searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            try
            {
                var filteredCategories = db.Categories
                    .Where(c => searchTerms.All(term =>
                        c.CategoryName.ToLower().Contains(term) ||
                        c.Description.ToLower().Contains(term) ||
                        c.CategoryId.ToLower().Contains(searchText)))
                    .Select(c => new
                    {
                        c.CategoryId,
                        c.CategoryName,
                        c.Description,
                        
                        Status = c.Status ? "Active" : "Inactive"
                    })
                    .ToList();
                CategoriesDataGrid.ItemsSource = filteredCategories;
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
                // Trigger search when Enter key is pressed
                SearchTextBox_TextChanged(sender, null);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = NameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;

            if (string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(description) || !isActive.HasValue && !isInactive.HasValue)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (isActive == true && isInactive == true)
            {
                MessageBox.Show("Please select only one status (Active or Inactive).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Categories.Any(c => c.CategoryName.ToLower() == categoryName.ToLower()))
            {
                MessageBox.Show("Category name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var newCategory = new Category
                {
                    CategoryId = IdGenerator.GenerateId("Category"),
                    CategoryName = categoryName,
                    Description = description,
                    Status = isActive.GetValueOrDefault()
                };
                db.Categories.Add(newCategory);
                db.SaveChanges();
                MessageBox.Show("Category added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Reload data to show the new category
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add category: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = CategoriesDataGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Please select a category to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy giá trị CategoryID từ item ẩn danh (anonymous object)
            var idProperty = selected.GetType().GetProperty("CategoryId");
            if (idProperty == null)
            {
                MessageBox.Show("Unable to determine selected category ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string categoryId = idProperty.GetValue(selected)?.ToString();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                MessageBox.Show("Selected category ID is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tìm category trong database
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                MessageBox.Show("Category not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Đổ dữ liệu lên form
            IdTextBox.Text = category.CategoryId;
            NameTextBox.Text = category.CategoryName ?? string.Empty;
            DescriptionTextBox.Text = category.Description ?? string.Empty;
            ActiveRadioButton.IsChecked = category.Status;
            InactiveRadioButton.IsChecked = !category.Status;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            string categoryName = NameTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            bool? isActive = ActiveRadioButton.IsChecked;
            bool? isInactive = InactiveRadioButton.IsChecked;

            if (string.IsNullOrEmpty(categoryName) || string.IsNullOrEmpty(description) || (!isActive.HasValue && !isInactive.HasValue))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (isActive == true && isInactive == true)
            {
                MessageBox.Show("Please select only one status (Active or Inactive).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected category from DataGrid
            var selected = CategoriesDataGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Please select a category to save changes.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the CategoryId property from the selected item (anonymous type)
            var idProperty = selected.GetType().GetProperty("CategoryId");
            if (idProperty == null)
            {
                MessageBox.Show("Unable to determine selected category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string categoryId = idProperty.GetValue(selected)?.ToString();
            if (string.IsNullOrEmpty(categoryId))
            {
                MessageBox.Show("Unable to determine selected category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Find the category in the database
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                MessageBox.Show("Category not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check for duplicate name (excluding current category)
            if (db.Categories.Any(c => c.CategoryName == categoryName && c.CategoryId != categoryId))
            {
                MessageBox.Show("Category name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update properties
            category.CategoryName = categoryName;
            category.Description = description;
            category.Status = isActive == true;

            try
            {
                db.SaveChanges();
                MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update category: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ClearFields_Click(sender, e);
            LoadData();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            // Get selected category from DataGrid
            var selected = CategoriesDataGrid.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show("Please select a category to save changes.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the CategoryId property from the selected item (anonymous type)
            var idProperty = selected.GetType().GetProperty("CategoryId");
            if (idProperty == null)
            {
                MessageBox.Show("Unable to determine selected category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string categoryId = idProperty.GetValue(selected)?.ToString();
            if (string.IsNullOrEmpty(categoryId))
            {
                MessageBox.Show("Unable to determine selected category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Find the category in the database
            var category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                MessageBox.Show("Category not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            category.Status = !category.Status; // Toggle status
            try
            {
                db.SaveChanges();
                MessageBox.Show("Category status changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData(); // Reload data to reflect the status change
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to change category status: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            NameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            ActiveRadioButton.IsChecked = false;
            InactiveRadioButton.IsChecked = false;
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }
}
