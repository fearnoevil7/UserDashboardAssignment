using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using userDashboard.Models;
using Microsoft.EntityFrameworkCore;
namespace userDashboard.Models
{
    public class Message
    {
        [Key]
        public int MessageId {get;set;}
        [Required]
        [MinLength(4, ErrorMessage = "Message must be atleast 4 characters long!")]
        public string Content {get;set;}
        [Required]
        public int UserId {get;set;}
        [ForeignKey("UserId")]
        public User Creator {get;set;}
        // public List<Comment> CommentsOnMessage {get;set;}

        // public bool IsComment {get;set;}
        // public int MessageCommentBelongsToId {get;set;}
        public int UserBeingMessagedId {get;set;}

        public List <Comment> ListOfComments {get;set;}

        // public string CommentId{get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
    }
}