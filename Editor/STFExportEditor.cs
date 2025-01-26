#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace com.squirrelbite.stf_unity.tools
{
	// UI to export STF-Unity intermediary scenes into STF. Apart from selecting the main asset, optionally multiple secondary assets can be included into the export.

	public class STFExportEditor : EditorWindow
	{
		private Vector2 scrollPos;
		public GameObject exportAsset;
		private bool DebugExport = true;


		[MenuItem("STF Tools/Export")]
		public static void Init()
		{
			STFExportEditor window = EditorWindow.GetWindow(typeof(STFExportEditor)) as STFExportEditor;
			window.titleContent = new GUIContent("Export STF (.stf)");
			window.minSize = new Vector2(600, 700);
		}

		void OnGUI()
		{
			GUILayout.Label("Export STF ", EditorStyles.whiteLargeLabel);
			drawHLine();
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);


			GUILayout.BeginHorizontal();
			GUILayout.Label("Select Asset", EditorStyles.whiteLargeLabel, GUILayout.ExpandWidth(false));
			exportAsset = (GameObject)EditorGUILayout.ObjectField(
				exportAsset,
				typeof(GameObject),
				true,
				GUILayout.ExpandWidth(true)
			);
			GUILayout.EndHorizontal();
			drawHLine();

			DebugExport = GUILayout.Toggle(DebugExport, "Export Debug Json File");

			GUILayout.Space(10);
			drawHLine();

			string defaultExportFilaName = "new";
			if(exportAsset)
			{
				//var exportSTFAsset = exportAsset.GetComponent<ISTFAsset>();
				//defaultExportFilaName = exportSTFAsset != null && !string.IsNullOrWhiteSpace(exportSTFAsset.OriginalFileName) ? exportSTFAsset.OriginalFileName : exportAsset.name;

				if(GUILayout.Button("Export", GUILayout.ExpandWidth(true))) {
					var path = EditorUtility.SaveFilePanel("STF Export", "Assets", defaultExportFilaName + ".stf", "stf");
					if(path != null && path.Length > 0) {
						// TODO actual Export

						var jsonDefinition = new JObject {
							{
								"stf", new JObject {
									/*{"version_major", exportAsset.Meta.DefinitionVersionMajor},
									{"version_minor", exportAsset.Meta.DefinitionVersionMinor},
									{"generator", exportAsset.Meta.Generator},
									{"timestamp", exportAsset.Meta.Timestamp},
									{"metric_multiplier", exportAsset.Meta.MetricMultiplier},
									{"root", exportAsset.Meta.Root},*/
									{"", ""}
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
