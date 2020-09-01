using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class Order
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string VoucherNo { get; set; }

        public DateTime OrderDate { get; set; }

        public double TotalAmt { get; set; }
        public double DeliveryFee { get; set; }
        public double NetAmt { get; set; }
        public int? TotalPoint { get; set; }        
        public bool IsDeleted { get; set; }
        public int OrderUserId { get; set; }        
        public int OrderStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int? PlatformId {get;set;}
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual OrderDeliveryInfo OrderDeliveryInfo { get; set; }

        public virtual ICollection<OrderPaymentInfo> OrderPaymentInfo { get; set; }
    }
}