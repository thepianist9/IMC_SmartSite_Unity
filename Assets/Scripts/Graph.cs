using System.Collections;
using UnityEngine;

namespace XRSpatiotemopralAuthoring
{

    
    enum GraphType
    {
        Single_Axis,
        Double_Axis,
        Triple_Axis
    }
    public class Graph
    {

        public string X_LABEL {  set; get; }
        public string Y_LABEL {  set; get; }
        public string Z_LABEL {  set; get; }

        private GraphType graphType;



        public Graph(string x, string y, string z) {
            X_LABEL = x;
            Y_LABEL = y;
            Z_LABEL = z;
            graphType = GraphType.Triple_Axis;
        }

        public Graph(string x, string y)
        {
            X_LABEL = x;
            Y_LABEL = y;
            graphType = GraphType.Double_Axis;
        }
        public Graph(string x)
        {
            X_LABEL = x;
            graphType = GraphType.Single_Axis;
        }
    }
}