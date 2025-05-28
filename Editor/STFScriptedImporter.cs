
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AssetImporters;

namespace com.squirrelbite.stf_unity.tools
{
	[ScriptedImporter(1, new string[] {"stf"})]
	public class STFScriptedImporter : ScriptedImporter
	{
		//public bool AuthoringImport = false;
		public string SelectedApplication = "default";

		public ImportOptions ImportConfig = new();

		public override void OnImportAsset(AssetImportContext ctx)
		{
			var file = new STF_File(ctx.assetPath);
			/*var importOptions = new ImportOptions
			{
				AuthoringImport = ImportConfig.AuthoringImport
			};
			importOptions.Parse(userData);*/

			var state = new ImportState(file, STF_Module_Registry.Modules, ImportConfig);
			var rootContext = new ImportContext(state);

			rootContext.ImportResource(state.RootID, "data");
			state.FinalizeImport();

			//userData = importOptions.Serialize();

			if (ImportConfig.AuthoringImport)
				foreach (var importedObject in state.ObjectToRegister)
					ctx.AddObjectToAsset(importedObject.name, importedObject);
			else
				foreach (var importedObject in state.ObjectToRegister)
					if (importedObject is not ISTF_Resource)
						ctx.AddObjectToAsset(importedObject.name, importedObject);

			var import = ScriptableObject.CreateInstance<STF_Import>();
			import.Init(state);
			ctx.AddObjectToAsset("main", import);
			if (import.Root)
			{
				if (ImportConfig.AuthoringImport)
					import.Root.AddComponent<STF_Meta_Info>().Meta = state.Meta;

				ctx.SetMainObject(import.Root);

				if (!ImportConfig.AuthoringImport)
					foreach (var stfResource in import.Root.GetComponentsInChildren<ISTF_Resource>())
						DestroyImmediate(stfResource as UnityEngine.Object);

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
