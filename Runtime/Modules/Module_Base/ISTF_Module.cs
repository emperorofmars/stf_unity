
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	// TODO make this a static interface, once that language feature is available in Unity
	public interface ISTF_Module
	{
		string STF_Type {get;}
		string STF_Kind {get;}
		int Priority {get;}
		List<string> LikeTypes {get;}
		List<Type> UnderstoodApplicationTypes {get;}
		int CanHandleApplicationObject(ISTF_Resource ApplicationObject);
		(ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject);
		(JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject);
		List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject);
	}
}

