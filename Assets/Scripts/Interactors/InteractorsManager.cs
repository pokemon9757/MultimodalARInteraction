using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMI
{
    public class InteractorsManager : MonoBehaviour
    {
        // Get the currently or lastly selected object
        public InteractableObject GetSelectedObject
        {
            get
            {
                return _selectedObject ? _selectedObject : _lastSelectedObject;
            }
        }
        public Color _selectedColor = Color.blue;
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
        static InteractorsManager _instance;
        InteractableObject _selectedObject;
        InteractableObject _lastSelectedObject;
        Dictionary<InteractableObject, bool> _objectsToGroupDict = new();
        Dictionary<InteractableObject, Color> _objectsOriginalColor = new();
        bool _isInGroupMode = false;

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
            if (obj == null) return;
            if (_selectedObject != obj)
            {
                // Trying to disable a not selected object
                if (!active) return;

                // Set active a new selected object
                _selectedObject?.SetSelected(false);
                _selectedObject = obj;
                _selectedObject.SetSelected(true);
                return;
            }

            // Else  _selectedObject == obj
            // Trying to select an already selected object
            if (active) return;
            // Deselect current object
            if (_selectedObject != null)
            {
                _selectedObject.SetSelected(false);
                if (_lastSelectedObject != _selectedObject) _lastSelectedObject = _selectedObject;
                _selectedObject = null;
            }
        }

        /// <summary>
        /// Start grouping mode
        /// </summary>
        public void StartGrouping()
        {
            if (_isInGroupMode)
            {
                Debug.LogWarning("Already started grouping, cannot start group");
                return;
            }
            _isInGroupMode = true;
        }

        /// <summary>
        /// End grouping mode, group all selected object
        /// </summary>
        public void CompleteGrouping()
        {
            if (!_isInGroupMode)
            {
                Debug.LogWarning("Haven't started grouping, cannot complete group ");
                return;
            }
            _isInGroupMode = false;

            // Perform grouping 
            if (_objectsToGroupDict.Count == 0) return;
            bool setParentPos = false;
            Transform parent = new GameObject("Grouped Objects").transform;
            foreach (InteractableObject obj in _objectsToGroupDict.Keys)
            {
                if (!obj.gameObject.activeSelf) continue;
                obj.RecoverUpdatedColors();
                Destroy(obj.GetComponent<InteractableObject>());

                // Set the parent transform to the first child
                if (!setParentPos)
                {
                    parent.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
                    parent.localScale = obj.transform.localScale;
                    setParentPos = true;
                }
                obj.transform.parent = parent;
            }
            parent.gameObject.AddComponent<InteractableObject>();
            _objectsToGroupDict.Clear();
            _selectedObject = null;
            _lastSelectedObject = null;
        }

        /// <summary>
        /// Add the currently selected object to a list of objects to group once the CompleteGroup function is called 
        /// </summary>
        /// <param name="active">Add the object if True, otherwise Remove  </param>
        public void SelectObjectToGroup(bool active)
        {
            string action = active ? "select" : "deselect";

            if (!_isInGroupMode)
            {
                Debug.LogError("Cannot perform " + action + ", not in group mode");
                return;
            }

            // Take the last selected object if the currently selected is null
            InteractableObject obj = _selectedObject ? _selectedObject : _lastSelectedObject;
            if (obj == null)
            {
                Debug.LogWarning("Cannot perform " + action + " since no object has been recorded");
                return;
            }

            // If selecting
            if (active)
            {
                obj.UpdateColor(_selectedColor);
                _objectsToGroupDict[obj] = active;
            }
            // Else if deselecting
            else
            {
                // Recover to the original color if the dict contains the obj
                if (_objectsToGroupDict.ContainsKey(obj))
                {
                    obj.RecoverUpdatedColors();
                    _objectsToGroupDict.Remove(obj);
                }
            }
        }

        /// <summary>
        /// Change the material color of the currently selected object
        /// </summary>
        /// <param name="color">The new color to change into</param>
        public void ChangeSelectedObjectColor(Color color)
        {
            var obj = GetSelectedObject;
            if (obj == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            if (_isInGroupMode && _objectsToGroupDict.ContainsKey(obj))
            {
                Debug.LogError("Cannot change color of a selected object in group mode");
                return;
            }
            obj.UpdateColor(color);
        }

        /// <summary>
        /// Scale the gameobject by the given percentage
        /// </summary>
        /// <param name="percentageString">A number string</param>
        /// <param name="scaleUp">Scale up if True, otherwise scale down</param>
        public void ScaleSelectedObject(int percent, bool scaleUp)
        {
            var obj = GetSelectedObject;
            if (obj == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            Vector3 origScale = obj.transform.localScale;
            obj.transform.localScale = scaleUp ? origScale + (origScale * percent / 100f) : origScale - (origScale * percent / 100f);
        }
    }
}
