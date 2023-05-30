using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMI
{
    public class InteractorsManager : MonoBehaviour
    {
        List<BaseObjectInteractor> _interactors;
        static InteractorsManager _instance;
        public InteractorsManager Instance
        {
            get
            {
                // Check if the instance is null
                if (_instance == null)
                {
                    // Find an existing instance in the scene
                    _instance = FindObjectOfType<InteractorsManager>();

                    // If no instance exists, create a new one
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<InteractorsManager>();
                        singletonObject.name = "InteractorsManager";
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }

        void Awake()
        {
            // Ensure that only one instance exists
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            _interactors = FindObjectsOfType<BaseObjectInteractor>().ToList();
            Debug.Log("Grabbed " + _interactors.Count + " interactors");
            // Sort interactors based on priority (highest to lowest)
            _interactors.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public InteractableObject GetSelectedObject()
        {
            foreach (var interactor in _interactors)
            {
                if (interactor.enabled && interactor.SelectedObject != null)
                {
                    // Return the selected object with highest priority
                    return interactor.SelectedObject;
                }
            }
            return null;
        }
    }
}
