using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ObjectView
{
    public class ForgotPasswordView
    {
        [Required]
        public string Email { get; set; }
    }
}
