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
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetail : Window
    {
        private string _orderId;
        public string OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                LoadOrderDetails(_orderId);
            }
        }

        private void LoadOrderDetails(string orderId)
        {
            MessageBox.Show($"Loading details for Order ID: {orderId} via Property.");
        }

        public OrderDetail()
        {
            InitializeComponent();
        }
    }
}
