using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MMI
{
    public class FlashingMaterial : MonoBehaviour
    {
        public bool Active = false;
        [SerializeField] float minIntensity = -0.5f;
        [SerializeField] float maxIntensity = 1.5f;
        [SerializeField] float transitionTime = 2f;
        float timer = 0f;
        bool tweeningForward = true;
        Dictionary<MeshRenderer, Color> renderersColors = new Dictionary<MeshRenderer, Color>();

        // Start is called before the first frame update
        void Start()
        {
            foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
            {
                renderersColors[m] = m.material.color;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Active)
            {
                foreach (KeyValuePair<MeshRenderer, Color> entry in renderersColors)
                {
                    float emissiveIntensity = Mathf.Lerp(minIntensity, maxIntensity, (float)timer / transitionTime);
                    entry.Key.material.color = entry.Value * emissiveIntensity;
                    entry.Key.material.SetColor("_EmissionColor", entry.Value * emissiveIntensity);
                }
                timer += tweeningForward ? Time.deltaTime : -Time.deltaTime;
                if (timer <= 0 || timer >= transitionTime)
                {
                    timer = timer <= 0 ? 0 : transitionTime;
                    tweeningForward = !tweeningForward;
                }
            }
        }

        public void EnableFlashing(bool active)
        {
            if (!Active)
            {
                foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer>())
                {
                    renderersColors[m] = m.material.color;
                }
            }
            Active = active;
            if (!active)
                foreach (KeyValuePair<MeshRenderer, Color> entry in renderersColors)
                {
                    entry.Key.material.color = entry.Value;
                    entry.Key.material.SetColor("_EmissionColor", entry.Value);
                }
        }
    }
}