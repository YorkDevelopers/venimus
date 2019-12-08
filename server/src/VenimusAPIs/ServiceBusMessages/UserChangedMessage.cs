namespace VenimusAPIs.ServiceBusMessages
{
    public class UserChangedMessage
    {
        /// <summary>
        ///     The unique internal ID of this user
        /// </summary>
        public string UserId { get; set; } = default!;
    }
}
