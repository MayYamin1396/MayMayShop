using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MayMayShop.API.Models
{
    public class TrnCartDeliveryInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhNo { get; set; }

        public string Remark { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string DeliveryFromTime { get; set; }

        public string DeliveryToTime { get; set; }

        public int DeliveryServiceId { get; set; }

        public int TownshipId { get; set; }

        public int CityId { get; set; }

        public int FromEstDeliveryDay { get; set; }

        public int ToEstDeliveryDay { get; set; }

        public double DeliveryAmt { get; set; }

        //public DeliveryService DeliveryService { get; set; }

        //public City City { get; set; }

        //public Township Township { get; set; }
        public DateTime? UpdatedDate { get; set; }



    }
}