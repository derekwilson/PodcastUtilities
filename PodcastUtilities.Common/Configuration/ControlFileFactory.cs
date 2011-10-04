namespace PodcastUtilities.Common.Configuration
{
	///<summary>
	/// Implementation of control file factory
	///</summary>
	public class ControlFileFactory : IControlFileFactory
	{
		#region Implementation of IControlFileFactory

	    ///<summary>
	    /// Opens an existing control file
	    ///</summary>
	    ///<param name="fileName"></param>
	    ///<returns></returns>
	    public IReadWriteControlFile OpenControlFile(string fileName)
		{
			return new ReadWriteControlFile(fileName);
		}

		#endregion
	}
}