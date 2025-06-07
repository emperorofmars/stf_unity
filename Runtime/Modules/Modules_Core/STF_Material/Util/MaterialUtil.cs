using System;
using System.Collections.Generic;
using TexPacker;
using UnityEngine;

namespace com.squirrelbite.stf_unity.modules.stf_material.util
{
	public class ImageChannelSetup
	{
		public class ImageChannel
		{
			public IMaterialValueType Source;
			public bool Invert;
			public ImageChannel(IMaterialValueType Source, bool Invert = false) { this.Source = Source; this.Invert = Invert; }
			public static ImageChannel Empty() { return new ImageChannel(null); }
		}
		public string Name;
		public ImageChannel Channel0;
		public ImageChannel Channel1;
		public ImageChannel Channel2;
		public ImageChannel Channel3;

		public ImageChannelSetup() {}

		public ImageChannelSetup(ImageChannel Channel0, ImageChannel Channel1, ImageChannel Channel2, ImageChannel Channel3)
		{
			this.Channel0 = Channel0; this.Channel1 = Channel1; this.Channel2 = Channel2; this.Channel3 = Channel3;
		}

		public ImageChannel this[int key]
		{
			get {
				return key switch
				{
					0 => Channel0,
					1 => Channel1,
					2 => Channel2,
					3 => Channel3,
					_ => throw new Exception("Invalid Channel Access!"),
				};
			}
		}
	}

	public static class ImageUtil
	{
		private static TextureChannel ChannelIdxToEnum(int Idx)
		{
			return Idx switch
			{
				0 => TextureChannel.ChannelRed,
				1 => TextureChannel.ChannelGreen,
				2 => TextureChannel.ChannelBlue,
				3 => TextureChannel.ChannelAlpha,
				_ => throw new Exception("Invalid Channel Access!"),
			};
		}

		public static Texture2D AssembleTextureChannels(ImageChannelSetup Channels)
		{
			var packer = new TexturePacker();
			packer.Initialize();
			var resolutions = new Dictionary<int, int>();
			for (int i = 0; i < 4; i++)
			{
				var channelSource = Channels[i];
				if (channelSource.Source != null)
				{
					var input = new TextureInput();
					if (channelSource.Source is ImageValue value)
					{
						input.texture = STFUtil.GetProcessed<Texture2D>(value.Image);
						input.SetChannelInput(ChannelIdxToEnum(i), new TextureChannelInput
						{
							enabled = true,
							invert = channelSource.Invert,
							output = ChannelIdxToEnum(i),
						});
						if (resolutions.ContainsKey(input.texture.width)) resolutions[input.texture.width]++;
						else resolutions.Add(input.texture.width, 1);
					}
					/*else if(channelSource.Source is ImageChannelValue)
					{
						var textureChannelProperty = (TextureChannelPropertyValue)channelSource.Source;
						input.texture = textureChannelProperty.Texture;
						input.SetChannelInput(ChannelIdxToEnum(textureChannelProperty.Channel), new TextureChannelInput {
							enabled = true,
							invert = channelSource.Invert,
							output = ChannelIdxToEnum(i),
						});
					}*/
					else
					{
						throw new Exception("Unsupported PropertyValue Type");
					}
					packer.Add(input);
				}
			}
			var selectedResolution = 0;
			var selectedResolutionCount = 0;
			foreach ((var resolution, var count) in resolutions)
			{
				if (count > selectedResolutionCount)
				{
					selectedResolution = resolution;
					selectedResolutionCount = count;
				}
			}
			packer.resolution = selectedResolution;
			return packer.Create();
		}
	}
}
