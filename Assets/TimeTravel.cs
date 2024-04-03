using IATK;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XRSpatiotemopralAuthoring;

public class TimeTravel : MonoBehaviour
{

    private GraphManager graphManagerInstance;
    private GraphsControlManager controlManager;
    private SpatioControlManager spatioControlManager;
    //get current visualisation
    [SerializeField] private Visualisation visualisation;
    [SerializeField] private Slider SliderGO;
    private int attributeIndex;

    //get time line value from the visualisation
    //set event listener to listen to the slider change event and filter out the values from the building


    public void initTimeline()
    {
        SliderGO = GetComponent<Slider>();
        graphManagerInstance = GraphManager.Instance;
        spatioControlManager = SpatioControlManager.Instance;

        controlManager = graphManagerInstance.currentGraphControlManager;
        visualisation = controlManager.visualisation;
        
        if (visualisation != null)
        {

            AttributeFilter af = new AttributeFilter();
            int index = controlManager.DataAttributesNames.IndexOf("milestone");
            af.Attribute = controlManager.DataAttributesNames[index];

            if(visualisation.attributeFilters.Length > 0 )
            {
                
                visualisation.attributeFilters[0] = af;

                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
                UpdateDropDownValues();
            }
            else
            {
                visualisation.attributeFilters = new AttributeFilter[1];
                visualisation.attributeFilters[0] = af;
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
                UpdateDropDownValues();
            }
            
        }
    }
    public void ValidateAttributeFilteringSliderMax()
    {
        if (visualisation != null && visualisation.attributeFilters.Length > 0)
        {
            visualisation.attributeFilters[0].maxFilter = SliderGO.value;
            visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
            //set unnormalized value to slider 
            UpdateDropDownValues();
            spatioControlManager.SetData();

        }
    }

    private void UpdateDropDownValues()
    {

        SliderGO.GetComponentInChildren<TMP_Text>().text = visualisation.dataSource.getOriginalValue(visualisation.attributeFilters[0].maxFilter, visualisation.attributeFilters[0].Attribute).ToString();
    }
}
