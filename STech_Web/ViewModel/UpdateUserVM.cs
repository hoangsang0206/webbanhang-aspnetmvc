using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.ViewModel
{
    public class UpdateUserVM
    {
        public string UserFullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
    }
}