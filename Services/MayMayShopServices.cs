using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Dtos;
using MayMayShop.API.Const;
using Amazon;
using Amazon.S3.Model;
using System.Data;
using OfficeOpenXml;
using System.Linq;
using System.Collections.Generic;
using OfficeOpenXml.Drawing;
using MayMayShop.API.Dtos.ProductDto;

namespace MayMayShop.API.Services
{
    public class MayMayShopServices : IMayMayShopServices
    {
        public async Task<ImageUrlResponse> UploadToS3(string base64encodedstring, string ext, string folder)
        {
            var imgUrlResponse = new ImageUrlResponse();

            using (var client = new AmazonS3Client(MayMayShopConst.AWS_KEY,
                        MayMayShopConst.AWS_SECRET, RegionEndpoint.APSoutheast1))
            {
                var bytes = Convert.FromBase64String(base64encodedstring);
                using (var memoryStream = new MemoryStream(bytes))
                {

                    using (var img = Image.FromStream(memoryStream))
                    {
                        MemoryStream memoryStreamForThumbnail = new MemoryStream();
                        MemoryStream memoryStreamForImg = new MemoryStream();

                        Image thumbnail = this.FixedSize(img, 481, 481);
                        Image fullSize = this.FixedSize(img, 1080, 1080);

                        thumbnail.Save(memoryStreamForThumbnail, ImageFormat.Jpeg);
                        fullSize.Save(memoryStreamForImg, ImageFormat.Jpeg);

                        Guid fileNameForThumbnail = Guid.NewGuid();
                        Guid fileNameForFullSize = Guid.NewGuid();

                        //For Thumbnail
                        var uploadRequestForThumbnail = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStreamForThumbnail,
                            Key = MayMayShopConst.AWS_KEY_PATH +"/" + folder + "/" + fileNameForThumbnail + '.' + ext,
                            BucketName = "aws-mhs-bucket",
                            CannedACL = S3CannedACL.PublicRead
                        };

                        //For Full Size
                        var uploadRequestForFullSize = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStreamForImg,
                            Key = MayMayShopConst.AWS_KEY_PATH +"/" + folder + "/" + fileNameForFullSize + '.' + ext,
                            BucketName = "aws-mhs-bucket",
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(client);

                        await fileTransferUtility.UploadAsync(uploadRequestForThumbnail);
                        imgUrlResponse.ThumbnailPath = MayMayShopConst.AWS_STATIC_IMG_PATH + folder + "/" + 
                            fileNameForThumbnail + '.' + ext;

                        await fileTransferUtility.UploadAsync(uploadRequestForFullSize);
                        imgUrlResponse.ImgPath = MayMayShopConst.AWS_STATIC_IMG_PATH + folder + "/" + 
                            fileNameForFullSize + '.' + ext;
                    }

                    memoryStream.Flush();
                }
            }

            return imgUrlResponse;
        }

        public async Task<ImageUrlResponse> UploadToS3NoFixedSize(string base64encodedstring, string ext, string folder)
        {
            var imgUrlResponse = new ImageUrlResponse();

            using (var client = new AmazonS3Client(MayMayShopConst.AWS_KEY,
                        MayMayShopConst.AWS_SECRET, RegionEndpoint.APSoutheast1))
            {
                var bytes = Convert.FromBase64String(base64encodedstring);
                using (var memoryStream = new MemoryStream(bytes))
                {

                    using (var img = Image.FromStream(memoryStream))
                    {
                        MemoryStream memoryStreamForThumbnail = new MemoryStream();
                        MemoryStream memoryStreamForImg = new MemoryStream();

                        // Image thumbnail = this.FixedSize(img, 481, 481);
                        // Image  = this.FixedSize(img, 1080, 1080);
                        // Image thumbnail = img;
                        Image fullSize = img;
                        // thumbnail.Save(memoryStreamForThumbnail, ImageFormat.Jpeg);
                        fullSize.Save(memoryStreamForImg, ImageFormat.Jpeg);

                        Guid fileNameForThumbnail = Guid.NewGuid();
                        Guid fileNameForFullSize = Guid.NewGuid();

                        // //For Thumbnail
                        // var uploadRequestForThumbnail = new TransferUtilityUploadRequest
                        // {
                        //     InputStream = memoryStreamForThumbnail,
                        //     Key = MayMayShopConst.AWS_KEY_PATH +"/" + folder + "/" + fileNameForThumbnail + '.' + ext,
                        //     BucketName = "aws-mhs-bucket",
                        //     CannedACL = S3CannedACL.PublicRead
                        // };

                        //For Full Size
                        var uploadRequestForFullSize = new TransferUtilityUploadRequest
                        {
                            InputStream = memoryStreamForImg,
                            Key = MayMayShopConst.AWS_KEY_PATH +"/" + folder + "/" + fileNameForFullSize + '.' + ext,
                            BucketName = "aws-mhs-bucket",
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(client);

                        // await fileTransferUtility.UploadAsync(uploadRequestForThumbnail);
                        // imgUrlResponse.ThumbnailPath = MayMayShopConst.AWS_STATIC_IMG_PATH + folder + "/" + 
                        //     fileNameForThumbnail + '.' + ext;

                        await fileTransferUtility.UploadAsync(uploadRequestForFullSize);
                        imgUrlResponse.ImgPath = MayMayShopConst.AWS_STATIC_IMG_PATH + folder + "/" + 
                            fileNameForFullSize + '.' + ext;
                    }

                    memoryStream.Flush();
                }
            }

            return imgUrlResponse;
        }



        public Image FixedSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(width, height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Gray);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
   
        public async Task DeleteFromS3(string ImgPath, string ThumbnailPath)
        {
            using (var client = new AmazonS3Client(MayMayShopConst.AWS_KEY,
                        MayMayShopConst.AWS_SECRET, RegionEndpoint.APSoutheast1))
                    {
                        try
                        {
                            //For Full Size
                            string strImgPath = ImgPath;
                            // int pos1 = strImgPath.IndexOf('M');
                            // strImgPath = strImgPath.Remove(0,pos1);
                            strImgPath = strImgPath.Replace(MayMayShopConst.AWS_IMG_HOSTED,"");

                            var deleteObjectRequestForFullSize = new DeleteObjectRequest
                            {
                                BucketName = "aws-mhs-bucket",
                                Key = strImgPath
                            };

                            //For Thumbnail
                            string strThumbnailPath = ThumbnailPath;
                            // int pos2 = strThumbnailPath.IndexOf('M');
                            // strThumbnailPath = strThumbnailPath.Remove(0,pos2);
                            strThumbnailPath = strThumbnailPath.Replace(MayMayShopConst.AWS_IMG_HOSTED,"");

                            var deleteObjectRequestForThumbnail = new DeleteObjectRequest
                            {
                                BucketName = "aws-mhs-bucket",
                                Key = strThumbnailPath
                            };

                            Console.WriteLine("Deleting an fullsize");
                            await client.DeleteObjectAsync(deleteObjectRequestForFullSize);

                            Console.WriteLine("Deleting an Thumbnail");
                            await client.DeleteObjectAsync(deleteObjectRequestForThumbnail);
                        }
                        catch (AmazonS3Exception e)
                        {
                            Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an image", e.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an image", e.Message);
                        }
                    }
        }
    
        public DataTable ToDataTable(Stream s,string sheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage package = new ExcelPackage(s);
            ExcelWorksheet workSheet = package.Workbook.Worksheets.Where(a=>a.Name== sheetName).SingleOrDefault();
            DataTable table = new DataTable();
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                var newRow = table.NewRow();
                foreach (var cell in row)
                {
                    newRow[cell.Start.Column - 1] = cell.Text;
                }
                table.Rows.Add(newRow);
            }
            return table;
        }

        public List<UploadProductExcelImage> ExcelPicture(Stream s, string sheetName,int rowCount)
        {
            List<UploadProductExcelImage> dataList=new List<UploadProductExcelImage>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(s))
            {
                var workbook = package.Workbook;
                var sheet = workbook.Worksheets[0];
                ExcelWorksheet ws = package.Workbook.Worksheets.Where(a=>a.Name== sheetName).SingleOrDefault();
                //Create a lookup of drawings per sheet
                var lkDrawings = ws.Drawings.ToLookup(x => $"{ x.From.Row}_{x.From.Column}");

                //Use this lookup while iterating your cells and save it as image.

                for(int i=1;i<=rowCount;i++)
                {
                    var lookUpKey = $"{i}_{16}"; // assume column 16 is image 
                    if(lkDrawings.Contains(lookUpKey))
                    {        
                        List<ExcelPicture> imgList=new List<ExcelPicture>();                
                        for(int j=0;j<lkDrawings[lookUpKey].ToList().Count();j++)
                        {
                            ExcelPicture image = lkDrawings[lookUpKey].ToList()[j] as ExcelPicture;
                            imgList.Add(image);                           
                        }

                        if(imgList.Count>0)
                        {
                            UploadProductExcelImage data=new UploadProductExcelImage()
                            {
                                index=i,
                                ExcelPicture=imgList
                            }; 
                            dataList.Add(data); 
                        }                                               
                    }
                }               
            }
            return dataList;
        }
    
        public string ImageToBase64(System.Drawing.Image image,  System.Drawing.Imaging.ImageFormat format)

                {

                    using (MemoryStream ms = new MemoryStream())

                    {

                        // Convert Image to byte[]

                        image.Save(ms, format);

                        byte[] imageBytes = ms.ToArray();


                        // Convert byte[] to Base64 String

                        string base64String = Convert.ToBase64String(imageBytes);

                        return base64String;

                    }

        }

        public string GetPrettyDate(DateTime d)
        {
        // 1.
        // Get time span elapsed since the date.
        TimeSpan s = DateTime.Now.Subtract(d);

        // 2.
        // Get total number of days elapsed.
        int dayDiff = (int)s.TotalDays;

        // 3.
        // Get total number of seconds elapsed.
        int secDiff = (int)s.TotalSeconds;

        // 4.
        // Don't allow out of range values.
        if (dayDiff < 0 || dayDiff >= 31)
        {
            return null;
        }

        // 5.
        // Handle same-day times.
        if (dayDiff == 0)
        {
            // A.
            // Less than one minute ago.
            if (secDiff < 60)
            {
                return "just now";
            }
            // B.
            // Less than 2 minutes ago.
            if (secDiff < 120)
            {
                return "1 minute ago";
            }
            // C.
            // Less than one hour ago.
            if (secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            // D.
            // Less than 2 hours ago.
            if (secDiff < 7200)
            {
                return "1 hour ago";
            }
            // E.
            // Less than one day ago.
            if (secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        // 6.
        // Handle previous days.
        if (dayDiff == 1)
        {
            return "yesterday";
        }
        if (dayDiff < 7)
        {
            return string.Format("{0} days ago",
                dayDiff);
        }
        if (dayDiff < 31)
        {
            return string.Format("{0} weeks ago",
                Math.Ceiling((double)dayDiff / 7));
        }
        return null;
    }

    }
}
