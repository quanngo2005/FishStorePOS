using FishStore.Models;
using FishStore.Service;
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
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public string customerID { get; private set; } = string.Empty;
        public AddCustomer()
        {
            InitializeComponent();
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string phone = PhoneTextBox.Text;

            CustomerService customerService = new CustomerService();
            var result = customerService.AddCustomer(name, phone);

            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            customerID = result.CustomerId;
            MessageBox.Show(result.Message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
