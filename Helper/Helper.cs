using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace FishStore.Helper
{
    internal class Helper
    {
        public static void ClearFormFields(Panel container)
        {
            foreach (var control in container.Children)
            {
                if (control is TextBox tb)
                    tb.Text = string.Empty;
                else if (control is ComboBox cb)
                    cb.SelectedIndex = -1;
                else if (control is RadioButton rb)
                    rb.IsChecked = false;
            }
        }
    }
    public static class Validator
    {
        /// <summary>
        /// Kiểm tra một loạt điều kiện và hiển thị thông báo lỗi nếu có.
        /// </summary>
        /// <param name="checks">Mỗi phần tử là một tuple (điều kiện sai, thông báo lỗi).</param>
        public static bool ValidateFields(params (bool Condition, string ErrorMessage)[] checks)
        {
            foreach (var check in checks)
            {
                if (check.Condition)
                {
                    ShowError(check.ErrorMessage);
                    return false;
                }
            }
            return true;
        }

        // ==============================
        // Các phương thức hỗ trợ riêng lẻ (tùy chọn sử dụng)
        // ==============================

        public static bool IsNullOrEmpty(string value) => string.IsNullOrWhiteSpace(value);

        public static bool IsInvalidNumber(decimal number, decimal min = 0) => number < min;

        public static bool IsInvalidNumber(int number, int min = 0) => number < min;

        public static bool IsInvalidDate(DateTime? date) => !date.HasValue;

        public static bool IsFutureDate(DateTime date) => date > DateTime.Now;

        public static bool IsUnchecked(bool? value) => value != true;

        public static bool IsDuplicate(Func<bool> condition, string errorMessage)
        {
            if (condition())
            {
                ShowError(errorMessage);
                return true;
            }
            return false;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    public static class Session
    {
        public static string? UserId { get; set; }
        public static string? Role { get; set; } 

    }
    public static class NavigationHelper
    {
        public static void GoBackTo(Window targetWindow)
        {
            Application.Current.MainWindow = targetWindow;
            targetWindow.Show();
        }
    }
}
