namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class UpdateBrandRequest
   {
       public int Id {get;set;}
        public string Name {get;set;}
        public CoverImage CoverImage{get;set;}

        public LogoImage LogoImage {get;set;}
    }
}