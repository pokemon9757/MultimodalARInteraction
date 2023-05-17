using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MMI
{
    public class ObjectModifier : MonoBehaviour
    {
        InteractableObject _collidedObject; // Only one object can be collided at a time

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject" || _collidedObject != null) return;
            _collidedObject = other.GetComponent<InteractableObject>();
            _collidedObject.SetSelected(true);
        }


        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject") return;
            if (_collidedObject == null || _collidedObject.gameObject != other.gameObject) return;

            _collidedObject.SetSelected(false);
            _collidedObject = null;
        }
    }
}