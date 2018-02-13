namespace ValleyNet.Core.Component.Entity
{
    using UnityEngine;


    public class PrefabSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        public float spawnFloor;


        void Start()
        {
            spawnFloor = transform.position.y;
        }


        // Trigger prefab spawn
        void Spawn()
        {
            if(_prefab != null)
            {
                Instantiate(_prefab, new Vector3(transform.position.x, spawnFloor, transform.position.z), Quaternion.identity);
            }
        }
    }
}