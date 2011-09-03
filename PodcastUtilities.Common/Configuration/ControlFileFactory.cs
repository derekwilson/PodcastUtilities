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
		public IControlFile OpenControlFile(string fileName)
		{
			return new ControlFile(fileName);
		}

		#endregion
	}
}