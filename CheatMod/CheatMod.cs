﻿using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.Batteries;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePartResources;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks;

using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;

namespace CheatMod
{
	[BepInPlugin("codes.omegavoid.nimbatus-mods.cheatmod", "Cheat Mod", "3.0.0.0")]
	public class CheatMod : BaseUnityPlugin
	{
		private static ConfigEntry<bool> _configInfiniteEnergy;
		private static ConfigEntry<bool> _configInfiniteFuel;
		private static ConfigEntry<bool> _configInfiniteResources;


		public void Awake()
		{
			_configInfiniteEnergy = Config.Bind("Cheats", "Energy", false, "Enable Infinite Energy");

			_configInfiniteFuel      = Config.Bind("Cheats", "Fuel",      false, "Enable Infinite Fuel");
			_configInfiniteResources = Config.Bind("Cheats", "Resources", false, "Enable Infinite Resources");


			Harmony.CreateAndPatchAll(typeof(CheatMod));
		}

		[HarmonyPatch(typeof(FuelTank), "Awake")]
		[HarmonyPrefix]
		private static bool InfiniteFuelAwake(

			// ReSharper disable InconsistentNaming
			ref float ___CurrentFuelAmount,
			ref float ___MaxFuelAmount,
			ref float ___RechargePerSecond

			// ReSharper restore InconsistentNaming
		)
		{
			if (!_configInfiniteFuel.Value)
				return true;
			___CurrentFuelAmount = 1E+19f;
			___MaxFuelAmount     = 1E+19f;
			___RechargePerSecond = 1E+19f;


			return true;
		}

		[HarmonyPatch(typeof(Battery), "Awake")]
		[HarmonyPrefix]
		private static bool InfiniteEnergyAwake(

			// ReSharper disable InconsistentNaming
			ref float ___CurrentEnergyAmount,
			ref float ___MaxEnergyAmount,
			ref float ___RechargePerSecond

			// ReSharper restore InconsistentNaming
		)
		{
			if (!_configInfiniteEnergy.Value)
				return true;
			___MaxEnergyAmount     = 1E+19f;
			___CurrentEnergyAmount = 1E+19f;
			___RechargePerSecond   = 1E+19f;


			return true;
		}

		[HarmonyPatch(typeof(ResourceTank), "GetRechargePerSecond")]
		[HarmonyPrefix]
		private static bool InfiniteResources(

			// ReSharper disable InconsistentNaming
			float     ___ResourceCapacity,
			ref float __result

			// ReSharper restore InconsistentNaming
		)
		{
			if (!_configInfiniteResources.Value)
				return true;
			__result = ___ResourceCapacity;


			return false;
		}
	}
}