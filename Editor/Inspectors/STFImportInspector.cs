#if UNITY_EDITOR

using UnityEditor;
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

			void createInfoField(string label, string text)
			{
				var field = new VisualElement();
				field.style.flexDirection = FlexDirection.Row;
				field.style.marginTop = field.style.marginBottom = 2;
				ret.Add(field);

				var labelField = new Label(label);
				labelField.style.flexBasis = 100;
				labelField.style.flexGrow = 100;
				labelField.style.minWidth = 120;
				field.Add(labelField);

				var valueField = new Label(text);
				valueField.style.flexBasis = 180;
				valueField.style.flexGrow = 180;
				field.Add(valueField);
			}

			createInfoField("Asset Name", asset.Meta?.STFAssetInfo?.AssetName);
			createInfoField("Asset Version", asset.Meta?.STFAssetInfo?.Version);
			createInfoField("Author", asset.Meta?.STFAssetInfo?.Author);
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.URL) && asset.Meta.STFAssetInfo.URL.StartsWith("https://"))
				createInfoField("URL", $"<a href=\"{asset.Meta?.STFAssetInfo?.URL}\">{asset.Meta?.STFAssetInfo?.URL}</a>");
			else
				createInfoField("URL", asset.Meta?.STFAssetInfo?.URL);
			createInfoField("License", asset.Meta?.STFAssetInfo?.License);
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.LicenseURL) && asset.Meta.STFAssetInfo.LicenseURL.StartsWith("https://"))
				createInfoField("License URL", $"<a href=\"{asset.Meta?.STFAssetInfo?.LicenseURL}\">{asset.Meta?.STFAssetInfo?.LicenseURL}</a>");
			else
				createInfoField("License URL", asset.Meta?.STFAssetInfo?.LicenseURL);
			if(!string.IsNullOrWhiteSpace(asset.Meta?.STFAssetInfo?.DocumentationURL) && asset.Meta.STFAssetInfo.DocumentationURL.StartsWith("https://"))
				createInfoField("License Documentation", $"<a href=\"{asset.Meta?.STFAssetInfo?.DocumentationURL}\">{asset.Meta?.STFAssetInfo?.DocumentationURL}</a>");
			else
				createInfoField("License Documentation", asset.Meta?.STFAssetInfo?.DocumentationURL);
			createInfoField("Binary Version", $"{asset.BinaryVersionMajor}.{asset.BinaryVersionMinor}");
			createInfoField("Definition Version", $"{asset.Meta?.DefinitionVersionMajor}.{asset.Meta?.DefinitionVersionMinor}");
			return ret;
		}
	}

}

#endif
