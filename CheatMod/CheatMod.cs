using Assets.Nimbatus.Scripts.ResourceCollection;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace CheatMod
{
	[BepInPlugin(Name: "Cheat Mod", Version: "2.0.0", GUID: "CheatMod")]
	public class CheatMod : BaseUnityPlugin
	{
		private readonly bool infiniteEnergy    = true;
		private readonly bool infiniteFuel      = true;
		private readonly bool infiniteResources = true;

		private readonly EResourceType infiniteType = EResourceType.CommonOre;

		public CheatMod()
		{
			Config = base.Config;
			Logger = base.Logger;
			var energyConf   = new ConfigDefinition("Cheats",    "Energy",    "Enable Infinite Energy");
			var fuelConf     = new ConfigDefinition("Cheats",    "Fuel",      "Enable Infinite Fuel");
			var resourceConf = new ConfigDefinition("Cheats",    "Resources", "Enable Infinite Resources");
			var oreConf      = new ConfigDefinition("Resources", "Ore",       "false: Common Ore\ntrue:Rare Ore");


			Config.SaveOnConfigSet = true;

			Config.Wrap<bool>(energyConf);
			Config.Wrap<bool>(fuelConf);
			Config.Wrap<bool>(resourceConf);
			Config.Wrap<bool>(oreConf);

			Logger.LogInfo(Config);
			OnEnable();
		}


		public new static ManualLogSource Logger { get; set; }
		public new static ConfigFile      Config { get; set; }

		public void OnEnable()
		{
			// += your hooks
			if (infiniteEnergy)
				On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries.Battery.Awake += Battery_Awake;
			if (infiniteFuel)
				On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks.FuelTank.Awake += FuelTank_Awake;
			if (infiniteResources)
				On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources.ResourceTank.Update +=
					ResourceTank_Update;
		}

		private void ResourceTank_Update(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources.ResourceTank.orig_Update orig,
			ResourceTank                                                                                         self
		)
		{
			self.SetResourceAmount(infiniteType, self.ResourceCapacity);
			orig(self);
		}

		private void FuelTank_Awake(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks.FuelTank.orig_Awake orig,
			FuelTank                                                                               self
		)
		{
			self.CurrentFuelAmount = 1E+19f;
			self.MaxFuelAmount     = 1E+19f;
			self.RechargePerSecond = 1E+19f;
			orig(self);
		}

		private void Battery_Awake(
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries.Battery.orig_Awake orig,
			Battery                                                                               self
		)
		{
			self.MaxEnergyAmount     = 1E+19f;
			self.CurrentEnergyAmount = 1E+19f;
			self.RechargePerSecond   = 1E+19f;
			orig(self);
		}


		public void OnDisable()
		{
			// -= your hooks (a future Partiality update will do this automatically)
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries.Battery.Awake  -= Battery_Awake;
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks.FuelTank.Awake -= FuelTank_Awake;
			On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources.ResourceTank.Update -=
				ResourceTank_Update;
		}
	}
}