using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SimpleChat.Models
{
    public class ChatMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatMessageId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChatId { get; set; }
        [Required]
        public string Message { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual Chat Chat { get; set; }
    }
}