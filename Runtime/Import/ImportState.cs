
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class ImportState
	{
		public readonly List<STF_Module> Modules = new();
		public STF_File File;
		public STF_Meta Meta;
		public JObject JsonResources;
		public JObject JsonBuffers;

		public readonly Dictionary<string, Object> ImportedObjects = new();

		public ImportState(STF_File File, List<STF_Module> Modules)
		{
			this.File = File;
			this.Modules = Modules;
		}

		public STF_Module DetermineModule(string ID)
		{
			return null;
		}
	}
}
