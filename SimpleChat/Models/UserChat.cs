using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class UserChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserChatId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ChatId { get; set; }

        // Navigation Property
        public virtual User User { get; set; }
        public virtual Chat Chat { get; set; }
    }
}