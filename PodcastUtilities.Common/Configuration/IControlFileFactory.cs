namespace PodcastUtilities.Common.Configuration
{
	///<summary>
	/// Abstracts creation/opening of control files
	///</summary>
	public interface IControlFileFactory
	{
		///<summary>
		/// Opens an existing control file
		///</summary>
        ///<param name="fileName"></param>
		///<returns></returns>
		IControlFile OpenControlFile(string fileName);
	}
}