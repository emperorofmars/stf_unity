using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class ImportContext
	{
		protected ImportState _ImportState;
		public ImportState ImportState => _ImportState;

		public ImportContext(ImportState ImportState)
		{
			this._ImportState = ImportState;
		}

		public virtual JObject GetJsonResource(string STF_Id)
		{
			return ImportState.GetJsonResource(STF_Id);
		}

		public ISTF_Resource ImportResource(string STF_Id, ISTF_Resource ContextObject = null)
		{
			if(ImportState.GetImportedResource(STF_Id) is ISTF_Resource @importedObject)
				return importedObject;

			var jsonResource = GetJsonResource(STF_Id);

			//Debug.Log($"Importing ID {STF_Id} - {jsonResource.GetValue("type")}");

			var module = ImportState.DetermineModule(jsonResource);
			if(jsonResource == null)
			{
				Report(new STFReport("Invalid Json Resource", ErrorSeverity.FATAL_ERROR, module?.STF_Type, null, null));
				return null;
			}
			if(module == null)
			{
				Report(new STFReport("Unrecognized Resource", ErrorSeverity.WARNING, (string)jsonResource.GetValue("type"), null, null));
				return HandleFallback(jsonResource, STF_Id, ContextObject);
			}

			(ISTF_Resource STFResource, object ApplicationObject) = module.Import(this, jsonResource, STF_Id, ContextObject);
			ImportState.RegisterImportedResource(STF_Id, STFResource, ApplicationObject);

			// handle components and what not

			return STFResource;
		}

		public virtual ISTF_Resource HandleFallback(JObject JsonResource, string STF_Id, ISTF_Resource ContextObject = null)
		{
			/*var fallbackObject = FallbackModule.Import(this, JsonResource, STF_Id, ContextObject);
			ImportState.RegisterImportedResource(STF_Id, fallbackObject);
			// handle components and what not
			return fallbackObject;*/
			return null;
		}

		public void Report(STFReport Report)
		{
			ImportState.Report(Report);
		}


		public void AddTask(Task Task) { ImportState.Tasks.Add(Task); }
		public void AddTrash(Transform Trash) { ImportState.Trash.Add(Trash); }
		public void AddTrash(IEnumerable<Transform> Trash) { ImportState.Trash.AddRange(Trash); }
	}
}
