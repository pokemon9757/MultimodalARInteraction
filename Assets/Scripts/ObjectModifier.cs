using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class ObjectModifier : MonoBehaviour
    {
        Dictionary<InteractableObject, bool> _selectedObjects = new(); // A dictionary of selected objects to perform actions to all
        InteractableObject _collidedObject; // Only one object can be collided at a time

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject") return;
            _collidedObject = other.GetComponent<InteractableObject>();
            _collidedObject.SetSelected(true);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject") return;
        }
    }
}