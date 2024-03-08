using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace STech_Web.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(15, ErrorMessage = "Tên đăng nhập không quá 15 kí tự.")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Tên đăng nhập không chứa kí tự đặc biệt.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(20, ErrorMessage = "Mật khẩu không vượt quá 20 kí tự.")]
        public string Password { get; set; }
    }
}