
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class STF_Buffer : ScriptableObject
	{
		public string STF_ID;

		[HideInInspector]
		public byte[] Data;

		public long BufferLength => Data.LongLength;
	}
}

