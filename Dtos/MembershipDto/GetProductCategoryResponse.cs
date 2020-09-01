namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetProductCategoryResponse : ResponseStatus
    {
        public int ProductCategoryId {get;set;} 
        public int ConfigMemberPointId {get;set;}
        public string ProductCategoryName {get;set;} 
        public string Url {get;set;} 
        
    }
}