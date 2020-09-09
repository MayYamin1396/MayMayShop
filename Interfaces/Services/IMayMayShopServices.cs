
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.ProductDto;
using OfficeOpenXml.Drawing;

namespace MayMayShop.API.Interfaces.Services
{
    public interface IMayMayShopServices
    {
        Task<ImageUrlResponse> UploadToS3(string encodedBase64, string ext, string folder);
        Task<ImageUrlResponse> UploadToS3NoFixedSize(string encodedBase64, string ext, string folder);  
        Image FixedSize(Image imgPhoto, int width, int height);
        Task DeleteFromS3(string ImgPath, string ThumbnailPath);
        DataTable ToDataTable(Stream s,string sheetName);
        List<UploadProductExcelImage> ExcelPicture(Stream s,string sheetName,int rowCount);
        string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format);
        string GetPrettyDate(DateTime d);
    }
}