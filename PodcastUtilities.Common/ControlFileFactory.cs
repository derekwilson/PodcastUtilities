namespace PodcastUtilities.Common
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
		///<param name="filename"></param>
		///<returns></returns>
		public IControlFile OpenControlFile(string filename)
		{
			return new ControlFile(filename);
		}

		#endregion
	}
}