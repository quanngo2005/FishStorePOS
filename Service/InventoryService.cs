using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FishStore.Service
{
    internal class InventoryService
    {
        private readonly ShopBanCaContext db;

        public InventoryService(ShopBanCaContext dbContext)
        {
            db = dbContext;
        }

        public bool SaveTransaction(
            string transactionId,
            string selectedType,
            string selectedItem,
            int quantity,
            decimal unitCost,
            DateTime transactionDate,
            string createdBy,
            out string errorMessage)
        {
            errorMessage = "";

            try
            {
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
                    errorMessage = "Unit cost cannot be greater than item's price.";
                    return false;
                }

                if (selectedType == "Fish")
                {
                    InventoryTransaction transaction;
                    int oldQuantity = 0;

                    if (!string.IsNullOrEmpty(transactionId))
                    {
                        transaction = db.InventoryTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
                        if (transaction == null)
                        {
                            errorMessage = "Transaction not found.";
                            return false;
                        }

                        oldQuantity = transaction.Quantity;

                        transaction.FishId = selectedItem;
                        transaction.Quantity = quantity;
                        transaction.UnitCost = unitCost;
                        transaction.TotalCost = quantity * unitCost;
                        transaction.TransactionDate = transactionDate;
                    }
                    else
                    {
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
                        int quantityDiff = quantity - oldQuantity;
                        fish.QuantityAvailable += quantityDiff;
                        if (fish.QuantityAvailable > 0)
                            fish.Status = true;
                    }
                }
                else if (selectedType == "Accessory")
                {
                    AccessoryTransaction transaction;
                    int oldQuantity = 0;

                    if (!string.IsNullOrEmpty(transactionId))
                    {
                        transaction = db.AccessoryTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
                        if (transaction == null)
                        {
                            errorMessage = "Transaction not found.";
                            return false;
                        }

                        oldQuantity = transaction.Quantity;

                        transaction.AccessoryId = selectedItem;
                        transaction.Quantity = quantity;
                        transaction.UnitCost = unitCost;
                        transaction.TotalCost = quantity * unitCost;
                        transaction.TransactionDate = transactionDate;
                    }
                    else
                    {
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
                            accessory.Status = true;
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Error saving transaction: " + ex.Message;
                return false;
            }
        }
        

    }
}
