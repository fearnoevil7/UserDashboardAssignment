using System.ComponentModel.DataAnnotations;
using System;
namespace userDashboard.Models
{
    public class UpdatedUser
    {
        [Required]
        [MinLength(2, ErrorMessage = "First name must be atleast 2 characters!")]
        public string firstName{get;set;}
        [Required]
        [MinLength(2, ErrorMessage = "Last name must be atleast 2 characters!")]
        public string lastName{get;set;}
        [Required]
        [EmailAddress]
        public string email{get;set;}
        public bool IsAdmin{get;set;}
        public string password{get;set;}
        public string confirmPassword{get;set;}
        public string description{get;set;}
    }
}