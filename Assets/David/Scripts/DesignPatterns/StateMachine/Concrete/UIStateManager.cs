using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Assets.David.Scripts.DesignPatterns.StateMachine.UIStateManager;
using Assets.David.UILayout;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public class UIStateManager : StateManager<UIState>
    {
        [SerializeField] private LayoutUIManager layoutUIManager;
        
        [SerializeField] private GameObject titleText;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject loginMenu;
        [SerializeField] private GameObject drawerMenu;
        [SerializeField] private GameObject navbarMenu;
        public enum UIState
        {
            MainMenu,
            LoginMenu,
            DrawerMenu,
            NavbarMenu,
        }
        private UIInteractionContext context;

        // Use this for initialization
        void Start()
        {
            ValidateConstraints();
            context = new UIInteractionContext(titleText, mainMenu, loginMenu, drawerMenu, navbarMenu, layoutUIManager);

            InitializeStates();
        }

        // Update is called once per frame
        void ValidateConstraints()
        {
            Assert.IsNotNull(mainMenu, "_mainMenu is null at start");
            Assert.IsNotNull(loginMenu, "_loginMenu is null at start");
            Assert.IsNotNull(drawerMenu, "_drawerMenu is null at start");
            Assert.IsNotNull(navbarMenu, "_navbarMenu is null at start");
        }

        void InitializeStates()
        {
            states.Add(UIState.MainMenu, new MainMenuState(context, UIState.MainMenu));
            states.Add(UIState.LoginMenu, new LoginMenuState(context, UIState.LoginMenu));
            states.Add(UIState.DrawerMenu, new DrawerMenuState(context, UIState.DrawerMenu));
            states.Add(UIState.NavbarMenu, new NavbarMenuState(context, UIState.NavbarMenu));

            currentState = states[UIState.MainMenu];
            currentState.EnterState();
        }

        public void SwitchNext()
        {
            context.appContext = UIInteractionContext.AppContext.switchNext;
        }
        public void SwitchPrevious() 
        {
            context.appContext = UIInteractionContext.AppContext.switchPrevious;
        }

    }
}