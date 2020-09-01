using System.Collections.Generic;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetSubCategoryWithVariantsResponse
    {
         public int Id {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public string Url {get;set;}
        public int? MainCategoryId {get;set;}

        public List<VariantInfo> Variants {get;set;}
    }
    public class VariantInfo {
        public int Id {get;set;}
        public string Name {get;set;}
    }
}