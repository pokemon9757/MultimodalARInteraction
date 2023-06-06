using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MMI
{
    public class DeleteObjectAction : IGameAction
    {
        GameObject _objToDelete;
        public DeleteObjectAction(GameObject objToDelete)
        {
            _objToDelete = objToDelete;
        }

        public void Execute()
        {
            _objToDelete.SetActive(false);
        }
    }
}