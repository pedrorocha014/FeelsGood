using AnimalSelector.Data;

namespace AnimalSelector.AsyncDataService{
    public interface IMessageBusClient{
        void PublishAnimalsRequest(ImageRequestDto imageRequest);
    }
}