using System;
namespace MayMayShop.API.Models
{
    public class Banner
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public string BannerType {get;set;}
        public int BannerLinkId {get;set;}
        public virtual BannerLink BannerLink {get;set;}
        public bool IsActive {get;set;}
        public int CreatedBy {get;set;}
        public DateTime CreatedDate {get;set;}
        public int? UpdatedBy {get;set;}
        public DateTime? UpdatedDate {get;set;}
    }
}