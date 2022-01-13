using AnimalSelector.Data;

namespace AnimalSelector.AsyncDataService{
    public interface IMessageBusClient{
        string PublishAnimalsRequest(ImageRequestDto imageRequest);
    }
}