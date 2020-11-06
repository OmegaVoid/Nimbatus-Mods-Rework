using BepInEx;

namespace API
{
	public interface NimbatusPlugin
	{
		bool GetEnabled();

		PluginInfo GetInfo();

		/// <summary>
		/// Called when Mod is Enabled
		/// </summary>
		void OnEnable();

		/// <summary>
		/// Called when Mod is Disabled
		/// </summary>
		void OnDisable();

		/// <summary>
		/// Called when Mod is Loaded
		/// </summary>
		void OnLoad();


		void Init();

		void DisablePlugin();

		void EnablePlugin();
	}
}