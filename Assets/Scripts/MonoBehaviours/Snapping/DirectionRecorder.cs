using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DirectionRecorder : MonoBehaviour
{
    public Vector3 estimatedDirection
    {
        get
        {
            //find closest recording (round to recording) and use current position

            return (GetCurrentPosition() - RetrieveRecording(recordGap)).normalized;
        }
    }
    private Vector3 previousDirection;
    
    private Queue<KeyValuePair<float, Vector3>> positionHistory;
    private float historyDistance;

    public float speedFactor;

    public Space space = Space.Self;
    public LocalRelativity relativity = LocalRelativity.Custom; //if space == space.self

    public float intervalLength;
    public float minRecordedDistance = 0.02f;

    public float recordGap;

    //public int minRecords = 5;
    //public int maxRecords = 20;
    public float minDistance = 5;
    public float maxDistance = 20;


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

    private void EnqueueRecord ()
    {
        if (positionHistory.Count > 0) {
            historyDistance += Vector3.Distance(GetCurrentPosition(), positionHistory.Last().Value);
        }
        positionHistory.Enqueue(new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition()));
    }

    private void DequeueRecord ()
    {
        if (positionHistory.Count >= 2) {
            historyDistance -= Vector3.Distance(positionHistory.First().Value, positionHistory.ToArray()[1].Value);
        }
        positionHistory.Dequeue();
    }

    private void RecordPosition()
    {
        //    if (positionHistory.Count == maxRecords)
        //    {
        //        DequeueRecord();
        //    }

        //    if (Vector3.Distance(positionHistory.Last().Value, GetCurrentPosition()) > minRecordedDistance && positionHistory.Count < maxRecords)
        //    {
        //        EnqueueRecord();
        //    }/* else if (Vector3.Distance(positionHistory.Last().Value, GetCurrentPosition()) > currentPositionRoundTo)
        //{
        //    //positionHistory.Enqueue(new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition()));
        //}*/


        //    if (positionHistory.Count > maxRecords)
        //    {
        //        while (positionHistory.Count > maxRecords)
        //        {
        //            DequeueRecord();
        //        }
        //    }



        if (historyDistance < minDistance || (Vector3.Distance(positionHistory.Last().Value, GetCurrentPosition()) > minRecordedDistance && historyDistance < maxDistance))
        {
            EnqueueRecord();
        }

        if (historyDistance > maxDistance)
        {
            while (historyDistance > maxDistance)
            {
                DequeueRecord();
            }
        }
    }

    public Vector3 RetrieveRecording(float seconds)
    {
        KeyValuePair<float, Vector3>[] historyAnalyse = positionHistory.ToArray();

        KeyValuePair<float, Vector3> greater = default;
        KeyValuePair<float, Vector3> lesser = default;

        for (int i = historyAnalyse.Length - 1; i > -1; i--)
        {
            /*if (historyAnalyse[i].Key < (Time.time - seconds))
            {
                lesser = historyAnalyse[i];
                if (historyAnalyse.Length > i + 1)
                {
                    greater = historyAnalyse[i + 1];
                } else
                {
                    greater = new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition());
                }

                break;
            }*/

            if (Vector3.Distance(historyAnalyse[i].Value, GetCurrentPosition()) >= recordGap)
            {
                lesser = historyAnalyse[i];
                if (historyAnalyse.Length > i + 1)
                {
                    greater = historyAnalyse[i + 1];
                }
                else
                {
                    greater = new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition());
                }

                break;
            }
        }
        
        if (lesser.Equals(default(KeyValuePair<float, Vector3>)))
        {
            lesser = new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition());
            greater = new KeyValuePair<float, Vector3>(Time.time, GetCurrentPosition());
        }

        /*return Vector3.Lerp(
            lesser.Value,
            greater.Value,
            ((Time.time - seconds) - lesser.Key) / (greater.Key - lesser.Key)
        );*/


        //FIND point between points that equals recordgap distance to current
        return Vector3.Lerp(
            lesser.Value,
            greater.Value,
            0.5f
        );


    }

    public void RestartLoop ()
    {
        CancelInvoke("RecordPosition");
        InvokeRepeating("RecordPosition", 0f, intervalLength);
    }
    
    void Start()
    {
        positionHistory = new Queue<KeyValuePair<float, Vector3>>();

        RestartLoop();
    }
}
