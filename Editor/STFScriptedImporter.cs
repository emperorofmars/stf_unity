
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

			var definition = ScriptableObject.CreateInstance<STF_Definition>();
			definition.Init(file);
			ctx.AddObjectToAsset("main", definition);

			Debug.Log("WOOOO");

			/*var unityContext = new RuntimeUnityImportContext();
			var (asset, _state) = Importer.Parse(unityContext, ctx.assetPath);
			ctx.AddObjectToAsset("main", asset.gameObject);
			ctx.SetMainObject(asset.gameObject);
			foreach(var resource in unityContext.AssetCtxObjects)
			{
				if(resource != null) ctx.AddObjectToAsset("resource" + resource.GetInstanceID(), resource);
			}*/
		}
	}
}

#endif
