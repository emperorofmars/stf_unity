#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AssetImporters;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity.tools
{
	[HelpURL("https://docs.stfform.at")]
	[ScriptedImporter(1, new string[] { "stf" })]
	public class STFScriptedImporter : ScriptedImporter
	{
		[SerializeField]
		public ImportOptions ImportConfig = new();

		public override void OnImportAsset(AssetImportContext ctx)
		{
			Debug.Log($"Importing {ctx.assetPath} for context {ImportConfig.SelectedApplication}");

			ImportConfig.ResourceImportOptionsConfirm = new();

			var file = new STF_File(ctx.assetPath);
			var state = new ImportState(file, STF_Module_Registry.Modules, STF_Module_Registry.Ignores, ImportConfig);
			var rootContext = new ImportContext(state);

			rootContext.ImportResource(state.RootID, "data");
			state.FinalizeImport();

			var import = ScriptableObject.CreateInstance<STF_Import>();
			import.Init(state);
			ctx.AddObjectToAsset("main", import);

			var processorState = new ProcessorState(state, import.Root);
			var processorContext = STF_Processor_Registry.CreateApplicationContext(ImportConfig.SelectedApplication, processorState);
			processorContext.Run();
			state.Cleanup();

			foreach (var importedObject in state.ObjectToRegister)
				if (importedObject != null && (importedObject is not ISTF_Resource || ImportConfig.AuthoringImport))
					ctx.AddObjectToAsset(DetermineImportAssetName(importedObject), importedObject);

			if (import.Root)
			{
				if (ImportConfig.AuthoringImport)
					import.Root.AddComponent<STF_Meta_Info>().Meta = state.Meta;

				ctx.SetMainObject(import.Root);

				if (!ImportConfig.AuthoringImport)
					foreach (var stfResource in import.Root.GetComponentsInChildren<ISTF_Resource>())
						DestroyImmediate(stfResource as UnityEngine.Object);

				ImportConfig.ResourceImportOptions = ImportConfig.ResourceImportOptionsConfirm;
				ImportConfig.ResourceImportOptionsConfirm = null;
				ImportConfig.IsFirstImport = false;

				Debug.Log("STF Import Success!");
			}
			else
			{
				Debug.Log("STF Import Failed! Check the reports.");
			}
		}

		private string DetermineImportAssetName(Object Resource)
		{
			if (Resource is Material) return "material" + Resource.name;
			if (Resource is Mesh) return "mesh" + Resource.name;
			if (Resource is Texture2D) return "texture" + Resource.name;
			if (Resource is Avatar) return "avatar" + Resource.name;
			if (Resource is AnimationClip) return "anim" + Resource.name;
			if (Resource is ISTF_Resource stfResource) return stfResource.STF_Id;
			return Resource.name;
		}
	}
}

#endif
