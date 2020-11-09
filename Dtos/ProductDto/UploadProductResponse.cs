using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class UploadProductResponse : ResponseStatus
    {
        public List<UploadProductIssues> IssuesList{get;set;}
    }
    public class UploadProductIssues{
        public string ProductName {get;set;}
        public string Reason {get;set;}
    }
}