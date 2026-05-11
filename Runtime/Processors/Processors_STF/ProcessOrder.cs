
namespace com.squirrelbite.stf_unity.processors
{
	public enum ProcessOrder
	{
		DEFAULT = 10000,
		TEXTURE  = 50000,
		IMAGE = 51000,
		MATERIAL = 60000,
		COMPONENT = 70000,
		ANIMATION = 100000000,
		FINALE = 200000000,
	}
}
