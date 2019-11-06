using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class RefreshModel
    {
        [Required]
        public string AuthenticateToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
