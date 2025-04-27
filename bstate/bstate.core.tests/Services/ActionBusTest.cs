using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bstate.core.Classes;
using bstate.core.Middlewares;
using bstate.core.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PipelineNet.Pipelines;

namespace bstate.core.tests.Services;

[TestClass]
[TestSubject(typeof(ActionBus))]
public class ActionBusTest
{
    private Mock<IServiceProvider> _serviceProviderMock;
    private Mock<IBStateConfiguration> _configurationMock;
    private Mock<IPipelineBuilder> _pipelineBuilderMock;
    private Mock<IAsyncPipeline<IAction>> _pipelineMock;

    private ActionBus _actionBus;

    [TestInitialize]
    public void Setup()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _configurationMock = new Mock<IBStateConfiguration>();
        _pipelineBuilderMock = new Mock<IPipelineBuilder>();
        _pipelineMock = new Mock<IAsyncPipeline<IAction>>();

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IPipelineBuilder)))
            .Returns(_pipelineBuilderMock.Object);

        _pipelineBuilderMock
            .Setup(pb => pb.AddBehaviours(It.IsAny<IEnumerable<Type>>())).Returns(_pipelineBuilderMock.Object);
        _pipelineBuilderMock
            .Setup(pb => pb.AddPreprocessors()).Returns(_pipelineBuilderMock.Object);
        _pipelineBuilderMock
            .Setup(pb => pb.AddActionRunner()).Returns(_pipelineBuilderMock.Object);
        _pipelineBuilderMock
            .Setup(pb => pb.AddPostprocessors()).Returns(_pipelineBuilderMock.Object);
        _pipelineBuilderMock
            .Setup(pb => pb.AddRenderer()).Returns(_pipelineBuilderMock.Object);
        _pipelineBuilderMock.Setup(pb => pb.Build()).Returns(_pipelineMock.Object);

        _actionBus = new ActionBus(_serviceProviderMock.Object, _configurationMock.Object);
    }

    [TestMethod]
    public async Task Send_ShouldExecutePipeline_WhenValidActionIsProvided()
    {
        // Arrange
        var actionMock = new Mock<IAction>();

        // Act
        await _actionBus.Send(actionMock.Object);

        // Assert
        _pipelineBuilderMock.Verify(pb => pb.AddBehaviours(It.IsAny<IEnumerable<Type>>()), Times.Once);
        _pipelineBuilderMock.Verify(pb => pb.AddPreprocessors(), Times.Once);
        _pipelineBuilderMock.Verify(pb => pb.AddActionRunner(), Times.Once);
        _pipelineBuilderMock.Verify(pb => pb.AddPostprocessors(), Times.Once);
        _pipelineBuilderMock.Verify(pb => pb.AddRenderer(), Times.Once);
        _pipelineMock.Verify(pipeline => pipeline.Execute(actionMock.Object), Times.Once);
    }

    [TestMethod]
    public async Task Send_ShouldThrowArgumentNullException_WhenActionIsNull()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _actionBus.Send(null));
    }

    [TestMethod]
    public async Task Send_ShouldHandleEmptyBehaviours()
    {
        // Arrange
        var actionMock = new Mock<IAction>();
        _configurationMock.Setup(c => c.GetBehaviours()).Returns(new List<Type>());

        // Act
        await _actionBus.Send(actionMock.Object);

        // Assert
        _pipelineBuilderMock.Verify(pb => pb.AddBehaviours(It.Is<IEnumerable<Type>>(b => b != null && !b.Any())),
            Times.Once);
        _pipelineMock.Verify(pipeline => pipeline.Execute(actionMock.Object), Times.Once);
    }

    [TestMethod]
    public async Task Send_ShouldUseConfiguredBehaviours()
    {
        // Arrange
        var actionMock = new Mock<IAction>();
        var behaviours = new List<Type> { typeof(Mock<IBehaviour>) };
        _configurationMock.Setup(c => c.GetBehaviours()).Returns(behaviours);

        // Act
        await _actionBus.Send(actionMock.Object);

        // Assert
        _pipelineBuilderMock.Verify(pb => pb.AddBehaviours(behaviours), Times.Once);
        _pipelineMock.Verify(pipeline => pipeline.Execute(actionMock.Object), Times.Once);
    }

    [TestMethod]
    public async Task Send_ShouldThrowException_WhenPipelineBuilderIsNotRegistered()
    {
        // Arrange
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(IPipelineBuilder))).Returns(null);

        var actionMock = new Mock<IAction>();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _actionBus.Send(actionMock.Object));
    }
}