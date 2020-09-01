using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace MayMayShop.API.Helpers
{
    public static class QRCodeHelper
    {
    public static string GenerateQRCode(string qrText)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
        QRCode qrCode = new QRCode(qrCodeData);
        Bitmap qrCodeImage = qrCode.GetGraphic(20);        
        System.IO.MemoryStream ms = new MemoryStream();
        qrCodeImage.Save(ms, ImageFormat.Jpeg);
        byte[] byteImage = ms.ToArray();
        var SigBase64= Convert.ToBase64String(byteImage); // Get Base64
        var imgUrl=string.Format("data:image/png;base64,{0}",SigBase64);
        return imgUrl;
    }
    private static Byte[] BitmapToBytes(Bitmap img)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
    }    
}