namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderIdByTransactionIdResponse : ResponseStatus
    {
        public int? OrderId {get;set;}
    }
}