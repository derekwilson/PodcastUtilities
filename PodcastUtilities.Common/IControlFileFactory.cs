using System;

namespace PodcastUtilities.Common
{
	///<summary>
	/// Abstracts creation/opening of control files
	///</summary>
	public interface IControlFileFactory
	{
		///<summary>
		/// Opens an existing control file
		///</summary>
		///<param name="filename"></param>
		///<returns></returns>
		IControlFile OpenControlFile(string filename);
	}
}