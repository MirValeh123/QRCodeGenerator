using QRCoder;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace QRTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetQRCodeWithIcon([FromQuery] string url)
        {
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            
            Color darkColor = ColorTranslator.FromHtml("#102A56");
            Color lightColor = Color.White; 
            Bitmap qrCodeImage = qrCode.GetGraphic(2, darkColor, lightColor, true); 

            Bitmap icon = LoadIcon("Images/Group.jpg");

            icon = ResizeImage(icon, 22, 22);

            Bitmap finalImage = EmbedIconIntoQRCode(qrCodeImage, icon);

            using (MemoryStream ms = new MemoryStream())
            {
                finalImage.Save(ms, ImageFormat.Png);
                byte[] qrCodeBytes = ms.ToArray();
                // Return the QR code image as a response
                return File(qrCodeBytes, "image/png");
            }
        }

        
        private Bitmap LoadIcon(string path)
        {
            using (var webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(path);
                using (MemoryStream mem = new MemoryStream(data))
                {
                    return new Bitmap(mem);
                }
            }
        }

        
        private Bitmap EmbedIconIntoQRCode(Bitmap qrCodeImage, Bitmap icon)
        {
            Graphics g = Graphics.FromImage(qrCodeImage);

            
            int xPos = (qrCodeImage.Width - icon.Width) / 2;
            int yPos = (qrCodeImage.Height - icon.Height) / 2;

          
            g.DrawImage(icon, new Rectangle(xPos, yPos, icon.Width, icon.Height));

            return qrCodeImage;
        }

        
        private Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }
    }
}
