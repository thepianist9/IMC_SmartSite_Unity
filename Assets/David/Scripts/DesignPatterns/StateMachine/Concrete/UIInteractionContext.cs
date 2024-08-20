using System.Collections;
using UnityEngine;
using Assets.David.UILayout;

namespace Assets.David.Scripts.DesignPatterns.StateMachine
{
    public class UIInteractionContext
    {
        public enum AppContext
        {
            idle,
            switchNext,
            switchPrevious
        }

        private GameObject _titleText;
        private GameObject _mainMenu;
        private GameObject _loginMenu;
        private GameObject _drawerMenu;
        private GameObject _navbarMenu;
        public AppContext appContext;

        private LayoutUIManager layoutUIManager;


        public GameObject GetTitleText { get => _titleText; }
        public GameObject GetMainMenu { get => _mainMenu; }    
        public GameObject GetLoginMenu { get => _loginMenu; }
        public GameObject GetDrawerMenu { get => _drawerMenu; }
        public GameObject GetNavbarMenu { get => _navbarMenu; }
        public LayoutUIManager LayoutUIManager { get => layoutUIManager; }



        // Use this for initialization
        public UIInteractionContext(GameObject titleText, GameObject mainMenu, GameObject loginMenu, GameObject drawerMenu, GameObject navbarMenu, LayoutUIManager layoutUIManager)
       {
            this._titleText = titleText;
            this._mainMenu = mainMenu;
            this._loginMenu = loginMenu;
            this._drawerMenu = drawerMenu;
            this._navbarMenu = navbarMenu;
            this.appContext = AppContext.idle;
            this.layoutUIManager = layoutUIManager;
       }

    }
}