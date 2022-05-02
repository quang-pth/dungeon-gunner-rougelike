using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PoolManager : SingletonMonobehavior<PoolManager>
{
    #region Tooltip
    [Tooltip("Populate this array with prefabs you want to add to the pool, and specify the number of gameobjects to be created for each")]
    #endregion
    [SerializeField] private Pool[] poolArray = null;
    private Transform objectPoolTransform;
    private Dictionary<int, Queue<Component>> poolDictionary = new Dictionary<int, Queue<Component>>();

    [System.Serializable]
    public struct Pool {
        public int poolSize; // number of pool object components
        public GameObject prefab; // root gameobject prefab
        public string componentType; // primary component
    }

    private void Start() {
        objectPoolTransform = this.gameObject.transform;

        for (int i = 0; i < poolArray.Length; i++) {
            CreatePool(poolArray[i].prefab, poolArray[i].poolSize, poolArray[i].componentType);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize, string componentType) {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;
        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform); // Set PoolManager gameobject as the parent of this new parent gameobject

        if (!poolDictionary.ContainsKey(poolKey)) {
            poolDictionary.Add(poolKey, new Queue<Component>());

            // Enqueue the gameobject components
            for (int i = 0; i < poolSize; i++) {
                GameObject newObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newObject.SetActive(false);
                poolDictionary[poolKey].Enqueue(newObject.GetComponent(Type.GetType(componentType)));
            }
        }
    }

    public Component ReuseComponent(GameObject prefab, Vector3 position /*world position*/, Quaternion rotation) {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey)) {
            Component componentToReuse = GetComponentFromPool(poolKey);

            // Reset object to a new position as well as adjust other transformation properties
            ResetObject(position, rotation, componentToReuse, prefab);

            return componentToReuse;
        } 
        else {
            Debug.Log("Not object pool for " + prefab);
            return null;
        }
    }

    private Component GetComponentFromPool(int poolKey) {
        Component componentToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(componentToReuse);

        if (componentToReuse.gameObject.activeSelf == true) {
            componentToReuse.gameObject.SetActive(false);
        }

        return componentToReuse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, Component componentToReuse, GameObject prefab) {
        componentToReuse.transform.position = position;
        componentToReuse.transform.rotation = rotation;
        componentToReuse.gameObject.transform.localScale = prefab.transform.localScale;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(poolArray), poolArray);
    }
#endif
    #endregion
}