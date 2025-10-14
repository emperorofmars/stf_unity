using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Image_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Image);
		public uint Order => 0;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var Image = STFResource as STF_Image;
			Texture2D ret;

			var nonColor = Image.data_type != "color";

			// TODO make this vastly more legit
			if (Image.Components.Find(c => c.GetType() == typeof(STF_Texture)) is STF_Texture texture)
			{
				ret = new Texture2D(8, 8, TextureFormat.RGBA32, texture.mipmaps, nonColor, true);
				ImageConversion.LoadImage(ret, Image.buffer.Data);

				if (texture.height != ret.height || texture.width != ret.width)
				{
					ret = Resize(ret, (int)texture.width, (int)texture.height, TextureFormat.RGBA32, texture.mipmaps, nonColor);
				}

				if (texture.quality <= 0.5)
					ret.Compress(false);
				else if (texture.quality <= 0.75)
					ret.Compress(true);
			}
			else
			{
				ret = new Texture2D(8, 8, TextureFormat.RGBA32, true, nonColor, true);
				ImageConversion.LoadImage(ret, Image.buffer.Data);
			}

			ret.name = Image.STF_Name;
			return (new() { ret }, new() { ret });
		}

		private Texture2D Resize(Texture2D Texture, int TargetWidth, int TargetHeight, TextureFormat Format, bool Mipmaps = true, bool Linear = false)
		{
			var tmp = new RenderTexture(TargetWidth, TargetHeight, Format == TextureFormat.RGBA32 ? 32 : 24);
			RenderTexture.active = tmp;
			Graphics.Blit(Texture, tmp);
			var ret = new Texture2D(TargetWidth, TargetHeight, Format, Mipmaps, Linear);
			ret.ReadPixels(new Rect(0, 0, TargetWidth, TargetHeight), 0, 0);
			ret.Apply();
			return ret;
		}
	}
}
