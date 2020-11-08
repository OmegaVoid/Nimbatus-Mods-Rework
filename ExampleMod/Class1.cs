using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.FuelTanks;

using BepInEx;
using BepInEx.Configuration;

using HarmonyLib;


// this is an Example Mod, use this to learn how to make a mod or as a template for your mod
namespace ExampleMod
{
	[BepInPlugin("codes.omegavoid.nimbatus-mods.examplemod", "Example Mod", "0.0.0.0")]
	public class ExampleMod : BaseUnityPlugin
	{
		private static ConfigEntry<bool>   _configInfiniteFuel;
		private        ConfigEntry<bool>   _configDisplayGreeting;
		private        ConfigEntry<string> _configGreeting;

		public void Awake()
		{
			_configGreeting = Config.Bind("General", // The section under which the option is shown
										  "GreetingText", // The key of the configuration option in the configuration file
										  "Hello, world!", // The default value
										  "A greeting text to show when the game is launched"); // Description of the option to show in the config file

			_configDisplayGreeting = Config.Bind("General.Toggles",
												 "DisplayGreeting",
												 true,
												 "Whether or not to show the greeting text");

			_configInfiniteFuel = Config.Bind("Patch.Toggles",
											  "InfiniteFuel",
											  false,
											  "Enable Infinite Fuel");
			if (_configDisplayGreeting.Value)
				Logger.LogInfo(_configGreeting.Value);


			var myLogSource = BepInEx.Logging.Logger.CreateLogSource("MyLogSource");
			myLogSource.LogInfo("Test");
			BepInEx.Logging.Logger.Sources.Remove(myLogSource);


			Harmony.CreateAndPatchAll(typeof(ExampleMod));
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


			return true; // Returning false in prefix patches skips running the original code
		}
	}
}