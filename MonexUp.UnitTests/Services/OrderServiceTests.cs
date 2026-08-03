using Microsoft.Extensions.Logging;
using Moq;
using MonexUp.Domain.Impl.Services;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.DTO.Order;
using MonexUp.Infra.Interfaces.AppServices;
using NAuth.ACL.Interfaces;
using Xunit;

namespace MonexUp.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderDomainFactory> _orderFactory;
        private readonly Mock<IOrderItemDomainFactory> _itemFactory;
        private readonly Mock<ILofnProductClient> _lofnProductClient;
        private readonly Mock<IUserClient> _userClient;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderFactory = new Mock<IOrderDomainFactory>();
            _itemFactory = new Mock<IOrderItemDomainFactory>();
            _lofnProductClient = new Mock<ILofnProductClient>();
            _userClient = new Mock<IUserClient>();

            _service = new OrderService(
                _orderFactory.Object,
                _itemFactory.Object,
                _lofnProductClient.Object,
                _userClient.Object,
                new Mock<ILogger<OrderService>>().Object);
        }

        private Mock<IOrderModel> SetupOrderLookup(long invoiceId, IOrderModel resolved)
        {
            var builder = new Mock<IOrderModel>();
            builder
                .Setup(m => m.GetByProxyPayInvoiceId(invoiceId, _orderFactory.Object))
                .Returns(resolved);
            _orderFactory.Setup(f => f.BuildOrderModel()).Returns(builder.Object);
            return builder;
        }

        [Fact]
        public void MarkPaidByInvoiceId_AdvancesIncomingToActive()
        {
            var order = new Mock<IOrderModel>();
            order.SetupAllProperties();
            order.Object.Status = OrderStatusEnum.Incoming;
            order.Setup(m => m.Update(_orderFactory.Object)).Returns(order.Object);
            SetupOrderLookup(7, order.Object);

            var result = _service.MarkPaidByInvoiceId(7);

            Assert.NotNull(result);
            Assert.Equal(OrderStatusEnum.Active, result.Status);
            order.Verify(m => m.Update(_orderFactory.Object), Times.Once);
        }

        [Fact]
        public void MarkPaidByInvoiceId_IsNoOpWhenAlreadyActive()
        {
            var order = new Mock<IOrderModel>();
            order.SetupAllProperties();
            order.Object.Status = OrderStatusEnum.Active;
            SetupOrderLookup(7, order.Object);

            var result = _service.MarkPaidByInvoiceId(7);

            Assert.NotNull(result);
            Assert.Equal(OrderStatusEnum.Active, result.Status);
            order.Verify(m => m.Update(It.IsAny<IOrderDomainFactory>()), Times.Never);
        }

        [Fact]
        public void MarkPaidByInvoiceId_ReturnsNullWhenNoMatchingOrder()
        {
            SetupOrderLookup(999, null);

            var result = _service.MarkPaidByInvoiceId(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderInfo_WhenNAuthUserIsMissing_ShouldDegradeInsteadOfThrowing()
        {
            // An order can outlive the NAuth account that placed it; NAuth then
            // throws on GetByIdAsync. That must not take down /order/list.
            const long userId = 42;
            const long sellerId = 43;

            var order = new Mock<IOrderModel>();
            order.SetupAllProperties();
            order.Object.OrderId = 1;
            order.Object.NetworkId = 1;
            order.Object.UserId = userId;
            order.Object.SellerId = sellerId;
            order.Object.Status = OrderStatusEnum.Active;
            order.Setup(m => m.ListItems(It.IsAny<IOrderItemDomainFactory>()))
                .Returns(new List<IOrderItemModel>());

            _userClient.Setup(c => c.GetByIdAsync(It.IsAny<long>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("404 (Not Found)"));

            var info = await _service.GetOrderInfo(order.Object, "bearer-token");

            Assert.NotNull(info);
            Assert.Equal(1, info.OrderId);
            Assert.Null(info.User);
            Assert.Null(info.Seller);
        }
    }
}
