
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class RootImportContext : IImportContext
	{
		protected ImportState _ImportState;
		public ImportState ImportState => _ImportState;

		public RootImportContext(ImportState ImportState)
		{
			this._ImportState = ImportState;
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
			var module = ImportState.DetermineModule(jsonResource);
			if(module == null || jsonResource == null)
				// TODO report error
				return null;

			(var applicationObject, _) = module.Import(this, jsonResource, ID, ParentApplicationObject);
			ImportState.RegisterImportedResource(ID, applicationObject);

			// TODO components and hooks ?

			return applicationObject;
		}
	}
}
