using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

namespace com.squirrelbite.stf_unity.processors
{
	public class STF_Image_Processor : ISTF_Processor
	{
		public System.Type TargetType => typeof(STF_Image);
		public uint Order => 5;
		public int Priority => 1;

		public (List<Object>, List<Object>) Process(ProcessorContextBase Context, ISTF_Resource STFResource)
		{
			var Image = STFResource as STF_Image;
			Texture2D ret = null;

			var nonColor = Image.data_type != "color";

			// Try to get processed texture from components
			foreach(var c in Image.Components)
			{
				if(c.ProcessedObjects.Find(t => t is Texture2D) is Texture2D texture)
				{
					ret = texture;
					break;
				}
			}
			if(!ret) // Otherwise create a basic uncompressed texture
			{
				ret = new Texture2D(8, 8, TextureFormat.RGBA32, true, nonColor, true);
				ImageConversion.LoadImage(ret, Image.buffer.Data);
				ret.name = Image.STF_Name;
			}

			return (new() { ret }, new() { ret });
		}
	}
}
