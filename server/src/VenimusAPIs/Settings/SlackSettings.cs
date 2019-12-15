using System;

namespace VenimusAPIs.Settings
{
    public class SlackSettings
    {
        /// <summary>
        ///     The webhook to send the approve membership messaeg to.
        /// </summary>
        public Uri ApproversWebhookUrl { get; set; } = default!;
    }
}
