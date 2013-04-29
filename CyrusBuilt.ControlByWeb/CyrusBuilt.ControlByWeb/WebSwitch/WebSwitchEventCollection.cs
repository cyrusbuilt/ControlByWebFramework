using System;
using System.Collections;
using CyrusBuilt.ControlByWeb.Events;

// TODO Consider writing a base event class that all event classes inheirit
// from, or implement a common interface, the have a single collection class
// that stores that type and refactor all controller modules that make use
// of event collections. This would save us from all these individual
// collection classes that ultimately accomplish the same thing.

namespace CyrusBuilt.ControlByWeb.WebSwitch
{
    /// <summary>
    /// A collection for storing WebSwitch Plus events.
    /// </summary>
    public sealed class WebSwitchPlusEventCollection : CollectionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.WebSwitch.WebSwitchPlusEventCollection</b>
        /// class. This is the default constructor.
        /// </summary>
        public WebSwitchPlusEventCollection()
            : base(EventConstants.EVENT_MAX_ID + 1) {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="WebSwitchPlusEvent"/> at the specified
        /// index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the <see cref="WebSwitchPlusEvent"/> to
        /// get or set.
        /// </param>
        /// <returns>
        /// The event at the specified index.
        /// </returns>
        public WebSwitchPlusEvent this[Int32 index] {
            get { return base.List[index] as WebSwitchPlusEvent; }
            set { base.List[index] = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds an <see cref="WebSwitchPlusEvent"/> to the collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="WebSwitchPlusEvent"/> to add.
        /// </param>
        /// <returns>
        /// The position into which the new <see cref="WebSwitchPlusEvent"/> was
        /// inserted.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Adding another event would exceed the current capacity of the
        /// collection.
        /// </exception>
        public Int32 Add(WebSwitchPlusEvent evt) {
            return base.List.Add(evt);
        }

        /// <summary>
        /// Inserts a new <see cref="WebSwitchPlusEvent"/> into the collection at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which the <see cref="WebSwitchPlusEvent"/> should be
        /// inserted.
        /// </param>
        /// <param name="evt">
        /// The <see cref="WebSwitchPlusEvent"/> to insert into the collection.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is not a valid index in the collection.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// evt is a null reference in the collection.
        /// </exception>
        public void Insert(Int32 index, WebSwitchPlusEvent evt) {
            base.List.Insert(index, evt);
        }

        /// <summary>
        /// Removes the first occurrence of a specified <see cref="WebSwitchPlusEvent"/>
        /// from the collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="WebSwitchPlusEvent"/> to remove from the collection.
        /// </param>
        public void Remove(WebSwitchPlusEvent evt) {
            base.List.Remove(evt);
        }

        /// <summary>
        /// Determines whether or not the collection contains a specific
        /// <see cref="WebSwitchPlusEvent"/>.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="WebSwitchPlusEvent"/> to locate in the collection.
        /// </param>
        /// <returns>
        /// <b>true</b> if the <see cref="WebSwitchPlusEvent"/> is found in the
        /// collection; Otherwise, <b>false</b>.
        /// </returns>
        public Boolean Contains(WebSwitchPlusEvent evt) {
            return base.List.Contains(evt);
        }

        /// <summary>
        /// Determines the index of a specific <see cref="WebSwitchPlusEvent"/> in the
        /// collection.
        /// </summary>
        /// <param name="evt">
        /// The <see cref="WebSwitchPlusEvent"/> to locate in the collection.
        /// </param>
        /// <returns>
        /// The index of the <see cref="WebSwitchPlusEvent"/> if found in the collection;
        /// Otherwise, -1.
        /// </returns>
        public Int32 IndexOf(WebSwitchPlusEvent evt) {
            return base.List.IndexOf(evt);
        }

        /// <summary>
        /// Copies the elements of the collection to a <see cref="WebSwitchPlusEvent"/>
        /// array, starting at a particular array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="WebSwitchPlusEvent"/> array that is the
        /// destination of the elements copied from the collection.  The
        /// <see cref="WebSwitchPlusEvent"/> array must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in the array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// array is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than zero.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// array is multidimensional. -or- The number of elements in the
        /// source array is greater than the available space from index to the
        /// end of the destination array.
        /// </exception>
        public void CopyTo(WebSwitchPlusEvent[] array, Int32 index) {
            base.List.CopyTo(array, index);
        }

        /// <summary>
        /// Determines if the specified <b>WebSwitchPlusEventCollection</b> is null or
        /// is an empty collection.
        /// </summary>
        /// <param name="collection">
        /// The <b>WebSwitchPlusEventCollection</b> to check.
        /// </param>
        /// <returns>
        /// <b>true</b> if the collection is null or empty; Otherwise,
        /// <b>false</b>.
        /// </returns>
        public static Boolean IsNullOrEmpty(WebSwitchPlusEventCollection collection) {
            if ((collection == null) || (collection.Count == 0)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets an <see cref="WebSwitchPlusEvent"/> from the collection by ID.
        /// </summary>
        /// <param name="id">
        /// The ID of the event to locate.
        /// </param>
        /// <returns>
        /// If successful, the <see cref="WebSwitchPlusEvent"/> with the matching ID;
        /// Otherwise, null.
        /// </returns>
        public WebSwitchPlusEvent GetEventById(Int32 id) {
            WebSwitchPlusEvent evt = null;
            foreach (WebSwitchPlusEvent thisEvent in this) {
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
        /// <code>true</code> if the event exists; Otherwise, <code>false</code>.
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
        public void Replace(WebSwitchPlusEvent current, WebSwitchPlusEvent replacement) {
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
