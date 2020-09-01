using System;

namespace MayMayShop.API.Models
{
    public class ActivityType
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public DateTime CreatedDate {get;set;}
        public int CreatedBy {get;set;}
    }
}