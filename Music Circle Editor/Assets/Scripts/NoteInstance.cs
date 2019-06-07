using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteInstance : MonoBehaviour
{
    public int Lane { get; set; } = -1;

    public int Beat { get; set; } = -1;

    public bool Located { get; set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Located) GetComponent<Image>().color = Color.red;
        else GetComponent<Image>().color = Color.white;
    }

    public void CallSetNoteState()
    {
        if (Located) Located = false;
        else Located = true;

        GameObject.Find("Note Interface").GetComponent<ChartEditManager>().SetNoteState(Beat, Lane, Located);
    }

}
