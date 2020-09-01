using System;

namespace MayMayShop.API.Models
{
    public class OrderPaymentInfo
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
        
        public int PaymentServiceId { get; set; }

        public virtual PaymentService PaymentService { get; set; }
        public int? BankId {get;set;}
        public virtual Bank Bank { get; set; }
        public DateTime TransactionDate { get; set; }

        public string ApprovalImgUrl { get; set; }

        public string PhoneNo { get; set; }

        public int PaymentStatusId { get; set; }

        public virtual PaymentStatus PaymentStatus { get; set; }

        public string Remark { get; set; }
        public string SellerRemark {get;set;}
        public bool IsApproved { get; set; }
    }
}