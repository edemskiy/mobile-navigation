using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

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

    // материал линий из которых состоит путь
    public Material pathLineMaterial;
    
    // целевая точка пути
    public GameObject target;
    
    // объект пути
    private NavMeshPath path;
    // объект пути в который будут "складываться" его части (отрезки пути)
    private GameObject pathStore;

    private UnityAction<string> floorChangeListener;

    private void Start() 
	{
        // строим NavMesh для всех поверхностей (долго!)
        /*
        for (int i = 0; i < navMeshSufaces.Length; i++)
        {
            navMeshSufaces[i].BuildNavMesh();
        }
        */
                
        player.SetActive(false);
        
        player.GetComponent<Renderer>().enabled = false;
        target.GetComponent<Renderer>().enabled = false;
    }

    // Функционал обработки событий. Начало.
    private void Awake()
    {
        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.Log("NavMesh component is not attached");
        }
        path = new NavMeshPath();
        pathStore = new GameObject("Path");

        floorChangeListener = new UnityAction<string>(OnFloorChange);
    }

    private void OnEnable()
    {
        EventManager.StartListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening(AppUtils.floorChanged, floorChangeListener);
    }

    private void OnFloorChange(string s)
    {
        ShowOnlyActiveFloorPath();
    }

    public void DisableNavAgent()
    {
        navMeshAgent.enabled = false;
    }

    public void EnableNavAgent()
    {
        navMeshAgent.enabled = true;
    }
    // Функционал обработки событий. Конец.

    // визуализация одного отрезка пути
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        if (start.y != end.y) // не отображаем наклонные линии пути (на лестницах)?
            return;

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
        // удаляем визуализацию предыдущего пути
        foreach (Transform line in pathStore.transform)
        {
            Destroy(line.gameObject);
        }
        path.ClearCorners();

        // активируем игрока
        player.SetActive(true); // можно убрать, так как он невидимый?

        // если маршрут построен, то визуализируем его, строя линии по точкам        
        if (navMeshAgent.CalculatePath(target.transform.position, path))
        {
            // if ((path.corners[path.corners.Length - 1] - target.transform.position).magnitude > 0.5f)
            if (Math.Abs(path.corners[path.corners.Length - 1].y - target.transform.position.y) > 2)
            {
                Debug.Log("Невозможно построить маршрут");
                
                //* Зачем?? 
                bool cond = (player.transform.position.y < LevelsController.activeLevelPosition.y)
                    && (player.transform.position != Vector3.zero);
                player.SetActive(cond);
                //*/

                return;
            }
            
            for (int i = 1; i < path.corners.Length; i++)
            {
                DrawLine(path.corners[i], path.corners[i - 1], AppUtils.LightYellowColor);
            }
        }

        // отображаем только нужную часть пути
        ShowOnlyActiveFloorPath();
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

    // показать участки пути только на активном этаже
    private void ShowOnlyActiveFloorPath()
    {
        foreach (Transform line in pathStore.transform)
        {
            line.gameObject.SetActive(line.gameObject.transform.position.y < LevelsController.activeLevelPosition.y - 2f);
        }
        player.SetActive((player.transform.position.y < LevelsController.activeLevelPosition.y) && (player.transform.position != Vector3.zero));
        target.SetActive((target.transform.position.y < LevelsController.activeLevelPosition.y) && (target.transform.position != Vector3.zero));
    }

    // построен ли путь? (объект пути содержит больше двух точек)
    public bool IsPathBuilt()
    {
        return path.corners.Length > 2;
    }

    // поменять местами точки "А" и "Б"
    public void SwitchRoutePoints()
    {
        Vector3 newSource = path.corners[path.corners.Length - 1];
        Vector3 newDestination = path.corners[0];

        SetSource(newSource);
        SetDestination(newDestination);
    }

    // сбросить путь и очистить карту
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
}
