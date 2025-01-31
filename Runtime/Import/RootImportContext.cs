
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class RootImportContext : IImportContext
	{
		protected ImportState _ImportState;
		public ImportState ImportState => _ImportState;
		public IJsonFallback_Module _FallbackModule = new JsonFallbackRoot_Module();
		public IJsonFallback_Module FallbackModule => _FallbackModule;

		public RootImportContext(ImportState ImportState, IJsonFallback_Module FallbackModule = null)
		{
			this._ImportState = ImportState;
			if(FallbackModule != null)
				this._FallbackModule = FallbackModule;
		}

		public virtual JObject GetJsonResource(string ID)
		{
			return ImportState.GetJsonResource(ID);
		}

		public object ImportResource(string ID, object ParentApplicationObject = null)
		{
			if(ImportState.GetImportedResource(ID) is object @importedObject)
				return importedObject;

			var jsonResource = GetJsonResource(ID);

			Debug.Log($"Importing ID {ID} - {jsonResource.GetValue("type")}");

			var module = ImportState.DetermineModule(jsonResource);
			if(jsonResource == null)
			{
				Report(new STFReport("Invalid Json Resource", ErrorSeverity.FATAL_ERROR, module?.STF_Type, null, null));
				return null;
			}
			if(module == null)
			{
				Report(new STFReport("Unrecognized Resource", ErrorSeverity.WARNING, (string)jsonResource.GetValue("type"), null, null));
				return HandleFallback(this, jsonResource, ID, ParentApplicationObject);
			}

			(var applicationObject, _) = module.Import(this, jsonResource, ID, ParentApplicationObject);
			ImportState.RegisterImportedResource(ID, applicationObject);

			// handle components and what not

			return applicationObject;
		}

		public virtual object HandleFallback(IImportContext Context, JObject JsonResource, string ID, object ParentApplicationObject = null)
		{
			(var fallbackObject, _) = FallbackModule.Import(Context, JsonResource, ID, ParentApplicationObject);
			ImportState.RegisterImportedResource(ID, fallbackObject);
			// handle components and what not
			return fallbackObject;
		}

		public void Report(STFReport Report)
		{
			ImportState.Report(Report);
		}
	}
}
