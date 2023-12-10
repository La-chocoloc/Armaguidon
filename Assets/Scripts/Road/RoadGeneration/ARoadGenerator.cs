using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace JonathonOH.RoadGeneration
{
    public abstract class ARoadGenerator : MonoBehaviour
    {
        [Serializable]
        private struct PresetPiece
        {
            public bool flipped;
            public int pieceIndex;
        }
        [SerializeField] private int _choiceEngineStepsPerFrame = 1;
        [SerializeField] private int _choiceEngineCheckDepth = 5;
        [SerializeField] protected List<RoadSection> _flippableRoadSectionChoices;
        [SerializeField] protected List<RoadSection> _notFlippableRoadSectionChoices;
        [SerializeField] protected RoadSectionDeadEnd _DeadEndRoadSection;
        [SerializeField] private Transform _roadSectionContainer;
        [SerializeField] private bool _autoHorizontalFlipPieces = true;
        [SerializeField] private bool placeAllPresetPiecesOnStart = true;
        [SerializeField] private List<PresetPiece> piecesToPlaceFirst;
        [SerializeField] protected int _targetRoadLength = 10;
        private RoadGeneratorChoiceEngine _choiceEngine;
        protected List<RoadSection> currentPieces;
        protected List<RoadSection> prototypes;
        private int presetPiecesPlaced;
        private bool AllPresetPiecesPlaced
        {
            get => presetPiecesPlaced == piecesToPlaceFirst.Count;
        }

        public static ARoadGenerator instance;
        public static ARoadGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogError("No instance found.");
                }
                return instance;
            }
        }

        public UnityEvent<RoadSection> placeNextRoads;

        protected void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            placeNextRoads.AddListener((RoadSection section) =>
            {
                _choiceEngine.Step(_choiceEngineStepsPerFrame, section);
                if (ShouldPlaceNewPiece())
                {
                    try
                    {
                        _choiceEngine.StepUntilChoiceIsFound(section);
                    }
                    catch (RoadGeneratorChoiceEngine.NoChoiceFoundException _)
                    {
                        // Debug.Log("Could not find valid road section choice!");
                    }
                    //if (_choiceEngine.HasFoundChoice())
                    //{

                    //}
                    //else
                    //{
                    //    Debug.LogError("Could not find valid road section choice!");
                    //}
                    _PlaceNewPiece(section);
                    OnNewPiecePlaced(currentPieces[currentPieces.Count - 1]);
                    while (currentPieces.Count > _targetRoadLength)
                    {
                        RoadSection sectionToDelete = currentPieces[0];
                        currentPieces.RemoveAt(0);
                        Destroy(sectionToDelete.gameObject);
                    }
                }
            });

            currentPieces = new List<RoadSection>();
            presetPiecesPlaced = 0;
            _CreatePrototypes();
            _choiceEngine = new RoadGeneratorChoiceEngine();
            PopulateCurrentPiecesFromWorld();
            if (placeAllPresetPiecesOnStart) PlaceAllPresetPieces();
            _ResetEngine();
        }

        private void PopulateCurrentPiecesFromWorld()
        {
            foreach (Transform child in _roadSectionContainer)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                RoadSection section = child.GetComponent<RoadSection>();
                currentPieces.Add(section);
            }
        }

        private void PlaceAllPresetPieces()
        {
            while (!AllPresetPiecesPlaced) PlaceNextPresetPiece();
        }

        private void PlaceNextPresetPiece()
        {
            PlacePresetPiece(piecesToPlaceFirst[presetPiecesPlaced++]);
        }

        private void PlacePresetPiece(PresetPiece presetPiece)
        {
            int prototypeIndex = presetPiece.pieceIndex;
            if (_autoHorizontalFlipPieces) prototypeIndex = prototypeIndex * 2 + (presetPiece.flipped ? 1 : 0);
            foreach (var end in prototypes[prototypeIndex].GetShape().Ends)
            {
                _PlaceNewPiece(prototypes[prototypeIndex], end);
            }
        }

        private void _CreatePrototypes()
        {
            if (_autoHorizontalFlipPieces)
            {
                Debug.LogWarning("RoadSection auto flip only works when start is along z axis!");
            }

            int i = 0;
            prototypes = new List<RoadSection>();
            // TODO
            foreach (RoadSection roadSection in _flippableRoadSectionChoices)
            {
                RoadSection section = _CreatePrototype(roadSection);
                section.PieceTypeId = i;
                prototypes.Add(section);

                RoadSection flippedSection = _CreatePrototype(roadSection);
                flippedSection.PieceTypeId = i;
                _Flip(flippedSection);
                flippedSection.IsFlipped = true;
                flippedSection.name += "Flipped";
                prototypes.Add(flippedSection);

                i++;
            }
            foreach (RoadSection roadSection in _notFlippableRoadSectionChoices)
            {
                RoadSection section = _CreatePrototype(roadSection);
                section.PieceTypeId = i++;
                prototypes.Add(section);
            }
            RoadSection deadEndSection = _CreatePrototype(_DeadEndRoadSection);
            deadEndSection.PieceTypeId = i++;
            prototypes.Add(deadEndSection);
        }

        private RoadSection _CreatePrototype(RoadSection toCopy)
        {
            GameObject prototype = Instantiate(toCopy.gameObject);
            prototype.name = toCopy.name + " Prototype";
            prototype.transform.parent = transform;
            prototype.SetActive(false);
            prototype.transform.SetGlobalScale(_roadSectionContainer.lossyScale);
            RoadSection instantiatedSection = prototype.GetComponent<RoadSection>();
            return instantiatedSection;
        }

        private void _Flip(RoadSection toFlip)
        {
            Vector3 localScale = toFlip.transform.localScale;
            localScale.x *= -1;
            toFlip.transform.localScale = localScale;
        }

        protected void Update()
        {


        }

        protected virtual void OnNewPiecePlaced(RoadSection newPiece) { }
        protected virtual void OnPieceRemoved() { }
        protected abstract bool ShouldPlaceNewPiece();
        protected abstract bool ShouldRemoveLastPiece();
        protected abstract List<RoadSection> GetPiecesInPreferenceOrder(List<RoadSection> sectionPrototypes);

        private List<RoadSection> _GetCurrentPiecesInWorld()
        {
            return currentPieces;
        }

        private void _RemoveLastPiece()
        {
            if (currentPieces.Count == 0) return;
            RoadSection lastSection = currentPieces[0];
            currentPieces.RemoveAt(0);
            // TODO not this
            Destroy(lastSection.gameObject);
            _ResetEngine();
        }

        private void _PlaceNewPiece(RoadSection currentSection)
        {
            foreach (var end in _GetNextPieceStartPosition(currentSection))
            {
                _PlaceNewPiece(_choiceEngine.GetChoicePrototype(), end, currentSection);

                _choiceEngine.Step(_choiceEngineStepsPerFrame, currentSection);
                try
                {
                    _choiceEngine.StepUntilChoiceIsFound(currentSection);
                }
                catch (RoadGeneratorChoiceEngine.NoChoiceFoundException _)
                {
                    Debug.LogError("Could not find valid road section choice!");
                }
            }
            _ResetEngine();
        }

        private void _PlaceNewPiece(RoadSection prototype, TransformData end, RoadSection previousSection = null)
        {
            RoadSection newSection = prototype.Clone();
            Vector3 sectionScaleOriginal = newSection.transform.localScale;
            newSection.transform.parent = _roadSectionContainer;
            newSection.transform.SetGlobalScale(sectionScaleOriginal);
            newSection.AlignByStartPoint(end);
            currentPieces.Add(newSection);
            if(previousSection != null)
            {
                previousSection.AddNextSection(newSection);
            }
        }

        private void _ResetEngine()
        {
            _choiceEngine.Reset(_GetCurrentPiecesInWorld(), GetPiecesInPreferenceOrder(prototypes).Cast<RoadSection>().ToList(), _choiceEngineCheckDepth);
        }

        private TransformData[] _GetNextPieceStartPosition(RoadSection currentSection = null)
        {
            if (currentSection == null)
            {
                TransformData startPosition = new TransformData(Vector3.zero, new Quaternion(0, 0, 0, 1), Vector3.one);
                return new TransformData[] { startPosition };
            }

            TransformData[] startPostitions = new TransformData[currentSection.GetShape().Ends.Count];

            for (int i = 0; i < currentSection.GetShape().Ends.Count; i++)
            {
                startPostitions[i] = currentSection.GetShape().Ends[i];
            }
            return startPostitions; // TODO : modify index to add multiple ends
        }

        public void PlaceNextRoads(RoadSection section)
        {
            placeNextRoads.Invoke(section);
        }
    }
}