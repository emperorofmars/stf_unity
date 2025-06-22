#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using com.squirrelbite.stf_unity.modules;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.tools
{
	public class STFExportEditor : EditorWindow
	{
		private Vector2 scrollPos;
		public STF_Prefab exportAsset;
		private bool DebugExport = true;


		// TODO unsomment when STF export works
		//[MenuItem("STF Tools/Export (TBD)")]
		public static void Init()
		{
			STFExportEditor window = EditorWindow.GetWindow(typeof(STFExportEditor)) as STFExportEditor;
			window.titleContent = new GUIContent("Export STF (.stf)");
			window.minSize = new Vector2(600, 700);
		}

		void OnGUI()
		{
			GUILayout.Label("Export STF (Doesn't work yet)", EditorStyles.whiteLargeLabel);
			drawHLine();
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);


			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Asset", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			exportAsset = (STF_Prefab)EditorGUILayout.ObjectField(
				exportAsset,
				typeof(STF_Prefab),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();
			drawHLine();

			DebugExport = GUILayout.Toggle(DebugExport, "Export Debug Json File");

			GUILayout.Space(10);
			drawHLine();

			if(exportAsset)
			{
				var exportMeta = exportAsset.GetComponent<STF_Meta_Info>();

				if(GUILayout.Button("Export", GUILayout.ExpandWidth(true))) {
					var path = EditorUtility.SaveFilePanel("STF Export", "Assets", exportAsset.name + ".stf", "stf");
					if(path != null && path.Length > 0) {
						// TODO actual Export

						var State = new ExportState();
						var Context = new ExportContext();

						var jsonDefinition = new JObject {
							{
								"stf", new JObject {
									{"version_major", 0},
									{"version_minor", 0},
									{"generator", "stf_unity"},
									{"timestamp", DateTime.Now.ToString()},
									{"metric_multiplier", exportMeta.Meta.MetricMultiplier},
									{"root", exportAsset.STF_Id},
								}
							},
							{
								"resources", ""//exportAsset.JsonResources
							},
							{
								"buffers", ""//exportAsset.JsonBuffers
							}
						};
						var file = new STF_File(jsonDefinition.ToString(Newtonsoft.Json.Formatting.None), new List<byte[]> {});

						File.WriteAllBytes(path, file.CreateSTFBinary().ToArray());
						if(DebugExport) File.WriteAllText(path + ".json", jsonDefinition.ToString(Newtonsoft.Json.Formatting.Indented));
					}
				}
			}

			GUILayout.EndScrollView();
		}

		private void drawHLine() {
			GUILayout.Space(10);
			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2), Color.gray);
			GUILayout.Space(10);
		}
	}
}
#endif
