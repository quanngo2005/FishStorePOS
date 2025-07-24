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
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;


namespace FishStore.Admin
{
    /// <summary>
    /// Interaction logic for OrderDetail.xaml
    /// </summary>
    public partial class OrderDetail : Window
    {
        private readonly ShopBanCaContext db = new ShopBanCaContext();
        private string _orderId;
        public string OrderId
        {
            get { return _orderId; }
            set
            {
                _orderId = value;
                LoadOrderDetails(_orderId);
            }
        }

        private void LoadOrderDetails(string orderId)
        {
            // Lấy Fish order items
            var fishItems = db.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Join(db.Fish,
                      od => od.FishId,
                      f => f.FishId,
                      (od, f) => new OrderItemViewModel
                      {
                          ItemType = "Fish",
                          ItemName = f.FishName,
                          Quantity = od.Quantity,
                          UnitPrice = od.UnitPrice
                      });

            // Lấy Accessory order items
            var accessoryItems = db.OrderAccessoryDetails
                .Where(ad => ad.OrderId == orderId)
                .Join(db.AquariumAccessories,
                      ad => ad.AccessoryId,
                      a => a.AccessoryId,
                      (ad, a) => new OrderItemViewModel
                      {
                          ItemType = "Accessory",
                          ItemName = a.AccessoryName,
                          Quantity = ad.Quantity,
                          UnitPrice = ad.UnitPrice
                      });

            // Gộp 2 loại và bind vào DataGrid
            var allItems = fishItems.Concat(accessoryItems).ToList();
            OrderItemGrid.ItemsSource = allItems;

            // Tính tổng giá
            decimal totalPrice = allItems.Sum(item => item.TotalPrice);
            TotalPriceText.Text = totalPrice.ToString("#,##0") + " ₫"; // Format tiền tệ: 20.000 ₫

        }
         public class OrderItemViewModel
                {
                    public string ItemType { get; set; } // "Fish" hoặc "Accessory"
                    public string ItemName { get; set; }
                    public int Quantity { get; set; }
                    public decimal UnitPrice { get; set; }
                    public decimal TotalPrice => Quantity * UnitPrice;
                }

        public OrderDetail()
        {
            InitializeComponent();
        }
       
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

            string? staffName = db.Orders
            .Where(o => o.OrderId == OrderId)
            .Join(db.Accounts,
                o => o.CreatedBy,
                a => a.UserId,
                (o, a) => a.FullName)
            .FirstOrDefault();

            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Hóa đơn {OrderId}";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont titleFont = new XFont("Arial", 18, XFontStyleEx.Bold);
            XFont subFont = new XFont("Arial", 12, XFontStyleEx.Regular);
            XFont itemFont = new XFont("Arial", 11, XFontStyleEx.Regular);

            double y = 40;

            // Tiêu đề
            gfx.DrawString("Bill", titleFont, XBrushes.Black,
                new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            // Mã đơn, người bán, thời gian
            gfx.DrawString($"Order Id: {OrderId}", subFont, XBrushes.Black, 40, y);
            y += 20;
            gfx.DrawString($"Staff: {staffName}", subFont, XBrushes.Black, 40, y);
            y += 20;
            gfx.DrawString($"Date: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", subFont, XBrushes.Black, 40, y);
            y += 30;

            // Danh sách mặt hàng
            foreach (OrderItemViewModel item in OrderItemGrid.ItemsSource)
            {
                string line = $"{item.ItemType} - {item.ItemName} | Amount: {item.Quantity} | Price: {item.UnitPrice:C} | Total: {item.TotalPrice:C}";
                gfx.DrawString(line, itemFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);
                y += 20;

                if (y > page.Height - 60)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            // Tổng giá
            y += 20;
            gfx.DrawString($"Total: {TotalPriceText.Text}", new XFont("Arial", 13, XFontStyleEx.Bold), XBrushes.Black,
                new XRect(40, y, page.Width - 80, 20), XStringFormats.TopLeft);

            // Lưu PDF
            string filename = $"Bill_{OrderId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = "D:\\Code\\C#\\FishStore\\Resource\\";
            document.Save(filePath);

            MessageBox.Show($"Bill have save with name: {filename}", "Export PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            
        }
    }
}
