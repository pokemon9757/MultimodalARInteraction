using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MMI
{
    public class FlashingMaterial : MonoBehaviour
    {
        [SerializeField] float _minIntensity = -0.5f;
        [SerializeField] float _maxIntensity = 1.5f;
        [SerializeField] float _transitionTime = 1f;
        bool _isActive = false;
        float _timer = 0f;
        bool _tweeningForward = true;
        Dictionary<MeshRenderer, Color> _renderersOrigColors = new Dictionary<MeshRenderer, Color>();
        MeshRenderer[] _renderers;

        // Update is called once per frame
        void Update()
        {
            if (_isActive)
            {
                foreach (KeyValuePair<MeshRenderer, Color> entry in _renderersOrigColors)
                {
                    float emissiveIntensity = Mathf.Lerp(_minIntensity, _maxIntensity, (float)_timer / _transitionTime);
                    entry.Key.material.color = entry.Value * emissiveIntensity;
                    entry.Key.material.SetColor("_EmissionColor", entry.Value * emissiveIntensity);
                }
                _timer += _tweeningForward ? Time.deltaTime : -Time.deltaTime;
                if (_timer <= 0 || _timer >= _transitionTime)
                {
                    _timer = _timer <= 0 ? 0 : _transitionTime;
                    _tweeningForward = !_tweeningForward;
                }
            }
        }

        public void EnableFlashing(bool active)
        {
            if (_renderers == null || _renderers.Length == 0) _renderers = GetComponentsInChildren<MeshRenderer>();
            // Enabling Flash
            if (!_isActive)
            {
                // Store the renderers original colors
                foreach (MeshRenderer m in _renderers)
                {
                    _renderersOrigColors[m] = m.material.color;
                }
            }
            _isActive = active;
            // Disabling flash
            if (!active)
                foreach (KeyValuePair<MeshRenderer, Color> entry in _renderersOrigColors)
                {
                    entry.Key.material.color = entry.Value;
                    entry.Key.material.SetColor("_EmissionColor", entry.Value);
                }
        }
    }
}