using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cross : MonoBehaviour, OnTouch3D
{
    public Text messageText;
    public GameObject arSessionOrigin;
    
    private int crossID;
    private GameplayManager gameplayManager;

    // Start is called before the first frame update
    void Start()
    {
        var crossName = gameObject.name;
        crossID = crossName[crossName.Length - 1] - '0';
        gameplayManager = arSessionOrigin.GetComponent<GameplayManager>();
    }

    public void OnTouch()
    {
        gameplayManager.PutCross(crossID);
    }

    public void SetCrossActive(bool isActive)
    {
        var childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in childrenMeshRenderers)
        {
            r.enabled = isActive;
        }
    }
}
