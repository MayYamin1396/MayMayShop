namespace MayMayShop.API.Models
{
    public class ProductClip
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ClipPath { get; set; }

        public int SeqNo { get; set; }

        public Product Product { get; set; }
    }
}