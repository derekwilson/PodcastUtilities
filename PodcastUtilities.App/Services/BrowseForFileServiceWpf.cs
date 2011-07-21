using Microsoft.Win32;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.App.Services
{
	public class BrowseForFileServiceWpf
		: IBrowseForFileService
	{
		#region Implementation of IBrowseForFileService

		public string BrowseForFileToOpen(string fileFilter)
		{
			var fileDialog = new OpenFileDialog
			                 	{
			                 		Filter = fileFilter
			                 	};

			var fileSelected = fileDialog.ShowDialog().GetValueOrDefault(false);

			return (fileSelected ? fileDialog.FileName : null);
		}

		#endregion
	}
}