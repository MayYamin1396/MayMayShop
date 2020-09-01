using System;

namespace MayMayShop.API.Dtos.UserDto
{
    public class GetUserInfoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public int CityId { get; set; }
        public int TownshipId { get; set; }
        public string Address { get; set; }
        public int UserTypeId {get;set;}
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}