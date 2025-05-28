
using System;
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stf_material;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Material : STF_DataResource
	{
		[System.Serializable]
		public class ShaderTarget
		{
			public string Target;
			public List<string> Shaders = new();
		}


		public const string STF_TYPE = "stf.material";
		public override string STF_Type => STF_TYPE;

		public List<string> StyleHints = new();
		public List<ShaderTarget> ShaderTargets = new();
		public List<STF_MaterialProperty> Properties = new();

		public override (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new NotImplementedException();
		}
	}

	public class STF_Material_Module : ISTF_Module
	{
		public string STF_Type => STF_Material.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"material"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Material)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return new List<ISTF_Resource>(((STF_Material)ApplicationObject).Components); }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Material>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Material");

			if(JsonResource.ContainsKey("style_hints"))
				ret.StyleHints = JsonResource["style_hints"].ToObject<List<string>>();
			if(JsonResource.ContainsKey("shader_targets")) foreach(JProperty jsonShaderTarget in JsonResource["shader_targets"])
			{
				ret.ShaderTargets.Add(new () {
					Target = jsonShaderTarget.Name,
					Shaders = jsonShaderTarget.Value.ToObject<List<string>>()
				});
			}

			foreach(JProperty jsonProperty in JsonResource["properties"])
			{
				var prop = new STF_MaterialProperty {
					PropertyType = jsonProperty.Name,
					ValueType = jsonProperty.Value.Value<string>("type")
				};
				switch(prop.ValueType)
				{
					case "float":
						foreach(var value in jsonProperty.Value["values"])
							if(value.Type == JTokenType.Float)
								prop.FloatValues.Add(new () {
									Value = (float)value
								});
						break;
					case "int":
						foreach(var value in jsonProperty.Value["values"])
							if(value.Type == JTokenType.Integer)
								prop.FloatValues.Add(new () {
									Value = (float)value
								});
						break;
					case "color":
						foreach(JArray value in jsonProperty.Value["values"])
							if(value.Type == JTokenType.Array)
								prop.ColorValues.Add(new () {
									Value = new Color((float)value[0], (float)value[1], (float)value[2], (float)value[3])
								});
						break;
					case "image":
						foreach(JObject value in jsonProperty.Value["values"])
							if(value.ContainsKey("image") && value["image"].Type == JTokenType.String)
								prop.ImageValues.Add(new () {
									Image = (STF_Image)Context.ImportResource((string)value["image"], "data")
								});
						break;
					default:
						foreach(var value in jsonProperty.Value["values"])
							prop.JsonFallbackValues.Add(new () {
								Value = value.ToString()
							});
						break;
				}
				ret.Properties.Add(prop);
			}

			return (ret, new() {ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var MaterialObject = ApplicationObject as STF_Material;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", MaterialObject.STF_Name},
			};

			return (ret, MaterialObject.STF_Id);
		}
	}
}
