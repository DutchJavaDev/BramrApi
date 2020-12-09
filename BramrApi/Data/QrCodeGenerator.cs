using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace BramrApi.Data
{
    public class QrCodeGenerator
    {
        public void CreateQR(string url)
        {
            Url generator = new Url(url);
            string payload = generator.ToString();
            QRCodeGenerator qrg = new QRCodeGenerator();
            QRCodeData qRCodeData = qrg.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qRCodeData);
            var bitmap = qrCode.GetGraphic(5);
            bitmap.Save(@"C:\Users\mathi\Desktop\qrCode.jpeg", ImageFormat.Jpeg);
            //var image = BitMapToBytes(bitmap);
            //var base64 = Convert.ToBase64String(image);
            //eturn base64;
        }
        //private static byte[] BitMapToBytes(Bitmap bitmap)
        //{
        //    using (var stream = new MemoryStream())
        //    {
        //        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        //        return stream.ToArray();
        //    }
        //}
    }
}
