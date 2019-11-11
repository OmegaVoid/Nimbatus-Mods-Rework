using System;

using API;

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
		private readonly ModManager        _modManager = ModManager.Instance;
		private          ConfigEntry<bool> _infiniteEnergy;
		private          ConfigEntry<bool> _infiniteFuel;
		private          ConfigEntry<bool> _infiniteResources;
		private          EResourceType     _infiniteType = EResourceType.CommonOre;
		private          ConfigEntry<bool> _oreType;


//		public void DisablePlugin() {
//			if( !enabled )
//				return;
//			enabled = false;
//			OnDisable();
//		}
//
//		public void EnablePlugin() {
//			if( enabled )
//				return;
//			enabled = true;
//			OnEnable();
//		}

		public CheatMod()
		{
			var boolValues = new AcceptableValueList<bool>(true, false);
			Config = base.Config;
			Logger = base.Logger;
			var energyDescription   = new ConfigDescription("Enable Infinite Energy",           boolValues);
			var fuelDescription     = new ConfigDescription("Enable Infinite Fuel",             boolValues);
			var resourceDescription = new ConfigDescription("Enable Infinite Resources",        boolValues);
			var oreDescription      = new ConfigDescription("false: Common Ore\ntrue:Rare Ore", boolValues);
			var energyConf          = new ConfigDefinition("Cheats",    "Energy");
			var fuelConf            = new ConfigDefinition("Cheats",    "Fuel");
			var resourceConf        = new ConfigDefinition("Cheats",    "Resources");
			var oreConf             = new ConfigDefinition("Resources", "Ore");

			_infiniteEnergy    = Config.Bind(energyConf,   false, energyDescription);
			_infiniteFuel      = Config.Bind(fuelConf,     false, fuelDescription);
			_infiniteResources = Config.Bind(resourceConf, false, resourceDescription);
			_oreType           = Config.Bind(oreConf,      false, oreDescription);


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

			//ModuleManager();
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