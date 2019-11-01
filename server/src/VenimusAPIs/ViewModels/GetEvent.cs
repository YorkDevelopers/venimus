using System.ComponentModel.DataAnnotations;

namespace VenimusAPIs.ViewModels
{
    public class GetEvent
    {
        /// <summary>
        ///     The unique external id for the event.  For example May2019
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; }
    }
}
