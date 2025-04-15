
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class ImportState
	{
		public readonly List<ISTF_Module> Modules = new();
		public STF_File File;
		public STF_Meta Meta;
		public JObject JsonResources;
		public JObject JsonBuffers;

		public string RootID => Meta.Root;

		public readonly Dictionary<string, ISTF_Resource> ImportedObjects = new();
		public readonly HashSet<Object> ObjectToRegister = new();

		public readonly Dictionary<string, STF_Buffer> ImportedBuffers = new();

		public ImportOptions ImportOptions = new();
		public readonly List<STFReport> Reports = new();

		public List<Task> Tasks = new();
		public readonly List<Transform> Trash = new();

		public ImportState(STF_File File, List<ISTF_Module> Modules, ImportOptions ImportOptions = null)
		{
			this.File = File;
			this.Modules = Modules;

			var json = JObject.Parse(File.Json);
			Meta = new STF_Meta(json["stf"] as JObject);
			JsonResources = json["resources"] as JObject;
			JsonBuffers = json["buffers"] as JObject;

			if(ImportOptions != null) this.ImportOptions = ImportOptions;
		}

		public JObject GetJsonResource(string ID)
		{
			if(JsonResources.GetValue(ID) is JObject jsonResource)
				return jsonResource;
			else
				return null;
		}

		public ISTF_Module DetermineModule(JObject JsonResource)
		{
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

		public void RegisterImportedResource(string ID, ISTF_Resource ImportedObject, List<object> ApplicationObjects)
		{
			ImportedObjects.Add(ID, ImportedObject);
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
			/*var ret = ScriptableObject.CreateInstance<STF_Buffer>();
			ret.Data = bytes;
			ret.STF_Id = STF_Id;
			ret.name = "Buffer: " + STF_Id;*/
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

			if(ImportOptions.AbortOnException && Report.Severity >= ErrorSeverity.ERROR)
				throw Report.Exception;
			Reports.Add(Report);
		}

		public void AddTrash(Transform Trash) { this.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { this.Trash.AddRange(Trash); }


		public void FinalizeImport()
		{
			// Run any Tasks added to the State during the processor execution
			var maxDepth = 100;
			while(Tasks.Count > 0)
			{
				var taskset = Tasks;
				Tasks = new List<Task>();
				foreach(var task in taskset)
				{
					task.RunSynchronously();
					if(task.Exception != null)
					{
						HandleTaskException(task.Exception);
					}
				}

				maxDepth--;
				if(maxDepth <= 0)
				{
					Debug.LogWarning("Maximum recursion depth reached!");
					break;
				}
			}

			foreach(var t in Trash)
			{
				if(t)
				{
					#if UNITY_EDITOR
					Object.DestroyImmediate(t.gameObject);
					#else
					Object.Destroy(t);
					#endif
				}
			}
		}

		private void HandleTaskException(System.AggregateException Exception)
		{
			foreach(var e in Exception.InnerExceptions)
			{
				if(e is STFException stfError)
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
