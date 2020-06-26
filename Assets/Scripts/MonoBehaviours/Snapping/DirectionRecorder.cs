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
            Vector3 temp = (GetCurrentPosition() - RetrieveRecording(recordGap * speedFactor)).normalized;

            if (temp == Vector3.zero)
            {
                return previous; //return previous
            } else
            {

                previous = temp;
                return temp;
            }
        }
    }
    
    private Queue<KeyValuePair<float, Vector3>> positionHistory;
    private float historyDistance;

    private float speedFactor = 1f;

    public Space space = Space.Self;
    public LocalRelativity relativity = LocalRelativity.Custom; //if space == space.self

    public float intervalLength;
    public float minRecordedDistance = 0.02f;

    public float recordGap;
    
    public float minDistance = 5;
    public float maxDistance = 20;

    private Vector3 previous;


    private Coroutine loop;

    //methods
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
        //if (positionHistory.Count >= 2) {
            historyDistance -= Vector3.Distance(positionHistory.First().Value, positionHistory.ToArray()[1].Value);
        //}
        positionHistory.Dequeue();
    }

    private IEnumerator RecordPosition()
    {
        while (true)
        {
            if (historyDistance < minDistance * speedFactor || (Vector3.Distance(positionHistory.Last().Value, GetCurrentPosition()) > minRecordedDistance && historyDistance < maxDistance * speedFactor))
            {
                EnqueueRecord();
            }

            if (historyDistance > maxDistance * speedFactor)
            {
                while (historyDistance > maxDistance * speedFactor)
                {
                    DequeueRecord();
                }
            }

            if (speedFactor < 1f)
            {
                yield return new WaitForSeconds(intervalLength * (1f - ((1f - speedFactor) / 4f)));
            }
            else if (speedFactor > 1f)
            {
                yield return new WaitForSeconds(intervalLength * (1f + ((speedFactor - 1f) / 4f)));
            }
            else if (speedFactor == 1f)
            {
                yield return new WaitForSeconds(intervalLength);
            }
        }
    }

    public Vector3 RetrieveRecording(float seconds)
    {
        Vector3 currentPos = GetCurrentPosition();


        KeyValuePair<float, Vector3>[] historyAnalyse = positionHistory.ToArray();

        KeyValuePair<float, Vector3> greater = default;
        KeyValuePair<float, Vector3> lesser = default;


        float greaterDistance = 0f;
        float lesserDistance = 0f;
        for (int i = historyAnalyse.Length - 1; i > -1; i--)
        {
            if (historyAnalyse.Length > i + 1) {
                greaterDistance += Vector3.Distance(historyAnalyse[i].Value, historyAnalyse[i + 1].Value);
            }

            if (greaterDistance >= recordGap * speedFactor)
            {
                speedFactor = greaterDistance / (Time.time - historyAnalyse[i].Key);

                lesser = historyAnalyse[i];
                if (historyAnalyse.Length > i + 1)
                {
                    greater = historyAnalyse[i + 1];
                }
                else
                {
                    greater = new KeyValuePair<float, Vector3>(Time.time, currentPos);
                }

                break;
            }
        }
        
        if (lesser.Equals(default(KeyValuePair<float, Vector3>)))
        {
            lesser = new KeyValuePair<float, Vector3>(Time.time, currentPos);
            greater = new KeyValuePair<float, Vector3>(Time.time, currentPos);
        }

        lesserDistance = greaterDistance - Vector3.Distance(lesser.Value, greater.Value);

        //Debug.Log(gameObject.name + positionHistory.Count);

        //FIND point between points that equals recordgap distance to current
        return Vector3.Lerp(
            lesser.Value,
            greater.Value,
            ((recordGap * speedFactor) - lesserDistance) / (greaterDistance - lesserDistance)
        );
    }
    
    void Start()
    {
        positionHistory = new Queue<KeyValuePair<float, Vector3>>();
        speedFactor = 1f;

        loop = _ETERNAL.R.StartCoroutine(RecordPosition());
    }

    private void OnDestroy()
    {
        StopCoroutine(loop);
    }
}
