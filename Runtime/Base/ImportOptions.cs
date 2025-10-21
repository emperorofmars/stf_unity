using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	[System.Serializable]
	public class ImportOptions
	{
		public bool AbortOnException = false;
		public bool AuthoringImport = false;
		public string SelectedApplication = "default";

		public bool IsFirstImport = true;

		[System.Serializable]
		public class MaterialMapping
		{
			public string ID;
			public string MaterialName;
			public string TargetShader;
		}
		public List<MaterialMapping> MaterialMappings = new();

		public bool ImportVertexColors = true;
		public int MaxWeights = 4;

		public void Parse(string ImportConfig)
		{
			try
			{
				var jsonConfig = JObject.Parse(ImportConfig);
				{
					if (jsonConfig.ContainsKey("material_mappings"))
					{
						foreach (JProperty mapping in jsonConfig["material_mappings"])
						{
							MaterialMappings.Add(new()
							{
								ID = mapping.Name,
								MaterialName = mapping.Value.Value<string>("material_name"),
								TargetShader = mapping.Value.Value<string>("target_shader"),
							});
						}
					}
				}
			}
			catch (System.Exception) { }
		}

		public string Serialize()
		{
			var ret = new JObject();
			var material_mappings = new JObject();
			foreach (var mapping in MaterialMappings)
			{
				material_mappings.Add(mapping.ID, new JObject()
				{
					{"material_name", mapping.MaterialName},
					{"target_shader", mapping.TargetShader},
				});
			}
			ret.Add("material_mappings", material_mappings);
			return ret.ToString();
		}
	}
}
