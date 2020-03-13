using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UI_Draw_Base : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    protected int Width, Height;
    protected int Scale = 2; //包括系统优化，优化的同时会降低质量，锯齿比较严重
    public float Radius = 100;
    public RawImage TheDrawingImage;
    protected Texture2D TheDrawingTexture;
    int[,] theBuffer;
    bool isDirty = false;
    bool hasDirty = false;
    Vector2 lastPointPos;
    public Color ClickColor = new Color(1, 0, 0, 1);
    public Color ResetColor = new Color(1, 1, 1, 1);
    RectTransform _rect;
    Rect rectOfRoot;
    Camera cam;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Init();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUIContent ui = new GUIContent();
        GUILayout.Space(10f);
        ui.text = string.Format("<color=#000000ff>{0}</color>", Radius.ToString());
        GUILayout.Label(ui, GUILayout.Width(30), GUILayout.Height(30));
        Radius = GUILayout.HorizontalSlider(Radius, 5, 60, GUILayout.Width(150), GUILayout.Height(30));
        GUILayout.EndHorizontal();
        GUILayout.Label("<color=#ffffffff>R来初始化书写界面</color>", GUILayout.Width(140));
        GUILayout.EndVertical();
    }
    public void Init()
    {
        _rect = TheDrawingImage.rectTransform;
        rectOfRoot = _rect.rect;
        Height = (int)(TheDrawingImage.rectTransform.rect.height / Scale);
        Width = (int)(TheDrawingImage.rectTransform.rect.width / Scale);
        theBuffer = new int[Width, Height];
        ResetTexture();
        CanDraw = true;
    }

    public void ResetTexture()
    {
        TheDrawingTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false); //默认default材质
        TheDrawingImage.material.mainTexture = TheDrawingTexture;
        if (hasDirty)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    theBuffer[i, j] = 0;
                    //TheDrawingTexture.SetPixel(i, j, ResetColor);
                }
            }
            hasDirty = false;
        }
        TheDrawingTexture.Apply();
        TheDrawingImage.color = ResetColor;
        TheDrawingImage.RecalculateMasking();
    }
    System.Action PointDwonAction;
    System.Action PointUpAction;
    protected bool CanDraw = false;
    public bool CanGameDraw = false;     // 控制书写
    int pointId = 25;
    int rightPointId = 26;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cam == null) cam = eventData.pressEventCamera;
        if (CanDraw)
        {
            PointDwonAction?.Invoke();
            CanDraw = false;
            Vector2 pos;
            eventData.pointerId = ++pointId;
            if (eventData.pointerId == rightPointId)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, eventData.position, cam, out pos))
                {
                    lastPointPos = pos;
                }
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == rightPointId)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, eventData.position, cam, out pos))
            {
                if (pos.x < -rectOfRoot.width / 2
                    || pos.y < -rectOfRoot.height / 2
                    || pos.x > rectOfRoot.width / 2
                    || pos.y > rectOfRoot.height / 2)
                    return;

                float dis = Vector2.Distance(pos, lastPointPos);
                if (dis > Radius / 3)
                {
                    //lerp
                    if (dis > Radius / 2)
                    {
                        int num = (int)dis / (int)(Radius / 3);
                        Vector2 dir = pos - lastPointPos;
                        dir = dir.normalized;
                        for (int i = 0; i < num - 1; i++)
                        {
                            Vector2 thePos = lastPointPos + dir * (i + 1) * (int)(Radius / 3);
                            DealTexture2D(new Vector2((int)thePos.x, (int)thePos.y));
                        }
                    }
                    DealTexture2D(pos);
                    lastPointPos = pos;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == rightPointId)
        {
            CanDraw = true;
            PointUpAction?.Invoke();
            pointId = 25;
        }
    }
    protected int max_process_pixel = 10000;
    private void LateUpdate()
    {
        if (isDirty)
        {
            int processCount = 0;
            for (int x = 0; x < Width; x++)
            {
                if (processCount > max_process_pixel) break;
                for (var y = 0; y < Height; y++)
                {

                    if (processCount > max_process_pixel) break;
                    if (theBuffer[x, y] == 3)
                    {
                        hasDirty = true;
                        theBuffer[x, y] = 1;
                        TheDrawingTexture.SetPixel(x, y, ClickColor);
                        processCount++;
                    }
                }
            }
            if (processCount < max_process_pixel) isDirty = false;
            TheDrawingTexture.Apply();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTexture();
        }
    }

    protected virtual void DealTexture2D(Vector2 po)
    {
        po += new Vector2((int)rectOfRoot.width / 2, (int)rectOfRoot.height / 2);

        int r = (int)Radius;
        int ox = (int)po.x, oy = (int)po.y;
        for (int i = -r; i < r; i++)
        {
            for (int j = -r; j < r; j++)
            {
                int x = (int)((i + ox) / Scale);
                int y = (int)((j + oy) / Scale);
                if (x < 0 || y < 0 || x >= Width || y >= Height) continue;
                if (theBuffer[x, y] > 0) continue;
                if (i * i + j * j <= r * r)
                {
                    theBuffer[x, y] = 3;
                    this.isDirty = true;
                }
            }
        }
    }
}
