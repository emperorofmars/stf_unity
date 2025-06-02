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

		public List<Object> Process(ProcessorContext Context, ISTF_Resource STFResource)
		{
			var Image = STFResource as STF_Image;
			Texture2D ret;

			// TODO make this vastly more legit
			if (Image.Components.Find(c => c is STF_Texture) is STF_Texture texture)
			{
				var textureformat = texture.quality < 0.65 ? TextureFormat.DXT5 : TextureFormat.ARGB32;
				ret = new Texture2D((int)texture.width, (int)texture.height, textureformat, true);
				ImageConversion.LoadImage(ret, Image.buffer.Data);
				if (texture.quality < 0.65)
					ret.Compress(false);
			}
			else
			{
				ret = new Texture2D(2, 2);
				ImageConversion.LoadImage(ret, Image.buffer.Data);
			}

			ret.name = Image.STF_Name;
			return new() { ret };
		}
	}
}