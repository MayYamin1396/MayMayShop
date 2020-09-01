using System;

namespace MayMayShop.API.Dtos.ReportDto
{
    public class GetProductSearchResponse
    {
        public string Name {get;set;}
        public int NoOfSearch {get;set;}
        public int? ResultCount {get;set;}
        public DateTime SearchDate {get;set;}
    }
}