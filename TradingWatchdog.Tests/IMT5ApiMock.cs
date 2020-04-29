using Moq;
using MT5Wrapper;
using MT5Wrapper.Interface;
using MT5Wrapper.Interface.EventSource;
using System;

namespace TradingWatchdog.Tests
{
    public class IMT5ApiMock
    {
        public Mock<IMT5Api> Get(ConnectionParams connectionParams) {
            var apiMock = new Mock<IMT5Api>();
            
            apiMock
                .Setup(api => api.Name)
                .Returns("Mocked MT5 API");
            
            apiMock
                .Setup(api => api.Connect(connectionParams))
                .Callback(
                    () => apiMock.Raise(m => m.ConnectionEvents.ConnectedEventHandler += null, EventArgs.Empty)
                );

            apiMock
                .Setup(api => api.Disconnect())
                .Callback(
                    () => apiMock.Raise(m => m.ConnectionEvents.DisconnectedEventHandler += null, EventArgs.Empty)
                );

            apiMock
                .SetupSequence(api => api.GetUserBalance(It.IsAny<ulong>()))
                .Returns(10)
                .Returns(5)
                .Returns(2);

        apiMock
            .SetupAdd(api => api.ConnectionEvents.ConnectedEventHandler += It.IsAny<EventHandler>());
            apiMock
                .SetupRemove(api => api.ConnectionEvents.ConnectedEventHandler -= It.IsAny<EventHandler>());

            apiMock
                .SetupAdd(api => api.ConnectionEvents.DisconnectedEventHandler += It.IsAny<EventHandler>());
            apiMock
                .SetupRemove(api => api.ConnectionEvents.DisconnectedEventHandler -= It.IsAny<EventHandler>());

            apiMock
                .SetupAdd(api => api.DealEvents.DealAddEventHandler += It.IsAny<DealEventHandler>());
            apiMock
                .SetupRemove(api => api.DealEvents.DealAddEventHandler -= It.IsAny<DealEventHandler>());

            return apiMock;            
        }
    }
}
