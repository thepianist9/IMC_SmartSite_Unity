using UnityEngine;

public class ConstructionModel 
{
    public int id;
    public string ObjectName;
    public string type;
    public Time time;
    public string description;
    public string milestones;


    public string Stringify()
    {
        return JsonUtility.ToJson(this);
    }

    public static ConstructionModel Parse(string json)
    {
        Debug.Log(json);
        return JsonUtility.FromJson<ConstructionModel>(json);
    }

}
