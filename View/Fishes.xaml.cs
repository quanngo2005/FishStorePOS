using FishStore.Helper;
using FishStore.Models;
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
    /// Interaction logic for Fishes.xaml
    /// </summary>
    public partial class Fishes : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        string role = Session.Role; // Lấy role từ Session
        public Fishes()
        {
            InitializeComponent();
            LoadData();
            LoadCategories();
            if (role == "Staff")
            {
                Add.Visibility = Visibility.Collapsed;
                Edit.Visibility = Visibility.Collapsed;
                Save.Visibility = Visibility.Collapsed;
                TextBoxes.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadCategories()
        {
            var categories = db.Categories.ToList(); // Lấy full Category
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.DisplayMemberPath = "CategoryName";        // Hiển thị tên
            CategoryComboBox.SelectedValuePath = "CategoryId";  // Giá trị là ID
        }

        private void LoadData()
        {
            try
            {
                var fishes = db.Fish.Select(f => new
                {
                    f.FishId,
                    f.FishName,
                    f.Size,
                    f.Species,
                    f.Color,
                    f.Price,
                    f.QuantityAvailable,
                    f.ImageUrl,
                    f.Description,
                    CategoryName = f.Category.CategoryName, // Lấy tên
                    Status = f.Status ? "Available" : "Unavailable"
                }).ToList();
                FishesDataGrid.ItemsSource = fishes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadData();
                return;
            }
            string [] searchTerms = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var filteredFishes = db.Fish.Where(f => searchTerms.All(term => f.FishName.ToLower().Contains(term) || f.Species.ToLower().Contains(term))).Select(f => new
            {
                f.FishId,
                f.FishName,
                f.Species,
                f.Color,
                f.Price,
                f.QuantityAvailable,
                f.ImageUrl,
                f.Description,
                Status = f.Status ? "Available" : "Unavailable"
            }).ToList();
            FishesDataGrid.ItemsSource = filteredFishes;

        }
         
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Implement search functionality here
                SearchTextBox_TextChanged(sender, null);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string fishName = FishNameTextBox.Text.Trim();
            string species = SpeciesTextBox.Text.Trim();
            string color = ColorTextBox.Text.Trim();
            string size = ((ComboBoxItem)SizeComboBox.SelectedItem)?.Content.ToString();
            string categoryId = CategoryComboBox.SelectedValue?.ToString();
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantityAvailable = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();
            Boolean status = ActiveRadioButton.IsChecked == true ? true : false;

            if (string.IsNullOrEmpty(fishName) || string.IsNullOrEmpty(species) || string.IsNullOrEmpty(color) ||  string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please fill in all fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Fish.Any(f => f.FishName.ToLower() == fishName.ToLower()))
            {
                MessageBox.Show("Fish with this name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (quantityAvailable <=0)
            {
                MessageBox.Show("Quantity available cannot be lower than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Price must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            

            try
            {
                Fish newFish = new Fish
                {
                    FishId = IdGenerator.GenerateId("Fish"),
                    FishName = fishName,
                    Species = species,
                    Color = color,
                    Size = size,
                    Price = price,
                    CategoryId = categoryId,
                    QuantityAvailable = quantityAvailable,
                    ImageUrl = imageUrl,
                    Description = description,
                    Status = true // Default to available
                };
                db.Fish.Add(newFish);
                db.SaveChanges();
                MessageBox.Show("Fish added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                string detailedMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Error adding fish: {detailedMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


            }

        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedFish = FishesDataGrid.SelectedItem as dynamic;
            if (selectedFish == null)
            {
                MessageBox.Show("Please select a fish to edit.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var idProperty = selectedFish.GetType().GetProperty("FishId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have a valid FishId.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string fishId = idProperty.GetValue(selectedFish).ToString();
            var fishToEdit = db.Fish.FirstOrDefault(f => f.FishId == fishId);
            if (fishToEdit == null)
            {
                MessageBox.Show("Fish not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FishIdTextBox.Text = fishToEdit.FishId;
            FishNameTextBox.Text = fishToEdit.FishName;
            SpeciesTextBox.Text = fishToEdit.Species;
            SizeComboBox.SelectedItem = fishToEdit.Size; // hoặc SelectedValue
            CategoryComboBox.SelectedValue = fishToEdit.CategoryId;
            ColorTextBox.Text = fishToEdit.Color;
            PriceTextBox.Text = fishToEdit.Price.ToString();
            QuantityTextBox.Text = fishToEdit.QuantityAvailable.ToString();
            ImageUrlTextBox.Text = fishToEdit.ImageUrl;
            DescriptionTextBox.Text = fishToEdit.Description;
            ActiveRadioButton.IsChecked = fishToEdit.Status;
            InactiveRadioButton.IsChecked = !fishToEdit.Status;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            string fishName = FishNameTextBox.Text.Trim();
            string species = SpeciesTextBox.Text.Trim();
            string color = ColorTextBox.Text.Trim();
            string size = ((ComboBoxItem)SizeComboBox.SelectedItem)?.Content.ToString();
            string categoryId = CategoryComboBox.SelectedValue?.ToString();
            decimal price = PriceTextBox.Text.Trim() != "" ? decimal.Parse(PriceTextBox.Text.Trim()) : 0;
            int quantityAvailable = QuantityTextBox.Text.Trim() != "" ? int.Parse(QuantityTextBox.Text.Trim()) : 0;
            string imageUrl = ImageUrlTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            if (string.IsNullOrEmpty(fishName) || string.IsNullOrEmpty(species) || string.IsNullOrEmpty(color) || string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Please fill in all fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (db.Fish.Any(f => f.FishName.ToLower() == fishName.ToLower()))
            {
                MessageBox.Show("Fish with this name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (quantityAvailable <= 0)
            {
                MessageBox.Show("Quantity available cannot be lower than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (price <= 0)
            {
                MessageBox.Show("Price must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var selectedFish = FishesDataGrid.SelectedItem as dynamic;
            if (selectedFish == null)
            {
                MessageBox.Show("Please select a fish to save changes.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var idProperty = selectedFish.GetType().GetProperty("FishId");
            if (idProperty == null)
            {
                MessageBox.Show("Selected item does not have a valid FishId.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string fishId = idProperty.GetValue(selectedFish).ToString();
            
            try
            {
                var fishToEdit = db.Fish.FirstOrDefault(f => f.FishId == fishId);
                if (fishToEdit == null)
                {
                    MessageBox.Show("Fish not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                fishToEdit.FishName = fishName;
                fishToEdit.Species = species;
                fishToEdit.Color = color;
                fishToEdit.Price = price;
                fishToEdit.Size = size;
                fishToEdit.CategoryId = categoryId;
                fishToEdit.QuantityAvailable = quantityAvailable;
                fishToEdit.ImageUrl = imageUrl;
                fishToEdit.Description = description;
                fishToEdit.Status = ActiveRadioButton.IsChecked == true;
                db.SaveChanges();
                MessageBox.Show("Fish updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearFields_Click(sender, e);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding fish: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ClearFields_Click(sender, e);
            LoadData();
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var selectedFish = FishesDataGrid.SelectedItem as dynamic;
            if (selectedFish is not null)
                {
                    var idProperty = selectedFish.GetType().GetProperty("FishId");
                    if (idProperty == null)
                    {
                        MessageBox.Show("Selected item does not have a valid FishId.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    string fishId = idProperty.GetValue(selectedFish).ToString();
                    var fishToEdit = db.Fish.FirstOrDefault(f => f.FishId == fishId);
                    if (fishToEdit != null)
                    {
                        fishToEdit.Status = !fishToEdit.Status; // Toggle status
                        db.SaveChanges();
                        MessageBox.Show($"Fish status updated to {(fishToEdit.Status ? "Available" : "Unavailable")}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Fish not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
        }
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            FishIdTextBox.Text = string.Empty;
            FishNameTextBox.Text = string.Empty;
            SpeciesTextBox.Text = string.Empty;
            ColorTextBox.Text = string.Empty;
            SizeComboBox.SelectedIndex = -1;
            PriceTextBox.Text = string.Empty;
            CategoryComboBox.SelectedIndex = -1; // Clear selection
            QuantityTextBox.Text = string.Empty;
            ImageUrlTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            ActiveRadioButton.IsChecked = false;
            InactiveRadioButton.IsChecked = false;

        }

        
    }
}
