using FishStore.Helper;
using FishStore.Manager;
using FishStore.Models;
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
        public Order()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var orders = db.Orders.ToList();
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
            Window targetWindow = null;

            if (Session.Role == "Admin")
            {
                targetWindow = new AdminWindow();
            }
            else if (Session.Role == "Manager")
            {
                targetWindow = new ManagerWindow();
            }
            else if (Session.Role == "Staff")
            {
                targetWindow = new MainWindow();
            }

            if (targetWindow != null)
            {
                Application.Current.MainWindow = targetWindow;
                targetWindow.Show();
                this.Close(); // Đóng cửa sổ hiện tại sau khi mở cửa sổ mới
            }
        }
    }
}
