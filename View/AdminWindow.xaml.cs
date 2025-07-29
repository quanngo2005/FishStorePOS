using FishStore.Models;
using FishStore.Service;
using FishStore.UserAuthenticaiton;
using FishStore.View;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        ShopBanCaContext dbContext = new ShopBanCaContext();
        public AdminWindow()
        {
            InitializeComponent();
            LoadData(); 
        }
        private void LoadData()
        {
            
            var service = new ReportService(dbContext);
            var reports = service.GetMonthlyReport();

            FishReportDataGrid.ItemsSource = reports;

        }

        private void AccountWindow_Click(object sender, RoutedEventArgs e)
        {
            Accounts accountsWindow = new Accounts();
            
            accountsWindow.Show();
            this.Hide();
            accountsWindow.Closed += (s, arg) => this.Show(); 
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

        private void TransactionWindow_Click(object sender, RoutedEventArgs e)
        {
            Transaction transactionWindow = new Transaction();
            transactionWindow.Show();
            this.Hide();
            transactionWindow.Closed += (s, arg) => this.Show(); // Hiển thị lại AdminWindow khi Transaction đóng
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
