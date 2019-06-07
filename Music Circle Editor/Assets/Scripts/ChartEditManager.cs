using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChartEditManager : MonoBehaviour
{
    public string _fileName;

    TextAsset chart = null;
    StringReader sr;

    FileStream f;
    private StreamWriter sw;

    private List<string> dataList = new List<string>();

    public GameObject _note;

    /// <summary>
    /// 현재 마디
    /// </summary>
    public int Bar { get; private set; } = 0;

    /// <summary>
    /// 현재 마디의 줄 숫자
    /// </summary>
    public int BarLineNum { get; private set; } = -1;

    // Start is called before the first frame update
    void Start()
    {
        chart = Resources.Load("Charts/" + _fileName, typeof(TextAsset)) as TextAsset;
        Debug.Log(chart);
        sr = new StringReader(chart.text);

        string line = sr.ReadLine();

        while (line != null)
        {
            dataList.Add(line.Trim());
            line = sr.ReadLine();
        }

        FindSetNextBarLineNum(); //첫번째 마디를 찾음

        DrawNote(BarLineNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool IsLineBar(int dataIndex)
    {
        return (dataList[dataIndex].Length >= 3 && dataList[dataIndex].Substring(0, 3) == "---");
    }
    void FindSetNextBarLineNum()
    {
        BarLineNum++;
        while (!IsLineBar(BarLineNum))
        {
            BarLineNum++;
        }
    }
    void FindSetPrevBarLineNum()
    {
        BarLineNum--;
        while (!IsLineBar(BarLineNum))
        {
            BarLineNum--;
        }
    }

    public void WriteData()
    {
        f = new FileStream("Assets/Resources/Charts/" + "edited" + ".txt", FileMode.Create, FileAccess.Write);
        sw = new StreamWriter(f, System.Text.Encoding.Unicode);

        foreach (string dataLine in dataList)
        {
            sw.WriteLine(dataLine);
        }

        sw.Close();
    }

    void DrawNote(int barLineIndex)
    {
        GameObject noteParent = GameObject.Find("Notes");

        foreach (Transform note in noteParent.transform)
        {
            Destroy(note.gameObject);
        }

        int beat = 0;
        int dataIndex = barLineIndex + 1;
        while (!IsLineBar(dataIndex))
        {
            dataIndex++;
            beat++;
        }

        if (beat == 0) return;

        int lane = dataList[barLineIndex + 1].Length;
        for (int beatIndex = 0; beatIndex < beat; beatIndex++)
        {
            for (int laneIndex = 0; laneIndex < lane; laneIndex++)
            {
                float distance = 180 * (1 - (float)beatIndex / beat);
                float angle = Mathf.Deg2Rad * (90 - (360 * (float)laneIndex / lane));

                GameObject noteCreated = Instantiate(_note);
                noteCreated.transform.SetParent(noteParent.transform);
                noteCreated.GetComponent<RectTransform>().anchoredPosition = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle));

                noteCreated.GetComponent<NoteInstance>().Beat = beatIndex;
                noteCreated.GetComponent<NoteInstance>().Lane = laneIndex;
                if (dataList[barLineIndex + 1 + beatIndex][laneIndex] == 'O') noteCreated.GetComponent<NoteInstance>().Located = true;
            }
        }
    }

    public void SetNoteState(int beat, int lane, bool located)
    {
        char noteChar = 'X';
        if (located) noteChar = 'O';
        dataList[BarLineNum + 1 + beat] = dataList[BarLineNum + 1 + beat].Substring(0, lane) + noteChar + dataList[BarLineNum + 1 + beat].Substring(lane + 1);
    }
    
    public void MoveNextBar(int num)
    {
        //FindSetNextBarLineNum();
        //if (dataList.Count == BarLineNum + 1)
        //{
        //    FindSetPrevBarLineNum();
        //    return;
        //}
        //FindSetPrevBarLineNum();

        int startLine = BarLineNum;
        for (int i = 0; i < 2 * num - 1; i++)
        {
            FindSetNextBarLineNum();
            if (dataList.Count == BarLineNum + 1)
            {
                BarLineNum = startLine;
                return;
            }
        }
        
        FindSetNextBarLineNum();

        Bar += num;

        DrawNote(BarLineNum);
    }

    public void MovePrevBar(int num)
    {
        if (Bar - num < 0) return;

        for (int i = 0; i < 2 * num; i++)
        {
            FindSetPrevBarLineNum();
        }

        Bar -= num;

        DrawNote(BarLineNum);
    }

    public void IncreaseBeat()
    {
        FindSetNextBarLineNum();
        if (!IsLineBar(BarLineNum - 1)) dataList.Insert(BarLineNum - 1, dataList[BarLineNum - 1]);
        else dataList.Insert(BarLineNum, "X");
        FindSetPrevBarLineNum();

        DrawNote(BarLineNum);
    }

    public void DecreaseBeat()
    {
        if (IsLineBar(BarLineNum + 1)) return;
        FindSetNextBarLineNum();
        dataList.RemoveAt(BarLineNum - 1);
        BarLineNum--;
        FindSetPrevBarLineNum();

        DrawNote(BarLineNum);
    }

    public void IncreaseLane()
    {
        int dataIndex = BarLineNum + 1;
        while (!IsLineBar(dataIndex))
        {
            dataList[dataIndex] += 'X';
            dataIndex++;
        }

        DrawNote(BarLineNum);
    }

    public void DecreaseLane()
    {
        if (string.IsNullOrEmpty(dataList[BarLineNum + 1])) return;
        int dataIndex = BarLineNum + 1;
        while (!IsLineBar(dataIndex))
        {
            dataList[dataIndex] = dataList[dataIndex].Substring(0, dataList[dataIndex].Length - 1);
            dataIndex++;
        }

        DrawNote(BarLineNum);
    }
}
