
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules
{
	public class STF_Image : STF_DataResource
	{
		public const string STF_TYPE = "stf.image";
		public override string STF_Type => STF_TYPE;

		public override (string RelativePath, Type Type, List<string> PropertyNames, Func<List<float>, List<float>> ConvertValueFunc) ConvertPropertyPath(List<string> STFPath)
		{
			throw new NotImplementedException();
		}

		public override List<string> ConvertPropertyPath(string UnityPath)
		{
			throw new NotImplementedException();
		}
	}

	public class STF_Image_Module : ISTF_Module
	{
		public string STF_Type => STF_Image.STF_TYPE;

		public string STF_Kind => "data";

		public int Priority => 0;

		public List<string> LikeTypes => new(){"image"};

		public List<Type> UnderstoodApplicationTypes => new(){typeof(STF_Image)};

		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return new List<ISTF_Resource>(((STF_Image)ApplicationObject).Components); }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var ret = ScriptableObject.CreateInstance<STF_Image>();
			ret.SetFromJson(JsonResource, STF_Id, "STF Image");
			return (ret, new(){ret});
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			var ImageObject = ApplicationObject as STF_Image;
			var ret = new JObject {
				{"type", STF_Type},
				{"name", ImageObject.STF_Name},
			};

			return (ret, ImageObject.STF_Id);
		}
	}
}
