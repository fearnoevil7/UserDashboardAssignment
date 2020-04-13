using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using userDashboard.Models;
using Microsoft.EntityFrameworkCore;
namespace userDashboard.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2, ErrorMessage = "First name must be atleast 2 characters long.")]
        public string FirstName{get; set;}
        [Required]
        [MinLength(2, ErrorMessage = "Last name must be atleast 2 characters.")]
        public string LastName{get;set;}
        [Required]
        [EmailAddress]
        public string Email{get;set;}
        [Required]
        [MinLength(8, ErrorMessage = "Password must be atleast 8 characters")]
        public string Password{get;set;}
        [NotMapped]
        [Required]
        [Compare("Password")]
        public string ConfirmPassword{get;set;}
        public bool IsAdmin{get;set;}
        public string Description{get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Message> CreatedMessages {get;set;}
        public List<Comment> CreatedComments {get;set;}
    }

}