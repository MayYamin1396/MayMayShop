namespace MayMayShop.API.Dtos.MembershipDto
{
    public class RedemptionMemberPointRequest : ApplicationRequest
    {
        public int UserId {get;set;}
        public int Point {get;set;}
        public string ProductName {get;set;}
         
    }
}