using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMI
{
    public class InteractorsManager : MonoBehaviour
    {
        InteractableObject _selectedObject;
        public InteractableObject SelectedObject { get { return _selectedObject; } }
        static InteractorsManager _instance;
        public static InteractorsManager Instance
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
        /// <summary>
        /// Trigger the change of selected object  
        /// </summary>
        public void TriggerSelectObject(InteractableObject obj, bool active)
        {
            if (_selectedObject != obj)
            {
                if (!active) return;
                _selectedObject?.SetSelected(false);
                _selectedObject = obj;
                _selectedObject.SetSelected(true);
                return;
            }

            if (active) return;
            _selectedObject.SetSelected(false);
            _selectedObject = null;
        }
    }
}
