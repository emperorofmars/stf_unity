using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources.stfexp;

namespace com.squirrelbite.stf_unity.resources
{
	public static class STF_Handler_Registry
	{
		public static readonly List<ISTF_Handler> DefaultHandlers = new() {
			new STF_Prefab_Handler(),
			new STF_Node_Handler(),
			new STF_Instance_Armature_Handler(),
			new STF_Armature_Handler(),
			new STF_Bone_Handler(),
			new STF_Instance_Mesh_Handler(),
			new STF_Mesh_Handler(),
			new STF_Material_Handler(),
			new STF_Image_Handler(),
			new STF_Texture_Handler(),
			new STF_Animation_Handler(),
			new STFEXP_Camera_Handler(),
			new STFEXP_Light_Handler(),
			new STFEXP_LightprobeAnchor_Handler(),
			new STFEXP_Collider_Sphere_Handler(),
			new STFEXP_Collider_Capsule_Handler(),
			new STFEXP_Collider_Plane_Handler(),
			new STFEXP_Constraint_Twist_Handler(),
			new STFEXP_Constraint_Rotation_Handler(),
			new STFEXP_Constraint_Parent_Handler(),
			new STFEXP_Constraint_IK_Handler(),
			new STFEXP_Humanoid_Armature_Handler(),
			new STFEXP_Instance_Text_Handler(),
			new STFEXP_Text_Handler(),
			new STFEXP_AnimationBlendtree_Handler(),
		};

		public static readonly HashSet<string> DefaultIgnores = new() {
			"stfexp.mesh.seams",
			"stfexp.mesh.creases",
			"org.blender",
		};

		private static readonly Dictionary<string, ISTF_Handler> RegisteredHandlers = new();

		private static readonly HashSet<string> RegisteredIgnores = new();

		public static void RegisterHandler(ISTF_Handler Module)
		{
			if (!RegisteredHandlers.ContainsKey(Module.STF_Type))
				RegisteredHandlers.Add(Module.STF_Type, Module);
		}

		public static void RegisterIgnore(string Ignore)
		{
			if (!RegisteredIgnores.Contains(Ignore))
				RegisteredIgnores.Add(Ignore);
		}

		public static Dictionary<string, ISTF_Handler> Handlers
		{
			get
			{
				var ret = new Dictionary<string, ISTF_Handler>(RegisteredHandlers);
				foreach(var module in DefaultHandlers)
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
