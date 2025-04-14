
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AssetImporters;

namespace com.squirrelbite.stf_unity.tools
{
	[ScriptedImporter(1, new string[] {"stf"})]
	public class STFScriptedImporter : ScriptedImporter
	{
		public override void OnImportAsset(AssetImportContext ctx)
		{
			var file = new STF_File(ctx.assetPath);
			var state = new ImportState(file, STF_Registry.Modules);
			var rootContext = new ImportContext(state);

			rootContext.ImportResource(state.RootID);
			state.FinalizeImport();

			foreach(var importedObject in state.ObjectToRegister)
			{
				ctx.AddObjectToAsset(@importedObject.name, importedObject);
			}

			var import = ScriptableObject.CreateInstance<STF_Import>();
			import.Init(state);
			ctx.AddObjectToAsset("main", import);
			if(import.Root)
			{
				ctx.SetMainObject(import.Root);
				Debug.Log("STF Import Success!");
			}
			else
			{
				Debug.Log("STF Import Failed! Check the reports.");
			}
		}
	}
}

#endif
