using System;

namespace MayMayShop.API.Dtos.UserDto
{
    public class GetAllSellerUserIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }

    }
}