using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class ReceivedMemberPointRequest : ApplicationRequest
    {
        public int UserId {get;set;}
        public string VoucherNo {get;set;}
        public List<ReceivedMemberPointProductCategory> ProductCategory {get;set;}
         
    }
    public class ReceivedMemberPointProductCategory
    {
        public int ProductCategoryId {get;set;}
        public double TotalAmount {get;set;}
    }
}