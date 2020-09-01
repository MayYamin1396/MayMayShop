namespace MayMayShop.API.Models
{
    public class ProductTag
    {
        public int ProductId { get; set; }

        public int  TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}