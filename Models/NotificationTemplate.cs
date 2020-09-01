using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class NotificationTemplate
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string ActionName { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(700)]
        public string Body { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}