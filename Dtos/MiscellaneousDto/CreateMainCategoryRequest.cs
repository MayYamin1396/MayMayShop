using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class CreateMainCategoryRequest
    {
        public string Name {get;set;}
        public string Url {get;set;}
        public string VideoUrl {get;set;}
        public List<SubCategoryRequest> SubCategory {get;set;}
    }

    public class SubCategoryRequest{
        public int MainCategoryId {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public List<CreateSubCategoryVariant> VariantList{get;set;}
    }
}