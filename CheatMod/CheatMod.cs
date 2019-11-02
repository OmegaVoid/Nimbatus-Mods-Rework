using Assets.Nimbatus.Scripts.ResourceCollection;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries;
using On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources;
using On.Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks;

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
				Battery.Awake += Battery_Awake;
			if (infiniteFuel)
				FuelTank.Awake += FuelTank_Awake;
			if (infiniteResources)
				ResourceTank.FixedUpdate +=
					ResourceTank_FixedUpdate;
		}

		private void ResourceTank_FixedUpdate(
			ResourceTank.orig_FixedUpdate                                                         orig,
			Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources.ResourceTank self
		)
		{
			self.SetResourceAmount(infiniteType, self.ResourceCapacity);
			orig(self);
		}

		private void FuelTank_Awake(
			FuelTank.orig_Awake                                                      orig,
			Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks.FuelTank self
		)
		{
			self.CurrentFuelAmount = 1E+19f;
			self.MaxFuelAmount     = 1E+19f;
			self.RechargePerSecond = 1E+19f;
			orig(self);
		}

		private void Battery_Awake(
			Battery.orig_Awake                                                      orig,
			Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries.Battery self
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
			Battery.Awake  -= Battery_Awake;
			FuelTank.Awake -= FuelTank_Awake;
			ResourceTank.FixedUpdate -=
				ResourceTank_FixedUpdate;
		}
	}
}