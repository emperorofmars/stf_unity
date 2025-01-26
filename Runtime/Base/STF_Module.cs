
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
		(object ApplicationObject, IImportContext Context) Import(IImportContext Context, JObject Json, string ID, object ParentApplicationObject);
		(JObject Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject);
		List<STF_Component> GetComponents(object ApplicationObject);
	}
}

