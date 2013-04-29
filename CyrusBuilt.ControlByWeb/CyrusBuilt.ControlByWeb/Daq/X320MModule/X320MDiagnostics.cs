using System;
using CyrusBuilt.ControlByWeb.Daq.X320Module;

namespace CyrusBuilt.ControlByWeb.Daq.X320MModule
{
    /// <summary>
    /// Used to store diagnostic information about a ControlByWeb X-320M module.
    /// The properties in this class are meant to be read-only. So if the
    /// values MUST be changed (such as by a method that is retrieving the
    /// diagnostic info), then you must call the appropriate method for 
    /// explicitly setting the value.
    /// </summary>
    public class X320MDiagnostics : X320Diagnostics
    {
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.Daq.X320MModule.X320MDiagnostics</b>
        /// class. This is the default constructor.
        /// </summary>
        public X320MDiagnostics()
            : base() {
            // The X-320M diagnostics are exactly the same as the X-320
            // diagnostics, so we just do a base implementation of
            // the same diagnostic class. No sense in re-inventing the
            // wheel or duplicating code.
        }
    }
}
