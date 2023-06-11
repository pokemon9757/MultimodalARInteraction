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
        [SerializeField] ShapePrefabKVP[] _prefabsKVP;
        [SerializeField] Vector3 _initialScale;
        [SerializeField] Material _initialMaterial;
        GameObject _createdObject;

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
            _createdObject = Instantiate(selectedPrefab);
            _createdObject.transform.position = pos;
            _createdObject.transform.localScale = _initialScale;
            var renderer = _createdObject.GetComponent<MeshRenderer>();
            renderer.material = _initialMaterial;
            var interactable = _createdObject.AddComponent<InteractableObject>();
            interactable.UpdateColor(color);
        }

    }
}