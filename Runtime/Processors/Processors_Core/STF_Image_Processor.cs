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
			// TODO vastly improve this, use information from an STF_Texture component if present on this image
			var ret = new Texture2D(8, 8);
			ret.name = Image.STF_Name;
			ret.LoadImage(Image.buffer.Data);
			return new() { ret };
		}
	}
}