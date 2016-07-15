using Colin.Gimbal;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ultra
{
    public static class TextureLoader
    {
        public static void LoadTexture(ComponentStaticData data, string Filename)
        {
            string path = Path.GetDirectoryName(ModLoader.ModMap[Assembly.GetCallingAssembly().FullName]);
            if (!GimbalGameManager.Headless)
            {
                data.BaseTexture = Texture2D.FromFile(GimbalGameManager.GraphicsDevice, Path.Combine(path, Filename));
                string nomount = Path.ChangeExtension(Filename, ".nomount.png");
                if (File.Exists(Path.Combine(path, nomount)))
                {
                    data.NoMountTexture = Texture2D.FromFile(GimbalGameManager.GraphicsDevice, Path.Combine(path, nomount));
                    Component.GenerateAreaFromImage(data.NoMountTexture, out data.NoMountArea);
                }
            }
            data.BaseTextureName = Path.Combine(path, Filename).Replace(Path.DirectorySeparatorChar, '-');
            data.TextureHash = Path.Combine(path, Filename).Replace(Path.DirectorySeparatorChar, '-').GetHashCode();
            if (data.BaseTexture != null)
            {
                data.BaseTextureWidth = data.BaseTexture.Width;
                data.BaseTextureHeight = data.BaseTexture.Height;
            }
        }

        public static Texture2D Load(string Filename)
        {
            if (!GimbalGameManager.Headless)
            {
                string path = Path.GetDirectoryName(ModLoader.ModMap[Assembly.GetCallingAssembly().FullName]);
                return Texture2D.FromFile(GimbalGameManager.GraphicsDevice, Path.Combine(path, Filename));
            }
            else
            {
                return null;
            }
        }
    }
}
