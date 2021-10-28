namespace AzCompare.Options
{
    public class AzCompareOptions
    {
        /// <summary>
        /// Scope to compare: subscription (default) or resourcegroup
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// ID of the subscription or resourcegroup to compare
        /// </summary>
        public string LeftId { get; set; }

        /// <summary>
        /// String pattern to ignore, like an environment acronym
        /// </summary>
        public string LeftFilter { get; set; }

        /// <summary>
        /// ID of the subscription or resourcegroup to compare to
        /// </summary>
        public string RightId { get; set; }

        /// <summary>
        /// String pattern to ignore, like an environment acronym
        /// </summary>
        public string RightFilter { get; set; }
    }
}
