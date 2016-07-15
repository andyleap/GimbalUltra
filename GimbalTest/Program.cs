using Colin.Gimbal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Media.Imaging;

namespace GimbalTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PngBitmapDecoder decoder = new PngBitmapDecoder(new FileStream(@"E:\SteamLibrary\SteamApps\common\Gimbal\Hangar\FZ60.png", FileMode.Open, FileAccess.Read), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapMetadata data = (BitmapMetadata)decoder.Frames[0].Metadata;
            string text = data.GetQuery("/Text/Description") as string;
            text = text.Remove(0, "[GimbalShipData:V001[".Length);
            text = text.Remove(text.Length - "]]".Length, "]]".Length);

            

            byte[] buffer = Utility.Decompress(Convert.FromBase64String(text));
            MemoryStream serializationStream = new MemoryStream(buffer);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Vehicle vehicle = null;
            vehicle = (Vehicle)binaryFormatter.Deserialize(serializationStream);
        }
    }
}
