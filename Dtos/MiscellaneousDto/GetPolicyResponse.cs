using System;

namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class GetPolicyResponse
    {
        public int Id {get;set;}
        public int SerNo {get;set;}
        public string Title {get;set;}
        public string Description {get;set;}
        public DateTime CreatedDate {get;set;}
        public int CreatedBy {get;set;}
    }
}