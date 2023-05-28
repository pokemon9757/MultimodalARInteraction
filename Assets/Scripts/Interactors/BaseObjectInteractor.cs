using UnityEngine;

namespace MMI
{
    public abstract class BaseObjectInteractor : MonoBehaviour
    {
        public int Priority = 1;
        protected InteractableObject _selectedObject; // Only one object can be collided at a time
        public InteractableObject SelectedObject { get { return _selectedObject; } }
        protected void SetObjectSelected(bool selected)
        {
            if (_selectedObject != null)
                _selectedObject.SetSelected(selected);
        }
    }
}
