
using com.squirrelbite.ava_base_setup.vrchat;
using UnityEngine;

namespace com.squirrelbite.stf_unity.ava.vrchat.processors
{
	public static class AVA_BaseSetup_Util
	{
		public static GameObject EnsureObjectSetup(AvatarBaseSetupVRChat Setup, string Path)
		{
			var current = Setup.transform;
			foreach(var pathElement in Path.Split("/"))
			{
				var success = false;
				for(int childIdx = 0; childIdx < current.childCount; childIdx++)
				{
					if(current.GetChild(childIdx).name.ToLower() == pathElement.ToLower())
					{
						current = current.GetChild(childIdx);
						success = true;
						break;
					}
				}
				if(!success)
				{
					var prev = current;
					current = new GameObject(pathElement).transform;
					current.transform.SetParent(prev.transform);
				}
			}
			return current.gameObject;
		}
	}

}
