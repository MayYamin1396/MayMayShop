namespace MayMayShop.API.Models
{
    public class ProductWholeSale
    {
        public int Id {get;set;}
        public int ProductId {get;set;}
        public virtual Product Product { get; set; }
        public int MinQty {get;set;}
        public double FixedPrice {get;set;}
        public int Percent {get;set;}
    }
}