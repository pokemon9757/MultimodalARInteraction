using System.Security.Cryptography;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

namespace MMI
{
    public class ObjectModifier : MonoBehaviour
    {
        public void OnTriggerEnterAction(InteractableObject interactableObject)
        {
            Debug.Log("On Trigger enter action " + name);
        }

        public void OnTriggerExitAction(InteractableObject interactableObject)
        {
            Debug.Log("On Trigger exit action " + name);
        }
    }
}