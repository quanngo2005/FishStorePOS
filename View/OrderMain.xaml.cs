using FishStore.Helper;
using FishStore.Models;
using FishStore.Service;
using FishStore.UserAuthenticaiton;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace FishStore.View
{
    /// <summary>
    /// Interaction logic for OrderMain.xaml
    /// </summary>
    public partial class OrderMain : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        private List<ProductViewModel> orderList = new List<ProductViewModel>();
        public OrderMain()
        {
            InitializeComponent();
            LoadData();
            LoadProducts();
            OrderListBox.ItemsSource = orderList;
        }

        private void LoadData()
        {
            var categories = db.Categories.ToList();
            CategoryListBox.ItemsSource = categories.Select(c => new Category
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        public class ProductViewModel : INotifyPropertyChanged {
            public string? Name { get; set; }

            public string? ID { get; set; } // Giả sử mỗi sản phẩm có một ID duy nhất
            public string? Type { get; set; }
            public decimal Price { get; set; }
            public int availableQuantity { get; set; } // Giả sử có 100 sản phẩm sẵn có
            private int quantity =1;
            public int Quantity
            {
                get => quantity;
                set
                {
                    if (quantity != value)
                    {
                        quantity = value;
                        OnPropertyChanged(nameof(Quantity));
                        OnPropertyChanged(nameof(Total));
                    }
                }

            }
            public string? imageUrl { get; set; }

            public decimal Total => Price * Quantity;

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        private List<ProductViewModel> _allProducts = new List<ProductViewModel>();
        private void LoadProducts()
        {
            var fishProducts = db.Fish.Select(f => new ProductViewModel
            {
                Name = f.FishName,
                ID = f.FishId, // Giả sử mỗi cá có một ID duy nhất
                Type = "Fish",
                Price = f.Price,  
                availableQuantity =f.QuantityAvailable,
                imageUrl = f.ImageUrl
            });

            var accessoryProducts = db.AquariumAccessories.Select(a => new ProductViewModel
            {
                Name = a.AccessoryName,
                Type = "Accessory",
                ID = a.AccessoryId, // Giả sử mỗi phụ kiện có một ID duy nhất
                Price = a.Price,
                availableQuantity = a.QuantityAvailable,
                imageUrl = a.ImageUrl
            });

            _allProducts = fishProducts.Concat(accessoryProducts).ToList();
            ProductItemsControl.ItemsSource = _allProducts;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                ProductItemsControl.ItemsSource = _allProducts;
            }
            else
            {
                ProductItemsControl.ItemsSource = _allProducts
                    .Where(p => p.Name?.ToLower().Contains(keyword) == true)
                    .ToList();
            }
        }
        private void CategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedItem is Category selectedCategory)
            {
                string categoryId = selectedCategory.CategoryId;
                var filteredProducts = _allProducts
                    .Where(p => (p.Type == "Fish" && db.Fish.Any(f => f.CategoryId == categoryId && f.FishName == p.Name)) ||
                                 (p.Type == "Accessory" && db.AquariumAccessories.Any(a => a.CategoryId == categoryId && a.AccessoryName == p.Name)))
                    .ToList();
                ProductItemsControl.ItemsSource = filteredProducts;
            }
            else
            {
                ProductItemsControl.ItemsSource = _allProducts;
            }
        }
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (orderList.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm sản phẩm vào đơn hàng trước khi đặt hàng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CustomerSelect customerWindow = new CustomerSelect();
            if (customerWindow.ShowDialog() != true)
            {
                MessageBox.Show("Customer not selected", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string customerId = customerWindow._selectedCustomerId;

            var service = new OrderService(db);
            if (service.PlaceOrder(Session.UserId, customerId, orderList, out string error))
            {
                MessageBox.Show("Đặt hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                orderList.Clear();
                OrderListBox.ItemsSource = null;
            }
            else
            {
                MessageBox.Show(error, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.DataContext as ProductViewModel;
            if (product == null) return;

            orderList.Add(product);

            OrderListBox.ItemsSource = null; // Refresh lại binding
            OrderListBox.ItemsSource = orderList;
            UpdateTotal();
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProductViewModel item)
            {
                item.Quantity++;
                if (item.Quantity > item.availableQuantity)
                {
                    MessageBox.Show($"Số lượng tối đa cho sản phẩm {item.Name} là {item.availableQuantity}.");
                    item.Quantity = item.availableQuantity; // Giới hạn số lượng không vượt quá số lượng có sẵn
                }
            }
            UpdateTotal();
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProductViewModel item)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
            }
            UpdateTotal();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProductViewModel item)
            {
                orderList.Remove(item);
                OrderListBox.ItemsSource = null;
                OrderListBox.ItemsSource = orderList; // cập nhật lại UI
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = orderList.Sum(p => p.Total);
            TotalOrderTextBlock.Text = $"Tổng đơn hàng: {total:N0}₫";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            Application.Current.MainWindow = login;
            login.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StaffWindow staffWindow = new StaffWindow();
            staffWindow.Show();
            this.Hide();    
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CategoryListBox.SelectedItem = null;
            SearchBox.Text = string.Empty;
        }
    }
}
