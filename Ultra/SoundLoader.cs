using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Colin.Gimbal;
using System.Reflection;

namespace Ultra
{
    public class SoundLoader
    {
        public static SoundEffect Load(string Filename)
        {
            if (!GimbalGameManager.Headless)
            {
                string path = Path.GetDirectoryName(ModLoader.ModMap[Assembly.GetCallingAssembly().FullName]);
                var wave = readWav(Path.Combine(path, Filename));
                var con = typeof(SoundEffect).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]
                {
                    typeof(byte[]),
                    typeof(byte[]),
                    typeof(int),
                    typeof(int),
                    typeof(int)
                }, null);

                int len = wave.data.Length / BitConverter.ToInt32(wave.format, 8);

                var se = (SoundEffect)con.Invoke(new object[] { wave.format, wave.data, 0, 0, len });
                return se;
            }
            return null;
        }

        class Wave
        {
            internal byte[] format;
            internal byte[] data;
        }
        static Wave readWav(string filename)
        {
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);
                    byte[] Format = new byte[18];
                    // chunk 0
                    int chunkID = reader.ReadInt32();
                    int fileSize = reader.ReadInt32();
                    int riffType = reader.ReadInt32();


                    // chunk 1
                    int fmtID = reader.ReadInt32();
                    int fmtSize = reader.ReadInt32(); // bytes for this chunk

                    reader.Read(Format, 0, 16);

                    if (fmtSize == 18)
                    {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    int dataID = reader.ReadInt32();
                    int bytes = reader.ReadInt32();

                    // DATA!
                    byte[] Data = reader.ReadBytes(bytes);
                    return new Wave { format = Format, data = Data };
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
