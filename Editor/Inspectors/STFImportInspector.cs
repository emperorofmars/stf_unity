#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.squirrelbite.stf_unity.tools
{

	[CustomEditor(typeof(STF_Import))]
	public class STFImportInspector : Editor
	{
		public override VisualElement CreateInspectorGUI()
		{
			var asset = target as STF_Import;
			var ret = new VisualElement();

			// Doesn't work, draw using the jank method below instead
			// https://discussions.unity.com/t/property-fields-not-showing-up-in-property-panel/1668143
			//ret.Add(new PropertyField(serializedObject.FindProperty("BinaryVersionMajor")));
			//ret.Add(new PropertyField(serializedObject.FindProperty("BinaryVersionMinor")));

			VisualElement createInfoField(string label, string text, bool selectable = false, bool copyButton = false, float margin = 2)
			{
				var field = new VisualElement();
				field.style.flexDirection = FlexDirection.Row;
				field.style.alignItems = Align.Center;
				field.style.marginTop = field.style.marginBottom = margin;

				var labelField = new Label(label);
				labelField.style.flexBasis = 100;
				labelField.style.flexGrow = 100;
				labelField.style.minWidth = 120;
				field.Add(labelField);

				var valueField = new VisualElement();
				valueField.style.flexDirection = FlexDirection.Row;
				valueField.style.alignItems = Align.Center;
				valueField.style.flexBasis = 180;
				valueField.style.flexGrow = 180;
				field.Add(valueField);

				var valueLabel = new Label(text);
				valueLabel.selection.isSelectable = selectable;
				valueField.Add(valueLabel);

				if(copyButton)
				{
					var button = new Button { tooltip = "Copy", text = "copy" };
					button.RegisterCallback<ClickEvent>(c => { GUIUtility.systemCopyBuffer = text; });
					button.style.scale = new StyleScale(new Scale(new Vector2(0.7f, 0.7f)));
					button.style.marginTop = button.style.marginBottom = 0;
					valueField.Add(button);
				}
				return field;
			}

			ret.Add(createInfoField("Asset Name", asset.Meta?.STFAssetInfo?.AssetName));
			ret.Add(createInfoField("Asset Version", asset.Meta?.STFAssetInfo?.Version));
			ret.Add(createInfoField("Author", asset.Meta?.STFAssetInfo?.Author));
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.URL) && asset.Meta.STFAssetInfo.URL.StartsWith("https://"))
				ret.Add(createInfoField("URL", $"<a href=\"{asset.Meta?.STFAssetInfo?.URL}\">{asset.Meta?.STFAssetInfo?.URL}</a>"));
			else
				ret.Add(createInfoField("URL", asset.Meta?.STFAssetInfo?.URL));
			ret.Add(createInfoField("License", asset.Meta?.STFAssetInfo?.License));
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.LicenseURL) && asset.Meta.STFAssetInfo.LicenseURL.StartsWith("https://"))
				ret.Add(createInfoField("License URL", $"<a href=\"{asset.Meta?.STFAssetInfo?.LicenseURL}\">{asset.Meta?.STFAssetInfo?.LicenseURL}</a>"));
			else
				ret.Add(createInfoField("License URL", asset.Meta?.STFAssetInfo?.LicenseURL));
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.DocumentationURL) && asset.Meta.STFAssetInfo.DocumentationURL.StartsWith("https://"))
				ret.Add(createInfoField("License Documentation", $"<a href=\"{asset.Meta?.STFAssetInfo?.DocumentationURL}\">{asset.Meta?.STFAssetInfo?.DocumentationURL}</a>"));
			else
				ret.Add(createInfoField("License Documentation", asset.Meta?.STFAssetInfo?.DocumentationURL));
			ret.Add(createInfoField("Binary Version", $"{asset.BinaryVersionMajor}.{asset.BinaryVersionMinor}"));
			ret.Add(createInfoField("Definition Version", $"{asset.Meta?.DefinitionVersionMajor}.{asset.Meta?.DefinitionVersionMinor}"));

			if(asset.Meta.AssetProperties != null && asset.Meta.AssetProperties.Count > 0)
			{
				var foldout = new Foldout { text = $"{asset.Meta.AssetProperties.Count} Custom Propert{(asset.Meta.AssetProperties.Count > 1 ? "ies" : "y")}", value = true, viewDataKey = $"stf_import_custom_properties_{asset.name}" };
				foldout.style.marginTop = 10;
				ret.Add(foldout);
				foreach(var property in asset.Meta.AssetProperties)
				{
					foldout.Add(createInfoField(property.Name, property.Value, true));
				}
			}

			if(asset.Reports != null && asset.Reports.Count > 0)
			{
				var severeReports = asset.Reports.FindAll(r => r.Severity >= ErrorSeverity.ERROR);
				asset.Reports.Sort((a, b) => a.Severity - b.Severity);

				var foldout = new Foldout { text = $"<size=+3><font-weight=700>{asset.Reports.Count} Report{(asset.Reports.Count > 1 ? "s" : "")}{(severeReports != null && severeReports.Count > 0 ? $" {severeReports.Count} severe" : "")}</font-weight></size>", value = severeReports != null && severeReports.Count > 0, viewDataKey = $"stf_import_reports{asset.name}" };
				foldout.style.marginTop = 10;
				ret.Add(foldout);

				VisualElement createReportField(STFReport Report)
				{
					var ret = new Box();
					ret.style.paddingTop = ret.style.paddingBottom = ret.style.paddingLeft = 4;
					ret.Add(createInfoField($"<font-weight=700>{Report.Severity}</font-weight>", $"{Report.Message}", true, false, 0));
					ret.Add(createInfoField("Resource Type", !string.IsNullOrWhiteSpace(Report.ResourceType) ? Report.ResourceType : "Unknown", true, !string.IsNullOrWhiteSpace(Report.ResourceType), 0));
					ret.Add(createInfoField("Resource ID", !string.IsNullOrWhiteSpace(Report.ResourceID) ? Report.ResourceID : "Unknown", true, !string.IsNullOrWhiteSpace(Report.ResourceID), 0));
					return ret;
				}

				foreach(var report in asset.Reports)
				{
					foldout.Add(createReportField(report));
				}
			}

			return ret;
		}
	}

}

#endif
