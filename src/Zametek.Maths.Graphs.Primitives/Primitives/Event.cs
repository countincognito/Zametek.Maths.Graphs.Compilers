﻿using System;

namespace Zametek.Maths.Graphs
{
    public class Event<T>
        : IEvent<T>
        where T : IComparable<T>, IEquatable<T>
    {
        #region Ctors

        public Event(T id)
            : this(id, null, null)
        { }

        public Event(T id, int? earliestFinishTime, int? latestFinishTime)
        {
            Id = id;
            EarliestFinishTime = earliestFinishTime;
            LatestFinishTime = latestFinishTime;
        }

        #endregion

        #region IActivity<T> Members

        public T Id
        {
            get;
        }

        public int? EarliestFinishTime
        {
            get;
            set;
        }

        public int? LatestFinishTime
        {
            get;
            set;
        }

        public bool CanBeRemoved
        {
            get;
            private set;
        }

        public void SetAsReadOnly()
        {
            CanBeRemoved = false;
        }

        public void SetAsRemovable()
        {
            CanBeRemoved = true;
        }

        public object WorkingCopy()
        {
            return new Event<T>(Id, EarliestFinishTime, LatestFinishTime);
        }

        #endregion
    }
}