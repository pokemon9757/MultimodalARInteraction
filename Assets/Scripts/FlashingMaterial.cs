using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MMI
{
    public class FlashingMaterial : MonoBehaviour
    {
        public bool IsActive = false;
        [SerializeField] float _minIntensity = -0.5f;
        [SerializeField] float _maxIntensity = 1.5f;
        [SerializeField] float _transitionTime = 1f;
        float _timer = 0f;
        bool _tweeningForward = true;
        Dictionary<MeshRenderer, Color> _renderersColors = new Dictionary<MeshRenderer, Color>();

        // Start is called before the first frame update
        void Start()
        {
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
            {
                _renderersColors[m] = m.material.color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsActive)
            {
                foreach (KeyValuePair<MeshRenderer, Color> entry in _renderersColors)
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
            if (!IsActive)
            {
                foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                {
                    _renderersColors[m] = m.material.color;
                }
            }
            IsActive = active;
            if (!active)
                foreach (KeyValuePair<MeshRenderer, Color> entry in _renderersColors)
                {
                    entry.Key.material.color = entry.Value;
                    entry.Key.material.SetColor("_EmissionColor", entry.Value);
                }
        }
    }
}