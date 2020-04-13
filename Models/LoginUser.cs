using System.ComponentModel.DataAnnotations;
using System;
namespace userDashboard.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string email{get;set;}
        [Required]
        public string password{get;set;}
    }
}