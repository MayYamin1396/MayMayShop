using System.Collections.Generic;
using MayMayShop.API.Dtos;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetMainCategoryResponse : ResponseStatus
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public string Url {get;set;}
        public List<GetSubCategoryResponse> SubCategory{get;set;}

    }
}