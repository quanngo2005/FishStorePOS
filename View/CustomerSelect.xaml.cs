using FishStore.Admin;
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

namespace FishStore.View
{
    /// <summary>
    /// Interaction logic for CustomerSelect.xaml
    /// </summary>
    public partial class CustomerSelect : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        public string _selectedCustomerId = string.Empty;
        public CustomerSelect()
        {
            InitializeComponent();
        }


        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddCustomer(); // Giả sử bạn đã có form thêm khách hàng
            if (addWindow.ShowDialog() == true)
            {
                _selectedCustomerId = addWindow.customerID; // Trả lại ID khách mới
                DialogResult = true;
                Close();
            }
        }

        private void SearchAndSelect_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.ToLower();
            var customer = db.Customers
               .FirstOrDefault(c => c.Phone.ToLower() == keyword || c.FullName.ToLower().Contains(keyword));

            if (customer != null)
            {
                _selectedCustomerId = customer.CustomerId;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
