using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    static public WorkerController current;
    public Sprite WorkerSprite;
    public string WorkerLayer;
    
    protected class WorkerVisual
    {
        private Worker worker;
        private GameObject visual;

        public Worker Worker
        {
            get
            {
                return worker;
            }
        }

        public GameObject Visual
        {
            get
            {
                return visual;
            }
        }

        public WorkerVisual(Worker worker, GameObject visual)
        {
            this.worker = worker;
            this.visual = visual;
        }
    }

    private List<WorkerVisual> workerVisuals;
    
    protected void Awake()
    {
        if (current == null) current = this;
    }

    public void Setup()
    {
        workerVisuals = new List<WorkerVisual>();

        Game.World.current.OnWorkerCreated += CreateWorker;
        Game.World.current.OnWorkerDestroyed += DestroyWorker;
    }

    protected void OnWorkerMove(Unit unit)
    {
        if (unit is Worker == false) return;
        Worker worker = unit as Worker;
        for (int i = 0; i < workerVisuals.Count; i++)
        {
            if (workerVisuals[i].Worker == worker)
            {
                workerVisuals[i].Visual.transform.position =
                    new Vector2(worker.Position.x * WorldController.TileSize,
                        worker.Position.y * WorldController.TileSize);
            }
        }
    }
    
    public void CreateWorker(Worker worker)
    {
        worker.OnMove += OnWorkerMove;
        GameObject workerGO = new GameObject("Worker");
        
        workerGO.transform.position = new Vector2(worker.Position.x * WorldController.TileSize, worker.Position.y * WorldController.TileSize);
        workerGO.transform.localScale = Vector2.one * WorldController.TileSize;
        
        SpriteRenderer sr = workerGO.AddComponent<SpriteRenderer>();
        sr.sprite = WorkerSprite;
        sr.sortingLayerName = WorkerLayer;
        //sr.color = Color.red;
        
        workerVisuals.Add(new WorkerVisual(worker, workerGO));
    }
    
    protected void DestroyWorker(Worker worker)
    {
        for (int i = 0; i < workerVisuals.Count; i++)
        {
            if (workerVisuals[i].Worker == worker)
            {
                Destroy(workerVisuals[i].Visual);
                workerVisuals.RemoveAt(i);
                return;
            }
        }
    }
}
