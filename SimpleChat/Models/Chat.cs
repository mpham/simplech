using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChatId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CreatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<UserChat> UserChats { get; set; }
    }
}