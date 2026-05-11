using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Texture_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Texture);
		public uint Order => 0;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var Texture = STFResource as STF_Texture;
			var Image = Texture.Image as STF_Image;

			var nonColor = Image.data_type != "color";

			var ret = new Texture2D(8, 8, TextureFormat.RGBA32, Texture.mipmaps, nonColor, true);
			ImageConversion.LoadImage(ret, Image.buffer.Data);

			if (Texture.height != ret.height || Texture.width != ret.width)
			{
				ret = Resize(ret, (int)Texture.width, (int)Texture.height, TextureFormat.RGBA32, Texture.mipmaps, nonColor);
			}

			if (Texture.quality <= 0.5)
				ret.Compress(false);
			else if (Texture.quality <= 0.75)
				ret.Compress(true);

			ret.name = Texture.STF_Name ?? Image.STF_Name;
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
