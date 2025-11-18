using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.modules
{
	/// <summary>
	/// For resources like meshes, images, etc..
	/// </summary>
	public abstract class STF_DataResource : STF_ScriptableObject
	{
		public override string STF_Kind => "data";
		public List<STF_ScriptableObject> Components = new();
	}
}
