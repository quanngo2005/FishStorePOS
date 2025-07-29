using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishStore.View.OrderMain;

namespace FishStore.Service
{
    public class OrderService
    {
        private readonly ShopBanCaContext db;

        public OrderService(ShopBanCaContext context)
        {
            db = context;
        }

        public bool PlaceOrder(string createdBy, string customerId, List<ProductViewModel> orderList, out string errorMessage)
        {
            errorMessage = "";

            if (orderList == null || !orderList.Any())
            {
                errorMessage = "Vui lòng thêm sản phẩm vào đơn hàng trước khi đặt hàng.";
                return false;
            }

            string orderId = IdGenerator.GenerateId("Order");

            Order newOrder = new Order
            {
                OrderId = orderId,
                CustomerId = customerId,
                CreatedBy = createdBy,
                OrderDate = DateTime.Now,
                TotalAmount = orderList.Sum(p => p.Total),
                Status = "Pending"
            };

            db.Orders.Add(newOrder);

            foreach (var item in orderList)
            {
                if (item.Type == "Fish")
                {
                    var fish = db.Fish.FirstOrDefault(f => f.FishId == item.ID);
                    if (fish == null)
                        continue;

                    if (fish.QuantityAvailable < item.Quantity)
                    {
                        errorMessage = $"Sản phẩm {fish.FishName} không đủ hàng trong kho.";
                        return false;
                    }

                    fish.QuantityAvailable -= item.Quantity;

                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = IdGenerator.GenerateId("OrderDetail"),
                        OrderId = orderId,
                        FishId = fish.FishId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        TotalPrice = item.Total
                    };

                    db.OrderDetails.Add(orderDetail);
                }
                else if (item.Type == "Accessory")
                {
                    var accessory = db.AquariumAccessories.FirstOrDefault(a => a.AccessoryId == item.ID);
                    if (accessory == null)
                        continue;

                    if (accessory.QuantityAvailable < item.Quantity)
                    {
                        errorMessage = $"Phụ kiện {accessory.AccessoryName} không đủ hàng trong kho.";
                        return false;
                    }

                    accessory.QuantityAvailable -= item.Quantity;

                    var accessoryDetail = new OrderAccessoryDetail
                    {
                        OrderAccessoryDetailId = IdGenerator.GenerateId("OrderAccessoryDetail"),
                        OrderId = orderId,
                        AccessoryId = accessory.AccessoryId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        TotalPrice = item.Total
                    };

                    db.OrderAccessoryDetails.Add(accessoryDetail);
                }
            }

            db.SaveChanges();
            return true;
        }
    }
}
