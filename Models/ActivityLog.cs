using System;

namespace MayMayShop.API.Models
{
    public class ActivityLog
    {
        public int Id {get;set;}
        public int UserId {get;set;}
        public int ActivityTypeId {get;set;}
        public virtual ActivityType ActivityType {get;set;}
        public string Value {get;set;}
        public DateTime CreatedDate {get;set;}
        public int CreatedBy {get;set;}
        public DateTime? UpdatedDate {get;set;}
        public int? UpdatedBy {get;set;}
        public string IPAddress {get;set;}
        public int PlatformId {get;set;}
        public int? ResultCount {get;set;}
        public virtual Platform Platform {get;set;}
    }
}