#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public class STF_Mesh_Import_Editor : ISTF_Module_Editor
	{
		public string STF_Type => STF_Mesh.STF_TYPE;

		public void Draw(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option)
		{
			EditorGUILayout.LabelField(Option.DisplayName);
			EditorGUI.indentLevel++;

			var options = JObject.Parse(Option.Json);
			if(options.ContainsKey("vertex_colors") && options.Value<bool>("vertex_colors") is bool vertexColors)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Import Vertex Colors");
				var newImportVertexColors = EditorGUILayout.Toggle(vertexColors);
				EditorGUILayout.EndHorizontal();

				if(newImportVertexColors != vertexColors)
				{
					options["vertex_colors"] = newImportVertexColors;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				}
			}

			options = JObject.Parse(Option.Json);
			if(options.ContainsKey("max_weights") && options.Value<int>("max_weights") is int maxWeights)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Max. Weights");
				var newMaxWeights = EditorGUILayout.IntSlider(maxWeights, 1, 32);
				EditorGUILayout.EndHorizontal();

				if(newMaxWeights != maxWeights)
				{
					options["max_weights"] = newMaxWeights;
					Option.Json = options.ToString();
					EditorUtility.SetDirty(Importer);
				}
			}
			EditorGUI.indentLevel--;
		}
	}
}

#endif
