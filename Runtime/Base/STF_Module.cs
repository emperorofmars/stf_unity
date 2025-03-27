
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	// TODO make this a static interface, once that language feature is available in Unity
	public interface STF_Module
	{
		string STF_Type {get;}
		string STF_Kind {get;}
		int Priority {get;}
		List<string> LikeTypes {get;}
		List<Type> UnderstoodApplicationTypes {get;}
		int CanHandleApplicationObject(object ApplicationObject);
		object Import(ImportContext Context, JObject Json, string ID, object ContextObject);
		(JObject Json, string ID) Export(ExportContext Context, object ApplicationObject, object ContextObject);
		List<STF_Component> GetComponents(object ApplicationObject);
	}
}

