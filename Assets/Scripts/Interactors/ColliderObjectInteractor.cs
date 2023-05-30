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
            if (other.gameObject.tag == "InteractableObject")
            {
                _selectedObject = other.GetComponent<InteractableObject>();
                InteractorsManager.Instance.TriggerSelectObject(_selectedObject, true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "InteractableObject")
            {
                InteractorsManager.Instance.TriggerSelectObject(_selectedObject, false);
                _selectedObject = null;
            }
        }
    }
}