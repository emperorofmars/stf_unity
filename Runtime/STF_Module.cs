
using System;
using System.Collections.Generic;

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
		(object ApplicationObject, IImportContext Context) Import(IImportContext Context, object Json, string ID, object ParentApplicationObject);
		(object Json, string ID, IExportContext Context) Export(IExportContext Context, object ApplicationObject, object ParentApplicationObject);
		List<STF_Component> GetComponents(object ApplicationObject);
	}
}

