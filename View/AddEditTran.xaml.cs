using FishStore.Helper;
using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for AddEditTran.xaml
    /// </summary>
    public partial class AddEditTran : Window
    {
        ShopBanCaContext db = new ShopBanCaContext();
        public string _transactionId; 
        public AddEditTran(string transactionId)
        {
            InitializeComponent();
            _transactionId = transactionId;
            TransactionLoad();
        }

 
        private void ItemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void TransactionLoad()
        {
            if (string.IsNullOrEmpty(_transactionId))
            {
                var result = MessageBox.Show("Do you want to add transaction ID?", "Warning", MessageBoxButton.YesNo);
                if ( result == MessageBoxResult.No) {
                    return;
                }
                
            }

            var fishTran = db.InventoryTransactions.FirstOrDefault(t => t.TransactionId == _transactionId);
            var accTran = db.AccessoryTransactions.FirstOrDefault(t => t.TransactionId == _transactionId);

            if (fishTran != null)
            {
                TransactionTypeComboBox.SelectedIndex = 0; // Fish
                LoadFishItems(); // Load danh sách cá vào ComboBox

                // Gán SelectedItem theo object, không dùng tên
                ItemComboBox.SelectedItem = db.Fish.FirstOrDefault(f => f.FishId == fishTran.FishId);

                QuantityTextBox.Text = fishTran.Quantity.ToString();
                UnitCostTextBox.Text = fishTran.UnitCost.ToString("F2");
                TransactionDatePicker.SelectedDate = fishTran.TransactionDate;
            }
            else if (accTran != null)
            {
                TransactionTypeComboBox.SelectedIndex = 1; // Accessory
                LoadAccessoryItems(); // Load danh sách phụ kiện

                ItemComboBox.SelectedItem = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == accTran.AccessoryId);

                QuantityTextBox.Text = accTran.Quantity.ToString();
                UnitCostTextBox.Text = accTran.UnitCost.ToString("F2");
                TransactionDatePicker.SelectedDate = accTran.TransactionDate;
            }
            else
            {
                MessageBox.Show("Transaction not found.");
            }
        }
        private void LoadFishItems()
        {
            ItemComboBox.ItemsSource = db.Fish.ToList();
            ItemComboBox.DisplayMemberPath = "FishName";
            ItemComboBox.SelectedValuePath = "FishId";
        }

        private void LoadAccessoryItems()
        {
            ItemComboBox.ItemsSource = db.AquariumAccessories.ToList();
            ItemComboBox.DisplayMemberPath = "AccessoryName";
            ItemComboBox.SelectedValuePath = "AccessoryId";
        }

        private void TransactionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)TransactionTypeComboBox.SelectedItem;
            if (selectedItem == null) return;

            string selectedType = selectedItem.Content.ToString();

            if (selectedType == "Fish")
            {
                var fishList = db.Fish
            .Select(f => new { f.FishId, f.FishName })
            .ToList();

                ItemComboBox.ItemsSource = fishList;
                ItemComboBox.DisplayMemberPath = "FishName";
                ItemComboBox.SelectedValuePath = "FishId";
            }
            else if (selectedType == "Accessory")
            {
                var accessoryList = db.AquariumAccessories
           .Select(a => new { a.AccessoryId, a.AccessoryName })
           .ToList();

                ItemComboBox.ItemsSource = accessoryList;
                ItemComboBox.DisplayMemberPath = "AccessoryName";
                ItemComboBox.SelectedValuePath = "AccessoryId";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = (TransactionTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var selectedItem = ItemComboBox.SelectedValue?.ToString();
            var createdBy = Session.UserId;

            if (string.IsNullOrEmpty(selectedType) || string.IsNullOrEmpty(selectedItem))
            {
                MessageBox.Show("Please select transaction type and item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Invalid quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(UnitCostTextBox.Text, out decimal unitCost) || unitCost <= 0)
            {
                MessageBox.Show("Invalid unit cost.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? transactionDate = TransactionDatePicker.SelectedDate;
            if (transactionDate == null)
            {
                MessageBox.Show("Please select transaction date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Lấy giá bán để kiểm tra chi phí không vượt quá
                decimal price = 0;
                if (selectedType == "Fish")
                {
                    price = db.Fish.Where(f => f.FishId == selectedItem).Select(f => f.Price).FirstOrDefault();
                }
                else if (selectedType == "Accessory")
                {
                    price = db.AquariumAccessories.Where(a => a.AccessoryId == selectedItem).Select(a => a.Price).FirstOrDefault();
                }

                if (unitCost > price)
                {
                    MessageBox.Show("Unit cost cannot be greater than item's price.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (selectedType == "Fish")
                {
                    InventoryTransaction transaction;
                    int oldQuantity = 0;

                    if (!string.IsNullOrEmpty(_transactionId))
                    {
                        // Chế độ sửa
                        transaction = db.InventoryTransactions.FirstOrDefault(t => t.TransactionId == _transactionId);
                        if (transaction == null)
                        {
                            MessageBox.Show("Transaction not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        TransactionTypeComboBox.IsEnabled = false;
                        ItemComboBox.IsHitTestVisible = false;
                        ItemComboBox.Focusable = false;


                        oldQuantity = transaction.Quantity;

                        transaction.FishId = selectedItem;
                        transaction.Quantity = quantity;
                        transaction.UnitCost = unitCost;
                        transaction.TotalCost = quantity * unitCost;
                        transaction.TransactionDate = transactionDate;
                    }
                    else
                    {
                        // Chế độ thêm mới
                        transaction = new InventoryTransaction
                        {
                            TransactionId = IdGenerator.GenerateId("InventoryTransaction"),
                            FishId = selectedItem,
                            Quantity = quantity,
                            UnitCost = unitCost,
                            TotalCost = quantity * unitCost,
                            CreatedBy = createdBy,
                            TransactionDate = transactionDate
                        };
                        db.InventoryTransactions.Add(transaction);
                    }

                    var fish = db.Fish.FirstOrDefault(f => f.FishId == selectedItem);
                    if (fish != null)
                    {
                        int quantityDiff = quantity - oldQuantity; // = mới - cũ
                        fish.QuantityAvailable += quantityDiff;

                        if (fish.QuantityAvailable > 0)
                        {
                            fish.Status = true;
                        }
                    }
                }
                else if (selectedType == "Accessory")
                {
                    AccessoryTransaction transaction;
                    int oldQuantity = 0;

                    if (!string.IsNullOrEmpty(_transactionId))
                    {
                        // Chế độ sửa
                        transaction = db.AccessoryTransactions.FirstOrDefault(t => t.TransactionId == _transactionId);
                        if (transaction == null)
                        {
                            MessageBox.Show("Transaction not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        TransactionTypeComboBox.IsEnabled = false;
                        ItemComboBox.IsHitTestVisible = false;
                        ItemComboBox.Focusable = false;


                        oldQuantity = transaction.Quantity;

                        transaction.AccessoryId = selectedItem;
                        transaction.Quantity = quantity;
                        transaction.UnitCost = unitCost;
                        transaction.TotalCost = quantity * unitCost;
                        transaction.TransactionDate = transactionDate;
                    }
                    else
                    {
                        // Chế độ thêm mới
                        transaction = new AccessoryTransaction
                        {
                            TransactionId = IdGenerator.GenerateId("AccessoryTransaction"),
                            AccessoryId = selectedItem,
                            Quantity = quantity,
                            UnitCost = unitCost,
                            TotalCost = quantity * unitCost,
                            CreatedBy = createdBy,
                            TransactionDate = transactionDate
                        };
                        db.AccessoryTransactions.Add(transaction);
                    }

                    var accessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == selectedItem);
                    if (accessory != null)
                    {
                        int quantityDiff = quantity - oldQuantity;
                        accessory.QuantityAvailable += quantityDiff;

                        if (accessory.QuantityAvailable > 0)
                        {
                            accessory.Status = true;
                        }
                    }
                }


                db.SaveChanges();
                MessageBox.Show("Transaction saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reset form
                if (string.IsNullOrEmpty(_transactionId))
                {
                    ItemComboBox.SelectedIndex = -1;
                    QuantityTextBox.Clear();
                    UnitCostTextBox.Clear();
                    TransactionDatePicker.SelectedDate = null;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving transaction: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
