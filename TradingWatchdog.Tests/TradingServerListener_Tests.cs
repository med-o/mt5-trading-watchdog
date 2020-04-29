using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MT5Wrapper;
using MT5Wrapper.Interface.EventSource;
using TradingWatchdog.Monitor;
using TradingWatchdog.ServerListener;

namespace TradingWatchdog.Tests
{
    [TestClass]
    public class TradingServerListener_Tests
    {
        [TestMethod]
        public void WhenListeningConnectionIsCreatedAndDisposed()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var connectionParams = new ConnectionParams()
            {
                Name = "mocked connection",
                IP = "localhost",
            };
            var apiMock = new IMT5ApiMock().Get(connectionParams);
            var monitor = new Mock<ITradesMonitor>();
            monitor.Setup(m => m.QueuedTrades).Returns(new ConcurrentQueue<Trade>());
            var underTest = new TradingServerListener(connectionParams, apiMock.Object, monitor.Object);

            // act
            var task = Task.Run(() => underTest.Listen(cancellationToken));
            cancellationTokenSource.CancelAfter(100);
            Task.WaitAll(task);

            // assert
            apiMock.Verify(api => api.Connect(connectionParams), Times.Once);
            apiMock.Verify(api => api.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void WhenListeningEventHandlersAreRegisteredAndDeregistered()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var connectionParams = new ConnectionParams()
            {
                Name = "mocked connection",
                IP = "localhost",
            };
            var apiMock = new IMT5ApiMock().Get(connectionParams);
            var monitor = new Mock<ITradesMonitor>();
            monitor.Setup(m => m.QueuedTrades).Returns(new ConcurrentQueue<Trade>());
            var underTest = new TradingServerListener(connectionParams, apiMock.Object, monitor.Object);

            // act
            var task = Task.Run(() => underTest.Listen(cancellationToken));
            cancellationTokenSource.CancelAfter(100);
            Task.WaitAll(task);

            // assert
            apiMock.VerifyAdd(api => api.ConnectionEvents.ConnectedEventHandler += It.IsAny<EventHandler>(), Times.Once);
            apiMock.VerifyAdd(api => api.ConnectionEvents.DisconnectedEventHandler += It.IsAny<EventHandler>(), Times.Once);
            apiMock.VerifyAdd(api => api.DealEvents.DealAddEventHandler += It.IsAny<DealEventHandler>(), Times.Once);
            apiMock.VerifyRemove(api => api.ConnectionEvents.ConnectedEventHandler -= It.IsAny<EventHandler>(), Times.Once);
            apiMock.VerifyRemove(api => api.ConnectionEvents.DisconnectedEventHandler -= It.IsAny<EventHandler>(), Times.Once);
            apiMock.VerifyRemove(api => api.DealEvents.DealAddEventHandler -= It.IsAny<DealEventHandler>(), Times.Once);
        }
    }
}
