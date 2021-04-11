using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    private EntityManager manager;
    public Entity CharacterEntity = Entity.Null;
    public float3 offset;

    public bool isManagerReady = false;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (isManagerReady == false)
        {
            foreach (var world in World.All)
            {
                if (world.Name.Equals("ClientWorld"))
                {
                    manager = world.EntityManager;
                    isManagerReady = true;
                    Debug.Log("查找到ClientWorld的EntityManager");
                }
                    
            }
        }
    }

    private void LateUpdate()
    {
        if (isManagerReady == false) return;
        if (CharacterEntity == Entity.Null) return;

        Translation characterPos = manager.GetComponentData<Translation>(CharacterEntity);
        transform.position = characterPos.Value + offset;
    }
}
