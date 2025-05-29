
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AssetImporters;
using com.squirrelbite.stf_unity.processors;

namespace com.squirrelbite.stf_unity.tools
{
	[ScriptedImporter(1, new string[] {"stf"})]
	public class STFScriptedImporter : ScriptedImporter
	{
		public ImportOptions ImportConfig = new();

		public override void OnImportAsset(AssetImportContext ctx)
		{
			var file = new STF_File(ctx.assetPath);

			var state = new ImportState(file, STF_Module_Registry.Modules, ImportConfig);
			var rootContext = new ImportContext(state);

			rootContext.ImportResource(state.RootID, "data");
			state.FinalizeImport();

			var import = ScriptableObject.CreateInstance<STF_Import>();
			import.Init(state);
			ctx.AddObjectToAsset("main", import);

			var processorState = new ProcessorState(state, import.Root);
			var processorContext = new ProcessorContext(processorState);
			state.DeleteTrash();


			if (ImportConfig.AuthoringImport)
				foreach (var importedObject in state.ObjectToRegister)
					ctx.AddObjectToAsset(importedObject.name, importedObject);
			else
				foreach (var importedObject in state.ObjectToRegister)
					if (importedObject is not ISTF_Resource)
						ctx.AddObjectToAsset(importedObject.name, importedObject);

			foreach (var mapping in ImportConfig.MaterialMappings)
				if (state.GetImportedResource(mapping.ID) == null)
					ImportConfig.MaterialMappings.Remove(mapping);

			if (import.Root)
			{
				if (ImportConfig.AuthoringImport)
					import.Root.AddComponent<STF_Meta_Info>().Meta = state.Meta;

				ctx.SetMainObject(import.Root);

				if (!ImportConfig.AuthoringImport)
					foreach (var stfResource in import.Root.GetComponentsInChildren<ISTF_Resource>())
#if UNITY_EDITOR
						DestroyImmediate(stfResource as UnityEngine.Object);
#else
						Destroy(stfResource as UnityEngine.Object);
#endif

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
