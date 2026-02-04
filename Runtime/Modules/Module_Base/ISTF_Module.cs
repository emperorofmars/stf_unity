using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity.modules
{
	// TODO make this a static interface, once that language feature is available in Unity
	/// <summary>
	/// Base interface for all implementations of STF modules.
	/// </summary>
	public interface ISTF_Module
	{
		string STF_Type {get;}
		string STF_Kind {get;}
		int Priority {get;}
		List<string> LikeTypes {get;}
		List<System.Type> UnderstoodApplicationTypes {get;}
		int CanHandleApplicationObject(ISTF_Resource ApplicationObject);
		(ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject);
		(JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject);
		List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject);


		// When a component happens to be on a bone inside an armature, it will get instantiated with the armature. Armature instances have 'component-mods', which can override values.
		public void ImportInstanceMod(ImportContext Context, ISTF_Resource Resource, JObject JsonResource) {}
	}
}

