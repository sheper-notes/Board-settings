using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class UserRoleRelation
    {
        public long Id { get; set; }
        [MaxLength(40)]
        public string UserId { get; set; }
        public long BoardId { get; set; }
        public Role Role { get; set; }
        public Board Board { get; set; }
    }
}
