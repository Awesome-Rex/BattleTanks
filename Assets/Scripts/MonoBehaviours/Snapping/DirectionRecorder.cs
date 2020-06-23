using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionRecorder : MonoBehaviour
{
    public Vector3 estimatedDirection
    {
        get
        {
            //find closest recording (round to recording) and use current position
            
            return null;
        }
    };

    public Queue<KeyValuePair<float, Vector3>> positionHistory;

    public Space space = Space.Self;
    public LocalRelativity relativity = LocalRelativity.Custom; //if space == space.self

    public float intervalLength;

    public float recordGap;

    public int maxRecords = 20;

    public Vector3 GetCurrentPosition (){
        if (space == Space.World)
        {
            return transform.position;
        }
        else
        {
            if (relativity == LocalRelativity.Natural)
            {
                return transform.localPosition;
            }
            else
            {
                return GetComponent<CustomPosition>().position;
            }
        }
    }

    private void RecordPosition ()
    {
        if (positionHistory.Count == maxRecords)
        {
            positionHistory.Dequeue();
        }

        positionHistory.Enqueue(new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition()));

        if (positionHistory.Count > maxRecords)
        {
            while (positionHistory.Count > maxRecords)
            {
                positionHistory.Dequeue();
            }
        } else if (positionHistory.Count < maxRecords)
        {
            //keep is as it is
        }
    }

    public Vector3 RetrieveRecording (float seconds)
    {
        KeyValuePair<float, Vector3>[] historyAnalyse = positionHistory.ToArray();

        Vector3 greater;
        Vector3 lesser;

        for (int i = historyAnalyse.Length - 1; i > -1; i--)
        {

        }

        //find whether greater or lesser is closer
    }

    public void RestartLoop ()
    {
        CancelInvoke("RecordPosition");
        InvokeRepeating("RecordPosition", 0f, intervalLength);
    }
    
    void Start()
    {
        positionHistory = new Queue<KeyValuePair<float, Vector3>>();
        InvokeRepeating("RecordPosition", 0f, intervalLength);
    }
}
