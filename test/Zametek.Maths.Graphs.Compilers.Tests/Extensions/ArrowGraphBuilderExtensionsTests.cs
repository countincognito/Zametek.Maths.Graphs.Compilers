﻿using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Zametek.Maths.Graphs.Tests
{
    public class ArrowGraphBuilderExtensionsTests
    {
        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPath_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6));
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10), new HashSet<int>(new[] { 5 }));

            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(2);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(2);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(2);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(8);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(1);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(1);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(8);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(8);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(11);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(22);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(16);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(7);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(15);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(22);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(4);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(22);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(26);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(4);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(22);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(26);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(26);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(26);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumFreeSlackInStartActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6) { MinimumFreeSlack = 10 });
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10), new HashSet<int>(new[] { 5 }));

            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(10);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(10);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(10);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(9);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(9);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(8);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(19);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(24);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(24);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(23);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(12);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(34);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(12);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(34);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(34);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(34);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumFreeSlackInNormalActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6));
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8) { MinimumFreeSlack = 15 }, new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10), new HashSet<int>(new[] { 5 }));

            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(2);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(17);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(17);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(16);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(15);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(26);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(37);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(16);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(15);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(23);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(31);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(22);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(37);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(19);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(37);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(41);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(19);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(37);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(41);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(31);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(41);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(31);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(41);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumFreeSlackInEndActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6));
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10) { MinimumFreeSlack = 15 }, new HashSet<int>(new[] { 5 }));

            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(2);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(17);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(17);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(16);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(15);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(23);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(26);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(37);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(16);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(23);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(31);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(22);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(37);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(19);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(37);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(41);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(19);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(19);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(37);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(41);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(26);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(15);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(31);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(41);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumEarliestStartTimeInStartActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6) { MinimumEarliestStartTime = 10 });
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10), new HashSet<int>(new[] { 5 }));

            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(10);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(16);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(10);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(9);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(9);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(8);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(16);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(19);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(24);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(16);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(24);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(15);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(23);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(12);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(34);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(12);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(12);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(30);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(34);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(34);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(34);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumEarliestStartTimeInNormalActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6));
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8) { MinimumEarliestStartTime = 10 }, new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10), new HashSet<int>(new[] { 5 }));

            graphBuilder.TransitiveReduction();
            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(4);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(4);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(10);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(3);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(3);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(10);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(2);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(2);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(10);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(6);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(13);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(24);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(10);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(10);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(18);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(9);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(17);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(24);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(6);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(6);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(28);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(6);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(6);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(24);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(28);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(28);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(28);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathWithMinimumEarliestStartTimeInEndActivity_ThenAsExpected()
        {
            int eventId = 0;
            int activityId1 = 1;
            int activityId2 = activityId1 + 1;
            int activityId3 = activityId2 + 1;
            int activityId4 = activityId3 + 1;
            int activityId5 = activityId4 + 1;
            int activityId6 = activityId5 + 1;
            int activityId7 = activityId6 + 1;
            int activityId8 = activityId7 + 1;
            int activityId9 = activityId8 + 1;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(activityId1, 6));
            graphBuilder.AddActivity(new Activity<int>(activityId2, 7));
            graphBuilder.AddActivity(new Activity<int>(activityId3, 8));
            graphBuilder.AddActivity(new Activity<int>(activityId4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(activityId5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(activityId7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(activityId8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(activityId9, 10) { MinimumEarliestStartTime = 20 }, new HashSet<int>(new[] { 5 }));

            graphBuilder.TransitiveReduction();
            graphBuilder.CalculateCriticalPath();

            graphBuilder.Activity(activityId1).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId1).EarliestFinishTime.Should().Be(6);
            graphBuilder.Activity(activityId1).FreeSlack.Should().Be(2);
            graphBuilder.Activity(activityId1).TotalSlack.Should().Be(6);
            graphBuilder.Activity(activityId1).LatestStartTime.Should().Be(6);
            graphBuilder.Activity(activityId1).LatestFinishTime.Should().Be(12);

            graphBuilder.Activity(activityId2).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId2).EarliestFinishTime.Should().Be(7);
            graphBuilder.Activity(activityId2).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId2).TotalSlack.Should().Be(5);
            graphBuilder.Activity(activityId2).LatestStartTime.Should().Be(5);
            graphBuilder.Activity(activityId2).LatestFinishTime.Should().Be(12);

            graphBuilder.Activity(activityId3).EarliestStartTime.Should().Be(0);
            graphBuilder.Activity(activityId3).EarliestFinishTime.Should().Be(8);
            graphBuilder.Activity(activityId3).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId3).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId3).LatestStartTime.Should().Be(4);
            graphBuilder.Activity(activityId3).LatestFinishTime.Should().Be(12);

            graphBuilder.Activity(activityId4).EarliestStartTime.Should().Be(7);
            graphBuilder.Activity(activityId4).EarliestFinishTime.Should().Be(18);
            graphBuilder.Activity(activityId4).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId4).TotalSlack.Should().Be(8);
            graphBuilder.Activity(activityId4).LatestStartTime.Should().Be(15);
            graphBuilder.Activity(activityId4).LatestFinishTime.Should().Be(26);

            graphBuilder.Activity(activityId5).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId5).EarliestFinishTime.Should().Be(16);
            graphBuilder.Activity(activityId5).FreeSlack.Should().Be(4);
            graphBuilder.Activity(activityId5).TotalSlack.Should().Be(4);
            graphBuilder.Activity(activityId5).LatestStartTime.Should().Be(12);
            graphBuilder.Activity(activityId5).LatestFinishTime.Should().Be(20);

            graphBuilder.Activity(activityId6).EarliestStartTime.Should().Be(8);
            graphBuilder.Activity(activityId6).EarliestFinishTime.Should().Be(15);
            graphBuilder.Activity(activityId6).FreeSlack.Should().Be(3);
            graphBuilder.Activity(activityId6).TotalSlack.Should().Be(11);
            graphBuilder.Activity(activityId6).LatestStartTime.Should().Be(19);
            graphBuilder.Activity(activityId6).LatestFinishTime.Should().Be(26);

            graphBuilder.Activity(activityId7).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId7).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId7).FreeSlack.Should().Be(8);
            graphBuilder.Activity(activityId7).TotalSlack.Should().Be(8);
            graphBuilder.Activity(activityId7).LatestStartTime.Should().Be(26);
            graphBuilder.Activity(activityId7).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId8).EarliestStartTime.Should().Be(18);
            graphBuilder.Activity(activityId8).EarliestFinishTime.Should().Be(22);
            graphBuilder.Activity(activityId8).FreeSlack.Should().Be(8);
            graphBuilder.Activity(activityId8).TotalSlack.Should().Be(8);
            graphBuilder.Activity(activityId8).LatestStartTime.Should().Be(26);
            graphBuilder.Activity(activityId8).LatestFinishTime.Should().Be(30);

            graphBuilder.Activity(activityId9).EarliestStartTime.Should().Be(20);
            graphBuilder.Activity(activityId9).EarliestFinishTime.Should().Be(30);
            graphBuilder.Activity(activityId9).FreeSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).TotalSlack.Should().Be(0);
            graphBuilder.Activity(activityId9).LatestStartTime.Should().Be(20);
            graphBuilder.Activity(activityId9).LatestFinishTime.Should().Be(30);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateCriticalPathPriorityList_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            List<int> priorityList = graphBuilder.CalculateCriticalPathPriorityList().ToList();

            priorityList.Should().BeEquivalentTo(new List<int>(new[] { 3, 2, 1, 5, 4, 6, 9, 7, 8 }));
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateResourceSchedulesByPriorityListOneResource_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            IList<IResourceSchedule<int>> resourceSchedule =
                graphBuilder.CalculateResourceSchedulesByPriorityList(
                    new List<IResource<int>>(new[]
                    {
                        new Resource<int>(1, string.Empty, false, InterActivityAllocationType.None, 1.0, 0)
                    })).ToList();
            resourceSchedule.Count.Should().Be(1);

            resourceSchedule[0].ScheduledActivities.Count.Should().Be(9);
            resourceSchedule[0].ScheduledActivities[0].Id.Should().Be(3);
            resourceSchedule[0].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedule[0].ScheduledActivities[0].FinishTime.Should().Be(8);

            resourceSchedule[0].ScheduledActivities[1].Id.Should().Be(2);
            resourceSchedule[0].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedule[0].ScheduledActivities[1].FinishTime.Should().Be(15);

            resourceSchedule[0].ScheduledActivities[2].Id.Should().Be(1);
            resourceSchedule[0].ScheduledActivities[2].StartTime.Should().Be(15);
            resourceSchedule[0].ScheduledActivities[2].FinishTime.Should().Be(21);

            resourceSchedule[0].ScheduledActivities[3].Id.Should().Be(5);
            resourceSchedule[0].ScheduledActivities[3].StartTime.Should().Be(21);
            resourceSchedule[0].ScheduledActivities[3].FinishTime.Should().Be(29);

            resourceSchedule[0].ScheduledActivities[4].Id.Should().Be(4);
            resourceSchedule[0].ScheduledActivities[4].StartTime.Should().Be(29);
            resourceSchedule[0].ScheduledActivities[4].FinishTime.Should().Be(40);

            resourceSchedule[0].ScheduledActivities[5].Id.Should().Be(6);
            resourceSchedule[0].ScheduledActivities[5].StartTime.Should().Be(40);
            resourceSchedule[0].ScheduledActivities[5].FinishTime.Should().Be(47);

            resourceSchedule[0].ScheduledActivities[6].Id.Should().Be(9);
            resourceSchedule[0].ScheduledActivities[6].StartTime.Should().Be(47);
            resourceSchedule[0].ScheduledActivities[6].FinishTime.Should().Be(57);

            resourceSchedule[0].ScheduledActivities[7].Id.Should().Be(7);
            resourceSchedule[0].ScheduledActivities[7].StartTime.Should().Be(57);
            resourceSchedule[0].ScheduledActivities[7].FinishTime.Should().Be(61);

            resourceSchedule[0].ScheduledActivities[8].Id.Should().Be(8);
            resourceSchedule[0].ScheduledActivities[8].StartTime.Should().Be(61);
            resourceSchedule[0].ScheduledActivities[8].FinishTime.Should().Be(65);

            resourceSchedule[0].ScheduledActivities.Last().FinishTime.Should().Be(65);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateResourceSchedulesByPriorityListTwoResources_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            IList<IResourceSchedule<int>> resourceSchedules =
                graphBuilder.CalculateResourceSchedulesByPriorityList(
                    new List<IResource<int>>(new[]
                    {
                        new Resource<int>(1, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(2, string.Empty, false, InterActivityAllocationType.None, 1.0, 0)
                    })).ToList();
            resourceSchedules.Count.Should().Be(2);

            resourceSchedules[0].ScheduledActivities.Count.Should().Be(5);
            resourceSchedules[0].ScheduledActivities[0].Id.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[0].ScheduledActivities[0].FinishTime.Should().Be(8);

            resourceSchedules[0].ScheduledActivities[1].Id.Should().Be(4);
            resourceSchedules[0].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[0].ScheduledActivities[1].FinishTime.Should().Be(19);

            resourceSchedules[0].ScheduledActivities[2].Id.Should().Be(6);
            resourceSchedules[0].ScheduledActivities[2].StartTime.Should().Be(19);
            resourceSchedules[0].ScheduledActivities[2].FinishTime.Should().Be(26);

            resourceSchedules[0].ScheduledActivities[3].Id.Should().Be(7);
            resourceSchedules[0].ScheduledActivities[3].StartTime.Should().Be(26);
            resourceSchedules[0].ScheduledActivities[3].FinishTime.Should().Be(30);

            resourceSchedules[0].ScheduledActivities[4].Id.Should().Be(8);
            resourceSchedules[0].ScheduledActivities[4].StartTime.Should().Be(30);
            resourceSchedules[0].ScheduledActivities[4].FinishTime.Should().Be(34);

            resourceSchedules[0].ScheduledActivities.Last().FinishTime.Should().Be(34);

            resourceSchedules[1].ScheduledActivities.Count().Should().Be(4);

            resourceSchedules[1].ScheduledActivities[0].Id.Should().Be(2);
            resourceSchedules[1].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[1].ScheduledActivities[0].FinishTime.Should().Be(7);

            resourceSchedules[1].ScheduledActivities[1].Id.Should().Be(1);
            resourceSchedules[1].ScheduledActivities[1].StartTime.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[1].FinishTime.Should().Be(13);

            resourceSchedules[1].ScheduledActivities[2].Id.Should().Be(5);
            resourceSchedules[1].ScheduledActivities[2].StartTime.Should().Be(13);
            resourceSchedules[1].ScheduledActivities[2].FinishTime.Should().Be(21);

            resourceSchedules[1].ScheduledActivities[3].Id.Should().Be(9);
            resourceSchedules[1].ScheduledActivities[3].StartTime.Should().Be(21);
            resourceSchedules[1].ScheduledActivities[3].FinishTime.Should().Be(31);

            resourceSchedules[1].ScheduledActivities.Last().FinishTime.Should().Be(31);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateResourceSchedulesByPriorityListThreeResources_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            IList<IResourceSchedule<int>> resourceSchedules =
                graphBuilder.CalculateResourceSchedulesByPriorityList(
                    new List<IResource<int>>(new[]
                    {
                        new Resource<int>(1, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(2, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(3, string.Empty, false, InterActivityAllocationType.None, 1.0, 0)
                    })).ToList();
            resourceSchedules.Count.Should().Be(3);

            resourceSchedules[0].ScheduledActivities.Count.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].Id.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[0].ScheduledActivities[0].FinishTime.Should().Be(8);

            resourceSchedules[0].ScheduledActivities[1].Id.Should().Be(5);
            resourceSchedules[0].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[0].ScheduledActivities[1].FinishTime.Should().Be(16);

            resourceSchedules[0].ScheduledActivities[2].Id.Should().Be(9);
            resourceSchedules[0].ScheduledActivities[2].StartTime.Should().Be(16);
            resourceSchedules[0].ScheduledActivities[2].FinishTime.Should().Be(26);

            resourceSchedules[0].ScheduledActivities.Last().FinishTime.Should().Be(26);

            resourceSchedules[1].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[1].ScheduledActivities[0].Id.Should().Be(2);
            resourceSchedules[1].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[1].ScheduledActivities[0].FinishTime.Should().Be(7);

            resourceSchedules[1].ScheduledActivities[1].Id.Should().Be(4);
            resourceSchedules[1].ScheduledActivities[1].StartTime.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[1].FinishTime.Should().Be(18);

            resourceSchedules[1].ScheduledActivities[2].Id.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[1].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[1].ScheduledActivities.Last().FinishTime.Should().Be(22);


            resourceSchedules[2].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[2].ScheduledActivities[0].Id.Should().Be(1);
            resourceSchedules[2].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[2].ScheduledActivities[0].FinishTime.Should().Be(6);

            resourceSchedules[2].ScheduledActivities[1].Id.Should().Be(6);
            resourceSchedules[2].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[1].FinishTime.Should().Be(15);

            resourceSchedules[2].ScheduledActivities[2].Id.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[2].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[2].ScheduledActivities.Last().FinishTime.Should().Be(22);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateResourceSchedulesByPriorityListFourResources_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            IList<IResourceSchedule<int>> resourceSchedules =
                graphBuilder.CalculateResourceSchedulesByPriorityList(
                    new List<IResource<int>>(new[]
                    {
                        new Resource<int>(1, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(2, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(3, string.Empty, false, InterActivityAllocationType.None, 1.0, 0),
                        new Resource<int>(4, string.Empty, false, InterActivityAllocationType.None, 1.0, 0)
                    })).ToList();
            resourceSchedules.Count.Should().Be(3);

            resourceSchedules[0].ScheduledActivities.Count.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].Id.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[0].ScheduledActivities[0].FinishTime.Should().Be(8);

            resourceSchedules[0].ScheduledActivities[1].Id.Should().Be(5);
            resourceSchedules[0].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[0].ScheduledActivities[1].FinishTime.Should().Be(16);

            resourceSchedules[0].ScheduledActivities[2].Id.Should().Be(9);
            resourceSchedules[0].ScheduledActivities[2].StartTime.Should().Be(16);
            resourceSchedules[0].ScheduledActivities[2].FinishTime.Should().Be(26);

            resourceSchedules[0].ScheduledActivities.Last().FinishTime.Should().Be(26);

            resourceSchedules[1].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[1].ScheduledActivities[0].Id.Should().Be(2);
            resourceSchedules[1].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[1].ScheduledActivities[0].FinishTime.Should().Be(7);

            resourceSchedules[1].ScheduledActivities[1].Id.Should().Be(4);
            resourceSchedules[1].ScheduledActivities[1].StartTime.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[1].FinishTime.Should().Be(18);

            resourceSchedules[1].ScheduledActivities[2].Id.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[1].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[1].ScheduledActivities.Last().FinishTime.Should().Be(22);


            resourceSchedules[2].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[2].ScheduledActivities[0].Id.Should().Be(1);
            resourceSchedules[2].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[2].ScheduledActivities[0].FinishTime.Should().Be(6);

            resourceSchedules[2].ScheduledActivities[1].Id.Should().Be(6);
            resourceSchedules[2].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[1].FinishTime.Should().Be(15);

            resourceSchedules[2].ScheduledActivities[2].Id.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[2].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[2].ScheduledActivities.Last().FinishTime.Should().Be(22);
        }

        [Fact]
        public void ArrowGraphBuilderExtensions_GivenCalculateResourceSchedulesByPriorityListUnlimitedResources_ThenCorrectOrder()
        {
            int eventId = 0;
            int dummyActivityId = 100;
            var graphBuilder = new ArrowGraphBuilder<int, IActivity<int>>(() => dummyActivityId = dummyActivityId.Next(), () => eventId = eventId.Next());
            graphBuilder.AddActivity(new Activity<int>(1, 6));
            graphBuilder.AddActivity(new Activity<int>(2, 7));
            graphBuilder.AddActivity(new Activity<int>(3, 8));
            graphBuilder.AddActivity(new Activity<int>(4, 11), new HashSet<int>(new[] { 2 }));
            graphBuilder.AddActivity(new Activity<int>(5, 8), new HashSet<int>(new[] { 1, 2, 3 }));
            graphBuilder.AddActivity(new Activity<int>(6, 7), new HashSet<int>(new[] { 3 }));
            graphBuilder.AddActivity(new Activity<int>(7, 4), new HashSet<int>(new[] { 4 }));
            graphBuilder.AddActivity(new Activity<int>(8, 4), new HashSet<int>(new[] { 4, 6 }));
            graphBuilder.AddActivity(new Activity<int>(9, 10), new HashSet<int>(new[] { 5 }));

            IList<IResourceSchedule<int>> resourceSchedules = graphBuilder.CalculateResourceSchedulesByPriorityList().ToList();
            resourceSchedules.Count.Should().Be(3);

            resourceSchedules[0].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].Id.Should().Be(3);
            resourceSchedules[0].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[0].ScheduledActivities[0].FinishTime.Should().Be(8);

            resourceSchedules[0].ScheduledActivities[1].Id.Should().Be(5);
            resourceSchedules[0].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[0].ScheduledActivities[1].FinishTime.Should().Be(16);

            resourceSchedules[0].ScheduledActivities[2].Id.Should().Be(9);
            resourceSchedules[0].ScheduledActivities[2].StartTime.Should().Be(16);
            resourceSchedules[0].ScheduledActivities[2].FinishTime.Should().Be(26);

            resourceSchedules[0].ScheduledActivities.Last().FinishTime.Should().Be(26);

            resourceSchedules[1].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[1].ScheduledActivities[0].Id.Should().Be(2);
            resourceSchedules[1].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[1].ScheduledActivities[0].FinishTime.Should().Be(7);

            resourceSchedules[1].ScheduledActivities[1].Id.Should().Be(4);
            resourceSchedules[1].ScheduledActivities[1].StartTime.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[1].FinishTime.Should().Be(18);

            resourceSchedules[1].ScheduledActivities[2].Id.Should().Be(7);
            resourceSchedules[1].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[1].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[1].ScheduledActivities.Last().FinishTime.Should().Be(22);


            resourceSchedules[2].ScheduledActivities.Count().Should().Be(3);
            resourceSchedules[2].ScheduledActivities[0].Id.Should().Be(1);
            resourceSchedules[2].ScheduledActivities[0].StartTime.Should().Be(0);
            resourceSchedules[2].ScheduledActivities[0].FinishTime.Should().Be(6);

            resourceSchedules[2].ScheduledActivities[1].Id.Should().Be(6);
            resourceSchedules[2].ScheduledActivities[1].StartTime.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[1].FinishTime.Should().Be(15);

            resourceSchedules[2].ScheduledActivities[2].Id.Should().Be(8);
            resourceSchedules[2].ScheduledActivities[2].StartTime.Should().Be(18);
            resourceSchedules[2].ScheduledActivities[2].FinishTime.Should().Be(22);

            resourceSchedules[2].ScheduledActivities.Last().FinishTime.Should().Be(22);
        }
    }
}
