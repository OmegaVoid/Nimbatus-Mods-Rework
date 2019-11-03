using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace OmegaBlocks
{
	[BepInPlugin("OmegaBlocks", "OmegaBlocks", "1.0.0")]
	public class OmegaBlocks : BaseUnityPlugin
	{
		public OmegaBlocks()
		{
			Config = base.Config;
			Logger = base.Logger;

			OnEnable();
		}

		public new static ManualLogSource Logger { get; set; }
		public new static ConfigFile      Config { get; set; }


		public void OnLoad()
		{
		}

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