using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class GetMainCategoryByIdResponse
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public string VideoUrl {get;set;}
        public List<GetSubCategoryByIdResponse> SubCategory {get;set;}
    }

    public class GetSubCategoryByIdResponse{
        public int Id {get;set;}
        public int? MainCategoryId {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public int ProductCount {get;set;}
        public List<GetVariantBySubCategoryResponse> Variant {get;set;}
    }
    public class GetVariantBySubCategoryResponse{
        public int SubCategoryId {get;set;}
        public int VariantId {get;set;}
        public string VariantName {get;set;}
    }
}