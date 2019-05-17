using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ChartEditManager : MonoBehaviour
{
    public string fileName;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset chart = Resources.Load("Charts/" + fileName, typeof(TextAsset)) as TextAsset;
        StringReader sr = new StringReader(chart.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
