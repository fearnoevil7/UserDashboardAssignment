using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace userDashboard.Models
{
    public class Comment
    {
        [Key]
        public int CommentId {get;set;}
        [Required]
        public string Content {get;set;}
        
        public int MessageId {get;set;}

        
        public int UserId {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        [ForeignKey("MessageId")]
        public Message MessageCommentBelongsTo {get;set;}

        public int UserBeingCommentedOnId {get;set;}
        [ForeignKey("UserId")]
        public User UserCreator {get;set;}

    }
}