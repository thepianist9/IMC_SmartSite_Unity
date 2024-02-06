using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Thesis.Scripts
{
    public class DataManager : MonoBehaviour
    {
        ConstructionModel constructionModel;

        // Use this for initialization
        void Start()
        {
            Debug.Log("Startin data manager");
            constructionModel = new ConstructionModel();
            constructionModel.id = 1;
            StartCoroutine(Download(constructionModel.id.ToString(), result => {
                Debug.Log(result);
            }));
        }
        IEnumerator Download(string id, System.Action<ConstructionModel> callback = null)
        {
            Debug.Log("Inside Download function");
            using (UnityWebRequest request = UnityWebRequest.Get("http://127.0.0.1:27017/Construction_DB" + id))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                    if (callback != null)
                    {
                        callback.Invoke(null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        Debug.Log(request.downloadHandler.text);
                        callback.Invoke(ConstructionModel.Parse(request.downloadHandler.text));
                    }
                }
            }
        }
    }
}