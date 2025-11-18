
namespace com.squirrelbite.stf_unity.modules
{
	/// <summary>
	/// For resources like mesh-instances, armature-instances, ...
	/// </summary>
	public abstract class STF_InstanceResource: STF_MonoBehaviour
	{
		public override string STF_Kind => "instance";
	}
}
