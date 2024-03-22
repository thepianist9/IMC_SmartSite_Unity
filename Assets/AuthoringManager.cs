using UnityEngine;
using UnityEngine.InputSystem;

public class AuthoringManager : MonoBehaviour
{
    [SerializeField] private GameObject AuthoringPanel;
    private GameObject AuthoringGO;
    
    private bool isAuthoringOn = false;
    private Ray ray;
    private RaycastHit hit;

   
   
    //method to set authoring content

    public void SetAuthoringContent(GameObject gameObject) 
    {
        //set authoring content and 
        if(gameObject != null)
            AuthoringGO = gameObject; 
        //toggleUI
        ToggleAuthoringUI();
        //SetAuthoring to active
        Author();
    }

    private void ToggleAuthoringUI()
    {
        if(AuthoringPanel != null)
        {
            AuthoringPanel.SetActive(!AuthoringPanel.activeSelf);
        }
    }

    //method to author content

    private void Author()
    {
        if(AuthoringGO != null) {
            isAuthoringOn = true;
        }
    }
    
    //method to manage authoring functionality

    // Update is called once per frame
    void Update()
    {
        if(isAuthoringOn)
        {

            Mouse mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Use the hit variable to determine what was clicked on.
                    Debug.Log($"hit object at {hit.transform.position}");
                    //spawn object and revert authoring mode
                    if(Instantiate(AuthoringGO, hit.transform.position, Quaternion.identity) != null )
                    {
                        isAuthoringOn = false;
                        ToggleAuthoringUI();
                    }

                    

                }
            }

        }
        
    }
}
