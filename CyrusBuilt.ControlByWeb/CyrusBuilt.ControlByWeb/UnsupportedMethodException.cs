using System;

namespace CyrusBuilt.ControlByWeb
{
    /// <summary>
    /// An exception that is thrown when a method is called by user code that
    /// the developer does not intend to be called. This could indicate that
    /// the method is only meant to be called by other methods in the same
    /// scope or to mark a method that is not yet safe to be implemented. This
    /// is particularly helpful for methods that are still being developed or
    /// are in alpha/beta stage.
    /// </summary>
    public sealed class UnsupportedMethodException : Exception
    {
        #region Fields
        private String _methodName = String.Empty;
        private String _reason = String.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.UnsupportedMethodException</b>
        /// with the name or signature of the method that is throwing this
        /// exception.
        /// </summary>
        /// <param name="methodName">
        /// The name or signature of the method that is throwing this exception.
        /// </param>
        public UnsupportedMethodException(String methodName)
            : base(methodName + " is not currently a supported method. Do not use.") {
                this._methodName = methodName;
        }

        /// <summary>
        /// Initializes a new instance of the <b>CyrusBuilt.ControlByWeb.UnsupportedMethodException</b>
        /// with the name or signature of the method that is throwing this
        /// exception and the reason the method throws this exception.
        /// </summary>
        /// <param name="methodName">
        /// The name or signature of the method that is throwing this exception.
        /// </param>
        /// <param name="reason">
        /// The reason the method throws this exception.
        /// </param>
        public UnsupportedMethodException(String methodName, String reason)
            : base("Cannot implement " + methodName + ". Reason: " + reason) {
                this._methodName = methodName;
                this._reason = reason;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name or signature of the method that is throwing this
        /// exception.
        /// </summary>
        public String MethodName {
            get {
                if (String.IsNullOrEmpty(this._methodName)) {
                    return base.TargetSite.ToString();
                }
                return this._methodName;
            }
        }

        /// <summary>
        /// Gets the reason the method throws this exception.
        /// </summary>
        public String Reason {
            get {
                if (String.IsNullOrEmpty(this._reason)) {
                    return "Use of the method called is currently unsupported.";
                }
                return this._reason;
            }
        }
        #endregion
    }
}
