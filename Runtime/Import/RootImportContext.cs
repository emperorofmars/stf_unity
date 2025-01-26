
namespace com.squirrelbite.stf_unity
{
	public class RootImportContext : IImportContext
	{
		protected ImportState ImportState;
		public RootImportContext(ImportState ImportState)
		{
			this.ImportState = ImportState;
		}

		public object ImportResource(string ID, object ParentApplicationObject = null)
		{
			if(ImportState.GetImportedResource(ID) is object @importedObject)
				return importedObject;

			(var module, var resource) = ImportState.DetermineModule(ID);
			if(module == null || resource == null)
				// TODO report error
				return null;

			(var applicationObject, _) = module.Import(this, resource, ID, ParentApplicationObject);
			ImportState.RegisterImportedResource(ID, applicationObject);

			return applicationObject;
		}
	}
}
