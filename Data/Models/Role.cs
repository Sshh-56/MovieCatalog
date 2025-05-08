using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        public int MovieId { get; set; }
        public int ActorId { get; set; }

        [Required]
        public string CharacterName { get; set; }

        [ForeignKey("MovieId")]
        public Movie? Movie { get; set; } // Navigation Property for Movie
        [ForeignKey("ActorId")]
        public Actor? Actor { get; set; } // Navigation Property for Actor
    }
}
