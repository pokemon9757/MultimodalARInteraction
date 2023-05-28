using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class ColliderObjectInteractor : BaseObjectInteractor
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "InteractableObject" && _selectedObject == null)
            {
                _selectedObject = other.GetComponent<InteractableObject>();
                SetObjectSelected(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "InteractableObject" && _selectedObject != null && _selectedObject.gameObject == other.gameObject)
            {
                SetObjectSelected(false);
                _selectedObject = null;
            }
        }
    }
}