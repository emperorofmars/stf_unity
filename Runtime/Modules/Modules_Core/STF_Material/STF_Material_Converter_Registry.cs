using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material
{
	public static class STF_Material_Converter_Registry
	{
		public static readonly List<IMaterialConverter> DefaultConverters = new();
		private static readonly Dictionary<string, IMaterialConverter> RegisteredConverters = new();

		public static readonly string DefaultShader;

		static STF_Material_Converter_Registry()
		{
			if (QualitySettings.renderPipeline != null)
			{
				var converter = new UniversalRenderPipeline_Lit();
				DefaultConverters.Add(converter);
				DefaultShader = converter.ShaderName;
			}
			else
			{
				var converter = new StandardConverter();
				DefaultConverters.Add(converter);
				DefaultShader = converter.ShaderName;
			}
		}

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
				foreach(var (shaderName, module) in RegisteredConverters)
				{
					if(ret.ContainsKey(shaderName))
						ret[shaderName] = module;
					else
						ret.Add(shaderName, module);
				}
				return ret;
			}
		}
	}

}
