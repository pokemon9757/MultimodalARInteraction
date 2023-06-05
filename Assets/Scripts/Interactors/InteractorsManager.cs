using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MMI
{
    public class InteractorsManager : MonoBehaviour
    {
        public InteractableObject SelectedObject { get { return _selectedObject; } }
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
                _lastSelectedObject = _selectedObject == null ? _lastSelectedObject : _selectedObject;
                _selectedObject = obj;
                _selectedObject.SetSelected(true);
                return;
            }

            // Else  _selectedObject == obj
            // Trying to select an already selected object
            if (active) return;
            // Deselect current object
            _selectedObject?.SetSelected(false);
            _lastSelectedObject = _selectedObject;
            _selectedObject = null;
        }

        public void StartGrouping()
        {
            if (_isInGroupMode)
            {
                Debug.LogWarning("Already started grouping, cannot start group");
                return;
            }
            _isInGroupMode = true;
        }

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
            if (active)
            {
                obj.UpdateColor(_selectedColor);
                _objectsToGroupDict[obj] = active;
            }
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

        public void ChangeSelectedObjectColor(string colorName)
        {
            var obj = _selectedObject == null ? _selectedObject : _lastSelectedObject;
            if (obj == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            Color color;
            if (!ColorUtility.TryParseHtmlString(colorName.ToLower(), out color))
            {
                Debug.LogError("Invalid color name " + colorName);
                color = Color.gray;
            }
            obj.UpdateColor(color);
        }

        public void ScaleSelectedObject(string percentageString, bool scaleUp)
        {
            var obj = _selectedObject == null ? _selectedObject : _lastSelectedObject;
            if (obj == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            int percent = Int32.Parse(percentageString);
            _selectedObject.transform.localScale *= scaleUp ? (percent + 100) / 100 : (100 - percent) / 100;
        }
    }
}
