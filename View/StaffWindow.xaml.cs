using FishStore.Admin;
using FishStore.UserAuthenticaiton;
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
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        public StaffWindow()
        {
            InitializeComponent();
        }

        

        private void CategoryWindow_Click(object sender, RoutedEventArgs e)
        {
            Categories categoriesWindow = new Categories();

            categoriesWindow.Show();
            this.Hide();
            categoriesWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Categories đóng

        }

        private void CustomerWindow_Click(object sender, RoutedEventArgs e)
        {
            Customer customerWindow = new Customer();

            customerWindow.Show();
            this.Hide();
            customerWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Customer đóng
        }

        

        private void FishWindow_Click(object sender, RoutedEventArgs e)
        {
            Fishes fishesWindow = new Fishes();

            fishesWindow.Show();
            this.Hide();
            fishesWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Fishes đóng
        }

        private void OrderWindow_Click(object sender, RoutedEventArgs e)
        {
            Order orderWindow = new Order();

            orderWindow.Show();
            this.Hide();
            orderWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Order đóng
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            Application.Current.MainWindow = login;
            login.Show();
            this.Close();
        }

        private void Accessory_Click(object sender, RoutedEventArgs e)
        {
            Accessory accessoryWindow = new Accessory();

            accessoryWindow.Show();
            this.Hide();
            accessoryWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Accessory đóng
        }
    }
}
