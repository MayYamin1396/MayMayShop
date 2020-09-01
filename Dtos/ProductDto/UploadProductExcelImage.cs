using System.Collections.Generic;
using OfficeOpenXml.Drawing;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class UploadProductExcelImage
    {
        public int index {get;set;}
        public List<ExcelPicture> ExcelPicture {get;set;}
    }
}