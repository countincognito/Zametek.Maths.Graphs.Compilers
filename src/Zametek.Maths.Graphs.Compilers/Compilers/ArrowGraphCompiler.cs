﻿using System;
using System.Linq;

namespace Zametek.Maths.Graphs
{
    public class ArrowGraphCompiler<T, TDependentActivity>
        : ArrowGraphCompilerBase<T, TDependentActivity, IActivity<T>, IEvent<T>>
        where TDependentActivity : IDependentActivity<T>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        #region Ctors

        protected ArrowGraphCompiler(ArrowGraphBuilderBase<T, TDependentActivity, IEvent<T>> arrowGraphBuilder)
            : base(arrowGraphBuilder)
        { }

        #endregion

        #region Public Methods

        public static ArrowGraphCompiler<T, TDependentActivity> Create()
        {
            T edgeId = default;
            T nodeId = default;
            var arrowGraphBuilder = new DependentActivityArrowGraphBuilder(
                () => edgeId = edgeId.Previous(),
                () => nodeId = nodeId.Previous());
            return new ArrowGraphCompiler<T, TDependentActivity>(arrowGraphBuilder);
        }

        #endregion

        #region Private Types

        private class DependentActivityArrowGraphBuilder
            : ArrowGraphBuilderBase<T, TDependentActivity, IEvent<T>>
        {
            #region Fields

            private static readonly Func<T, IEvent<T>> s_EventGenerator = (id) => new Event<T>(id);
            private static readonly Func<T, int?, int?, IEvent<T>> s_EventGeneratorEventWithTimes = (id, earliestFinishTime, latestFinishTime) => new Event<T>(id, earliestFinishTime, latestFinishTime);
            private static readonly Func<T, TDependentActivity> s_DummyActivityGenerator = (id) => (TDependentActivity)DependentActivity<T>.CreateDependentActivityDummy(id);

            #endregion

            #region Ctors

            public DependentActivityArrowGraphBuilder(
                Func<T> edgeIdGenerator,
                Func<T> nodeIdGenerator)
                : base(
                      edgeIdGenerator,
                      nodeIdGenerator,
                      s_EventGenerator,
                      s_EventGeneratorEventWithTimes,
                      s_DummyActivityGenerator)
            { }

            public DependentActivityArrowGraphBuilder(
                Graph<T, TDependentActivity, IEvent<T>> graph,
                Func<T> edgeIdGenerator,
                Func<T> nodeIdGenerator)
                : base(
                      graph,
                      edgeIdGenerator,
                      nodeIdGenerator,
                      s_EventGenerator)
            { }

            #endregion

            #region Overrides

            public override object WorkingCopy()
            {
                Graph<T, TDependentActivity, IEvent<T>> arrowGraphCopy = ToGraph();
                T minNodeId = arrowGraphCopy.Nodes.Select(x => x.Id).DefaultIfEmpty().Min();
                minNodeId = minNodeId.Previous();
                T minEdgeId = arrowGraphCopy.Edges.Select(x => x.Id).DefaultIfEmpty().Min();
                minEdgeId = minEdgeId.Previous();
                return new DependentActivityArrowGraphBuilder(
                    arrowGraphCopy,
                    () => minEdgeId = minEdgeId.Previous(),
                    () => minNodeId = minNodeId.Previous());
            }

            #endregion
        }

        #endregion
    }
}
