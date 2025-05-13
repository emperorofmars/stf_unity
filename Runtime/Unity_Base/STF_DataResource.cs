
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_DataResource : STF_ScriptableObject
	{
		public override string STF_Kind => "data";
		public List<STF_ScriptableObject> Components = new();
	}
}
