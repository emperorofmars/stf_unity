using System;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	[Serializable]
	public class STF_Buffer
	{
		public string STF_Id;

		[HideInInspector]
		public byte[] Data;

		public long BufferLength => Data.LongLength;
	}
}

