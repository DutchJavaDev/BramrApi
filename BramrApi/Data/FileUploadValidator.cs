using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace BramrApi.Data
{
    public static class FileUploadValidator
    {
        public static bool FileIsImage(Stream stream) 
        {
            try
            {
                //Read an image from the stream...
                var i = Image.FromStream(stream);

                //Move the pointer back to the beginning of the stream
                stream.Seek(0, SeekOrigin.Begin);
                
                if (ImageFormat.Jpeg.Equals(i.RawFormat))
                    return true;
                return ImageFormat.Png.Equals(i.RawFormat) || ImageFormat.Gif.Equals(i.RawFormat);
            }
            catch
            {
                return false;
            }
        }

    }
}
