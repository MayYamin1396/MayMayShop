namespace MayMayShop.API.Models
{
    public class Setting
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsActive { get; set; }  
    }
}