namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetActivityLogResponse
    {
        public int Id {get;set;}
        public string UserName {get;set;}
        public string Description {get;set;}
        public string ActivityType {get;set;}
        public string TimeAgo{get;set;}
        public string Platform{get;set;}
    }
}