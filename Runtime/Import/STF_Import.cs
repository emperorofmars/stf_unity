using System.Collections.Generic;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class STF_Import : ScriptableObject
	{
		public uint BinaryVersionMajor;
		public uint BinaryVersionMinor;
		public string OriginalFileName;
		public STF_Meta Meta;
		public List<STF_Buffer> Buffers = new();
		public GameObject Root;

		// TODO Reports

		public void Init(ImportState State)
		{
			name = "STF_Import";
			BinaryVersionMajor = State.File.VersionMajor;
			BinaryVersionMinor = State.File.VersionMinor;
			Meta = State.Meta;
			OriginalFileName = State.File.OriginalFileName;
			if(State.ImportedObjects.GetValueOrDefault(State.RootID) is GameObject @rootObject)
				Root = @rootObject;
			// TODO
		}
	}
}

