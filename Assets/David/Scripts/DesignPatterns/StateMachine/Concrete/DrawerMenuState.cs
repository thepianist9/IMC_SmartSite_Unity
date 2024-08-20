using System.Collections;
using UnityEngine;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public class DrawerMenuState : UIInteractionState
    {
        public DrawerMenuState(UIInteractionContext context, UIStateManager.UIState estate) : base(context, estate)    
        {
             UIInteractionContext Context = context;
        }
        public override void InitState()
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState()
        {
            //set layout via UILayoutManager
            context.LayoutUIManager.SetUILayout(false, false, false, true);
            ChangeTitleText(stateKey.ToString());
            context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = stateKey.ToString();
            context.GetDrawerMenu.SetActive(true);
            context.appContext = UIInteractionContext.AppContext.idle;
        }

        public override void ExitState()
        {
            context.GetDrawerMenu.SetActive(false);
            context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = "Exiting " + stateKey.ToString();
        }

        public override UIStateManager.UIState GetNextState()
        {
            if (context.appContext == UIInteractionContext.AppContext.switchNext)
            {
                return UIStateManager.UIState.NavbarMenu;
            }
            else if (context.appContext == UIInteractionContext.AppContext.switchPrevious)
            {
                return UIStateManager.UIState.LoginMenu;
            }
            return stateKey;
        }

       

        public override void UpdateState()
        {
            Debug.Log("Updating DrawerMenuState");
        }
    }
}