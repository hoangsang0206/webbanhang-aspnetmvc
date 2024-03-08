using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace STech_Web.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(15, ErrorMessage = "Tên đăng nhập không quá 15 kí tự.")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Tên đăng nhập không chứa kí tự đặc biệt.")]
        public string ResUsername { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(20, ErrorMessage = "Mật khẩu không vượt quá 20 kí tự.")]
        public string ResPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("ResPassword", ErrorMessage = "Xác nhận mật khẩu không đúng.")]
        public string ResConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email không để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }
    }
}