using System;
using System.Collections.Generic;
using System.IO;

using Assets.Nimbatus.GUI.MainMenu.Scripts;
using Assets.Nimbatus.Scripts.Persistence.SaveSystem;
using Assets.Nimbatus.Scripts.WorldObjects.Items;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts;
using Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.SensorParts;

using BepInEx;

using I2.Loc;

using MonoMod;

using UnityEngine;

using Object = UnityEngine.Object;

// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global

#pragma warning disable CS0626
#pragma warning disable CS0649
#pragma warning disable CS0108
namespace API
{
	[MonoModPatch("Assets.Nimbatus.GUI.MainMenu.Scripts.ShowVersionNumber")]

	// ReSharper disable once InconsistentNaming
	internal class patch_ShowVersionNumber : ShowVersionNumber
	{
		[NonSerialized]
		public int labelSizeAdd;

		public void Start()
		{
			// labelSizeAdd = 10;
			// Label.SetDimensions(Label.width + labelSizeAdd, Label.height + labelSizeAdd);

			//this.Mod = new OmegaModLoader();
			Debug.Log("Running OmegaMod");

			// this.Mod.Startup();
			//Label.gameObject.AddComponent<ModConfigurator>();
		}


		public void Update()
		{
			Label.text = LocalizationManager.GetTranslation("MainMenu/Version") +
						 SaveManager.CurrentGameVersion                         + " Modded";
		}

		// public OmegaModLoader Mod;
		// public extern void orig_Update();
	}

	#region API

	[MonoModPatch("Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.DronePart")]
	public abstract class patch_DronePart : DronePart
	{
	}

	[MonoModPatch("Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.BindableDronePart")]

	// ReSharper disable once InconsistentNaming
	internal abstract class patch_BindableDronePart : BindableDronePart
	{
		[MonoModIgnore]
		internal List<KeyBinding> KeyBindings;

		// Adds KeyBindings to a BindableDronePart
		/// <summary>
		///     Adds any number of KeyBindings to a BindableDronePart
		/// </summary>
		/// <param name="keys">The KeyBindings to Add</param>
		public virtual void AddKeyBindings(params KeyBinding[] keys)
		{
			KeyBindings.AddRange(keys);
		}

		// Removes KeyBindings from a BindableDronePart
		/// <summary>
		///     Removes any number of KeyBindings from a BindableDronePart
		/// </summary>
		/// <param name="keys">The KeyBindings to Remove</param>
		public virtual void RemoveKeyBindings(params KeyBinding[] keys)
		{
			foreach (var key in keys)
				KeyBindings.Remove(key);
		}
	}

	[MonoModPatch("Assets.Nimbatus.Scripts.WorldObjects.Items.DroneParts.SensorParts.SensorPart")]

	// ReSharper disable once InconsistentNaming
	internal abstract class patch_SensorPart : SensorPart
	{
		[MonoModIgnore]
		internal List<KeyBinding> EventBindings;

		// Adds EventBindings to a SensorPart
		/// <summary>
		///     Adds any number of EventBindings to a SensorPart
		/// </summary>
		/// <param name="keys">The EventBindings to Add</param>
		public virtual void AddEventBindings(params KeyBinding[] keys)
		{
			EventBindings.AddRange(keys);
		}

		// Removes EventBindings from a SensorPart
		/// <summary>
		///     Removes any number of EventBindings from a SensorPart
		/// </summary>
		/// <param name="keys">The EventBindings to Remove</param>
		public virtual void RemoveEventBindings(params KeyBinding[] keys)
		{
			foreach (var key in keys)
				EventBindings.Remove(key);
		}
	}

#if FALSE
	[MonoModPatch("Assets.Nimbatus.Scripts.WorldObjects.Items.ItemManager")]

	// ReSharper disable once InconsistentNaming
	internal class patch_ItemManager : ItemManager
	{
		public List<NimbatusItem> ModItemPrefabs { get; set; }

		public void AddItems(params NimbatusItem[] Items)
		{
			foreach (NimbatusItem nimbatusItem in Items)
			{
				nimbatusItem.InitStackSettings();
				nimbatusItem.InitDronePerkSettings(SerializableMonobehaviour<DronePerkManager, DronePerkManagerData>
												  .Instance.ActivePerk);
				var dynItem = new DynData<NimbatusItem>(nimbatusItem);
				if (SaveGameManager.CurrentGameSettings.HasPartUnlocking)
				{
					dynItem.Set("Unlocked", true);

					if (nimbatusItem.DoNotImport || dynItem.Get<bool>("IsStackable"))
						dynItem.Set("Unlocked", false);
					if (nimbatusItem is WeaponAttributeUpgrade)
						dynItem.Set("Unlocked", nimbatusItem.AlwaysUnlocked);
					dynItem.Set("UnlimitedStackSize", !dynItem.Get<bool>("IsStackable"));
					dynItem.Set("CurrentStackSize",   0);
				}
				else if (nimbatusItem.DoNotImport)
				{
					dynItem.Set("Unlocked", false);
				}
				else
				{
					dynItem.Set("Unlocked",
								!(nimbatusItem is WeaponAttributeUpgrade) ||
								RuntimeGlobals.GameModeSettings.AllTechnologyUnlocked);
					if (nimbatusItem.AlwaysUnlocked)
						dynItem.Set("Unlocked", true);
					dynItem.Set("UnlimitedStackSize", true);
				}

				ModItemPrefabs.Add(nimbatusItem);
				ItemPrefabs.AddRange(ModItemPrefabs);
			}
		}
	}

#endif

	#endregion

	public static class AssetBundleModule
	{
		public static Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>();
		public static Dictionary<string, object>      Cache        = new Dictionary<string, object>();

		// Loads a new AssetBundle
		/// <summary>
		///     Loads an AssetBundle into the AssetBundles Dictionary
		/// </summary>
		/// <param name="name">The Name of the AssetBundle to Load</param>
		/// <exception cref="FileLoadException">Thrown when the AssetBundle cannot be found</exception>
		public static void Load(string name)
		{
			var myLoadedAssetBundle =
				AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
			if (myLoadedAssetBundle == null)
			{
				Debug.Log("Failed to load AssetBundle!");
				throw new FileLoadException("Failed to load AssetBundle");
			}

			AssetBundles.Add(name, myLoadedAssetBundle);
		}

		public static T LoadAssetFrom<T>(string name, string assetName) where T : Object
		{
			if (!AssetBundles.ContainsKey(name))
				throw new InvalidOperationException("AssetBundle is not loaded");

			object result;
			if (!Cache.TryGetValue(assetName, out result))
			{
				result = AssetBundles[name].LoadAsset<T>(assetName);
				Cache.Add(assetName, result);
			}


			return (T) result;
		}

		// Unloads an AssetBundle
		/// <summary>
		///     Unloads an AssetBundle from The AssetBundles Dictionary
		/// </summary>
		/// <param name="name">The Name of the AssetBundle to Unload</param>
		/// <param name="unloadAllLoadedObjects"></param>
		/// <exception cref="System.NullReferenceException">
		///     Thrown when there is no AssetBundle with the Specified Name in the
		///     AssetBundles Dictionary
		/// </exception>
		public static void Unload(string name, bool unloadAllLoadedObjects)
		{
			if (AssetBundles[name] == null)
			{
				Debug.Log("Failed to unload AssetBundle!");
				throw new NullReferenceException("Failed to unload AssetBundle");
			}

			AssetBundles[name].Unload(unloadAllLoadedObjects);
		}

		// Loads a Prefab from a loaded AssetBundle
		/// <summary>
		///     Loads a Prefab from a loaded AssetBundle in the AssetBundles Dictionary
		/// </summary>
		/// <param name="name">The Name of the AssetBundle to Load from</param>
		/// <param name="assetName">The Name of the Prefab to Load</param>
		/// <returns>The Prefab</returns>
		/// <exception cref="System.TypeLoadException">Thrown when the Prefab doesnt exist</exception>
		public static GameObject LoadPrefabFrom(string name, string assetName)
		{
			var prefab = LoadAssetFrom<GameObject>(name, assetName);
			if (prefab == null)
				throw new TypeLoadException("Failed to load Prefab");

			var script = prefab.GetComponent<NimbatusItemPlaceholder>();
			if (prefab.GetComponent<NimbatusItemPlaceholder>())
			{
				prefab.AddComponent(typeof(NimbatusItem));
				prefab.GetComponent<NimbatusItem>().Icon           = script.Icon;
				prefab.GetComponent<NimbatusItem>().AlwaysUnlocked = script.AlwaysUnlocked;
				prefab.GetComponent<NimbatusItem>().DoNotImport    = script.DoNotImport;
			}


			return prefab;
		}
	}


	public static class FolderStructure
	{
		public static readonly string RootFolder = Paths.BepInExRootPath; //Application.dataPath;

		//public static readonly string DataFolder   = Path.Combine(RootFolder, "OmegaData");
		//public static readonly string ModsFolder   = Path.Combine(DataFolder, "Mods");
		public static readonly string ConfigFolder = Paths.ConfigPath;
		public static readonly string AssetsFolder = Path.Combine(RootFolder, "Assets");

		static FolderStructure()
		{
			if (!Directory.Exists(AssetsFolder))
				Directory.CreateDirectory(AssetsFolder);
		}
	}


//	public class ModManager
//	{
//		private static readonly Lazy<ModManager>
//			lazy =
//				new Lazy<ModManager>(() => new ModManager());
//
//		public Dictionary<string, NimbatusPlugin> NimbatusPlugins = new Dictionary<string, NimbatusPlugin>();
//
//		private ModManager()
//		{
//		}
//
//		public static ModManager Instance
//		{
//			get { return lazy.Value; }
//		}
//
//
//		public void Init()
//		{
//			foreach (var plugin in NimbatusPlugins)
//			{
//				plugin.Value.Init();
//				plugin.Value.OnLoad();
//				EnablePlugin(plugin.Key);
//			}
//		}
//
//		public void Register(NimbatusPlugin plugin)
//		{
//			NimbatusPlugins.Add(plugin.GetInfo().Metadata.GUID, plugin);
//		}
//
//		public void DisablePlugin(string pluginID)
//		{
//			if (NimbatusPlugins.ContainsKey(pluginID))
//				DisablePlugin(NimbatusPlugins[pluginID]);
//		}
//
//		public void DisablePlugin(NimbatusPlugin plugin)
//		{
//			plugin.DisablePlugin();
//		}
//
//		public void EnablePlugin(string pluginID)
//		{
//			if (NimbatusPlugins.ContainsKey(pluginID))
//				EnablePlugin(NimbatusPlugins[pluginID]);
//		}
//
//		public void EnablePlugin(NimbatusPlugin plugin)
//		{
//			plugin.EnablePlugin();
//		}
//	}
}