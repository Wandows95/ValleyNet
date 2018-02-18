namespace ValleyNet.Core.Asset
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Networking;
    
    public static class AssetManager
    {
        public static void RegisterPrefab(GameObject prefab)
        {
            ClientScene.RegisterPrefab(prefab);
        }

        public static void RegisterPrefabs(GameObject[] prefabs)
        {
            for(int i = 0; i < prefabs.Length; i++)
            {
                ClientScene.RegisterPrefab(prefabs[i]);
            }
        }
    }
}