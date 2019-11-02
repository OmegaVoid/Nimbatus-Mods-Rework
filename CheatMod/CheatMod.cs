using System;

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
		private readonly ConfigWrapper<bool> _infiniteEnergy;
		private readonly ConfigWrapper<bool> _oreType;
		private          ConfigWrapper<bool> _infiniteFuel;
		private          ConfigWrapper<bool> _infiniteResources;
		private          EResourceType       _infiniteType = EResourceType.CommonOre;


		public CheatMod()
		{
			Config = base.Config;
			Logger = base.Logger;
			var energyConf   = new ConfigDefinition("Cheats",    "Energy",    "Enable Infinite Energy");
			var fuelConf     = new ConfigDefinition("Cheats",    "Fuel",      "Enable Infinite Fuel");
			var resourceConf = new ConfigDefinition("Cheats",    "Resources", "Enable Infinite Resources");
			var oreConf      = new ConfigDefinition("Resources", "Ore",       "false: Common Ore\ntrue:Rare Ore");


			Config.SaveOnConfigSet = true;
			_infiniteEnergy        = Config.Wrap<bool>(energyConf);
			_infiniteFuel          = Config.Wrap<bool>(fuelConf);
			_infiniteResources     = Config.Wrap<bool>(resourceConf);
			_oreType               = Config.Wrap<bool>(oreConf);

			Logger.LogInfo(Config);
			Config.ConfigReloaded += UpdateConfig;
			OnEnable();
		}

		public new static ManualLogSource Logger { get; set; }
		public new static ConfigFile      Config { get; set; }

		private void UpdateConfig(object sender, EventArgs eventArgs)
		{
			if (_oreType.Value)
				_infiniteType = EResourceType.RareOre;
			else
				_infiniteType = EResourceType.CommonOre;
			ModuleManager();
		}


		public void OnEnable()
		{
			// += your hooks
			if (_infiniteEnergy.Value)
				Battery.Awake += Battery_Awake;
			if (_infiniteEnergy.Value)
				FuelTank.Awake += FuelTank_Awake;
			if (_infiniteEnergy.Value)
				ResourceTank.FixedUpdate +=
					ResourceTank_FixedUpdate;
		}

		public void ModuleManager()
		{
			if (_infiniteEnergy.Value)
				Battery.Awake += Battery_Awake;
			else
				Battery.Awake -= Battery_Awake;
			if (_infiniteEnergy.Value)
				FuelTank.Awake += FuelTank_Awake;
			else
				FuelTank.Awake -= FuelTank_Awake;
			if (_infiniteEnergy.Value)
				ResourceTank.FixedUpdate +=
					ResourceTank_FixedUpdate;
			else
				ResourceTank.FixedUpdate -=
					ResourceTank_FixedUpdate;
		}

		private void ResourceTank_FixedUpdate(
			ResourceTank.orig_FixedUpdate                                                         orig,
			Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources.ResourceTank self
		)
		{
			self.SetResourceAmount(_infiniteType, self.ResourceCapacity);
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