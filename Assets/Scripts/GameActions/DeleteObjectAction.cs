using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MMI
{
    public class DisableObjectAction : MonoBehaviour
    {
        GameObject _objToDelete;
        public void DisableObject(GameObject objToDelete)
        {
            objToDelete.SetActive(false);

        }
    }
}