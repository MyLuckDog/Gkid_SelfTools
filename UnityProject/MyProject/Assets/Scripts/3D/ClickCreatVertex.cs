using MyTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 不考虑算法优化，如有卡顿、堵塞自行优化
/// </summary>

[RequireComponent(typeof(MeshCollider))]
public class ClickCreatVertex : MonoBehaviour
{
    Mesh currentMesh;
    Mesh NewMesh;
    public Material NewMeshMaterial;
    [Header("当前三角面向外扩散圈数,1是当前三角形，最多扩散2圈")]
    [Range(1, 3)]
    public int RangCount = 1;
    private void Start()
    {
        currentMesh = GetComponent<MeshFilter>().mesh;
        OldVerList.AddRange(currentMesh.vertices);
        OldNormalList.AddRange(currentMesh.normals);
        OldTangentList.AddRange(currentMesh.tangents);
        OldTriangleList.AddRange(currentMesh.triangles);

        NewMesh = new Mesh();

        GameObject g = new GameObject("新物体");
        MeshFilter mesf = g.AddComponent<MeshFilter>();
        mesf.mesh = NewMesh;
        MeshRenderer render = g.AddComponent<MeshRenderer>();
        render.material = NewMeshMaterial;
        render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        render.receiveShadows = false;


        g.transform.position = transform.position;
        g.transform.localScale = transform.localScale;
        g.transform.localRotation = transform.localRotation;
    }
    List<int> OldTriangleList = new List<int>();
    List<Vector3> OldVerList = new List<Vector3>();
    List<int> oldVerIndexList = new List<int>(); //存放顶点索引
    List<Vector3> OldNormalList = new List<Vector3>();
    List<Vector4> OldTangentList = new List<Vector4>();

    List<int> UseTriangleList = new List<int>();  //三角形索引

    List<int> NewTriangleList = new List<int>();
    List<Vector3> NewVerList = new List<Vector3>();
    List<Vector3> NewNormalList = new List<Vector3>();
    List<Vector4> NewTangentList = new List<Vector4>();

    float Radius;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                int index = hit.triangleIndex;
                RayClickGO(index);
            }
        }

    }

    void RayClickGO(int clickIndex)
    {
        secondBound.Clear();
        lastBound.Clear();
        DealPointRange(RangCount, clickIndex);

        NewMesh.vertices = NewVerList.ToArray();
        NewMesh.triangles = NewTriangleList.ToArray();
        NewMesh.normals = NewNormalList.ToArray();
        NewMesh.tangents = NewTangentList.ToArray();

        for (int i = 0; i < secondBound.Count; i++)
        {
            if (lastBound.Contains(secondBound[i]))
                lastBound.Remove(secondBound[i]);
        }
    }

    List<int> secondBound = new List<int>();
    List<int> lastBound = new List<int>();
    int BoundNum = 2;
    void DealPointRange(int count, params int[] indexs)  //往外扩展count圈
    {
        count--;
        List<int> nextRang = new List<int>();

        for (int i = 0; i < indexs.Length; i++)
        {
            int index = indexs[i];
            if (UseTriangleList.Contains(index))
            {
                if (count > 0)
                    nextRang.AddRange(NextRange(index));
                continue;
            }
            UseTriangleList.Add(index);

            switch (count)
            {
                case 1:
                    secondBound.AddRange(AddDealOneVer(index));
                    break;
                case 0:
                    lastBound.AddRange(AddDealOneVer(index));
                    break;
                default:
                    AddDealOneVer(index);
                    break;
            }

            if (count > 0)
                nextRang.AddRange(NextRange(index));
        }
        if (nextRang.Count > 0)
            DealPointRange(count, nextRang.ToArray());
    }
    List<int> AddDealOneVer(int index)
    {
        int[] verIndex = new int[3];
        for (int i = 0; i < verIndex.Length; i++)
        {
            int theIndex = OldTriangleList[index * 3 - i + 2];
            int thisVerIndex = 0;
            if (oldVerIndexList.Contains(theIndex))
            {
                thisVerIndex = oldVerIndexList.IndexOf(theIndex);
            }
            else
            {
                oldVerIndexList.Add(theIndex);
                NewVerList.Add(OldVerList[theIndex] + OldNormalList[theIndex].normalized * 0.03f);
                NewNormalList.Add(OldNormalList[theIndex]);
                NewTangentList.Add(OldTangentList[theIndex]);
                thisVerIndex = NewVerList.Count - 1;
            }
            verIndex[i] = thisVerIndex;
        }
        for (int i = verIndex.Length - 1; i >= 0; i--)
        {
            NewTriangleList.Add(verIndex[i]);
        }
        return verIndex.ToList();
    }
    List<int> NextRange(int index)
    {
        List<int> backList = new List<int>();
        List<int> OneItemIndexs = new List<int>();
        OneItemIndexs.Add(index * 3 + 2);
        OneItemIndexs.Add(index * 3 + 1);
        OneItemIndexs.Add(index * 3);


        for (int j = 0; j < OneItemIndexs.Count; j++)
        {
            var oneList = OldTriangleList.FindAllIndex(OldTriangleList[OneItemIndexs[j]]);    // 其中一个顶点的周围信息
            foreach (var item in oneList)
            {
                int next = item / 3;
                if (next == index) continue;
                if (backList.Contains(next)) continue;
                backList.Add(item / 3);
            }
        }
        return backList;
    }

    bool draw = true;
    private void OnDrawGizmos()
    {
        if (NewVerList.Count > 0 && draw)
        {
            draw = false;
            Gizmos.DrawSphere(NewVerList[0], 1f);
        }
    }
}
