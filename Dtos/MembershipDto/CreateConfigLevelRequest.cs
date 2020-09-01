using MayMayShop.API.Dtos;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class CreateConfigLevelRequest : ApplicationRequest
    {
        public string Name {get;set;}
        public string Description {get;set;}
        public int MinPoint {get;set;}
        public int MaxPoint {get;set;}
        public string ImageContent { get; set; }
        public string Extension { get; set; }
    }
}