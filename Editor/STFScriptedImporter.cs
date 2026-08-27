#if UNITY_EDITOR

using UnityEngine;
using UnityEditor.AssetImporters;
using com.squirrelbite.stf_unity.processors;
using com.squirrelbite.stf_unity.resources;

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
			var timeStart = Time.realtimeSinceStartupAsDouble;

			ImportConfig.ResourceImportOptionsConfirm = new();

			var file = new STF_File(ctx.assetPath);

			var importState = new ImportState(file, STF_Handler_Registry.Handlers, STF_Handler_Registry.Ignores, ImportConfig);
			var importContext = new ImportContext(importState);
			importContext.ImportResource(importState.RootID, "data");
			importState.FinalizeImport();

			var import = ScriptableObject.CreateInstance<STF_Import>();
			import.Init(importState);
			ctx.AddObjectToAsset("main", import);

			var processorState = new ProcessorState(importState, import.Root);
			var processorContext = STF_Processor_Registry.CreateApplicationContext(ImportConfig.SelectedApplication, processorState);
			processorContext.Run();
			importState.Cleanup();

			foreach (var importedObject in importState.ObjectToRegister)
				if (importedObject != null && (importedObject is not ISTF_Resource || ImportConfig.AuthoringImport))
					ctx.AddObjectToAsset(DetermineImportAssetName(importedObject), importedObject);

			if (import.Root)
			{
				if (ImportConfig.AuthoringImport)
					import.Root.AddComponent<STF_Meta_Info>().Meta = importState.Meta;

				ctx.SetMainObject(import.Root);

				if (!ImportConfig.AuthoringImport)
					foreach (var stfResource in import.Root.GetComponentsInChildren<ISTF_Resource>())
						DestroyImmediate(stfResource as UnityEngine.Object);

				ImportConfig.ResourceImportOptions = ImportConfig.ResourceImportOptionsConfirm;
				ImportConfig.ResourceImportOptionsConfirm = null;
				ImportConfig.IsFirstImport = false;

				var timeEnd = Time.realtimeSinceStartupAsDouble;

				Debug.Log($"Successfully imported STF asset \"{ ctx.assetPath }\" in { System.Math.Round(timeEnd - timeStart, 4) } s.");
			}
			else
			{
				Debug.Log($"Importing STF asset \"{ ctx.assetPath }\" failed! Check the reports.");
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
