using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace XRSpatiotemopralAuthoring
{
    public class GraphsControlManager : MonoBehaviour
    {

        private static GraphsControlManager _Instance;
        public static GraphsControlManager Instance { get { return _Instance; } }


        public Visualisation visualisation;
        public Slider sizeSlider;
        public TMP_Dropdown X_AxisDropDown;
        public TMP_Dropdown Y_AxisDropDown;
        public TMP_Dropdown Z_AxisDropDown;
        public TMP_Dropdown SizeAttributeDropDown;
        public TMP_Dropdown FilterAttributeDropDown;
        public Slider FilterMinSlider;
        public Slider FilterMaxSlider;


        //holds the string names of the data attributes
        public List<string> DataAttributesNames;
        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        // Use this for initialization
        public void Start()
        {
            DataAttributesNames = GetAttributesList();
            X_AxisDropDown.AddOptions(DataAttributesNames);
            Y_AxisDropDown.AddOptions(DataAttributesNames);
            Z_AxisDropDown.AddOptions(DataAttributesNames);
            SizeAttributeDropDown.AddOptions(DataAttributesNames);
            FilterAttributeDropDown.AddOptions(DataAttributesNames);

            X_AxisDropDown.value = DataAttributesNames.IndexOf(visualisation.xDimension.Attribute);
            Y_AxisDropDown.value = DataAttributesNames.IndexOf(visualisation.yDimension.Attribute);
            Z_AxisDropDown.value = DataAttributesNames.IndexOf(visualisation.zDimension.Attribute);
            
            if (visualisation.attributeFilters.Length > 0)
                FilterAttributeDropDown.value = DataAttributesNames.IndexOf(visualisation.attributeFilters[0].Attribute);
        }


        private List<string> GetAttributesList()
        {
            List<string> dimensions = new List<string>();
            dimensions.Add("Undefined");
            for (int i = 0; i < visualisation.dataSource.DimensionCount; ++i)
            {
                dimensions.Add(visualisation.dataSource[i].Identifier);
            }
            return dimensions;
        }

        public void ValidateX_AxisDropdown()
        {
            if (visualisation != null)
            {
                visualisation.xDimension = DataAttributesNames[X_AxisDropDown.value];
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.X);
            }
        }

        public void ValidateY_AxisDropdown()
        {
            if (visualisation != null)
            {
                visualisation.yDimension = DataAttributesNames[Y_AxisDropDown.value];
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Y);

            }
        }

        public void ValidateZ_AxisDropdown()
        {
            if (visualisation != null)
            {
                visualisation.zDimension = DataAttributesNames[Z_AxisDropDown.value];
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Z);

            }
        }

        public void ValidateSizeChangeSlider()
        {
            if (visualisation != null)
            {
                visualisation.size = sizeSlider.value;
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.SizeValues);
            }
        }

        public void ValidateAttributeSizeDropDown()
        {
            if (visualisation != null)
            {
                visualisation.sizeDimension = DataAttributesNames[SizeAttributeDropDown.value];
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.Size);
            }
        }

        public void ValidateAttributeFilteringDropDown()
        {
            if (visualisation != null)
            {

                AttributeFilter af = new AttributeFilter();
                af.Attribute = DataAttributesNames[FilterAttributeDropDown.value];
                visualisation.attributeFilters = new AttributeFilter[1];
                visualisation.attributeFilters[0] = af;
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
                UpdateDropDownValues();
            }
        }

        public void ValidateAttributeFilteringSliderMin()
        {
            if (visualisation != null && visualisation.attributeFilters.Length > 0)
            {
                visualisation.attributeFilters[0].minFilter = FilterMinSlider.value;
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);

                //set unnormalized value to slider 
                UpdateDropDownValues();
            }
        }

        public void ValidateAttributeFilteringSliderMax()
        {
            if (visualisation != null && visualisation.attributeFilters.Length > 0)
            {
                visualisation.attributeFilters[0].maxFilter = FilterMaxSlider.value;
                visualisation.updateViewProperties(AbstractVisualisation.PropertyType.AttributeFiltering);
                //set unnormalized value to slider 
                UpdateDropDownValues();

            }
        }

        private void UpdateDropDownValues()
        {
            FilterMinSlider.gameObject.GetComponentInChildren<TMP_Text>().text = visualisation.dataSource.getOriginalValue(visualisation.attributeFilters[0].minFilter, visualisation.attributeFilters[0].Attribute).ToString();
            FilterMaxSlider.gameObject.GetComponentInChildren<TMP_Text>().text = visualisation.dataSource.getOriginalValue(visualisation.attributeFilters[0].maxFilter, visualisation.attributeFilters[0].Attribute).ToString();
        }
    }
}
