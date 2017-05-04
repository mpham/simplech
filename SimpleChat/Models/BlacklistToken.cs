using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class BlacklistToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlacklistTokenId { get; set; }
        [Required]
        public string Token { get; set; }
    }
}