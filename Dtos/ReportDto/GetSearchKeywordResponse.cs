using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetSearchKeywordResponse
    {
        public int NoOfSearch {get;set;}
        public int MostSearch {get;set;}
        public string AverageSearch {get;set;}
        public List<TopKeyword> TopKeyword{get;set;}
    }
    public class TopKeyword{
        public int KeywordId {get;set;}
        public string Name{get;set;}
        public int Count {get;set;}
        public double Percent {get;set;}
    }
}