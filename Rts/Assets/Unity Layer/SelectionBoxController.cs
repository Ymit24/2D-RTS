using System.Collections.Generic;
using Game;
using UnityEngine;
using Ymit;

public class SelectionBoxController : MonoBehaviour
{
    static public SelectionBoxController current;
    private Vector2 startWorldSpace;
    private Vector2 endWorldSpace;
    private bool showing;

    private GameObject SelectionBoxGo;

    public Sprite SelectionBoxSprite;

    private List<Worker> selectedWorkers;

    public Worker[] GetSelectedWorkers()
    {
        return selectedWorkers.ToArray();
    }
    protected void Awake()
    {
        if (current == null) current = this;

        MouseController.CreateIfNeeded();
        MouseController.OnClick += OnClick;
        
        SelectionBoxGo = new GameObject("SelectionBoxVisual");
        SpriteRenderer sr = SelectionBoxGo.AddComponent<SpriteRenderer>();

        selectedWorkers = new List<Worker>();
        
        sr.sprite = SelectionBoxSprite;
        sr.sortingLayerName = "CanvasLayer";
        sr.sortingOrder = -1;
        sr.color = new Color(0.98f, 0.4f, 1f, 0.5f);
    }

    protected void OnClick(Vector2 position, int button, bool down)
    {
        // left click down
        // left click up
        if (down == true && button == 0)
        {
            startWorldSpace = Utils.WorldMouse();
            showing = true;
            UpdateVisual();
            SelectionBoxGo.SetActive(true);
        }

        if (down == false && button == 0)
        {
            showing = false;
            SelectionBoxGo.SetActive(false);

            SelectUnits();
        }
    }

    private void Update()
    {
        if (showing == false) return;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        endWorldSpace = Utils.WorldMouse();
        
        Vector2 center = new Vector2(startWorldSpace.x + (endWorldSpace.x - startWorldSpace.x)/2, startWorldSpace.y + (endWorldSpace.y - startWorldSpace.y)/2);
            
        SelectionBoxGo.transform.position = center;
        SelectionBoxGo.transform.localScale = new Vector2((endWorldSpace.x-startWorldSpace.x)/1, (endWorldSpace.y-startWorldSpace.y)/1);
    }

    private void SelectUnits()
    {
        if (Input.GetKey(KeyCode.LeftShift) == false)
        {
            selectedWorkers.Clear();
        }

        TileCoord startBounds = new TileCoord(startWorldSpace.x, startWorldSpace.y);
        TileCoord endBounds = new TileCoord(endWorldSpace.x, endWorldSpace.y);

        if (endBounds.x < startBounds.x)
        {
            float tx = startBounds.x;
            startBounds.x = endBounds.x;
            endBounds.x = tx;
        }

        if (endBounds.y < startBounds.y)
        {
            float ty = startBounds.y;
            startBounds.y = endBounds.y;
            endBounds.y = ty;
        }
        
        Worker[] workers = World.current.GetWorkers();
        for (int i = 0; i < workers.Length; i++)
        {
            if (Ymit.Utils.AABB(startBounds.x, startBounds.y, endBounds.x - startBounds.x, endBounds.y - startBounds.y,
                workers[i].Position.x * WorldController.TileSize - WorldController.TileSize / 2,
                workers[i].Position.y * WorldController.TileSize - WorldController.TileSize / 2,
                WorldController.TileSize, WorldController.TileSize))
            {
                if (selectedWorkers.Contains(workers[i]) == false)
                {
                    selectedWorkers.Add(workers[i]);
                }
            }
        }

        Ymit.UI.DebugFadeLabelMouse("Selected " + selectedWorkers.Count + " Workers!");
    }
}