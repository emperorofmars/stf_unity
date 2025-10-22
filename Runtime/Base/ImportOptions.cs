using System.Collections.Generic;

namespace com.squirrelbite.stf_unity
{
	[System.Serializable]
	public class ImportOptions
	{
		public bool AbortOnException = false;
		public bool AuthoringImport = false;
		public string SelectedApplication = "default";

		public bool IsFirstImport = true;

		[System.Serializable]
		public class MaterialMapping
		{
			public string ID;
			public string MaterialName;
			public string TargetShader;
		}
		public List<MaterialMapping> MaterialMappings = new();

		public bool ImportVertexColors = true;
		public int MaxWeights = 4;
	}
}
