
using System;

namespace com.squirrelbite.stf_unity
{
	public class RootImportContext : IImportContext
	{
		protected ImportState ImportState;
		public RootImportContext(ImportState ImportState)
		{
			this.ImportState = ImportState;
		}

		public Object ImportResource(string ID)
		{
			return null;
		}
	}
}
