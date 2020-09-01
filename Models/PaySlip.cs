namespace MayMayShop.API.Models
{
    public class PaySlip
    {
        public int Id {get;set;}
        public string Url {get;set;}
        public int OrderPaymentInfoId {get;set;}
        public virtual OrderPaymentInfo OrderPaymentInfo { get; set; }
    }
}