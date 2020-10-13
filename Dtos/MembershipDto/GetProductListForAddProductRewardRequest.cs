namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetProductListForAddProductRewardRequest
    {
        public string SearchText {get;set;}
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
    }
}