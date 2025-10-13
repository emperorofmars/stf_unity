using System.Collections.Generic;
using com.squirrelbite.stf_unity.modules;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public class STF_Import : ScriptableObject
	{
		public uint BinaryVersionMajor;
		public uint BinaryVersionMinor;
		public string OriginalFileName;
		public STF_Meta Meta;
		public GameObject Root;

		public List<STFReport> Reports = new();

		public void Init(ImportState State)
		{
			name = "STF_Import";
			BinaryVersionMajor = State.File.VersionMajor;
			BinaryVersionMinor = State.File.VersionMinor;
			Meta = State.Meta;
			OriginalFileName = State.File.OriginalFileName;
			if(State.ImportedObjects.GetValueOrDefault(State.RootID) is STF_Prefab @rootObject)
				Root = @rootObject.gameObject;
			Reports = State.Reports;
		}
	}
}
