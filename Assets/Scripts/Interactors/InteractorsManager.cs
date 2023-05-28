using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MMI
{
    public class InteractorsManager : MonoBehaviour
    {
        [SerializeField] List<BaseObjectInteractor> _interactors;

        void Start()
        {
            // Sort interactors based on priority (highest to lowest)
            _interactors.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public InteractableObject GetSelectedObject()
        {
            foreach (var interactor in _interactors)
            {
                if (interactor.enabled && interactor.SelectedObject != null)
                {
                    // Return the selected object with highest priority
                    return interactor.SelectedObject;
                }
            }
            return null;
        }
    }
}
