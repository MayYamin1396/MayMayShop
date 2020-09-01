using System;
namespace MayMayShop.API.Models
{
    public class BannerLink
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public int CreatedBy {get;set;}
        public DateTime CreatedDate {get;set;}
        public bool IsActive{get;set;}
    }
}