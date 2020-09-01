using MayMayShop.API.Dtos;
using MayMayShop.API.Models;
using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetConfigMemberPointResponse : ResponseStatus
    {
        public int Id {get;set;}
        public string Description {get;set;}
        public double Amount {get;set;}
        public int Point {get;set;}
        public DateTime? UpdatedDate {get;set;}
        public List<GetConfigMemberPointProductCategory> ProductCategoryList {get;set;}
    }
    public class GetConfigMemberPointProductCategory{
        public int ProductCategoryId {get;set;}
        public string ProductCategoryName {get;set;} 
        public string Url {get;set;}
        public int ConfigMemberPointId {get;set;} 
        public int ApplicationConfigId {get;set;}
    }
}