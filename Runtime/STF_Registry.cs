
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Registry
	{
		public static readonly List<STF_Module> Modules = new() {
			new STF_Prefab_Module(),
			new STF_Node_Module(),
		};

	}
}
