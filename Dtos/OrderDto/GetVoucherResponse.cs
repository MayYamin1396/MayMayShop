using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetVoucherResponse
    {
        public string ShopUrl {get;set;}
        public string ShopName {get;set;}
        public string Address {get;set;}
        public string PhoneNo {get;set;}
        public string BuyerName {get;set;}
        public string BuyerPhoneNo {get;set;}
        public string BuyerRemark {get;set;}
        public string BuyerAddress {get;set;}
        public string VoucherNo {get;set;}
        public DateTime OrderDate {get;set;}
        public double TotalAmount {get;set;}
        public double Discount {get;set;}
        public double DeliveryAmount {get;set;}
        public double CommercialTax {get;set;}
        public double NetAmount {get;set;}
        public string PaymentType {get;set;}
        public string BankName {get;set;}
        public List<GetVoucherItem> ItemList{get;set;}
        public string QRCode {get;set;}
    }
    public class GetVoucherItem {
        public int ProductId {get;set;}
        public int Qty{get;set;}
        public string Name {get;set;}
        public int SkuId {get;set;}
        public string Sku {get;set;}
        public double Price {get;set;}
    }
}