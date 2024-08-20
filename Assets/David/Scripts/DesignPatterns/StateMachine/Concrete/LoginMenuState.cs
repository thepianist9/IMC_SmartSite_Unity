using System.Collections;
using UnityEngine;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public class LoginMenuState : UIInteractionState
    {
        public LoginMenuState(UIInteractionContext context, UIStateManager.UIState estate) : base(context, estate)    
        {
             UIInteractionContext Context = context;
        }

        public override void EnterState()
        {
            //set layout via UILayoutManager
            context.LayoutUIManager.SetUILayout(false, false, true, true);
            ChangeTitleText(stateKey.ToString());
            context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = stateKey.ToString();
            context.GetLoginMenu.SetActive(true);
            context.appContext = UIInteractionContext.AppContext.idle;
        }

        public override void ExitState()
        {
            context.GetLoginMenu.SetActive(false);
            context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = "Exiting " + stateKey.ToString();
        }

        public override UIStateManager.UIState GetNextState()
        {
            if (context.appContext == UIInteractionContext.AppContext.switchNext)
            {
                return UIStateManager.UIState.DrawerMenu;
            }
            else if (context.appContext == UIInteractionContext.AppContext.switchPrevious)
            {
                return UIStateManager.UIState.MainMenu;
            }
            return stateKey;
        }

        public override void InitState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            Debug.Log("Updating LoginMenuState");
        }
    }
}