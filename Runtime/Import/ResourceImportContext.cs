
using Newtonsoft.Json.Linq;
using UnityEngine;

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
	}
}
