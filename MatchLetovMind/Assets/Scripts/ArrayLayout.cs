using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout
{

    [System.Serializable]
    public struct rowData
    {
        public bool[] Row; 
    }
     
    public rowData[] Rows = new rowData[6]; //Grid of 6x10

}
