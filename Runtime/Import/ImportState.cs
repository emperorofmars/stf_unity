
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity
{
	public class ImportState
	{
		public readonly Dictionary<string, ISTF_Module> Modules = new();
		public readonly HashSet<string> Ignores = new();
		public STF_File File;
		public STF_Meta Meta;
		public JObject JsonResources;
		public JObject JsonBuffers;

		public string RootID => Meta.Root;

		public readonly Dictionary<string, ISTF_Resource> ImportedObjects = new();
		public readonly HashSet<Object> ObjectToRegister = new();

		public readonly Dictionary<string, STF_Buffer> ImportedBuffers = new();

		public ImportOptions ImportConfig = new();
		public readonly List<STFReport> Reports = new();

		public List<Task> Tasks = new();
		public readonly List<Object> Trash = new();
		public readonly List<Object> DeleteOnNonAuthoring = new();

		public ImportState(STF_File File, Dictionary<string, ISTF_Module> Modules, HashSet<string> Ignores, ImportOptions ImportConfig = null)
		{
			this.File = File;
			this.Modules = Modules;
			this.Ignores = Ignores;

			var json = JObject.Parse(File.Json);
			Meta = new STF_Meta(json["stf"] as JObject);
			JsonResources = json["resources"] as JObject;
			JsonBuffers = json["buffers"] as JObject;

			if (ImportConfig != null) this.ImportConfig = ImportConfig;

			if (ImportConfig.IsFirstImport && Meta.STFAssetInfo.CustomProperties.Find(p => p.Name == "import.context.default") is var defaultContext && defaultContext != null && !string.IsNullOrWhiteSpace(defaultContext.Value) && STF_Processor_Registry.GetAvaliableContexts().Contains(defaultContext.Value))
			{
				ImportConfig.SelectedApplication = defaultContext.Value;
			}
		}

		public JObject GetJsonResource(string ID)
		{
			if(JsonResources.GetValue(ID) is JObject jsonResource)
				return jsonResource;
			else
				return null;
		}

		public ISTF_Module DetermineModule(JObject JsonResource, string ExpectedKind)
		{
			var type = (string)JsonResource.GetValue("type");

			foreach (var ignore in Ignores)
				if (type.StartsWith(ignore)) return null;

			if (Modules.ContainsKey(type))
			{
				return Modules[type];
			}
			else
			{
				Report(new STFReport("Unrecognized Resource", ErrorSeverity.WARNING, (string)JsonResource.GetValue("type"), null, null));
				return null;
			}
		}

		public object GetImportedResource(string STF_Id)
		{
			if(ImportedObjects.ContainsKey(STF_Id))
				return ImportedObjects[STF_Id];
			else
				return null;
		}

		public void RegisterImportedResource(string STF_Id, ISTF_Resource ImportedObject, List<object> ApplicationObjects)
		{
			ImportedObjects.Add(STF_Id, ImportedObject);
			if(ApplicationObjects != null && ApplicationObjects.Count > 0)
			{
				foreach(var ApplicationObject in ApplicationObjects)
				{
					if(ApplicationObject is Object @object)
						ObjectToRegister.Add(@object);
				}
			}
		}

		public STF_Buffer ImportBuffer(string STF_Id)
		{
			if(ImportedBuffers.ContainsKey(STF_Id))
				Report(new STFReport($"Buffer with ID \"{STF_Id}\" imported twice", ErrorSeverity.FATAL_ERROR));

			if(!JsonBuffers.ContainsKey(STF_Id))
				Report(new STFReport($"Buffer with ID \"{STF_Id}\" doesn't exist", ErrorSeverity.FATAL_ERROR));

			var jsonBuffer = (JObject)JsonBuffers[STF_Id];
			if(!jsonBuffer.ContainsKey("type") || (string)jsonBuffer["type"] != "stf.buffer.included")
				Report(new STFReport($"Buffer with ID \"{STF_Id}\" is of not supported type \"{(string)jsonBuffer["type"]}\"" + STF_Id, ErrorSeverity.FATAL_ERROR));

			var bytes = File.Buffers[(int)jsonBuffer["index"]];
			var ret = new STF_Buffer{Data = bytes, STF_Id = STF_Id};
			ImportedBuffers.Add(STF_Id, ret);

			return ret;
		}

		public void Report(STFReport Report) {
			if(Report.Severity == ErrorSeverity.FATAL_ERROR)
				throw new STFException(Report);
			else if(Report.Severity == ErrorSeverity.ERROR)
				Debug.LogError(Report.ToString());
			else if(Report.Severity == ErrorSeverity.WARNING)
				Debug.LogWarning(Report.ToString());
			else
				Debug.Log(Report.ToString());

			if(ImportConfig.AbortOnException && Report.Severity >= ErrorSeverity.ERROR)
				throw Report.Exception;
			Reports.Add(Report);
		}

		public void AddUnityObject(Object Resource) { this.ObjectToRegister.Add(Resource); }
		public void AddDeleteNonAuthoring(Object AuthoringResource) { this.DeleteOnNonAuthoring.Add(AuthoringResource); }
		public void AddTrash(Object Trash) { this.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Object> Trash) { this.Trash.AddRange(Trash); }


		public void FinalizeImport()
		{
			// Run any Tasks added to the State during the processor execution
			var maxDepth = 100;
			while (Tasks.Count > 0)
			{
				var taskset = Tasks;
				Tasks = new List<Task>();
				foreach (var task in taskset)
				{
					task.RunSynchronously();
					if (task.Exception != null)
					{
						HandleTaskException(task.Exception);
					}
				}

				maxDepth--;
				if (maxDepth <= 0)
				{
					Debug.LogWarning("Maximum recursion depth reached!");
					break;
				}
			}

			DeleteTrash();
		}

		public void DeleteTrash()
		{
			foreach (var t in Trash)
			{
				if (t)
				{
#if UNITY_EDITOR
					Object.DestroyImmediate(t);
#else
					Object.Destroy(t);
#endif
				}
			}
		}

		public void Cleanup()
		{
			DeleteTrash();

			if (!ImportConfig.AuthoringImport)
			{
				foreach (var t in DeleteOnNonAuthoring)
				{
					if (t)
					{
#if UNITY_EDITOR
						Object.DestroyImmediate(t);
#else
						Object.Destroy(t);
#endif
					}
				}
			}
		}

		private void HandleTaskException(System.AggregateException Exception)
		{
			foreach (var e in Exception.InnerExceptions)
			{
				if (e is STFException stfError)
				{
					Report(stfError.Report);
				}
				else
				{
					Report(new STFReport(e.Message, ErrorSeverity.FATAL_ERROR, null, null, e));
				}
			}
		}
	}
}
