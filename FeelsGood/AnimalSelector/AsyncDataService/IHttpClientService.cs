using System.Threading.Tasks;

namespace AnimalSelector.AsyncDataService{
    public interface IHttpClientService{
        Task<string> GetSingleDog();
        Task<string> GetSingleCat();
    }
}