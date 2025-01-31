
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public class ResourceImportContext : RootImportContext
	{
		protected IImportContext ParentContext;
		protected object Resource;

		public ResourceImportContext(IImportContext ParentContext, object Resource) : base(ParentContext.ImportState)
		{
			this.ParentContext = ParentContext;
			this.Resource = Resource;
		}

		public override JObject GetJsonResource(string ID)
		{
			return ParentContext.GetJsonResource(ID);
		}

		public override object HandleFallback(IImportContext Context, JObject JsonResource, string ID, object ParentApplicationObject = null)
		{
			return ParentContext.HandleFallback(Context, JsonResource, ID, ParentApplicationObject);
		}
	}
}
