using System;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetSearchKeywordRequest
    {
        public DateTime FromDate {get;set;}
        public DateTime ToDate {get;set;}
        public int Top {get;set;}
    }
}