using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
namespace MMI
{
    [RequireComponent(typeof(Rigidbody), typeof(FlashingMaterial), typeof(ObjectManipulator))]
    [RequireComponent(typeof(MinMaxScaleConstraint))]
    public class InteractableObject : MonoBehaviour
    {
        private MeshRenderer[] _renderers;
        private FlashingMaterial _materialFlash;
        private Rigidbody _rb;

        void Start()
        {
            Init();
        }

        void Init()
        {
            gameObject.tag = "InteractableObject";
            _renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            _materialFlash = GetComponent<FlashingMaterial>();
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        public void UpdateColor(Color c)
        {
            if (_renderers == null || _renderers.Length == 0) _renderers = GetComponentsInChildren<MeshRenderer>();
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
            Destroy(GetComponent<MinMaxScaleConstraint>());
            Destroy(GetComponent<ObjectManipulator>());
            Destroy(GetComponent<Rigidbody>());
        }
    }
}