namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class SearchCategoryResponse :ResponseStatus
    {
        public int Id {get;set;}
        public string Url {get;set;}
        public string Name {get;set;}
        public int SubCount {get;set;}

    }
}