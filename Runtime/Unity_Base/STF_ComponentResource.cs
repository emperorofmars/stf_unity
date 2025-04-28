
using System;
using Newtonsoft.Json.Linq;

namespace com.squirrelbite.stf_unity
{
	public abstract class STF_ComponentResource: STF_MonoBehaviour
	{
		public override string STF_Kind => "component";
	}
}
