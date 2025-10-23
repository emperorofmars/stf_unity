#if UNITY_EDITOR

using com.squirrelbite.stf_unity.tools;

namespace com.squirrelbite.stf_unity.modules.editors
{
	public interface ISTF_Module_Editor
	{
		string STF_Type {get;}
		void Draw(STFScriptedImporter Importer, ImportOptions.ResourceImportOption Option);
	}
}

#endif
