using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
namespace MMI
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable), typeof(FlashingMaterial))]
    public class InteractableObject : MonoBehaviour
    {
        private MeshRenderer[] renderers;
        private FlashingMaterial flashing;
        private XRGrabInteractable grab;
        private Rigidbody rb;
        private bool initialized = false;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            if (initialized) return;
            initialized = true;
            renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            flashing = GetComponent<FlashingMaterial>();
            grab = GetComponent<XRGrabInteractable>();
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            grab.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            ObjectModifier mod = other.gameObject.GetComponent<ObjectModifier>();
            if (mod != null)
            {
                mod.OnTriggerEnterAction(this);
            }
        }

        void OnTriggerExit(Collider other)
        {
            ObjectModifier mod = other.gameObject.GetComponent<ObjectModifier>();
            if (mod != null)
            {
                mod.OnTriggerExitAction(this);
            }
        }

        public void UpdateColor(Color c)
        {
            foreach (Renderer r in renderers)
            {
                r.material.color = c;
                r.material.SetColor("_EmissionColor", c);
            }
        }

        public void SetSelected(bool active)
        {
            flashing.EnableFlashing(active);
            grab.enabled = active;
        }

        void OnDestroy()
        {
            Destroy(GetComponent<FlashingMaterial>());
            Destroy(GetComponent<XRGrabInteractable>());
            Destroy(GetComponent<Rigidbody>());
        }
    }
}