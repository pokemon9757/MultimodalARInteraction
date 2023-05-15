using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MMI
{
    public class GameActionHandler : MonoBehaviour
    {
        Stack<IGameAction> _actionsList = new();

        public void AddGameAction(IGameAction action)
        {
            _actionsList.Push(action);
            action.Execute();
        }
    }
}