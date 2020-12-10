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
            using QRCodeGenerator qrg = new QRCodeGenerator();
            using QRCodeData qRCodeData = qrg.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using QRCode qrCode = new QRCode(qRCodeData);
            using var bitmap = qrCode.GetGraphic(5, Color.DarkBlue, Color.White, true);
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\");
            }

            bitmap.Save($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\qrCode.jpeg", ImageFormat.Jpeg);

            //var image = BitMapToBytes(bitmap);
            //var base64 = Convert.ToBase64String(image);
            //return base64;
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
