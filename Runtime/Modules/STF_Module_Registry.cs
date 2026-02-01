using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules.stfexp;

namespace com.squirrelbite.stf_unity.modules
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
			new STF_Texture_Module(),
			new STF_Animation_Module(),
			new STFEXP_Camera_Module(),
			new STFEXP_Light_Module(),
			new STFEXP_LightprobeAnchor_Module(),
			new STFEXP_Collider_Sphere_Module(),
			new STFEXP_Collider_Capsule_Module(),
			new STFEXP_Collider_Plane_Module(),
			new STFEXP_Constraint_Twist_Module(),
			new STFEXP_Constraint_Rotation_Module(),
			new STFEXP_Constraint_Parent_Module(),
			new STFEXP_Humanoid_Armature_Module(),
		};

		public static readonly HashSet<string> DefaultIgnores = new() {
			"stfexp.mesh.seams",
			"stfexp.mesh.creases",
			"org.blender",
		};

		private static readonly Dictionary<string, ISTF_Module> RegisteredModules = new();

		private static readonly HashSet<string> RegisteredIgnores = new();

		public static void RegisterModule(ISTF_Module Module)
		{
			if (!RegisteredModules.ContainsKey(Module.STF_Type))
				RegisteredModules.Add(Module.STF_Type, Module);
		}

		public static void RegisterIgnore(string Ignore)
		{
			if (!RegisteredIgnores.Contains(Ignore))
				RegisteredIgnores.Add(Ignore);
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

		public static HashSet<string> Ignores
		{
			get
			{
				var ret = new HashSet<string>(RegisteredIgnores);
				foreach(var ignore in DefaultIgnores)
				{
					if(!ret.Contains(ignore))
						ret.Add(ignore);
				}
				return ret;
			}
		}
	}
}
