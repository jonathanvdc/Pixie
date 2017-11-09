namespace Pixie
{
    /// <summary>
    /// Defines severity levels for messages to the user.
    /// </summary>
    public enum Severity : int
    {
        /// <summary>
        /// The severity level for messages that are printed
        /// to inform the user of progress that is being made.
        /// </summary>
        Progress,

        /// <summary>
        /// The severity level for messages to the user without
        /// implication of fault.
        /// </summary>
        Message,

        /// <summary>
        /// The severity level for warnings.
        /// </summary>
        Warning,

        /// <summary>
        /// The severity level for error messages.
        /// </summary>
        Error
    }
}