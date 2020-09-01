using System;

namespace MayMayShop.API.Models
{
    public class SearchKeywordTrns
    {
        public int Id {get;set;}
        public int SearchKeywordId {get;set;}
        public virtual SearchKeyword SearchKeyword{get;set;}
        public int Count{get;set;}
        public DateTime CreatedDate{get;set;}
    }
}