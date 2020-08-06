using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimationCurve_DrawLine
{
    /// <summary>
    /// 路线规划
    /// </summary>
    public class WalkRoute : MonoBehaviour
    {
        private List<Transform> pointsList = new List<Transform>();
        private PositionCurve _positionCurve = new PositionCurve();
        private PositionCurve _positionLoopCurve = new PositionCurve();
        private RotateCurve _rotationCurve = new RotateCurve();
        private RotateCurve _rotationloopCurve = new RotateCurve();
        private float RoadLength;
        private float _loopRoadLength;
        private List<float> disList = new List<float>();

        public bool CanMove = false;
        public bool RunningShowDraw = false;
        public bool LookAtForward = false;
        public int LineNum = 200;
        public float Speed;
        public Transform WayRoot;
        public Transform MoveItem;
        public bool Loop = false;

        /// <summary>
        /// 如果重构，必须基于基类，或者调用 ResetWayState
        /// </summary>
        public virtual void Start()
        {
            ResetWayState();
        }

        /// <summary>
        /// 如果重构，必须基于的绘制基类
        /// </summary>
        public virtual void OnDrawGizmos()
        {
            if (Application.isPlaying && !RunningShowDraw)
                return;
            if (WayRoot == null)
                return;
            if (ResetWayState())
            {
                float norm = 1f / LineNum;
                for (int i = 0; i < LineNum; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(_positionCurve.GetValue(i * norm), _positionCurve.GetValue((i + 1) * norm));
                    Gizmos.color = Color.blue;
                    if (LookAtForward)
                        Gizmos.DrawLine(_positionCurve.GetValue(i * norm), _positionCurve.GetValue(i * norm) + _rotationCurve.GetValue(norm * i) * Vector3.forward * 2);
                }
            }
        }
        float _wayDis = 0;
        /// <summary>
        /// 如果重构，必须基于基类
        /// </summary>
        public virtual void Update()
        {
            if (CanMove && MoveItem != null)
            {
                _wayDis += Speed * Time.deltaTime;
                float time = _wayDis / RoadLength;
                MoveItem.position = _positionCurve.GetValue(time);
                if (LookAtForward)
                {
                    float upTime = (_wayDis + 0.01f) / RoadLength;
                    MoveItem.rotation = Quaternion.LookRotation(_positionCurve.GetValue(upTime) - _positionCurve.GetValue(time), _rotationCurve.GetValue(time) * Vector3.up);
                }
            }
        }
        /// <summary>
        /// 获取时间线上的位置
        /// </summary>
        /// <param name="time">当前归一化时间</param>
        /// <returns></returns>
        public Vector3 GetPosition(float time)
        {
            return _positionCurve.GetValue(time);
        }

        /// <summary>
        /// 重置路程状态
        /// </summary>
        public void ResetTheWayState()
        {
            Loop = false;
            LookAtForward = false;
            CanMove = false;
            _wayDis = 0;
            ResetWayState();
        }
     

        bool ResetWayState()
        {
            _positionCurve.ResetCurveState();
            _rotationCurve.ResetCurveState();
            if (WayRoot.childCount > 2)
            {
                pointsList.Clear();
                disList.Clear();
                RoadLength = 0;
                _loopRoadLength = 0;

                for (int i = 0; i < WayRoot.childCount; i++)
                {
                    pointsList.Add(WayRoot.GetChild(i));
                }
                ComputeRoad();
                AddKey();
                return true;
            }
            return false;
        }

        void ComputeRoad()
        {
            if (Loop)
            {
                for (int i = 0; i < pointsList.Count; i++)
                {
                    float dis = Vector3.Distance(pointsList[(i + 1) % pointsList.Count].position, pointsList[i].position);
                    RoadLength += dis;
                    disList.Add(dis);
                }
                _loopRoadLength = disList[0] + disList[1] + disList[disList.Count - 1] + disList[disList.Count - 2];
            }
            else
            {
                for (int i = 1; i < pointsList.Count; i++)
                {
                    float dis = Vector3.Distance(pointsList[i - 1].position, pointsList[i].position);
                    RoadLength += dis;
                    disList.Add(dis);
                }
            }

        }
        void AddKey()
        {
            if (Loop)      //如果循环，取四个点来确定结尾和开始的入射和出射方向
            {
                float timelop = 0;
                _positionLoopCurve.SetKey(0, pointsList[pointsList.Count - 2].position);
                timelop = disList[disList.Count - 2] / _loopRoadLength;
                _positionLoopCurve.SetKey(timelop, pointsList[pointsList.Count - 1].position);
                timelop = disList[disList.Count - 1] / _loopRoadLength;
                _positionLoopCurve.SetKey(timelop, pointsList[0].position);
                _positionLoopCurve.SetKey(1, pointsList[1].position);

                List<Vector2> tangent = _positionLoopCurve.GetTangent();
                _rotationloopCurve.AddKey(0, Quaternion.LookRotation(pointsList[0].position - pointsList[pointsList.Count - 1].position, pointsList[pointsList.Count - 1].up));
                _rotationloopCurve.AddKey(timelop, Quaternion.LookRotation(pointsList[1].position - pointsList[0].position, pointsList[0].up));
                _rotationloopCurve.AddKey(1, Quaternion.LookRotation(pointsList[1].position - pointsList[0].position, pointsList[1].up));

                _rotationCurve.SetModule(WrapMode.Loop);
                _positionCurve.SetModule(WrapMode.Loop);
                float dis = 0;
                _positionCurve.SetKey(0, pointsList[0].position);
                _rotationCurve.AddKey(0, Quaternion.LookRotation(pointsList[1].position - pointsList[0].position, pointsList[0].up), _rotationloopCurve.GetTangent());
                for (int i = 1; i < pointsList.Count; i++)
                {
                    dis += disList[i - 1];
                    float time = dis / RoadLength;
                    _positionCurve.SetKey(time, pointsList[i].position);
                    if (i == pointsList.Count - 1)
                    {
                        _rotationCurve.AddKey(time, Quaternion.LookRotation(pointsList[i].position - pointsList[i - 1].position, pointsList[i].up));
                    }
                    else
                    {
                        _rotationCurve.AddKey(time, Quaternion.LookRotation(pointsList[i + 1].position - pointsList[i].position, pointsList[i].up));
                    }
                }
                _positionCurve.SetKey(1, pointsList[0].position);
            }
            else
            {
                float dis = 0;
                _positionCurve.SetKey(0, pointsList[0].position);
                for (int i = 1; i < pointsList.Count; i++)
                {
                    dis += disList[i - 1];
                    float time = dis / RoadLength;
                    _positionCurve.SetKey(time, pointsList[i].position);
                    if (i == pointsList.Count - 1)
                    {
                        _rotationCurve.AddKey(time, Quaternion.LookRotation(pointsList[i].position - pointsList[i - 1].position, pointsList[i].up));
                    }
                    else
                    {
                        _rotationCurve.AddKey(time, Quaternion.LookRotation(pointsList[i + 1].position - pointsList[i].position, pointsList[i].up));
                    }
                }
            }
        }
    }

    class PositionCurve
    {
        AnimationCurve X, Y, Z;
        List<Keyframe> Xkey = new List<Keyframe>();
        List<Keyframe> Ykey = new List<Keyframe>();
        List<Keyframe> Zkey = new List<Keyframe>();
        public PositionCurve()
        {
            ResetCurveState();
        }
        void Getkeys()
        {
            Xkey.Clear();
            Ykey.Clear();
            Zkey.Clear();

            Xkey.AddRange(X.keys);
            Ykey.AddRange(Y.keys);
            Zkey.AddRange(Z.keys);
        }
        public List<Vector2> GetTangent()
        {
            Getkeys();
            List<Vector2> back = new List<Vector2>();
            int index = Xkey.Count % 2;

            Vector2 xv = new Vector2(Xkey[index].inTangent, Xkey[index].outTangent);
            Vector2 yv = new Vector2(Ykey[index].inTangent, Ykey[index].outTangent);
            Vector2 zv = new Vector2(Zkey[index].inTangent, Zkey[index].outTangent);

            back.Add(xv);
            back.Add(yv);
            back.Add(zv);
            return back;
        }


        public void SetKey(float time, Vector3 position, List<Vector2> tangent = null)
        {
            if (tangent != null)
            {
                Keyframe xk = new Keyframe(time, position.x, tangent[0].x, tangent[0].y);
                X.AddKey(xk);
                Keyframe yk = new Keyframe(time, position.y, tangent[1].x, tangent[1].y);
                Y.AddKey(yk);
                Keyframe zk = new Keyframe(time, position.z, tangent[2].x, tangent[2].y);
                Z.AddKey(zk);
                xk.inWeight = xk.outWeight = yk.inWeight = yk.outWeight = zk.inWeight = zk.outWeight = 0.5f;
            }
            else
            {
                X.AddKey(time, position.x);
                Y.AddKey(time, position.y);
                Z.AddKey(time, position.z);
            }
        }

        public Vector3 GetValue(float time)
        {
            return new Vector3(X.Evaluate(time), Y.Evaluate(time), Z.Evaluate(time));
        }

        public void SetModule(WrapMode mode)
        {
            X.postWrapMode = mode;
            X.preWrapMode = mode;

            Y.postWrapMode = mode;
            Y.preWrapMode = mode;

            Z.postWrapMode = mode;
            Z.preWrapMode = mode;
        }

        public void ResetCurveState()
        {
            X = new AnimationCurve();
            Y = new AnimationCurve();
            Z = new AnimationCurve();
        }
    }
    class RotateCurve
    {

        AnimationCurve X, Y, Z, W;
        List<Keyframe> Xkey = new List<Keyframe>();
        List<Keyframe> Ykey = new List<Keyframe>();
        List<Keyframe> Zkey = new List<Keyframe>();
        List<Keyframe> Wkey = new List<Keyframe>();
        public RotateCurve()
        {
            ResetCurveState();
        }

        public void AddKey(float time, Quaternion qua, List<Vector2> tangent = null)
        {
            if (tangent != null)
            {
                Keyframe xk = new Keyframe(time, qua.x, tangent[0].x, tangent[0].y);
                X.AddKey(xk);
                Keyframe yk = new Keyframe(time, qua.y, tangent[1].x, tangent[1].y);
                Y.AddKey(yk);
                Keyframe zk = new Keyframe(time, qua.z, tangent[2].x, tangent[2].y);
                Z.AddKey(zk);
                Keyframe wk = new Keyframe(time, qua.w, tangent[3].x, tangent[3].y);
                W.AddKey(wk);
            }
            else
            {
                X.AddKey(time, qua.x);
                Y.AddKey(time, qua.y);
                Z.AddKey(time, qua.z);
                W.AddKey(time, qua.w);
            }

        }

        void Getkeys()
        {
            Xkey.Clear();
            Ykey.Clear();
            Zkey.Clear();
            Wkey.Clear();

            Xkey.AddRange(X.keys);
            Ykey.AddRange(Y.keys);
            Zkey.AddRange(Z.keys);
            Wkey.AddRange(W.keys);
        }
        public List<Vector2> GetTangent()
        {
            Getkeys();
            List<Vector2> back = new List<Vector2>();
            int index = Xkey.Count % 2;

            Vector2 xv = new Vector2(Xkey[index].inTangent, Xkey[index].outTangent);
            Vector2 yv = new Vector2(Ykey[index].inTangent, Ykey[index].outTangent);
            Vector2 zv = new Vector2(Zkey[index].inTangent, Zkey[index].outTangent);
            Vector2 wv = new Vector2(Wkey[index].inTangent, Wkey[index].outTangent);

            back.Add(xv);
            back.Add(yv);
            back.Add(zv);
            back.Add(wv);
            return back;
        }
        public Quaternion GetValue(float time)
        {
            return new Quaternion(X.Evaluate(time), Y.Evaluate(time), Z.Evaluate(time), W.Evaluate(time));
        }
        public void ResetCurveState()
        {
            X = new AnimationCurve();
            Y = new AnimationCurve();
            Z = new AnimationCurve();
            W = new AnimationCurve();
        }
        public void SetModule(WrapMode mode)
        {
            X.postWrapMode = mode;
            X.preWrapMode = mode;

            Y.postWrapMode = mode;
            Y.preWrapMode = mode;

            Z.postWrapMode = mode;
            Z.preWrapMode = mode;

            W.postWrapMode = mode;
            W.preWrapMode = mode;
        }
    }
}
