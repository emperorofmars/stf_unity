
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Registry
	{
		public static readonly List<ISTF_Module> Modules = new() {
			new STF_Prefab_Module(),
			new STF_Node_Module(),
			new STF_Instance_Armature_Module(),
			new STF_Armature_Module(),
			new STF_Bone_Module(),
			new STF_Instance_Mesh_Module(),
			new STF_Mesh_Module(),
		};

	}
}
