using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MayMayShop.API.Dtos;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetCityResponse : ResponseStatus
    {
        public List<GetCityResponseArry> CityList { get; set; }
    }

    public class GetCityResponseArry
    {
       public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Remark { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

    }
}