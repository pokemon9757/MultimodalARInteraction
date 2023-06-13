using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class RaycastObjectInteractor : BaseObjectInteractor
    {
        /// <summary>
        /// Perform Raycast and trigger InteractorManager.TriggerSelect object if an Interactable object is hit  
        /// </summary>
        /// <param name="origin">The origin of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="distance">How far the ray can go</param>
        public void PerformRaycast(Vector3 origin, Vector3 direction, float distance)
        {
            // Perform a raycast in a given direction
            Ray ray = new Ray(origin, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance))
            {
                // If the raycast hits an object with the tag "InteractableObject"
                if (hit.collider.gameObject.tag == "InteractableObject")
                {
                    _selectedObject = hit.collider.GetComponent<InteractableObject>();
                    // Might have been grouped
                    if (!_selectedObject) _selectedObject = hit.collider.gameObject.GetComponentInParent<InteractableObject>();
                    if (!_selectedObject) return;
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
