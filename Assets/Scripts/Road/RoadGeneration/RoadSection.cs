using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEditor.Compilation;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSection : MonoBehaviour
    {
        [SerializeField] protected Transform _startPoint;
        [SerializeField] protected List<Transform> _endPoints; // add multiple ends
        [SerializeField] protected List<RoadSection> _nextRoads; // nextRoads
        [SerializeField] protected RoadSection _previousRoad = null; // nextRoads
        [SerializeField] protected MeshFilter _boundingMesh;
        [SerializeField] protected List<Vector2> topology;

        [SerializeField][ReadOnly] protected bool _infiniteHeight = true;
        [SerializeField][ReadOnly] public int PieceTypeId;
        [SerializeField][ReadOnly] public bool IsFlipped;

        public static List<Type> possibleNeighbours;

        protected RoadSectionShape _shapeRelativeToStart
        {
            get
            {
                if (_localShapeReal == null) _SetShape();
                return _localShapeReal;
            }
            set
            {
                _localShapeReal = value;
            }
        }
        protected RoadSectionShape _localShapeReal;

        private void OnDrawGizmos()
        {
            if (_localShapeReal != null) _shapeRelativeToStart.DebugDraw();
            _DrawEndPoints();
        }

        private void _SetShape()
        {
            _localShapeReal = new RoadSectionShape();
            _localShapeReal.Start = TransformData.FromTransform(_startPoint);
            _localShapeReal.Start.Scale = Vector3.one;
            _localShapeReal.Ends = new();
            foreach (Transform endPoint in _endPoints)
            {
                _localShapeReal.Ends.Add(new TransformData(endPoint.position, endPoint.rotation, Vector3.one));
            }
            _localShapeReal.SetBoundaryFromMesh(_boundingMesh.sharedMesh, TransformData.FromTransform(_boundingMesh.transform), _shapeRelativeToStart.Start, _infiniteHeight);
            _localShapeReal.DebugDraw();
        }

        protected void Start()
        {
            Camera.main.transform.position = new Vector3(
                _startPoint.position.x,
                Camera.main.transform.position.y,
                _startPoint.position.z
            );
        }

        void Update()
        {
            topology = _localShapeReal?._topologyGlobal.GetVertices();
        }

        private void _DrawEndPoints()
        {
            if (_startPoint != null)
            {
                Vector3 startDir = _startPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles;
                DrawArrow.ForGizmo(_startPoint.position - startDir, startDir);
            }
            if (_endPoints != null)
            {
                foreach (Transform endPoint in _endPoints)
                {
                    DrawArrow.ForGizmo(endPoint.position, endPoint.rotation * Quaternion.Euler(0, 0, 1).eulerAngles);
                }
            }
        }

        public void AlignByStartPoint(TransformData newStartPoint)
        {
            TransformData currentStart = TransformData.FromTransform(_startPoint);
            Vector3 rotationChange = newStartPoint.Rotation.eulerAngles - currentStart.Rotation.eulerAngles;
            transform.RotateAround(currentStart.Position, Vector3.up, rotationChange.y);
            Vector3 positionChange = newStartPoint.Position - currentStart.Position;
            transform.position += positionChange;
            _SetShape();
        }

        public RoadSection Clone()
        {
            GameObject clone = Instantiate(gameObject);
            clone.SetActive(true);
            return clone.GetComponent<RoadSection>();
        }

        public RoadSectionShape GetShape()
        {
            return _shapeRelativeToStart;
        }
        public void SetShape(RoadSectionShape roadSectionShape)
        {
            _shapeRelativeToStart = roadSectionShape;
        }

        public void AddNextSection(RoadSection section)
        {
            _nextRoads.Add(section);
            section.SetPreviousSection(this);
        }

        public void SetPreviousSection(RoadSection section)
        {
            _previousRoad = section;
        }

        public RoadSection GetPreviousSection()
        {
            return _previousRoad;
        }

        public List<Transform> GetEndPoints()
        {
            return _endPoints;
        }

        public Transform GetStartPoint()
        {
            return _startPoint;
        }

        //private void OnMouseUp()
        //{
        //    Debug.Log($"Searching follow up for {this.GetType()}");
        //    ARoadGenerator.Instance.PlaceNextRoads(this);
        //}

        public void createNextRoad()
        {
            Debug.Log($"Searching follow up for {this.GetType()}");
            ARoadGenerator.Instance.PlaceNextRoads(this);
        }

        public virtual List<Type> GetPossibleNeighbours()
        {
            return possibleNeighbours;
        }

        public virtual bool IsValidForNextSection(RoadSection section)
        {
            if (GetPossibleNeighbours().Contains(section.GetType()))
            {
                return true;
            }
            return false;
        }
    }
}