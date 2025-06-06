using System.Collections.Generic;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Image_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Image);
		public uint Order => 0;
		public int Priority => 1;

		public List<Object> Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var Image = STFResource as STF_Image;
			Texture2D ret;

			var nonColor = Image.data_type != "color";

			// TODO make this vastly more legit
			if (Image.Components.Find(c => c.GetType() == typeof(STF_Texture)) is STF_Texture texture)
			{
				ret = new Texture2D(8, 8, TextureFormat.RGBA32, texture.mipmaps, nonColor, true);

				ImageConversion.LoadImage(ret, Image.buffer.Data);

				if (texture.quality <= 0.5)
					ret.Compress(false);
				else if (texture.quality <= 0.75)
					ret.Compress(false);
			}
			else
			{
				ret = new Texture2D(8, 8, TextureFormat.RGBA32, true, nonColor, true);

				ImageConversion.LoadImage(ret, Image.buffer.Data);
			}

			//if (Image.data_type == "normal_map")
			if (Image.data_type == "non_color")
			{
				//DealWithNormals(ret);
			}

			ret.name = Image.STF_Name;
			return new() { ret };
		}
	}
}
