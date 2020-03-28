﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zametek.Maths.Graphs
{
    internal static class GraphBuilderExtensions
    {
        internal static void ClearCriticalPathVariables<T, TEdgeContent, TNodeContent, TActivity, TEvent>
            (this GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent> graphBuilder)
            where T : struct, IComparable<T>, IEquatable<T>
            where TEdgeContent : IHaveId<T>, ICloneObject
            where TNodeContent : IHaveId<T>, ICloneObject
            where TActivity : IActivity<T>
            where TEvent : IEvent<T>
        {
            if (graphBuilder == null)
            {
                throw new ArgumentNullException(nameof(graphBuilder));
            }
            foreach (TActivity tmpActivity in graphBuilder.Activities)
            {
                tmpActivity.FreeSlack = null;
                tmpActivity.EarliestStartTime = null;
                tmpActivity.LatestFinishTime = null;
            }
            foreach (TEvent tmpEvent in graphBuilder.Events)
            {
                tmpEvent.EarliestFinishTime = null;
                tmpEvent.LatestFinishTime = null;
            }
        }

        internal static IList<T> CalculateCriticalPathPriorityList<T, TEdgeContent, TNodeContent, TActivity, TEvent>
            (this GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent> graphBuilder)
            where T : struct, IComparable<T>, IEquatable<T>
            where TEdgeContent : IHaveId<T>, ICloneObject
            where TNodeContent : IHaveId<T>, ICloneObject
            where TActivity : IActivity<T>
            where TEvent : IEvent<T>
        {
            if (graphBuilder == null)
            {
                throw new ArgumentNullException(nameof(graphBuilder));
            }
            var tmpGraphBuilder = (GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent>)graphBuilder.CloneObject();
            var priorityList = new List<T>();
            bool cont = true;
            while (cont)
            {
                tmpGraphBuilder.CalculateCriticalPath();

                // Get the critical path in order of earliest start time.
                int minFloat = tmpGraphBuilder.Activities
                    .Where(x => !x.IsDummy && x.TotalSlack.HasValue)
                    .Select(x => x.TotalSlack.Value)
                    .DefaultIfEmpty()
                    .Min();

                IList<T> criticalActivityIds =
                    tmpGraphBuilder.Activities
                    .Where(x => x.TotalSlack == minFloat && !x.IsDummy)
                    .OrderBy(x => x.EarliestStartTime)
                    .Select(x => x.Id)
                    .ToList();

                if (criticalActivityIds.Any())
                {
                    T criticalActivityId = criticalActivityIds.First();
                    priorityList.Add(criticalActivityId);

                    // Set the processed activity to dummy.
                    tmpGraphBuilder.Activity(criticalActivityId).Duration = 0;
                }
                else
                {
                    cont = false;
                }
            }
            if (tmpGraphBuilder.Activities.Any(x => !x.IsDummy))
            {
                throw new InvalidOperationException(Properties.Resources.CannotCalculateCriticalPathPriorityList);
            }
            return priorityList;
        }

        internal static IEnumerable<IResourceSchedule<T>> CalculateResourceSchedulesByPriorityList<T, TEdgeContent, TNodeContent, TActivity, TEvent>
            (this GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent> graphBuilder, IList<IResource<T>> resources)
            where T : struct, IComparable<T>, IEquatable<T>
            where TEdgeContent : IHaveId<T>, ICloneObject
            where TNodeContent : IHaveId<T>, ICloneObject
            where TActivity : IActivity<T>
            where TEvent : IEvent<T>
        {
            if (graphBuilder == null)
            {
                throw new ArgumentNullException(nameof(graphBuilder));
            }
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            if (resources.Count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(resources), Properties.Resources.ValueCannotBeNegative);
            }
            if (!graphBuilder.Activities.Any())
            {
                return Enumerable.Empty<IResourceSchedule<T>>();
            }

            // If resources are 0, assume infinite.
            bool infiniteResources = !resources.Any();

            // If resources are limited, check to make sure all activities can be accepted.
            if (!infiniteResources)
            {
                HashSet<T> allTargetResources = graphBuilder.Activities
                    .Select(x => x.TargetResources)
                    .Aggregate((previous, next) => new HashSet<T>(previous.Union(next)));

                bool allTargetResourcesAreSubsetOfResources = allTargetResources.IsSubsetOf(resources.Select(x => x.Id));
                if (!allTargetResourcesAreSubsetOfResources)
                {
                    throw new InvalidOperationException(Properties.Resources.AtLeastOneSpecifiedTargetResourcesAreNotPresentInResourcesProvided);
                }

                // If all resources are explicit targets, check to make sure all activities
                // targeted to at least one.
                bool allResourcesAreExplicitTargets = resources.All(x => x.IsExplicitTarget);
                bool atLeastOneActivityRequiresNonExplicitTargetResource = graphBuilder.Activities.Any(x => !x.IsDummy && !x.TargetResources.Any());
                if (allResourcesAreExplicitTargets
                    && atLeastOneActivityRequiresNonExplicitTargetResource)
                {
                    throw new InvalidOperationException(Properties.Resources.AtLeastOneActivityRequiresNonExplicitTargetResourceButAllProvidedResourcesAreExplicitTargets);
                }
            }

            var tmpGraphBuilder = (GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent>)graphBuilder.CloneObject();
            IList<T?> priorityList = tmpGraphBuilder
                .CalculateCriticalPathPriorityList()
                .Select(x => new T?(x))
                .ToList();

            IList<ResourceScheduleBuilder<T>> resourceScheduleBuilders = resources
                .OrderBy(x => x.AllocationOrder)
                .Select(x => new ResourceScheduleBuilder<T>(x))
                .ToList();

            var completed = new HashSet<T>();
            var started = new HashSet<T>();
            var ready = new List<T?>(Enumerable.Repeat((T?)null, priorityList.Count));
            int timeCounter = 0;
            while (priorityList.Any(x => x.HasValue) || started.Any() || ready.Any(x => x.HasValue))
            {
                // Any started activities that are currently not running must now be completed.
                var running = new HashSet<T>(resourceScheduleBuilders
                    .Select(x => x.ActivityAt(timeCounter))
                    .Where(x => x.HasValue)
                    .Select(x => x.GetValueOrDefault()));
                IList<T> notYetCompleted = started.Intersect(running).ToList();

                // Now work out which of the started jobs are now completed.
                started.ExceptWith(notYetCompleted);
                completed.UnionWith(started);

                // Refresh the started set.
                started.Clear();
                started.UnionWith(notYetCompleted);

                // Get the activities that have completed direct dependencies.
                // Add these to the ready queue since there is nothing preventing them from starting.
                var indicesToRemove = new HashSet<int>();
                for (int activityIndex = 0; activityIndex < priorityList.Count; activityIndex++)
                {
                    T? nullableActivityId = priorityList[activityIndex];
                    if (!nullableActivityId.HasValue)
                    {
                        continue;
                    }
                    T activityId = nullableActivityId.GetValueOrDefault();

                    var directDependencies =
                        new HashSet<T>(tmpGraphBuilder.StrongActivityDependencyIds(activityId));
                    if (directDependencies.IsSubsetOf(completed)
                        && !completed.Contains(activityId)
                        && !started.Contains(activityId))
                    {
                        ready[activityIndex] = activityId;
                        indicesToRemove.Add(activityIndex);
                    }
                }

                // Now clear the activities that have been added to the ready queue.
                foreach (int indexToRemove in indicesToRemove)
                {
                    priorityList[indexToRemove] = null;
                }

                // Cycle through each ready activity and find the first currently available schedule builder.
                bool keepLooking = true;
                while (ready.Any(x => x.HasValue) && keepLooking)
                {
                    keepLooking = false;
                    bool availableResourceScheduleBuilderExists = false;

                    for (int activityIndex = 0; activityIndex < ready.Count; activityIndex++)
                    {
                        T? nullableActivityId = ready[activityIndex];
                        if (!nullableActivityId.HasValue)
                        {
                            continue;
                        }
                        T activityId = nullableActivityId.GetValueOrDefault();
                        IActivity<T> activity = tmpGraphBuilder.Activity(activityId);
                        activity.AllocatedToResources.Clear();

                        // Check to see if the activity has to be targeted to specific resources,
                        // and that this resource is one of those specific targets.
                        bool activityMustBeTargetedToSpecificResource = !infiniteResources && activity.TargetResources.Any();

                        if (!activityMustBeTargetedToSpecificResource)
                        {
                            foreach (ResourceScheduleBuilder<T> resourceScheduleBuilder in resourceScheduleBuilders)
                            {
                                if (resourceScheduleBuilder.EarliestAvailableStartTimeForNextActivity <= timeCounter)
                                {
                                    availableResourceScheduleBuilderExists = true;
                                    if (resourceScheduleBuilder.IsExplicitTarget)
                                    {
                                        continue;
                                    }

                                    if (activity.EarliestStartTime.GetValueOrDefault() > timeCounter)
                                    {
                                        continue;
                                    }

                                    resourceScheduleBuilder.AddActivity(activity, timeCounter);

                                    //if (resourceScheduleBuilder.ResourceId.HasValue)
                                    //{
                                    //    activity.AllocatedToResources.Add(resourceScheduleBuilder.ResourceId.GetValueOrDefault());
                                    //}

                                    started.Add(activityId);
                                    keepLooking = true;
                                    ready[activityIndex] = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            var availableResourceSchedulers = new HashSet<ResourceScheduleBuilder<T>>();

                            foreach (ResourceScheduleBuilder<T> resourceScheduleBuilder in resourceScheduleBuilders)
                            {
                                if (resourceScheduleBuilder.EarliestAvailableStartTimeForNextActivity <= timeCounter)
                                {
                                    availableResourceScheduleBuilderExists = true;
                                    bool resourceCannotAcceptThisActivity = resourceScheduleBuilder.ResourceId != null
                                        && !activity.TargetResources.Contains(resourceScheduleBuilder.ResourceId.GetValueOrDefault());

                                    if (resourceCannotAcceptThisActivity)
                                    {
                                        continue;
                                    }

                                    if (activity.EarliestStartTime.GetValueOrDefault() > timeCounter)
                                    {
                                        continue;
                                    }

                                    // Find just one resource that can accommodate the activity.
                                    if (activity.TargetResourceOperator == LogicalOperator.OR)
                                    {
                                        resourceScheduleBuilder.AddActivity(activity, timeCounter);

                                        //if (resourceScheduleBuilder.ResourceId.HasValue)
                                        //{
                                        //    activity.AllocatedToResources.Add(resourceScheduleBuilder.ResourceId.GetValueOrDefault());
                                        //}

                                        started.Add(activityId);
                                        keepLooking = true;
                                        ready[activityIndex] = null;
                                        break;
                                    }
                                    // Find all the resources that must accommodate the activity.
                                    else if (activity.TargetResourceOperator == LogicalOperator.AND)
                                    {
                                        availableResourceSchedulers.Add(resourceScheduleBuilder);

                                        var targetResources = new HashSet<T>(activity.TargetResources);
                                        if (targetResources.SetEquals(availableResourceSchedulers.Select(x => x.ResourceId.GetValueOrDefault())))
                                        {
                                            foreach (ResourceScheduleBuilder<T> availableResourceScheduler in availableResourceSchedulers)
                                            {
                                                availableResourceScheduler.AddActivity(activity, timeCounter);

                                                //if (availableResourceScheduler.ResourceId.HasValue)
                                                //{
                                                //    activity.AllocatedToResources.Add(availableResourceScheduler.ResourceId.GetValueOrDefault());
                                                //}

                                                started.Add(activityId);
                                            }
                                            keepLooking = true;
                                            ready[activityIndex] = null;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        throw new NotImplementedException($@"Unknown TargetResourceOperator value ({activity.TargetResourceOperator})");
                                    }
                                }
                            }
                        }
                    }

                    if (infiniteResources
                        && !availableResourceScheduleBuilderExists
                        && !keepLooking)
                    {
                        resourceScheduleBuilders.Add(new ResourceScheduleBuilder<T>());
                        keepLooking = true;
                    }
                }
                timeCounter++;
            }

            int finishTime = resourceScheduleBuilders.Select(x => x.LastActivityFinishTime).DefaultIfEmpty().Max();

            return resourceScheduleBuilders
                .Select(x => x.ToResourceSchedule(finishTime))
                .Where(x => x.ScheduledActivities.Any());
        }

        internal static IEnumerable<IResourceSchedule<T>> CalculateResourceSchedulesByPriorityList<T, TEdgeContent, TNodeContent, TActivity, TEvent>
            (this GraphBuilderBase<T, TEdgeContent, TNodeContent, TActivity, TEvent> graphBuilder)
            where T : struct, IComparable<T>, IEquatable<T>
            where TEdgeContent : IHaveId<T>, ICloneObject
            where TNodeContent : IHaveId<T>, ICloneObject
            where TActivity : IActivity<T>
            where TEvent : IEvent<T>
        {
            if (graphBuilder == null)
            {
                throw new ArgumentNullException(nameof(graphBuilder));
            }
            return graphBuilder.CalculateResourceSchedulesByPriorityList(new List<IResource<T>>());
        }
    }
}
