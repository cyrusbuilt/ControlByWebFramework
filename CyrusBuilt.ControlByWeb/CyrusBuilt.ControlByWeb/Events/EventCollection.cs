using System;
using System.Collections;
using CyrusBuilt.ControlByWeb.Events;

namespace CyrusBuilt.ControlByWeb.Events
{
    /// <summary>
    /// A collection of device events.
    /// </summary>
    public class EventCollection : CollectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Events.EventCollection</b>
        /// class. This is the default constructor.
        /// </summary>
        public EventCollection()
            : base(EventConstants.EVENT_MAX_ID + 1) {
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Gets or sets the <see cref="EventBase"/> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="EventBase"/> to get or set.
        /// </param>
        /// <returns>
        /// The <see cref="EventBase"/> at the specified index.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The specified index is not within the range of possible index values.
        /// </exception>
        public EventBase this[Int32 index] {
            get { return base.List[index] as EventBase; }
            set { base.List[index] = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds an <see cref="EventBase"/> to the collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="EventBase"/> to add.
        /// </param>
        /// <returns>
        /// The position into which the new <see cref="EventBase"/> was
        /// inserted, or -1 if the event was not inserted.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Adding another event would exceed the current capacity of the
        /// collection.
        /// </exception>
        public Int32 Add(EventBase evt) {
            return base.List.Add(evt);
        }

        /// <summary>
        /// Inserts a new <see cref="EventBase"/> into the collection at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which the <see cref="EventBase"/> should be
        /// inserted.
        /// </param>
        /// <param name="evt">
        /// The <see cref="EventBase"/> to insert into the collection.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the collection.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// evt is a null reference in the collection.
        /// </exception>
        public void Insert(Int32 index, EventBase evt) {
            base.List.Insert(index, evt);
        }

        /// <summary>
        /// Removes the first occurrence of a specified <see cref="EventBase"/>
        /// from the collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="EventBase"/> to remove from the collection.
        /// </param>
        public void Remove(EventBase evt) {
            base.List.Remove(evt);
        }

        /// <summary>
        /// Determines whether or not the collection contains a specific
        /// <see cref="EventBase"/>.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="EventBase"/> to locate in the collection.
        /// </param>
        /// <returns>
        /// <code>true</code> if the <see cref="EventBase"/> is found in the
        /// collection; Otherwise, <code>false</code>.
        /// </returns>
        public Boolean Contains(EventBase evt) {
            return base.List.Contains(evt);
        }

        /// <summary>
        /// Determines the index of a specific <see cref="EventBase"/> in the
        /// collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="EventBase"/> to locate in the collection.
        /// </param>
        /// <returns>
        /// The index of the <see cref="EventBase"/> if found in the collection;
        /// Otherwise, -1.
        /// </returns>
        public Int32 IndexOf(EventBase evt) {
            return base.List.IndexOf(evt);
        }

        /// <summary>
        /// Copies the elements of the collection to a <see cref="EventBase"/>
        /// array, starting at a particular array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="EventBase"/> array that is the
        /// destination of the elements copied from the collection.  The
        /// <see cref="EventBase"/> array must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in the array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="array"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="array"/> is multidimensional. -or- The number of
        /// elements in the source array is greater than the available space
        /// from index to the end of the destination array.
        /// </exception>
        public void CopyTo(EventBase[] array, Int32 index) {
            base.List.CopyTo(array, index);
        }

        /// <summary>
        /// Determines if the specified <b>EventCollection</b> is null or
        /// is an empty collection.
        /// </summary>
        /// <param name="events">
        /// The <b>EventCollection</b> to check.
        /// </param>
        /// <returns>
        /// <code>true</code> if the collection is null or empty; Otherwise,
        /// <code>false</code>.
        /// </returns>
        public static Boolean IsNullOrEmpty(EventCollection events) {
            if ((events == null) || (events.Count == 0)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets an <see cref="EventBase"/> from the collection by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the event to locate.
        /// </param>
        /// <returns>
        /// If successful, the <see cref="EventBase"/> with the matching ID;
        /// Otherwise, null.
        /// </returns>
        public EventBase GetEventById(Int32 id) {
            EventBase evt = null;
            foreach (EventBase thisEvent in this) {
                if (thisEvent.Id == id) {
                    evt = thisEvent;
                    break;
                }
            }
            return evt;
        }

        /// <summary>
        /// Convenience method for determining if a specific event exists in
        /// the collection.
        /// </summary>
        /// <param name="id">
        /// The ID of the event to search for.
        /// </param>
        /// <returns>
        /// true if the event exists; Otherwise, false.
        /// </returns>
        public Boolean Exists(Int32 id) {
            return (this.GetEventById(id) != null);
        }

        /// <summary>
        /// Replaces a given event with another. If the event being replaced
        /// does not exist or the replacement is null. This does nothing.
        /// </summary>
        /// <param name="current">
        /// The event to replace.
        /// </param>
        /// <param name="replacement">
        /// The replacement event. If the replacement event already exists in
        /// the collection, it will be ignored.
        /// </param>
        public void Replace(EventBase current, EventBase replacement) {
            if ((current != null) && (replacement != null)) {
                // We could probably use Contains() here, but we want to make
                // sure that we are replacing an event with a matching ID and
                // that the current event already exists in the collection. We
                // do not assume that we can go ahead and add the event if the
                // current one does not exist.
                if (current.Id == replacement.Id) {
                    if (this.GetEventById(current.Id) != null) {
                        this.Remove(current);
                        this.Add(replacement);
                    }
                }
            }
        }
        #endregion
    }
}
