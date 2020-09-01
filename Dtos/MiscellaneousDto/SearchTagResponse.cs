using MayMayShop.API.Dtos;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class SearchTagResponse : ResponseStatus
    {
        public int Id {get;set;}
        public string Name {get;set;}
    }
}