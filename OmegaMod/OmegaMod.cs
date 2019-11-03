using System.Collections.Generic;

using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Thruster;

using BepInEx;

using MonoMod;
using MonoMod.ModInterop;

using UnityEngine;

#pragma warning disable CS0626
namespace OmegaMod
{
	[BepInPlugin("OmegaMod", "OmegaMod", "2.0.0")]
	public class OmegaMod : BaseUnityPlugin
	{
		public void OnLoad()
		{
		}

		public void OnEnable()
		{
			// += your hooks
		}

		public void OnDisable()
		{
			// -= your hooks (a future Partiality update will do this automatically)
		}
	}


	#region Hookers

	[MonoModPatch("global::Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield")]
	internal class patch_EnergyShield : EnergyShield
	{
		[MonoModIgnore] private KeyBinding _activateShield;

		public KeyBinding _decreaseSize;

		public        KeyBinding _increaseSize;
		public        float      SizePerSecond;
		public extern void       orig_Start();

		protected override void Start()
		{
			SizePerSecond = 1f;
			orig_Start();

			AddKeyBindings();
		}

		public override List<KeyBinding> GetKeyBindings()
		{
			_activateShield = new KeyBinding("Activate", KeyCode.None);
			_increaseSize   = new KeyBinding("Grow",     KeyCode.None);
			_decreaseSize   = new KeyBinding("Shrink",   KeyCode.None);
			return new List<KeyBinding> {_activateShield, _increaseSize, _decreaseSize};
		}

		//TODO: Fix Shield Tooltips
		//public override string GetDetailedTooltip()
		//{
		//    string text = base.GetDetailedTooltip() + LabelHelper.NewLine;
		//    string text2 = text;
		//    text = string.Concat(new object[]
		//    {
		//        text2,
		//        LabelHelper.White,
		//        "Shield Size: ",
		//        LabelHelper.Orange,
		//        this.ShieldSize,
		//        LabelHelper.NewLine
		//    });
		//    text2 = text;
		//    text = string.Concat(new object[]
		//    {
		//        text2,
		//        LabelHelper.White,
		//        "Growth Rate: ",
		//        LabelHelper.Orange,
		//        this.SizePerSecond,
		//        LabelHelper.NewLine
		//    });
		//    text2 = text;
		//    return string.Concat(new object[]
		//    {
		//        text2,
		//        LabelHelper.White,
		//        "Energy per Second: ",
		//        LabelHelper.Orange,
		//        this.EnergyPerSecond
		//    });
		//}
		public extern void orig_Update();

		public override void Update()
		{
			if (_increaseSize.IsPressed(KeyEventHub))
				ShieldSize += SizePerSecond;

			if (_decreaseSize.IsPressed(KeyEventHub))
				ShieldSize -= SizePerSecond;

			orig_Update();
		}
	}

	[MonoModPatch("global::Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Thruster.Thruster")]
	internal class patch_Thruster : Thruster
	{
		[MonoModIgnore] private KeyBinding _giveThrust;

		private KeyBinding _reverseThrust;


		public override List<KeyBinding> GetKeyBindings()
		{
			_giveThrust    = new KeyBinding("Activate", KeyCode.W);
			_reverseThrust = new KeyBinding("Reverse",  KeyCode.None);
			if (ChargeUp)
				return new List<KeyBinding> {_giveThrust, _reverseThrust};
			return new List<KeyBinding> {_giveThrust};
		}

		public extern void orig_FixedUpdate();

		public override void FixedUpdate()
		{
			if (ChargeUp)
			{
				if (_reverseThrust.IsPressed(KeyEventHub))
					Force = -100f;
				else
					Force = 100f;
			}

			orig_FixedUpdate();
		}
	}

	#endregion

	[ModExportName("OmegaMod")] // Defaults to the mod assembly name.
	public static class ModExports
	{
		// Methods are exported.
	}
}