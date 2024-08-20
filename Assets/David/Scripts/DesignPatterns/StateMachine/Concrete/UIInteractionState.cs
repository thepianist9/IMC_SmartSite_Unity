using TMPro;
 
namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public abstract class UIInteractionState : BaseState<UIStateManager.UIState>
    {
        protected UIInteractionContext context;   

        public UIInteractionState(UIInteractionContext context, UIStateManager.UIState uIState) : base(uIState) 
        {
            this.context = context;
        }

        public void ChangeTitleText(string text)
        {
            context.GetTitleText.GetComponent<TMP_Text>().text = text;
        }
    }
}