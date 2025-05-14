using UnityEngine;

namespace com.squirrelbite.stf_unity
{
	public static class UnityUtil
	{
		public static string getPath(Transform root, Transform transform, bool relative = false)
		{
			string path = "/" + transform.name;
			while (transform.parent != root && transform.parent != null)
			{
				transform = transform.parent;
				path = "/" + transform.name + path;
			}
			if(relative) path = path[1..];
			return path;
		}
	}
}
