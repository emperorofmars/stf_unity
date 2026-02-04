using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class ImportContext
	{
		protected readonly ImportState State;

		public ImportContext(ImportState State)
		{
			this.State = State;
		}

		public virtual JObject GetJsonResource(string STF_Id)
		{
			return State.GetJsonResource(STF_Id);
		}

		public ISTF_Resource ImportResource(string STF_Id, string ExpectedKind, ISTF_Resource ContextObject = null)
		{
			if(State.GetImportedResource(STF_Id) is ISTF_Resource @importedObject)
				return importedObject;

			var jsonResource = GetJsonResource(STF_Id);

			//Debug.Log($"Importing ID {STF_Id} - {jsonResource.GetValue("type")}");
			if(jsonResource == null)
				Report(new STFReport("Invalid Json Resource", ErrorSeverity.FATAL_ERROR, (string)jsonResource.GetValue("type"), null, null));

			var module = State.DetermineModule(jsonResource, ExpectedKind);
			if(module == null)
			{
				return HandleFallback(jsonResource, STF_Id, ExpectedKind, ContextObject);
			}

			(ISTF_Resource STFResource, List<object> ApplicationObjects) = module.Import(this, jsonResource, STF_Id, ContextObject);
			State.RegisterImportedResource(STF_Id, STFResource, ApplicationObjects);

			// handle components and what not
			if(STFResource is STF_DataResource dataResource && jsonResource.ContainsKey("components"))
			{
				foreach(var componentId in jsonResource["components"])
				{
					var component = ImportResource((string)componentId, "component", STFResource);
					if (component is STF_DataComponentResource resource)
					{
						dataResource.Components.Add(resource);
						resource.ParentObject = dataResource;
					}
					else if (component is STF_Fallback_ScriptableObject fallbackResource)
						dataResource.Components.Add(fallbackResource);
					else
						Report(new STFReport("Invalid Component", ErrorSeverity.ERROR, (string)jsonResource.GetValue("type"), null, null));
				}
			}
			else if(STFResource is STF_PrefabResource prefabResource && jsonResource.ContainsKey("components"))
			{
				foreach(var componentId in jsonResource["components"])
				{
					var component = ImportResource((string)componentId, "component", STFResource);
					if(component is STF_MonoBehaviour resource)
						prefabResource.Components.Add(resource);
					else
						Report(new STFReport("Invalid Component", ErrorSeverity.ERROR, (string)jsonResource.GetValue("type"), null, null));
				}
			}
			else if(STFResource is STF_NodeResource nodeResource && jsonResource.ContainsKey("components"))
			{
				foreach(var componentId in jsonResource["components"])
				{
					var component = ImportResource((string)componentId, "component", STFResource);
					if(component is STF_MonoBehaviour resource)
						nodeResource.Components.Add(resource);
					else
						Report(new STFReport("Invalid Component", ErrorSeverity.ERROR, (string)jsonResource.GetValue("type"), null, null));
				}
			}

			return STFResource;
		}

		public STF_Buffer ImportBuffer(string STF_Id)
		{
			return State.ImportBuffer(STF_Id);
		}

		public void ImportInstanceMod(ISTF_Resource Resource, JObject JsonResource)
		{
			if(State.DetermineModule(Resource.STF_Type, "component") is var module)
			{
				module.ImportInstanceMod(this, Resource, JsonResource);
			}
		}

		public virtual ISTF_Resource HandleFallback(JObject JsonResource, string STF_Id, string ExpectedKind, ISTF_Resource ContextObject = null)
		{
			if(ExpectedKind == "data")
			{
				var fallbackObject = STF_Data_Fallback_Module.Import(this, JsonResource, STF_Id, ContextObject);
				State.RegisterImportedResource(STF_Id, fallbackObject, new() {fallbackObject});

				if(JsonResource.ContainsKey("components"))
				{
					foreach(var componentId in JsonResource["components"])
					{
						var component = ImportResource((string)componentId, "component", fallbackObject);
						if(component is STF_ScriptableObject resource)
							((STF_DataResource)fallbackObject).Components.Add(resource);
						else
							Report(new STFReport("Invalid Component", ErrorSeverity.ERROR, (string)JsonResource.GetValue("type"), null, null));
					}
				}
				return fallbackObject;
			}
			else if(ExpectedKind == "node")
			{
				var fallbackObject = STF_Node_Fallback_Module.Import(this, JsonResource, STF_Id, ContextObject);
				State.RegisterImportedResource(STF_Id, fallbackObject, null);

				if(JsonResource.ContainsKey("components"))
				{
					foreach(var componentId in JsonResource["components"])
					{
						var component = ImportResource((string)componentId, "component", fallbackObject);
						if(component is STF_MonoBehaviour resource)
							((STF_NodeResource)fallbackObject).Components.Add(resource);
						else
							Report(new STFReport("Invalid Component", ErrorSeverity.ERROR, (string)JsonResource.GetValue("type"), null, null));
					}
				}
				return fallbackObject;
			}
			else if(ContextObject is STF_MonoBehaviour && (ExpectedKind == "component" || ExpectedKind == "instance"))
			{
				var fallbackObject = STF_Fallback_MonoBehaviour_Module.Import(this, JsonResource, STF_Id, ContextObject);
				State.RegisterImportedResource(STF_Id, fallbackObject, null);
				return fallbackObject;
			}
			else if(ExpectedKind == "component")
			{
				var fallbackObject = STF_Fallback_ScriptableObject_Module.Import(this, JsonResource, STF_Id, ContextObject);
				State.RegisterImportedResource(STF_Id, fallbackObject, new() {fallbackObject});
				return fallbackObject;
			}
			else
			{
				Report(new STFReport("Invalid Json Resource", ErrorSeverity.FATAL_ERROR, (string)JsonResource.GetValue("type"), null, null));
			}
			return null;
		}

		public void AddUnityObject(Object Resource) { State.AddUnityObject(Resource); }

		public ImportOptions ImportConfig => State.ImportConfig;

		public AssetInfo GetMeta()
		{
			return State.Meta.STFAssetInfo;
		}
		public string GetMetaCustomValue(string Key)
		{
			return State.Meta.STFAssetInfo.CustomProperties.FirstOrDefault(e => e.Name == Key)?.Value;
		}

		public void Report(STFReport Report)
		{
			State.Report(Report);
		}

		public void AddTask(Task Task) { State.Tasks.Add(Task); }
		public void AddDeleteNonAuthoring(Object AuthoringResource) { State.DeleteOnNonAuthoring.Add(AuthoringResource); }
		public void AddTrash(Transform Trash) { State.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { State.Trash.AddRange(Trash); }
	}
}
