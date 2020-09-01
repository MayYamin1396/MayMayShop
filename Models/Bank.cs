using System;

namespace MayMayShop.API.Models
{
    public class Bank
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public string SelectUrl {get;set;}
        public string AccountNo {get;set;}
        public string HolderName {get;set;}
        public DateTime CreatedDate {get;set;}
        public int CreatedBy {get;set;}
    }
}