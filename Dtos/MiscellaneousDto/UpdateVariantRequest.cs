namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class UpdateVariantRequest
    {
        public int Id {get;set;}
        public int ProductCategoryId {get;set;}
        public string Name {get;set;}
    }
}