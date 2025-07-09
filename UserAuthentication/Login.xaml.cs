using FishStore.Admin;
using FishStore.Manager;
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

namespace FishStore.UserAuthenticaiton
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

        }

        private void username_TextChanged(object sender, TextChangedEventArgs e)
        {
            var username = sender as TextBox;
            if (username != null)
            {
                string user = username.Text;

            }
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Handle the PasswordChanged event here
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                string password = passwordBox.Password;
             
            }
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string usernameInput = username.Text;
            string passwordInput = passwordHash.Password;

            if (string.IsNullOrEmpty(usernameInput) || string.IsNullOrEmpty(passwordInput))
            {
                MessageBox.Show("Please enter both username and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new ShopBanCaContext())
                {
                    var account = db.Accounts.FirstOrDefault(a => a.Username == usernameInput);

                    if (account == null || account.PasswordHash != passwordInput)
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!account.Status)
                    {
                        MessageBox.Show("Your account is inactive. Please contact support.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    MessageBox.Show($"Welcome, {account.Username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Phân quyền theo Role (string)
                    Window mainWindow;

                    switch (account.Role?.Trim().ToLower())
                    {
                        case "admin":
                            mainWindow = new AdminWindow(); // tạo AdminWindow.xaml
                            break;
                        case "manager":
                            mainWindow = new ManagerWindow(); // tạo ManagerWindow.xaml
                            break;
                        case "staff":
                            mainWindow = new MainWindow(); // tạo StaffWindow.xaml
                            break;
                        default:
                            MessageBox.Show($"Unknown role: {account.Role}", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                    }

                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed due to an error:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
   }
