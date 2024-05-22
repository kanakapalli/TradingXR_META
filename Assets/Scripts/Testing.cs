using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Experimental.Rendering;

[System.Serializable]
public class Testing : MonoBehaviour
{
    [SerializeField] bool useJobs;

    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        if (useJobs)
        {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle jobHandle = ReallyToughTaskJob();
                jobHandleList.Add(jobHandle);
            }
            JobHandle.CompleteAll(jobHandleList);
            jobHandleList.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }

    private void ReallyToughTask()
    {
        float value = 0f;
        for (int i = 0; i < 2; i++)
        {
            value = math.exp10(math.sqrt(value));
            var m_prefab = Instantiate(FindObjectOfType<API_Call>().m_Stock_Button);
        }
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        return job.Schedule();
    }

    public struct ReallyToughJob : IJob
    {
        public void Execute()
        {
            float value = 0f;
            for (int i = 0; i < 2; i++)
            {
                value = math.exp10(math.sqrt(value));
            }
        }
    }

    private void LateUpdate()
    {
        // Instantiate the prefab in the main thread after all jobs are completed
        var m_prefab = Instantiate(FindObjectOfType<API_Call>().m_Stock_Button);
    }
}
