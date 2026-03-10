#if UNITY_EDITOR
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using com.squirrelbite.stf_unity.resources;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using NUnit.Framework;
using com.squirrelbite.stf_unity.resources.stfexp;

namespace com.squirrelbite.stf_unity.squirrelbite
{
	[AddComponentMenu("STF/Resources/com.squirrelbite/com.squirrelbite.avatar_setup")]
	//[HelpURL("https://docs.stfform.at/resources/thirdparty/com.squirrelbite/com_squirrelbite_avatar_setup.html")]
	public class Squirrelbite_AvatarSetup : STF_NodeComponentResource
	{
		public const string _STF_Type = "com.squirrelbite.avatar_setup";
		public override string STF_Type => _STF_Type;

		[System.Serializable]
		public class Toggle
		{
			public string Name;
			public STF_DataResource On;
			public STF_DataResource Off;
		}

		[System.Serializable]
		public class PersistentPuppet
		{
			public string Name;
			public string PuppetType;
			public string ParameterEnabled;
			public string ParameterX;
			public string ParameterY;
			public STF_DataResource Blendtree;
		}

		[System.Serializable]
		public class Puppet
		{
			public string Name;
			public STF_DataResource Blendtree;
		}

		public List<Toggle> TogglesPre = new();
		public List<PersistentPuppet> PersistentPuppetsPre = new();
		public List<Toggle> Toggles = new();
		public List<Puppet> Puppets = new();

		public STF_DataResource BreathingNormal;
		public STF_DataResource BreathingIntense;
		public STF_DataResource AdditiveIdle;
		public STF_DataResource AdditiveExcited;
	}

	public class Squirrelbite_AvatarSetup_Handler : ISTF_Handler
	{
		public string STF_Type => Squirrelbite_AvatarSetup._STF_Type;
		public string STF_Category => "component";
		public int Priority => 1;
		public List<string> LikeTypes => new(){"voice_position"};
		public List<System.Type> UnderstoodApplicationTypes => new(){typeof(Squirrelbite_AvatarSetup)};
		public List<ISTF_Resource> GetComponents(ISTF_Resource ApplicationObject) { return null; }
		public int CanHandleApplicationObject(ISTF_Resource ApplicationObject) { return 0; }

		public (ISTF_Resource STFResource, List<object> ApplicationObjects) Import(ImportContext Context, JObject JsonResource, string STF_Id, ISTF_Resource ContextObject)
		{
			var go = ContextObject as STF_MonoBehaviour;
			var ret = go.gameObject.AddComponent<Squirrelbite_AvatarSetup>();
			ret.SetFromJson(JsonResource, STF_Id, ContextObject, "Squirrelbite Avatar Setup");

			if(JsonResource.ContainsKey("toggles_pre")) foreach(JObject toggleJson in JsonResource["toggles_pre"].Cast<JObject>())
			{
				var toggle = new Squirrelbite_AvatarSetup.Toggle { Name = toggleJson.ContainsKey("name") ? toggleJson.Value<string>("name") : "", };
				if(toggleJson.ContainsKey("on") && STFUtil.ImportResource(Context, JsonResource, toggleJson["on"], "data") is STF_DataResource onClip)
					toggle.On = onClip;
				if(toggleJson.ContainsKey("off") && STFUtil.ImportResource(Context, JsonResource, toggleJson["off"], "data") is STF_DataResource offClip)
					toggle.Off = offClip;
				ret.TogglesPre.Add(toggle);
			}

			if(JsonResource.ContainsKey("puppets_pre")) foreach(JObject puppetJson in JsonResource["puppets_pre"].Cast<JObject>())
			{
				var puppet = new Squirrelbite_AvatarSetup.PersistentPuppet { Name = puppetJson.ContainsKey("name") ? puppetJson.Value<string>("name") : "", };
				if(puppetJson.ContainsKey("puppet_type")) puppet.PuppetType = puppetJson.Value<string>("puppet_type");
				if(puppetJson.ContainsKey("property_enabled")) puppet.ParameterEnabled = puppetJson.Value<string>("property_enabled");
				if(puppetJson.ContainsKey("property_x")) puppet.ParameterX = puppetJson.Value<string>("property_x");
				if(puppetJson.ContainsKey("property_y")) puppet.ParameterY = puppetJson.Value<string>("property_y");
				if(puppetJson.ContainsKey("blendtree") && STFUtil.ImportResource(Context, JsonResource, puppetJson["blendtree"], "data") is STF_DataResource blendtree)
					puppet.Blendtree = blendtree;
				ret.PersistentPuppetsPre.Add(puppet);
			}

			if(JsonResource.ContainsKey("toggles")) foreach(JObject toggleJson in JsonResource["toggles"].Cast<JObject>())
			{
				var toggle = new Squirrelbite_AvatarSetup.Toggle { Name = toggleJson.ContainsKey("name") ? toggleJson.Value<string>("name") : "", };
				if(toggleJson.ContainsKey("on") && STFUtil.ImportResource(Context, JsonResource, toggleJson["on"], "data") is STF_DataResource onClip)
					toggle.On = onClip;
				if(toggleJson.ContainsKey("off") && STFUtil.ImportResource(Context, JsonResource, toggleJson["off"], "data") is STF_DataResource offClip)
					toggle.Off = offClip;
				ret.Toggles.Add(toggle);
			}

			if(JsonResource.ContainsKey("puppets")) foreach(JObject puppetJson in JsonResource["puppets"].Cast<JObject>())
			{
				var puppet = new Squirrelbite_AvatarSetup.Puppet { Name = puppetJson.ContainsKey("name") ? puppetJson.Value<string>("name") : "", };
				if(puppetJson.ContainsKey("blendtree") && STFUtil.ImportResource(Context, JsonResource, puppetJson["blendtree"], "data") is STF_DataResource blendtree)
					puppet.Blendtree = blendtree;
				ret.Puppets.Add(puppet);
			}

			if(JsonResource.ContainsKey("breathing") && JsonResource["breathing"] is JObject breathingJson)
			{
				if(breathingJson.ContainsKey("normal") && STFUtil.ImportResource(Context, JsonResource, breathingJson["normal"], "data") is STF_DataResource anim_bn)
					ret.BreathingNormal = anim_bn;
				if(breathingJson.ContainsKey("intense") && STFUtil.ImportResource(Context, JsonResource, breathingJson["intense"], "data") is STF_DataResource anim_bi)
					ret.BreathingIntense = anim_bi;
			}

			if(JsonResource.ContainsKey("additive") && JsonResource["additive"] is JObject additiveJson)
			{
				if(additiveJson.ContainsKey("idle") && STFUtil.ImportResource(Context, JsonResource, additiveJson["idle"], "data") is STF_DataResource anim_idle)
					ret.AdditiveIdle = anim_idle;
				if(additiveJson.ContainsKey("excited") && STFUtil.ImportResource(Context, JsonResource, additiveJson["excited"], "data") is STF_DataResource anim_add_excited)
					ret.AdditiveExcited = anim_add_excited;
			}

			if (JsonResource.ContainsKey("enabled") && JsonResource.Value<bool>("enabled") == false)
				ret.enabled = false;

			return (ret, null);
		}

		public (JObject Json, string STF_Id) Export(ExportContext Context, ISTF_Resource ApplicationObject, ISTF_Resource ContextObject)
		{
			throw new System.NotImplementedException();
		}
	}

	[InitializeOnLoad]
	class Register_Squirrelbite_AvatarSetup_Handler
	{
		static Register_Squirrelbite_AvatarSetup_Handler()
		{
			STF_Handler_Registry.RegisterHandler(new Squirrelbite_AvatarSetup_Handler());
		}
	}
}

#endif
