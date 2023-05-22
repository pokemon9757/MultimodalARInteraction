using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
namespace MMI
{
    [RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable), typeof(FlashingMaterial))]
    public class InteractableObject : MonoBehaviour
    {
        private MeshRenderer[] _renderers;
        private FlashingMaterial _materialFlash;
        private XRGrabInteractable _grab;
        private Rigidbody _rb;

        void Start()
        {
            Init();
        }

        void Init()
        {
            _renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            _materialFlash = GetComponent<FlashingMaterial>();
            _grab = GetComponent<XRGrabInteractable>();
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        public void UpdateColor(Color c)
        {
            if (_renderers == null) _renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (Renderer r in _renderers)
            {
                r.material.color = c;
                r.material.SetColor("_EmissionColor", c);
            }
        }

        public void SetSelected(bool active)
        {
            _materialFlash.EnableFlashing(active);
        }


        void OnDestroy()
        {
            Destroy(GetComponent<FlashingMaterial>());
            Destroy(GetComponent<XRGrabInteractable>());
            Destroy(GetComponent<Rigidbody>());
        }
    }
}