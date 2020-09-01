using System;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetNotificationResponse
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string Url {get;set;}
        public string RedirectAction { get; set; }
        public string ReferenceAttribute { get; set; }
        public DateTime NotificationDate{get;set;}
        public int Count { get; set; }
        public bool IsSeen { get; set; }
    }
}