using System;
using System.Linq;
using FishStore.Models;

public static class IdGenerator
{
    public static string GenerateId(string type)
    {
        using (var db = new ShopBanCaContext())
        {
            string prefix;
            int count;

            switch (type)
            {
                case "User":
                    prefix = "U";
                    count = db.Accounts.Count();
                    break;

                case "Customer":
                    prefix = "CUS";
                    count = db.Customers.Count();
                    break;

                case "Category":
                    prefix = "CAT";
                    count = db.Categories.Count();
                    break;

                case "Fish":
                    prefix = "F";
                    count = db.Fish.Count();
                    break;

                case "Accessory":
                    prefix = "AC";
                    count = db.AquariumAccessories.Count();
                    break;

                case "Order":
                    prefix = "ORD";
                    count = db.Orders.Count();
                    break;

                case "OrderDetail":
                    prefix = "OD";
                    count = db.OrderDetails.Count();
                    break;

                case "OrderAccessoryDetail":
                    prefix = "OAD";
                    count = db.OrderAccessoryDetails.Count();
                    break;

                case "InventoryTransaction":
                    prefix = "TRF";
                    count = db.InventoryTransactions.Count();
                    break;

                case "AccessoryTransaction":
                    prefix = "TRA";
                    count = db.AccessoryTransactions.Count();
                    break;

                default:
                    throw new ArgumentException("Unknown ID type.");
            }

            string newId = $"{prefix}{(count + 1).ToString("D3")}";
            return newId;
        }
    }
}
