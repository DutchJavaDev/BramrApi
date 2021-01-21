using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BramrApi.Utils;
using static QRCoder.PayloadGenerator;

namespace BramrApi.Data
{
    public class QrCodeGenerator
    {
        public void CreateQR(string url, string filename)
        {
            Url generator = new Url(url);
            string payload = generator.ToString();
            using QRCodeGenerator qrg = new QRCodeGenerator();
            using QRCodeData qRCodeData = qrg.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using QRCode qrCode = new QRCode(qRCodeData);
            using var bitmap = qrCode.GetGraphic(5, Color.DarkBlue, Color.White, true);

#if DEBUG
            if (!Directory.Exists(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\"))
            {
                Directory.CreateDirectory(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\");
            }

            bitmap.Save($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\temp\{filename}.jpeg", ImageFormat.Jpeg);
#else
            var path = Utility.CreatePathFromBegin(@$"usr/share/temp");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //bitmap.Save($@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\temp\{filename}.jpeg", ImageFormat.Jpeg);
            bitmap.Save($@"{Utility.CreatePathFromBegin(@$"{path}/{filename}.jpeg")}", ImageFormat.Jpeg);
#endif

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
