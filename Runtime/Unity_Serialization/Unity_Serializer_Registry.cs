using System.Collections.Generic;

namespace com.squirrelbite.stf_unity.serialization
{
	/// <summary>
	/// Register your own NNA Json Serializer here.
	/// </summary>
	public static class Unity_Serializer_Registry
	{
		public static readonly List<IUnity_Serializer> DefaultSerializers = new() {
			new Unity_Twist_Serializer(),
		};

		private static readonly List<IUnity_Serializer> RegisteredSerializers = new();

		public static void RegisterSerializer(IUnity_Serializer Serializer) { RegisteredSerializers.Add(Serializer); }

		public static List<IUnity_Serializer> Serializers { get {
			var ret = new List<IUnity_Serializer>(DefaultSerializers);
			ret.AddRange(RegisteredSerializers);
			return ret;
		}}
	}
}
