using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
namespace MMI
{
    [RequireComponent(typeof(FlashingMaterial), typeof(ObjectManipulator))]
    [RequireComponent(typeof(MinMaxScaleConstraint))]
    public class InteractableObject : MonoBehaviour
    {
        private MeshRenderer[] _renderers;
        private FlashingMaterial _materialFlash;
        private Dictionary<MeshRenderer, Color> _cachedColorsDict = new();
        void Awake()
        {
            gameObject.tag = "InteractableObject";
            _renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var r in _renderers)
            {
                if (r.gameObject.GetComponent<NearInteractionGrabbable>() == null)
                    r.gameObject.AddComponent<NearInteractionGrabbable>();
            }
            _materialFlash = GetComponent<FlashingMaterial>();
        }

        /// <summary>
        /// Update the material color of all renderers in the object, cache the previous colors into a dict for recovery 
        /// </summary>
        /// <param name="c">New color</param>
        public void UpdateColor(Color c)
        {
            SetSelected(false);
            foreach (MeshRenderer r in _renderers)
            {
                _cachedColorsDict[r] = r.material.color;
                r.material.color = c;
                r.material.SetColor("_EmissionColor", c);
            }
        }

        /// <summary>
        /// Recover the material color of all renderers to the cached dict
        /// </summary>
        public void RecoverUpdatedColors()
        {
            SetSelected(false);
            if (_cachedColorsDict == null || _cachedColorsDict.Count == 0)
            {
                Debug.LogError("Cannot recover updated color as dict is empty");
                return;
            }

            foreach (MeshRenderer r in _renderers)
            {
                Color c = _cachedColorsDict[r];
                r.material.color = c;
                r.material.SetColor("_EmissionColor", c);
            }
        }

        /// <summary>
        /// Enable flashing of the object to indicate that it's being hovered by a pointer
        /// </summary>
        /// <param name="active">Enable if True, Disable if False</param>
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