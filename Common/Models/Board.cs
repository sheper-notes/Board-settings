using Common.Enums;
using EntityFrameworkCore.EncryptColumn.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Board
    {
        public long Id { get; set; }
        [MaxLength(20)]
        [EncryptColumn]
        public string Name { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public List<UserRoleRelation> Users { get; set; }
    }
}
