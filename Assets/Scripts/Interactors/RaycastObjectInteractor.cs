using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class RaycastObjectInteractor : BaseObjectInteractor
    {
        [SerializeField] float _raycastDistance = Mathf.Infinity;
        [SerializeField] EyeTracking _eyeTracking;
        void Update()
        {
            // Perform a raycast in a given direction
            Ray ray = new Ray(_eyeTracking.GazeOrigin, _eyeTracking.GazeDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _raycastDistance))
            {
                // If the raycast hits an object with the tag "InteractableObject"
                if (hit.collider.gameObject.tag == "InteractableObject")
                {
                    _selectedObject = hit.collider.GetComponent<InteractableObject>();
                    InteractorsManager.Instance.TriggerSelectObject(_selectedObject, true);
                }
            }
            else
            {
                // If the raycast hits nothing, perform the same action as OnTriggerExit
                if (_selectedObject != null)
                {
                    InteractorsManager.Instance.TriggerSelectObject(_selectedObject, false);
                    _selectedObject = null;
                }
            }
        }
    }
}
