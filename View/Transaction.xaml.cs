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
using FishStore.Helper;
using FishStore.Models;

namespace FishStore.View
{
    /// <summary>
    /// Interaction logic for Transaction.xaml
    /// </summary>
    public partial class Transaction : Window
    {
        private readonly ShopBanCaContext db = new ShopBanCaContext();
        string role = Session.Role; // Lấy role từ Session
        public Transaction()
        {
            InitializeComponent();
            LoadAllTransactions();
            if (role != "Manager")
            {
                AddButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadAllTransactions()
        {
            // Fish transactions
            var fishTransactions = db.InventoryTransactions
                .Select(t => new TransactionViewModel
                {
                    Type = "Fish",
                    ID = t.TransactionId, 
                    Name = t.Fish != null ? t.Fish.FishName : "",
                    Quantity = t.Quantity,
                    UnitCost = t.UnitCost,
                    CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                    TransactionDate = t.TransactionDate
                });

            // Accessory transactions
            var accessoryTransactions = db.AccessoryTransactions
                .Select(t => new TransactionViewModel
                {
                    Type = "Accessory",
                    ID = t.TransactionId,
                    Name = t.Accessory != null ? t.Accessory.AccessoryName : "",
                    Quantity = t.Quantity,
                    UnitCost = t.UnitCost,
                    CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                    TransactionDate = t.TransactionDate
                });

            // Gộp tất cả
            var all = fishTransactions.Concat(accessoryTransactions).ToList();

            // Bind to DataGrid
            TransactionDataGrid.ItemsSource = all;

            List<string> transactionTypes = new List<string>
            {
                "All", // Thêm tùy chọn "All"
                "Fish",
                "Accessory"
            };
            TransactionTypeComboBox.ItemsSource = transactionTypes;
            TransactionTypeComboBox.SelectedIndex = 0; // Mặc định chọn "All"
        }
        

        public class TransactionViewModel
        {
            public string Type { get; set; } // "Fish" hoặc "Accessory"
            public string ID { get; set; } // TransactionId
            public string Name { get; set; } // FishName hoặc AccessoryName
            public int Quantity { get; set; }
            public decimal UnitCost { get; set; }
            public decimal TotalCost => Quantity * UnitCost;
            public string CreatedBy { get; set; }
            public DateTime? TransactionDate { get; set; }
        }


        private void TransactionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = TransactionTypeComboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedType) || selectedType == "All")
            {
                // Hiển thị tất cả giao dịch nếu chọn "All" hoặc không chọn gì
                LoadAllTransactions();
                return;
            }

            List<TransactionViewModel> result;

            if (selectedType == "Fish")
            {
                result = db.InventoryTransactions
                    .Select(t => new TransactionViewModel
                    {
                        Type = "Fish",
                        Name = t.Fish != null ? t.Fish.FishName : "",
                        Quantity = t.Quantity,
                        UnitCost = t.UnitCost,
                        CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                        TransactionDate = t.TransactionDate
                    }).ToList();
            }
            else if (selectedType == "Accessory")
            {
                result = db.AccessoryTransactions
                    .Select(t => new TransactionViewModel
                    {
                        Type = "Accessory",
                        Name = t.Accessory != null ? t.Accessory.AccessoryName : "",
                        Quantity = t.Quantity,
                        UnitCost = t.UnitCost,
                        CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                        TransactionDate = t.TransactionDate
                    }).ToList();
            }
            else
            {
                // Nếu có giá trị lạ, load tất cả
                LoadAllTransactions();
                return;
            }

            TransactionDataGrid.ItemsSource = result;


        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                // Nếu không có từ khóa tìm kiếm, hiển thị tất cả giao dịch
                LoadAllTransactions();
                return;
            }

            // Lọc giao dịch theo từ khóa tìm kiếm

            var filteredTransactions = db.InventoryTransactions
                .Where(t => t.Fish != null && t.Fish.FishName.ToLower().Contains(searchText))
                .Select(t => new TransactionViewModel
                {
                    Type = "Fish",
                    Name = t.Fish.FishName,
                    Quantity = t.Quantity,
                    UnitCost = t.UnitCost,
                    CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                    TransactionDate = t.TransactionDate
                })
                .Concat(db.AccessoryTransactions
                    .Where(t => t.Accessory != null && t.Accessory.AccessoryName.ToLower().Contains(searchText))
                    .Select(t => new TransactionViewModel
                    {
                        Type = "Accessory",
                        Name = t.Accessory.AccessoryName,
                        Quantity = t.Quantity,
                        UnitCost = t.UnitCost,
                        CreatedBy = t.CreatedByNavigation != null ? t.CreatedByNavigation.Username : "",
                        TransactionDate = t.TransactionDate
                    }))
                .ToList();
            TransactionDataGrid.ItemsSource = filteredTransactions;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.Enter)
            {
                
                SearchBox_TextChanged(sender, null);
            }
        }

        private void BackToAdminPanel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddEditTran addEditWindow = new AddEditTran(null);
            addEditWindow.ShowDialog();
            LoadAllTransactions(); // Tải lại danh sách giao dịch sau khi thêm mới
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedTransaction = TransactionDataGrid.SelectedItem as dynamic;
            if (selectedTransaction == null)
            {
                MessageBox.Show("Please select a transaction to edit.");
                return;
            }
            var transactionId = selectedTransaction.ID; // Lấy ID giao dịch từ dòng đã chọn


            AddEditTran addEditWindow = new AddEditTran(transactionId);
            var result = addEditWindow.ShowDialog();
            if (result == true)
            {
                LoadAllTransactions(); // Load lại danh sách
            }
        }
    }
}
