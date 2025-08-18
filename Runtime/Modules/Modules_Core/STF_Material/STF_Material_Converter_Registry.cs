
using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules.stf_material
{

	public static class STF_Material_Converter_Registry
	{
		public static readonly List<IMaterialConverter> DefaultConverters = new() {
			new StandardConverter(),
			new UniversalRenderPipeline_Lit(),
		};
		private static readonly Dictionary<string, IMaterialConverter> RegisteredConverters = new();

		public static void RegisterConverter(IMaterialConverter Converter)
		{
			if(!RegisteredConverters.ContainsKey(Converter.ShaderName))
				RegisteredConverters.Add(Converter.ShaderName, Converter);
		}

		public static Dictionary<string, IMaterialConverter> Converters
		{
			get
			{
				var ret = new Dictionary<string, IMaterialConverter>(RegisteredConverters);
				foreach(var module in DefaultConverters)
				{
					if(!ret.ContainsKey(module.ShaderName))
						ret.Add(module.ShaderName, module);
				}
				return ret;
			}
		}
	}

}
