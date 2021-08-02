// CartControllerTest.cs

using System;
using Services;
using Moq;
using api.Controllers;
using System.Linq;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace test
{
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            :base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
  public class Tests
  {
      private CartController controller;

      [Theory]
      [AutoDomainData]
      public void CheckOutShouldReturnCharged(
          [Frozen] Mock<IPaymentService> paymentServiceMock, 
          [Frozen] Mock<IShipmentService> shipmentServiceMock,
          [Frozen] Mock<ICartService> cartServiceMock, 
          [Frozen] Mock<ICard> cardMock, 
          [Frozen] Mock<IAddressInfo> addressInfoMock,
          List<ICartItem> items)
      {   
          cartServiceMock.Setup(c => c.Items())
              .Returns(items.AsEnumerable());
          
          controller = new CartController(
              cartServiceMock.Object, paymentServiceMock.Object, shipmentServiceMock.Object);
          
          paymentServiceMock.Setup(p => 
                  p.Charge(It.IsAny<double>(), cardMock.Object))
              .Returns(true);

          // act
          var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

          // assert
          shipmentServiceMock.Verify(s => 
              s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Once());

          result.Should().Be("charged");
      }

      [Theory]
      [AutoDomainData]
      public void CheckOutShouldReturnNotCharged(
          [Frozen] Mock<IPaymentService> paymentServiceMock, 
          [Frozen] Mock<IShipmentService> shipmentServiceMock,
          [Frozen] Mock<ICartService> cartServiceMock, 
          [Frozen] Mock<ICard> cardMock, 
          [Frozen] Mock<IAddressInfo> addressInfoMock,
          List<ICartItem> items)
      {
          cartServiceMock.Setup(c => c.Items())
              .Returns(items.AsEnumerable());
          
          controller = new CartController(
              cartServiceMock.Object, paymentServiceMock.Object, shipmentServiceMock.Object);
          
          paymentServiceMock.Setup(p => 
              p.Charge(It.IsAny<double>(), cardMock.Object)).Returns(false);

          // act
          var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

          // assert
          shipmentServiceMock.Verify(s => 
              s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Never());
          result.Should().Be("not charged");
      }
  }
}