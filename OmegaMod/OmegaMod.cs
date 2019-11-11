using Assets.Nimbatus.GUI.Common.Scripts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using MonoMod.Utils;

using UnityEngine;

#pragma warning disable CS0626
namespace OmegaMod
{
	[BepInPlugin("OmegaMod", "OmegaMod", "2.0.0")]
	public class OmegaMod : BaseUnityPlugin
	{
		private ConfigEntry<float> _sizePerSecond;

		public OmegaMod()
		{
			Config = base.Config;
			Logger = base.Logger;
			var sizeDescription = new ConfigDescription("Shield Growth Rate");
			var sizeDefinition  = new ConfigDefinition("Shield", "SizePerSecond");

			_sizePerSecond = Config.Bind(sizeDefinition, 1f, sizeDescription);
		}

		public new static ManualLogSource Logger { get; set; }
		public new static ConfigFile      Config { get; set; }

		public void OnEnable()
		{
			// += your hooks
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.Start +=
				EnergyShield_Start;
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.FixedUpdate +=
				EnergyShield_FixedUpdate;
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.GetDetailedTooltip +=
				EnergyShield_GetDetailedTooltip;
		}


		public void OnDisable()
		{
			// -= your hooks (a future Partiality update will do this automatically)
		}

		#region ShieldKeys

		public KeyBinding increaseSize = new KeyBinding("Grow",   KeyCode.None);
		public KeyBinding decreaseSize = new KeyBinding("Shrink", KeyCode.None);

		#endregion

		#region ThrusterKeys

		#endregion

		#region EnergyShield

		public void EnergyShield_FixedUpdate(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.orig_FixedUpdate orig,
			EnergyShield                                                                                          self
		)
		{
			var KeyEventHub = self.FindEventKeyHubRecursive();
			if (increaseSize.IsPressed(KeyEventHub))
				self.ShieldSize += _sizePerSecond.Value;

			if (decreaseSize.IsPressed(KeyEventHub))
				self.ShieldSize -= _sizePerSecond.Value;

			orig(self);
		}

		public void EnergyShield_Start(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.orig_Start orig,
			EnergyShield                                                                                    self
		)
		{
			self.AddKeyBindings(increaseSize, decreaseSize);
			orig(self);
		}

		public string EnergyShield_GetDetailedTooltip(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DefensiveParts.EnergyShield.orig_GetDetailedTooltip
				orig,
			EnergyShield self
		)
		{
			DynData<EnergyShield> dynEnergy = new DynData<EnergyShield>(self);

			string buildStringBase()
			{
				return orig(self);
			}

			string str = buildStringBase() + "\nSomething";
			return str               + LabelHelper.White +
				   "Size per Second" + ": "              + LabelHelper.Orange +
				   (object) _sizePerSecond;
		}
	}

	#endregion

	#region Thruster

	#endregion


	#region Hookers

//	[MonoModPatch("global::Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Thruster.Thruster")]
//	internal class patch_Thruster : Thruster
//	{
//		[MonoModIgnore] private KeyBinding _giveThrust;
//
//		private KeyBinding _reverseThrust;
//
//
//		public override List<KeyBinding> GetKeyBindings()
//		{
//			_giveThrust    = new KeyBinding("Activate", KeyCode.W);
//			
//			if (ChargeUp)
//				return new List<KeyBinding> {_giveThrust, _reverseThrust};
//			return new List<KeyBinding> {_giveThrust};
//		}
//
//		public extern void orig_FixedUpdate();
//
//		public override void FixedUpdate()
//		{
//			if (ChargeUp)
//			{
//				if (_reverseThrust.IsPressed(KeyEventHub))
//					Force = -100f;
//				else
//					Force = 100f;
//			}
//
//			orig_FixedUpdate();
//		}
//	}

	#endregion
}