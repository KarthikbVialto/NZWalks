using NZWalks.Models.Domains;

namespace NZWalks.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
