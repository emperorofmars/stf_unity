
using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;

namespace com.squirrelbite.stf_unity
{
	public static class STF_Module_Registry
	{
		public static readonly List<ISTF_Module> DefaultModules = new() {
			new STF_Prefab_Module(),
			new STF_Node_Module(),
			new STF_Instance_Armature_Module(),
			new STF_Armature_Module(),
			new STF_Bone_Module(),
			new STF_Instance_Mesh_Module(),
			new STF_Mesh_Module(),
			new STF_Material_Module(),
			new STF_Image_Module(),
			new STF_Animation_Module(),
		};

		private static readonly Dictionary<string, ISTF_Module> RegisteredModules = new();

		public static void RegisterModule(ISTF_Module Module)
		{
			if(!RegisteredModules.ContainsKey(Module.STF_Type))
				RegisteredModules.Add(Module.STF_Type, Module);
		}

		public static Dictionary<string, ISTF_Module> Modules
		{
			get
			{
				var ret = new Dictionary<string, ISTF_Module>(RegisteredModules);
				foreach(var module in DefaultModules)
				{
					if(!ret.ContainsKey(module.STF_Type))
						ret.Add(module.STF_Type, module);
				}
				return ret;
			}
		}
	}
}
