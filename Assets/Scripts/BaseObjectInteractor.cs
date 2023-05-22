using UnityEngine;

namespace MMI
{
    public abstract class BaseObjectInteractor : MonoBehaviour
    {
        protected InteractableObject _collidedObject; // Only one object can be collided at a time

        protected virtual void SetObjectSelected(bool selected)
        {
            if (_collidedObject != null)
                _collidedObject.SetSelected(selected);
        }
    }
}
