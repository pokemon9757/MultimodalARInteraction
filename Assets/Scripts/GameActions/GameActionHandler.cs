using System.Collections.Generic;
namespace MMI
{
    public class GameActionHandler
    {
        Stack<IGameAction> _actionsList = new();

        public void AddGameAction(IGameAction action)
        {
            _actionsList.Push(action);
            action.Execute();
        }
    }
}