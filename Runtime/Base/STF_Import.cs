using System.Collections.Generic;
using com.squirrelbite.stf_unity.resources;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	/// <summary>
	/// Holds meta information of an imported STF file.
	/// </summary>
	public class STF_Import : ScriptableObject
	{
		public uint BinaryVersion;
		public string OriginalFileName;
		public STF_Meta Meta;
		public GameObject Root;

		public List<STFReport> Reports = new();

		public void Init(ImportState State)
		{
			name = "STF_Import";
			BinaryVersion = State.File.Version;
			Meta = State.Meta;
			OriginalFileName = State.File.OriginalFileName;
			if(State.ImportedObjects.GetValueOrDefault(State.RootID) is STF_Prefab @rootObject)
				Root = @rootObject.gameObject;
			Reports = State.Reports;
		}
	}
}
