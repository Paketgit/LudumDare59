using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int CountOfTasks;
    private int CoundOfTasksEnded;
    public static Action TaskEnded;
    public static Action TaskIncomplite;    
    void Start()
    {
        G.currentGameManager = this;
        TaskEnded += TaskCheck;
        TaskIncomplite += TaskIncomplitet;
    }

    private void TaskIncomplitet()
    {
        CoundOfTasksEnded--;
    }

    void Update()
    {
            
    }

    public void TaskCheck()
    {
        CoundOfTasksEnded += 1;
        Debug.Log("Tasks ended " + CoundOfTasksEnded);
        if(CountOfTasks == CoundOfTasksEnded)
        {
            SceneManager.UnloadSceneAsync("TestCurveMove");
        }
    }
}
