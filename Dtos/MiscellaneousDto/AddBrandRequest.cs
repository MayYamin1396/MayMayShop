namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class AddBrandRequest
    {
        public string Name {get;set;}
        public CoverImage CoverImage{get;set;}

        public LogoImage LogoImage {get;set;}
    }

    public class CoverImage{
        public int ImageId {get;set;}
        public string ImageContent { get; set; }
        public string Extension { get; set; }
    }

    public class LogoImage{
        public int ImageId {get;set;}
        public string ImageContent { get; set; }
        public string Extension { get; set; }
    }
}