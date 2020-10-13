using System;

namespace MayMayShop.API.Models
{
    public class PaymentService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImgPath { get; set; }
        public string BackgroundUrl {get;set;}
        public string HolderName {get;set;}
        public string AccountNo {get;set;}
        public string GroupName {get;set;}
        public bool IsPaymentGateWay{get;set;}
        public int SerNo {get;set;}
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public string PaymentType {get;set;}
    }
}