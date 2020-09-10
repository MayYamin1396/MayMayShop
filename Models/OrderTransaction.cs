using System;

namespace MayMayShop.API.Models
{
    public class OrderTransaction
    {
        public string Id {get;set;}
        public string TransactionData {get;set;}
        public int? OrderId {get;set;}
        public DateTime CreatedDate {get;set;}
        public int CreatedBy {get; set;}
        public DateTime? UpdatedDate {get;set;}
        public int UpdatedBy {get; set;}
        public string mm_order_id {get;set;}
    }
}