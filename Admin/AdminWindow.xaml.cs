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

namespace FishStore.Admin
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void AccountWindow_Click(object sender, RoutedEventArgs e)
        {
            Accounts accountsWindow = new Accounts();
            Application.Current.MainWindow = accountsWindow;
            accountsWindow.Show();
            this.Close();
        }

        private void CategoryWindow_Click(object sender, RoutedEventArgs e)
        {
            Categories categoriesWindow = new Categories();
            Application.Current.MainWindow = categoriesWindow;
            categoriesWindow.Show();    
            this.Close();
        }

        private void CustomerWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TransactionWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FishWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OrderWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            Application.Current.MainWindow = login;
            login.Show();
            this.Close();
        }
    }
}
