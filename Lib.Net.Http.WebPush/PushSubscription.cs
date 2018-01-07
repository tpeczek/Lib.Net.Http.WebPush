using System.Collections.Generic;

namespace Lib.Net.Http.WebPush
{
    /// <summary>
    /// Class representing a push subscription
    /// </summary>
    public class PushSubscription
    {
        #region Properties
        /// <summary>
        /// Gets or sets the subscription endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets client keys shared as part of subscription.
        /// </summary>
        public IDictionary<string, string> Keys { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets specific client key shared as part of subscription.
        /// </summary>
        /// <param name="keyName">The key name.</param>
        /// <returns>The key.</returns>
        public string GetKey(PushEncryptionKeyName keyName)
        {
            string key = null;

            if (Keys != null)
            {
                string keyNameStringified = StringifyKeyName(keyName);

                if (Keys.ContainsKey(keyNameStringified))
                {
                    key = Keys[keyNameStringified];
                }
            }

            return key;
        }

        /// <summary>
        /// Sets specific client key shared as part of subscription.
        /// </summary>
        /// <param name="keyName">The key name.</param>
        /// <param name="key">The key.</param>
        public void SetKey(PushEncryptionKeyName keyName, string key)
        {
            if (Keys == null)
            {
                Keys = new Dictionary<string, string>();
            }

            Keys[StringifyKeyName(keyName)] = key;
        }

        private string StringifyKeyName(PushEncryptionKeyName keyName)
        {
            return keyName.ToString().ToLowerInvariant();
        }
        #endregion
    }
}
