using System.Collections;
using UnityEngine;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public class MainMenuState : UIInteractionState
    {
        private UIInteractionContext Context;
        public MainMenuState(UIInteractionContext context, UIStateManager.UIState estate) : base(context, estate)    
        {
             this.Context = context;
        }

        public override void EnterState()
        {
            //edit layout via UILayoutManager
            context.LayoutUIManager.SetUILayout(false, true, true, true);
            ChangeTitleText(stateKey.ToString());
            Context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = stateKey.ToString();
            Context.GetMainMenu.SetActive(true);
            Context.appContext = UIInteractionContext.AppContext.idle;
        }

        public override void ExitState()
        {
            Context.GetMainMenu.SetActive(false);
            context.GetTitleText.GetComponent<TMPro.TMP_Text>().text = "Exiting " + stateKey.ToString();
        }

        public override UIStateManager.UIState GetNextState()
        {
            if (Context.appContext == UIInteractionContext.AppContext.switchNext) {
                return UIStateManager.UIState.LoginMenu;
            }
            else if (Context.appContext == UIInteractionContext.AppContext.switchPrevious)
            {
                return UIStateManager.UIState.NavbarMenu;
            }
            return stateKey;
        }

        public override void InitState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            Debug.Log("Updating MainMenuState");
        }
    }
}