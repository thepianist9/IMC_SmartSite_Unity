using UnityEngine;
using UnityEngine.UI;

namespace Assets.David.UILayout 
{ 
    public class MouseFollow : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private Vector3 pos;
        [SerializeField] private Image image;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

        private Transform target = null;
        private Vector3 offset;


        void Update()
        {
            pos = Input.mousePosition;
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = cam.ScreenPointToRay(pos);

                Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

                Debug.Log(pos);
                Debug.Log("Mouse Clicked");


                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit: " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.name == "Spatial Panel Manipulator") 
                    {
                        GameObject selectedObject = hit.collider.gameObject;
                        image.enabled = true;
                        skinnedMeshRenderer.material.color = Color.blue;

                        selectedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
                    }
                    else
                    {
                        image.enabled = false;
                        skinnedMeshRenderer.material.color = Color.white;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                image.enabled = false;
                skinnedMeshRenderer.material.color = Color.white;
            }

        }
        Vector3 GetMouseWorldPosition()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero); // Assuming a horizontal plane
            float distance =  10;
            plane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    }
}

