namespace PodcastUtilities.Presentation.Services
{
	public interface IBrowseForFileService
	{
		string BrowseForFileToOpen(string fileFilter);
	    string BrowseForFileToSave(string fileFilter);
	}
}