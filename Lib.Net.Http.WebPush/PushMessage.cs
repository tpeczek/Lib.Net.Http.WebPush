using System;

namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// Class representing a push message.
    /// </summary>
    public class PushMessage
    {
        #region Fields
        private int? _timeToLive;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the topic (used to correlate messages sent to the same subscription).
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the time (in seconds) for which the message should be retained by push service.
        /// </summary>
        public int? TimeToLive
        {
            get { return _timeToLive; }

            set
            {
                if (value.HasValue && (value.Value < 0))
                {
                    throw new ArgumentOutOfRangeException(nameof(TimeToLive), "The TTL must be a non-negative integer");
                }

                _timeToLive = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new instance of <see cref="PushMessage"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public PushMessage(string content)
        {
            Content = content;
        }
        #endregion
    }
}
