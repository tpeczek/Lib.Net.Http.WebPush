namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// The push message urgency.
    /// </summary>
    public enum PushMessageUrgency
    {
        /// <summary>
        /// Very low (e.g. advertisements).
        /// </summary>
        VeryLow,
        /// <summary>
        /// Low (e.g. topic updates).
        /// </summary>
        Low,
        /// <summary>
        /// Normal (e.g. chat message).
        /// </summary>
        Normal,
        /// <summary>
        /// High (e.g. time-sensitive alert).
        /// </summary>
        High
    }
}
