using UnityEngine;
using UnityEngine.UI;

public class DrawerController : MonoBehaviour
{
    public RectTransform drawerRect;
    public float showPositionX = 0f;  // Position when the drawer is fully visible
    public float hidePositionX = -300f;  // Position when the drawer is hidden
    public float triggerZoneWidth = 50f;  // Width of the zone to trigger the drawer
    public float animationSpeed = 500f;  // Speed of the drawer animation

    private bool isMouseOverZone = false;
    private bool isDrawerVisible = false;

    void Update()
    {
        Debug.Log($"Mouse Position: {Input.mousePosition.x}");
        // Check if the mouse is in the trigger zone
        if (Input.mousePosition.x <= triggerZoneWidth)
        {
            isMouseOverZone = true;
        }
        else
        {
            isMouseOverZone = false;
        }

        // Move the drawer based on mouse position
        if (isMouseOverZone && !isDrawerVisible)
        {
            // Slide the drawer in
            drawerRect.anchoredPosition = Vector2.MoveTowards(drawerRect.anchoredPosition, new Vector2(showPositionX, drawerRect.anchoredPosition.y), animationSpeed * Time.deltaTime);
            if (drawerRect.anchoredPosition.x >= showPositionX - 1f)  // Consider it fully open when close enough to target
            {
                isDrawerVisible = true;
            }
        }
        else if (!isMouseOverZone && isDrawerVisible)
        {
            // Slide the drawer out
            drawerRect.anchoredPosition = Vector2.MoveTowards(drawerRect.anchoredPosition, new Vector2(hidePositionX, drawerRect.anchoredPosition.y), animationSpeed * Time.deltaTime);
            if (drawerRect.anchoredPosition.x <= hidePositionX + 1f)  // Consider it fully closed when close enough to target
            {
                isDrawerVisible = false;
            }
        }
    }
}
