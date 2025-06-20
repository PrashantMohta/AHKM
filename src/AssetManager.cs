using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AHKM
{
    /// <summary>
    /// Manages all loaded assets in the mod.
    /// </summary>
    internal static class AssetManager
    {
        internal static Dictionary<Type, Dictionary<string, Object>> _assets = new();

        /// <summary>
        /// Load all assets in any embedded asset bundles.
        /// </summary>
        internal static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                var bundle = AssetBundle.LoadFromStream(stream);
                var allAssets = bundle.LoadAllAssets();
                foreach (var asset in allAssets)
                {
                    if (!asset) continue;
                    var assetType = asset.GetType();
                    if (_assets.ContainsKey(assetType))
                    {
                        if (_assets[assetType].TryAdd(asset.name, asset))
                        {
                            Modding.Logger.LogDebug($"Added asset {asset.name} of type {assetType}");
                        }
                        else
                        {
                            Modding.Logger.LogError($"Failed to add {asset.name} of type {assetType}!");
                        }
                    }
                    else
                    {
                        if (_assets.TryAdd(assetType, new Dictionary<string, Object> { [asset.name] = asset }))
                        {
                            Modding.Logger.LogDebug($"Added new sub-dictionary of type {assetType} with initial asset {asset.name}");
                        }
                        else
                        {
                            Modding.Logger.LogError($"Failed to add new sub-dictionary of type {assetType} with initial asset {assetType} {asset.name}!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unload all saved assets.
        /// </summary>
        internal static void Unload()
        {
            foreach (var assetDict in _assets.Values)
            {
                foreach (var asset in assetDict.Values)
                {
                    Object.DestroyImmediate(asset);
                }
            }
            _assets.Clear();
            GC.Collect();
        }

        /// <summary>
        /// Fetch an asset.
        /// </summary>
        /// <param name="assetName">The name of the asset to fetch.</param>
        /// <param name="asset">The variable to output the found asset to.</param>
        /// <typeparam name="T">The type of asset to fetch.</typeparam>
        internal static bool TryGet<T>(string assetName, out T? asset) where T : Object
        {
            if (_assets.TryGetValue(typeof(T), out var subDict))
            {
                if (subDict.TryGetValue(assetName, out var assetObj))
                {
                    asset = assetObj as T;
                    return true;
                }
                Modding.Logger.LogError($"Failed to get asset {assetName}!");
                asset = null;
                return false;
            }
            Modding.Logger.LogError($"Failed to get sub-dictionary of type {typeof(T)}!");
            asset = null;
            return false;
        }

        /// <summary>
        /// Instantiate an asset as a parent's child. 
        /// </summary>
        /// <param name="assetName">The name of the asset to instantiate.</param>
        /// <param name="parent">The <see cref="Transform">transform</see> of the parent game object.</param>
        /// <typeparam name="T">The type of asset to instantiate.</typeparam>
        /// <exception cref="NullReferenceException">Thrown when an asset could not be found.</exception>
        internal static T? Inst<T>(string assetName, Transform? parent) where T : Object
        {
            if (TryGet<T>(assetName, out var asset))
            {
                var assetInst = Object.Instantiate(asset, parent);
                return assetInst;
            }
            throw new NullReferenceException($"Failed to instantiate asset {assetName} as a child of {parent.name}: asset is null!");
        }

        /// <summary>
        /// Instantiate an asset as a parent's child. 
        /// </summary>
        /// <param name="assetName">The name of the asset to instantiate.</param>
        /// <param name="position">The <see cref="Transform.position">position</see> to instantiate the asset at.</param>
        /// <typeparam name="T">The type of asset to instantiate.</typeparam>
        /// <exception cref="NullReferenceException">Thrown when an asset could not be found.</exception>
        internal static T? Inst<T>(string assetName, Vector2 position) where T : Object
        {
            if (TryGet<T>(assetName, out var asset))
            {
                var assetInst = Object.Instantiate(asset, position, Quaternion.identity);
                return assetInst;
            }
            throw new NullReferenceException($"Failed to instantiate asset {assetName} at {position}: asset is null!");
        }
    }
}
