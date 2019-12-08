namespace VenimusAPIs.ServiceBusMessages
{
    public class GroupChangedMessage
    {
        /// <summary>
        ///     The unique internal ID of this group
        /// </summary>
        public string GroupId { get; set; } = default!;
    }
}
