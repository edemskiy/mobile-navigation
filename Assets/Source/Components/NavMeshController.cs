using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController: MonoBehaviour 
{
    /* для генерации navmesh в runtime
    [SerializeField]
    NavMeshSurface[] navMeshSufaces;
    */

    public GameObject player;
    public LevelsController levelsController;
    public GameObject defaultLevel;
    NavMeshAgent navMeshAgent;
    private float prevActiveLevelPositionY;

    // материал линий из которых состоит путь
    public Material pathLineMaterial;
    
    // целевая точка пути
    public GameObject target;
    
    // объект пути в котором будут храниться точки
    private NavMeshPath path;
    // объект пути в который будут "складываться" его части (отрезки пути)
    private GameObject pathStore;

    private void Start() 
	{
        // проверяем есть ли компонент NavMeshAgent
        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.Log("NavMesh component is not attached");
        }
        
        // строим NavMesh для всех поверхностей (долго!)
        /*
        for (int i = 0; i < navMeshSufaces.Length; i++)
        {
            navMeshSufaces[i].BuildNavMesh();
        }
        */

        levelsController.SetActive(defaultLevel);

        path = new NavMeshPath();
        pathStore = new GameObject("Path");
        prevActiveLevelPositionY = LevelButton.activeLevelPositionY;
        player.SetActive(false);

        string userPositionString = PlayerPrefs.GetString(AppUtils.JSON_LOCATION, "NaN");
        if(userPositionString != "NaN")
        {
            SetSource(AppUtils.stringToVector3(userPositionString));
        }

        int userPositionLevel = PlayerPrefs.GetInt(AppUtils.JSON_FLOOR, 0);
        /*
        if (userPositionLevel != 0)
        {
            levelsController.SetActive(levelsController.levels[userPositionLevel-1]);
        }
        else
        {
            levelsController.SetActive(defaultLevel);
        }
        */
        
    }

    public void DisableNavAgent()
    {
        navMeshAgent.enabled = false;
    }
    public void EnableNavAgent()
    {
        navMeshAgent.enabled = true;
    }

    // визуализация одного отрезка пути
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start + Vector3.up * 0.2f;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();

        myLine.GetComponent<Renderer>().material = pathLineMaterial;
        myLine.GetComponent<Renderer>().material.color = color;

        lr.startWidth = 0.3f;
        lr.endWidth = 0.3f;
        lr.numCapVertices = 21; // уровень скругленности 0-21
        lr.SetPosition(0, start + Vector3.up * 0.2f);
        lr.SetPosition(1, end + Vector3.up * 0.2f);
        myLine.transform.parent = pathStore.transform;
    }

    // расчет и визуализация пути
    public void DrawPath()
    {
        //Debug.Log(LevelSwithcer.activeLevelPositionY);
        // удаляем визуализацию предыдущего пути
        foreach (Transform line in pathStore.transform)
        {
            Destroy(line.gameObject);
        }
        path.ClearCorners();

        player.SetActive(true);
        // если маршрут построен, то визуализируем его, строя линии по точкам        
        if (navMeshAgent.CalculatePath(target.transform.position, path))
        {
            if ((path.corners[path.corners.Length - 1] - target.transform.position).magnitude > 0.1f)
            {
                Debug.Log("Невозможно построить маршрут");
                player.SetActive((player.transform.position.y < levelsController.getActiveLevelPosition().y) && (player.transform.position != Vector3.zero));
                return;
            }
            
            for (int i = 1; i < path.corners.Length; i++)
            {
                DrawLine(path.corners[i], path.corners[i - 1], Color.green);
            }
        }
        ShowOnlyActiveFloorPath();
    }

    public Vector3 getPosition()
	{
		return navMeshAgent.transform.position;
	}

    // установка исходной точки пути
	public void SetSource(Vector3 position)
	{
        player.SetActive(true);
        DisableNavAgent();
        navMeshAgent.Warp(position);
        EnableNavAgent();
        DrawPath();        
    }

    // установка конечной точки пути
    public void SetDestination(Vector3 position)
    {
        target.SetActive(true);
        target.transform.position = position;
        DrawPath();
    }

    private void ShowOnlyActiveFloorPath()
    {
        foreach (Transform line in pathStore.transform)
        {
            line.gameObject.SetActive(line.gameObject.transform.position.y < levelsController.getActiveLevelPosition().y + 1.3f);
        }
        player.SetActive((player.transform.position.y < levelsController.getActiveLevelPosition().y) && (player.transform.position != Vector3.zero));
        target.SetActive((target.transform.position.y < levelsController.getActiveLevelPosition().y) && (target.transform.position != Vector3.zero));
    }

    public void ResetPath()
    {
        foreach (Transform line in pathStore.transform)
        {
            Destroy(line.gameObject);
        }
        path.ClearCorners();
        SetSource(Vector3.zero);
        SetDestination(Vector3.zero);
        player.SetActive(false);
        target.SetActive(false);
    }

    private void Update()
    {

        /*
        if (prevActiveLevelPositionY != levelsController.getActiveLevelPosition().y)
        {
            ShowOnlyActiveFloorPath();           
            prevActiveLevelPositionY = levelsController.getActiveLevelPosition().y;
        }
        */
        
    }
}
