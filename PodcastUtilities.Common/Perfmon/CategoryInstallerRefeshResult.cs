namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// the result of running the installer
    /// </summary>
    public enum CategoryInstallerRefeshResult
    {
        /// <summary>
        /// result is unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// the category did not exist and has been created
        /// </summary>
        CatagoryCreated,
        /// <summary>
        /// the category did exist and has been deleted and recreated
        /// </summary>
        CatagoryUpdated,
        /// <summary>
        /// the category has been deleted
        /// </summary>
        CatagoryDeleted
    }
}