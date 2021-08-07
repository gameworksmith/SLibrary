using UnityEngine;
using ZXing;

namespace QRCode
{
    public static class QRCodeUtil
    {
        /// <summary>
        /// 将制定字符串信息转换成二维码图片信息
        /// </summary>
        /// <param name="formatStr">要产生二维码的字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns></returns>
        public static Color32[] GeneQRCode(string formatStr, int width, int height)
        {
            ZXing.QrCode.QrCodeEncodingOptions options = new ZXing.QrCode.QrCodeEncodingOptions(); //绘制二维码之前 进行设置

            options.CharacterSet = "UTF-8"; //设置字符编码，确保字符串信息保持正确

            options.Width = width; //设置二维码宽
            options.Height = height; //设置二维码高
            options.Margin = 1; //设置二维码留白 （值越大，留白越大，二维码越小）

            BarcodeWriter writer = new BarcodeWriter {Format = BarcodeFormat.QR_CODE, Options = options}; //实例化字符串绘制二维码工具

            return writer.Write(formatStr);
        }


        /// <summary>
        /// 根据二维码图片信息绘制制定字符串信息的二维码到指定区域
        /// </summary>
        /// <param name="str">字符串信息</param>
        /// <param name="width">二维码的宽度</param>
        /// <param name="height">二维码的高度</param>
        /// <returns></returns>
        public static Texture2D ShowQRCode(string str, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);

            Color32[] colors = GeneQRCode(str, width, height);

            texture.SetPixels32(colors);

            texture.Apply();

            return texture;
        }
    }
}