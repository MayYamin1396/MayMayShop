using MayMayShop.API.Dtos;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetSubCategoryResponse : ResponseStatus
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public string Url {get;set;}
        public int MainCategoryId {get;set;}
    }
    public class GetSubCategoryVariant {
        public int Id {get;set;}
        public int SubCategoryId {get;set;}
        public string Name {get;set;}
    }
    
}