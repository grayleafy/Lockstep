using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderGroup
{
    public Vector3 position;
    public GameObject model;
    public Camera camera;
    public RenderTexture renderTexture;
}




public class ModelRenderMgr : Singleton<ModelRenderMgr>
{
    //项目中需要设置
    public string ab = "displaymodel";
    public string[] masks = { "UI" };


    Vector3 deltaPos = new Vector3(100, 0, 0);
    Queue<Vector3> freePosition = new Queue<Vector3>();
    Dictionary<string, RenderGroup> renderGroups = new Dictionary<string, RenderGroup>();

    public Vector3 cameraPos = new Vector3(0.43f, 1.57f, 1.3f);
    public Quaternion cameraRot = Quaternion.Euler(16.0f, -158.7f, 0);

    public int pixelWidth = 1024;
    public int pixelHeight = 1024;

    public ModelRenderMgr()
    {
        //初始化空闲区域
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 100; i++)
        {
            freePosition.Enqueue(pos);
            pos += deltaPos;
        }
    }

    /// <summary>
    /// 获取指定角色的渲染纹理
    /// </summary>
    /// <param name="characterFullName"></param>
    /// <returns></returns>
    public RenderTexture GetRenderTexture(string characterFullName)
    {
        if (renderGroups.ContainsKey(characterFullName) == false)
        {
            renderGroups.Add(characterFullName, CreateRenderGroup(characterFullName));
        }

        return renderGroups[characterFullName].renderTexture;
    }

    /// <summary>
    /// 清理一个
    /// </summary>
    /// <param name="characterFullName"></param>
    public void ClearRenderGroup(string characterFullName)
    {
        if (renderGroups.ContainsKey(characterFullName))
        {
            DestroyRenderGroup(renderGroups[characterFullName]);
            renderGroups.Remove(characterFullName);
        }
    }

    public void ClearAll()
    {
        foreach (RenderGroup renderGroup in renderGroups.Values)
        {
            DestroyRenderGroup(renderGroup);
        }

        renderGroups.Clear();
    }

    RenderGroup CreateRenderGroup(string characterFullName)
    {
        RenderGroup renderGroup = new RenderGroup();
        renderGroup.position = freePosition.Dequeue();
        renderGroup.model = GameObject.Instantiate(ABMgr.Instance.LoadRes<GameObject>(ab, characterFullName));
        renderGroup.model.transform.position = renderGroup.position;
        renderGroup.model.SetLayerWithAllChildren(LayerMask.NameToLayer(masks[0]));
        renderGroup.camera = (new GameObject("renderCamera" + characterFullName)).AddComponent<Camera>();
        renderGroup.camera.transform.position = cameraPos + renderGroup.position;
        renderGroup.camera.transform.rotation = cameraRot;
        renderGroup.camera.cullingMask = LayerMask.GetMask(masks);
        renderGroup.camera.clearFlags = CameraClearFlags.Color;
        renderGroup.renderTexture = RenderTexture.GetTemporary(pixelWidth, pixelHeight, 32);
        renderGroup.camera.targetTexture = renderGroup.renderTexture;

        return renderGroup;
    }

    void DestroyRenderGroup(RenderGroup renderGroup)
    {
        renderGroup.camera.targetTexture = null;
        RenderTexture.ReleaseTemporary(renderGroup.renderTexture);

        GameObject.Destroy(renderGroup.model);
        GameObject.Destroy(renderGroup.camera.gameObject);

        freePosition.Enqueue(renderGroup.position);
    }
}
