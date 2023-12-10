using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Other;
using Codice.CM.Client.Differences.Merge;
using Unity.Collections.LowLevel.Unsafe;

namespace JonathonOH.RoadGeneration
{
    /// <summary>
    /// Must call Reset() before anything else
    /// </summary>
    public class RoadGeneratorChoiceEngine
    {
        public class NoChoiceFoundException : Exception { }

        public DFSCombinationGenerator _combinationGenerator;
        private List<RoadSection> _currentPiecesInWorld;
        private List<RoadSection> _candidatePrototypes;

        private const int MAX_ITERATIONS = 10000000;
        private bool _impossible;

        public void Reset(List<RoadSection> currentPiecesInWorld, List<RoadSection> possibleChoicesInPreferenceOrder, int checkDepth)
        {
            _combinationGenerator = new DFSCombinationGenerator(possibleChoicesInPreferenceOrder.Count, checkDepth);
            _currentPiecesInWorld = currentPiecesInWorld;
            _candidatePrototypes = possibleChoicesInPreferenceOrder;
            _impossible = false;
        }

        public void StepUntilChoiceIsFound(RoadSection currentSection)
        {
            int i;
            for (i = 0; i < MAX_ITERATIONS; i++)
            {
                if (_combinationGenerator.IsImpossible()) break;
                if (HasFoundChoice()) break;
                Step(1, currentSection);
            }
            if (Debug.isDebugBuild && i > 0) Debug.Log($"RoadGeneratorChoiceEngine force stepped {i} times.");
        }

        public void Step(int choiceEngineStepsPerFrame, RoadSection currentSection = null)
        {
            if (HasFoundChoice() || _impossible) return;
            if (_combinationGenerator.IsImpossible()) return;
            if (_DoesLastCandidateSectionOverlapWithOthers() || _IsLastCandidateAnInvalidNeighbour(currentSection))
            {
                try
                {
                    _combinationGenerator.StepInvalid();
                }
                catch (DFSCombinationGenerator.OutOfCombinationsException _)
                {
                    Debug.Log("No Choice Found!");
                    //Debug.Log(_combinationGenerator.GetState().Average());
                    //if (_combinationGenerator.GetState().Average() == -1)
                    //{
                    //    //todo set candidate
                    Debug.Log(HasFoundChoice());
                    _combinationGenerator.StepValid();

                    //}
                    _impossible = true;
                }
            }
            else
            {
                _combinationGenerator.StepValid();
            }
        }

        private bool _IsLastCandidateAnInvalidNeighbour(RoadSection currentSection)
        {
            List<RoadSection> notAlignedSections = _GetCandidatesNotAligned(); 

            RoadSection candidate = notAlignedSections[notAlignedSections.Count - 1];

            if (currentSection == null) return candidate.GetType() == typeof(RoadSectionStart);

            return !currentSection.IsValidForNextSection(candidate);
        }

        private bool _DoesLastCandidateSectionOverlapWithOthers()
        {
            List<RoadSectionShape> allPiecesAligned = _GetCandidatesAndCurrentPiecesInWorldAligned(); 

            // TODO can allPiecesAligned.Count ever be zero? If so, prevent it from breaking
            RoadSectionShape currentCandidate = allPiecesAligned[allPiecesAligned.Count - 1];

            foreach (RoadSectionShape worldRoadSectionShape in allPiecesAligned.Take(allPiecesAligned.Count - 1))
            {
                if (currentCandidate.DoesOverlapWith(worldRoadSectionShape)) return true;
            }
            return false;
        }

        private List<RoadSectionShape> _GetCandidatesAndCurrentPiecesInWorldAligned()
        {
            return _GetCurrentPiecesInWorldShapes().Concat(_GetCandidatesAligned()).ToList();
        }

        private List<RoadSectionShape> _GetCurrentPiecesInWorldShapes()
        {
            return _currentPiecesInWorld.Select(section => section.GetShape()).ToList();
        }

        private List<RoadSectionShape> _GetCandidatesAligned()
        {
            // Figuring out the architecture so this method could exist was a nightmare.
            // Both big redesigns were a result of this.
            // I hope it looks obvious and easy to make yourself - that means I've done it right
            List<RoadSectionShape> alignedCandidates = new List<RoadSectionShape>();
            TransformData[] nextStartPoint = _GetFirstCandidateStartPoint();
            foreach (RoadSection candidateSection in _GetCandidatesNotAligned())
            {
                List<TransformData> tmpStartPoints = new List<TransformData>();
                foreach (var start in nextStartPoint) 
                {
                    RoadSectionShape alignedCandidateShape = candidateSection.GetShape().GetTranslatedCopy(start);
                    alignedCandidates.Add(alignedCandidateShape);
                    tmpStartPoints = alignedCandidateShape.Ends;
                }
                nextStartPoint = tmpStartPoints.ToArray();
            }
            return alignedCandidates;
        }

        private List<RoadSection> _GetCandidatesNotAligned()
        {
            List<RoadSection> candidates = new List<RoadSection>();
            foreach (int candidateChoiceIndex in _combinationGenerator.GetState())
            {
                if (candidateChoiceIndex == -1) break;
                candidates.Add(_candidatePrototypes[candidateChoiceIndex]);
            }
            return candidates;
        }

        private TransformData[] _GetFirstCandidateStartPoint()
        {
            // TODO this is copied in RoadGenerator - they both define start points - should not be separate
            if (_currentPiecesInWorld.Count == 0)
            {
                TransformData startPosition = new TransformData(Vector3.zero, new Quaternion(0, 0, 0, 1), Vector3.one);
                return new TransformData[] { startPosition };
            }

            TransformData[] startPostitions = new TransformData[_currentPiecesInWorld[_currentPiecesInWorld.Count - 1].GetShape().Ends.Count];
            for (int i = 0; i < _currentPiecesInWorld[_currentPiecesInWorld.Count - 1].GetShape().Ends.Count; i++)
            {
                startPostitions[i] = _currentPiecesInWorld[_currentPiecesInWorld.Count - 1].GetShape().Ends[i];
            }
            return startPostitions;
        }

        public bool HasFoundChoice()
        {
            return _combinationGenerator.HasFoundSolution();
        }

        public RoadSection GetChoicePrototype()
        {
            if (!HasFoundChoice())
            {
                Reset(_currentPiecesInWorld, _candidatePrototypes, _combinationGenerator.GetCurrentDepth());
                return _candidatePrototypes.Find(new Predicate<RoadSection>((RoadSection r) => { return r.GetType() == typeof(RoadSectionDeadEnd); }));

                //throw new NoChoiceFoundException();
            }
            return _candidatePrototypes[_combinationGenerator.GetState()[0]];
        }
    }
}