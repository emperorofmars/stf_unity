
#if UNITY_EDITOR

using System.Buffers;
using System.IO;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[UnityEditor.AssetImporters.ScriptedImporter(1, new string[] {"stf"})]
	public class STFScriptedImporter : UnityEditor.AssetImporters.ScriptedImporter
	{
		public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
		{
			var file = new STF_File(ctx.assetPath);

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
