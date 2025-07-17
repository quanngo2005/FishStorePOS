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
                .Where(o => o.CustomerId.ToLower().Contains(keyword) || 
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

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng cửa sổ hiện tại

            // Tùy theo role mà mở lại cửa sổ phù hợp
            if (Session.Role == "Admin")
            {
                NavigationHelper.GoBackTo(new AdminWindow());
            }
            else if (Session.Role == "Manager")
            {
                NavigationHelper.GoBackTo(new ManagerWindow());
            }
            else if (Session.Role == "Staff")
            {
                NavigationHelper.GoBackTo(new MainWindow());
            }
        }
    }
}
