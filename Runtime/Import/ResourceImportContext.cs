
namespace com.squirrelbite.stf_unity
{
	public class ResourceImportContext : IImportContext
	{
		public ResourceImportContext(IImportContext ParentContext, object Resource)
		{

		}

		public object ImportResource(string ID, object ParentApplicationObject = null)
		{
			throw new System.NotImplementedException();
		}
	}
}
