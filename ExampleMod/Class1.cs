using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

// this is an Example Mod, use this to learn how to make a mod or as a template for your mod
namespace ExampleMod
{
	[BepInPlugin("ExampleMod", "Example Mod", "0.0.0.0")]
	public class ExampleMod : BaseUnityPlugin
	{
		private ConfigWrapper<bool> _example;


		public ExampleMod()
		{
			Config = base.Config;
			Logger = base.Logger;
			var exampleConf = new ConfigDefinition("Example Section", "Example Key", "Example Description");


			Config.SaveOnConfigSet = true;
			_example               = Config.Wrap<bool>(exampleConf);
			Logger.LogMessage("Example");
			OnEnable();
		}

		public new static ManualLogSource Logger { get; set; }
		public new static ConfigFile      Config { get; set; }

		public void OnEnable()
		{
			// += your hooks
		}

		public void OnDisable()
		{
			// -= your hooks
		}
	}
}