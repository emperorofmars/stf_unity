
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class ImportState
	{
		public readonly List<STF_Module> Modules = new();
		public STF_File File;
		public STF_Meta Meta;
		public JObject JsonResources;
		public JObject JsonBuffers;

		public string RootID => Meta.Root;

		public readonly Dictionary<string, object> ImportedObjects = new();

		public ImportState(STF_File File, List<STF_Module> Modules)
		{
			this.File = File;
			this.Modules = Modules;

			var json = JObject.Parse(File.Json);
			Meta = new STF_Meta(json["stf"] as JObject);
			JsonResources = json["resources"] as JObject;
			JsonBuffers = json["buffers"] as JObject;
		}

		public JObject GetJsonResource(string ID)
		{
			if(JsonResources.GetValue(ID) is JObject jsonResource)
				return jsonResource;
			else
				return null;
		}

		public STF_Module DetermineModule(JObject JsonResource)
		{
			Debug.Log(JsonResource);
			var type = (string)JsonResource.GetValue("type");
			foreach(var module in Modules)
			{
				if(module.STF_Type == type)
				{
					return module;
				}
			}
			return null;
		}

		public object GetImportedResource(string ID)
		{
			if(ImportedObjects.ContainsKey(ID))
				return ImportedObjects[ID];
			else
				return null;
		}

		public void RegisterImportedResource(string ID, object ImportedObject)
		{
			ImportedObjects.Add(ID, ImportedObject);
		}
	}
}
