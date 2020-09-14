namespace MayMayShop.API.Dtos.OrderDto
{
    public class PostOrderByWavePayResponse
    {
        public string message {get;set;}
        public string transaction_id {get;set;}
        public string WaveUrl {get;set;}
    }
}