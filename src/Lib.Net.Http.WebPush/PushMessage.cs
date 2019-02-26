using System;
using System.Net.Http;

namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// Class representing a push message.
    /// </summary>
    public class PushMessage
    {
        #region Fields
        private static readonly string NOT_INSTANTIATED_THROUGH_STRING_BASED_CONSTRUCTOR = $"The {nameof(PushMessage)} instance hasn't been instantianted through string based constructor.";

        private readonly bool _stringBased;

        private string _content;
        private int? _timeToLive;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the topic (used to correlate messages sent to the same subscription).
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the content when a <see cref="PushMessage"/> instance has been instantiated through <see cref="PushMessage.PushMessage(string)"/>.
        /// </summary>
        public string Content
        {
            get
            {
                if (!_stringBased)
                {
                    throw new InvalidOperationException(NOT_INSTANTIATED_THROUGH_STRING_BASED_CONSTRUCTOR);
                }

                return _content;
            }

            set
            {
                if (!_stringBased)
                {
                    throw new InvalidOperationException(NOT_INSTANTIATED_THROUGH_STRING_BASED_CONSTRUCTOR);
                }

                _content = value;
                HttpContent = (_content is null) ? null : new StringContent(_content);
            }
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public HttpContent HttpContent { get; private set; }

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

        /// <summary>
        /// Gets or sets the message urgency.
        /// </summary>
        public PushMessageUrgency Urgency { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates new instance of <see cref="PushMessage"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public PushMessage(string content)
        {
            _stringBased = true;
            Content = content;
            Urgency = PushMessageUrgency.Normal;
        }

        /// <summary>
        /// Creates new instance of <see cref="PushMessage"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public PushMessage(HttpContent content)
        {
            _stringBased = false;
            HttpContent = content;
            Urgency = PushMessageUrgency.Normal;
        }
        #endregion
    }
}
