using FishStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStore.Service
{
    internal class CustomerService
    {
        private readonly ShopBanCaContext _db;

        public CustomerService()
        {
            _db = new ShopBanCaContext();
        }

        public (bool Success, string Message, string CustomerId) AddCustomer(string name, string phone)
        {
            name = name?.Trim();
            phone = phone?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                return (false, "Vui lòng nhập đầy đủ thông tin.", null);
            }

            bool exists = _db.Customers.Any(c => c.Phone == phone);
            if (exists)
            {
                return (false, "Số điện thoại đã tồn tại.", null);
            }

            var newCustomer = new Customer
            {
                CustomerId = IdGenerator.GenerateId("Customer"),
                FullName = name,
                Phone = phone
            };

            _db.Customers.Add(newCustomer);
            _db.SaveChanges();

            return (true, "Thêm khách hàng thành công!", newCustomer.CustomerId);
        }
    }
}
