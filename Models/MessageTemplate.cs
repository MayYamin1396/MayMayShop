using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public partial class MessageTemplate
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string ActionName { get; set; }
        
        [StringLength(500)]

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
