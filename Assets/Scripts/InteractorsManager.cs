using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MMI
{
    public enum InteractorType
    {
        ControllerRay = 0,
        EyeGaze = 1,
        FingerPoint = 2
    }

    public class InteractorsManager : MonoBehaviour
    {
        public Dictionary<InteractorType, BaseObjectInteractor> interactors;
        public InteractorType activeInteractor = InteractorType.ControllerRay;

        void Start()
        {
            // Deactivate all interactors except the initially active one
            foreach (var interactor in interactors.Values)
            {
                interactor.enabled = false;
            }

            interactors[activeInteractor].enabled = true;
        }

        void Update()
        {
#if !UNITY_EDITOR
            // For debugging, check for input to switch between interactors
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchInteractor(InteractorType.Method1);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SwitchInteractor(InteractorType.Method2);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SwitchInteractor(InteractorType.Method3);
#endif
        }

        void SwitchInteractor(InteractorType interactorType)
        {
            if (interactors.ContainsKey(interactorType))
            {
                // Deactivate the current active interactor
                interactors[activeInteractor].enabled = false;

                // Activate the new interactor
                interactors[interactorType].enabled = true;
                activeInteractor = interactorType;
            }
        }
    }
}