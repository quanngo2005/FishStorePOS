using FishStore.Helper;
using FishStore.Models;
using FishStore.View;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        string role = Session.Role; // Lấy role từ session
        public Order()
        {
            InitializeComponent();
            LoadData();
            if (role =="Admin")
            {
                AddOrderButton.Visibility = Visibility.Visible;
                DeleteOrderButton.Visibility = Visibility.Visible;
            }
            else
            {
                AddOrderButton.Visibility = Visibility.Collapsed;
                DeleteOrderButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            var orders = db.Orders
            .Include(o => o.Customer)
            .Include(o => o.CreatedByNavigation)
            .ToList();
            OrdersDataGrid.ItemsSource = orders;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim().ToLower();

            var filteredOrders = db.Orders
                .Where(o => o.CreatedBy.ToLower().Contains(keyword) ||
                            o.OrderId.ToString().Contains(keyword) ||
                            o.OrderDate.ToString().Contains(keyword))
                .ToList();
        }
        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchTextBox_TextChanged(sender, null);
            }
        }
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button?.Tag is string orderId && !string.IsNullOrWhiteSpace(orderId))
            {
                // Gọi màn hình detail
                var orderDetailWindow = new OrderDetail();
                // If you need to pass orderId, set a property or call a method here
                orderDetailWindow.OrderId = orderId;
                orderDetailWindow.ShowDialog(); // Hiển thị cửa sổ chi tiết đơn hàng
            }
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
           
               this.Close(); // Đóng cửa sổ hiện tại sau khi mở cửa sổ mới
           
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
