using UnityEngine;

namespace MMI
{
    public class CreateObjectAction : MonoBehaviour
    {
        [System.Serializable]
        public struct ShapePrefabKVP
        {
            public string ShapeName;
            public GameObject Prefab;
        }
        [SerializeField, Tooltip("The shape name and corresponding prefab to create")] ShapePrefabKVP[] _prefabsKVP;
        [SerializeField, Tooltip("The scale of created object at the start")] Vector3 _initialScale;
        [SerializeField, Tooltip("The default material of created object")] Material _initialMaterial;


        /// <summary>
        /// Create a new game object with given params
        /// </summary>
        /// <param name="pos">World Position</param>
        /// <param name="color">Color</param>
        /// <param name="shapeName">Shape to create</param>
        public void CreateNewObject(Vector3 pos, Color color, string shapeName)
        {
            // Get prefab based on shapeName
            GameObject selectedPrefab = null;
            foreach (ShapePrefabKVP kvp in _prefabsKVP)
            {
                if (kvp.ShapeName.ToLower() == shapeName.ToLower())
                {
                    selectedPrefab = kvp.Prefab;
                    break;
                }
            }
            if (selectedPrefab == null)
            {
                Debug.LogError("Cannot find what you are looking for with ... " + shapeName);
                return;
            }
            GameObject _createdObject = Instantiate(selectedPrefab);
            _createdObject.transform.position = pos;
            _createdObject.transform.localScale = _initialScale;
            var renderer = _createdObject.GetComponent<MeshRenderer>();
            renderer.material = _initialMaterial;
            var interactable = _createdObject.AddComponent<InteractableObject>();
            interactable.UpdateColor(color);
        }

    }
}