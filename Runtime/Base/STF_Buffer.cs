using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	/// <summary>
	/// Representation of an STF definitions `buffer` of type `stf.buffer.included`.
	/// </summary>
	[System.Serializable]
	public class STF_Buffer
	{
		public string STF_Id;

		[HideInInspector]
		public byte[] Data;

		public long BufferLength => Data.LongLength;
	}
}

