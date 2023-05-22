using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class RaycastObjectInteractor : BaseObjectInteractor
    {
        public LayerMask interactableLayer;
        public float raycastDistance = 10f;
        void Update()
        {
            // Perform a raycast in a given direction
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
            {
                // If the raycast hits an object with the tag "InteractableObject"
                if (hit.collider.gameObject.tag == "InteractableObject" && _collidedObject == null)
                {
                    _collidedObject = hit.collider.GetComponent<InteractableObject>();
                    SetObjectSelected(true);
                }
            }
            else
            {
                // If the raycast hits nothing, perform the same action as OnTriggerExit
                if (_collidedObject != null)
                {
                    SetObjectSelected(false);
                    _collidedObject = null;
                }
            }
        }
    }
}
