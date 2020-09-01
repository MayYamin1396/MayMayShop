using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class Store
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string Address { get; set; }

        [StringLength(30)]
        public string PhNo { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}