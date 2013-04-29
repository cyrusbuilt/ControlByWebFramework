using CyrusBuilt.ControlByWeb.Daq.X300Module;
using System;

namespace CyrusBuilt.ControlByWeb.Daq.X301Module
{
    /// <summary>
    /// Used to store diagnostic information about a ControlByWeb X-301 module.
    /// The properties in this class are meant to be read-only. So if the
    /// values MUST be changed (such as by a method that is retrieving the
    /// diagnostic info), then you must call the appropriate method for 
    /// explicitly setting the value.
    /// </summary>
    public class X301Diagnostics : X300Diagnostics
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X301Module.X301Diagnostics</b>
        /// class. This is the default constructor.
        /// </summary>
        public X301Diagnostics()
            : base() {
            // This class is functionally identical to the X300Diagnostics class.
            // So here we just do a base implementation of X300Diagnostics.
        }
        #endregion
    }
}
