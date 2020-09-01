using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Body { get; set; }

        public int? BodyReferenceId { get; set; }

        [StringLength(255)]
        public string ImgUrl { get; set; }

        [StringLength(255)]
        public string RedirectAction { get; set; }

        [StringLength(255)]
        public string ReferenceAttribute { get; set; }

        public int UserId { get; set; }

        public bool IsSeen { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}