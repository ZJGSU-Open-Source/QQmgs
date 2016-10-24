using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class GroupPlugin
    {
        [Key]
        public int Id { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public bool HasCheckInPad { get; set; }

        public bool HasDisplayWall { get; set; }

        public bool HasInfoFocus { get; set; }
    }
}
