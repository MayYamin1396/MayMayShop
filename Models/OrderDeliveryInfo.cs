using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class OrderDeliveryInfo
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int DeliveryServiceId { get; set; }

        //public virtual DeliveryService DeliveryService { get; set; }

        public int? CityId { get; set; }

        //public virtual City City { get; set; }

        public int? TownshipId { get; set; }

        //public virtual Township Township { get; set; }
        
        
        public string Address { get; set; }

        
        public string PhNo { get; set; }

        
        public string Remark { get; set; }     
        public DateTime DeliveryDate{get;set;}  

        [StringLength(10)] 
        public string FromTime { get; set; }

        [StringLength(10)] 
        public string ToTime { get; set; }
    }
}