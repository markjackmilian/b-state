using System;
using System.Threading.Tasks;
using bstate.core.Components;
using bstate.core.Services;
using bstate.core.Services.Lifecycle;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace bstate.core.tests.Components;

[TestClass]
[TestSubject(typeof(BStateComponent))]
public class BStateComponentTest
{
    private Mock<IComponentRegister> _mockComponentRegister;
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IOnInitialize> _mockOnInitialize;
    private Mock<IOnAfterRenderAsync> _mockOnAfterRenderAsync;
    private Mock<IOnBStateRender> _mockOnBStateRender;
    private Mock<IOnDisposeAsync> _mockOnDisposeAsync;
    private Mock<IOnParametersSet> _mockOnParametersSet;
    private TestBStateComponent _testComponent;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockComponentRegister = new Mock<IComponentRegister>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockOnInitialize = new Mock<IOnInitialize>();
        _mockOnAfterRenderAsync = new Mock<IOnAfterRenderAsync>();
        _mockOnBStateRender = new Mock<IOnBStateRender>();
        _mockOnDisposeAsync = new Mock<IOnDisposeAsync>();
        _mockOnParametersSet = new Mock<IOnParametersSet>();

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IOnInitialize)))
            .Returns(_mockOnInitialize.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IOnAfterRenderAsync)))
            .Returns(_mockOnAfterRenderAsync.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IOnBStateRender)))
            .Returns(_mockOnBStateRender.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IOnDisposeAsync)))
            .Returns(_mockOnDisposeAsync.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IOnParametersSet)))
            .Returns(_mockOnParametersSet.Object);

        _testComponent = new TestBStateComponent
        {
            ComponentRegister = _mockComponentRegister.Object,
            ServiceProvider = _mockServiceProvider.Object
        };
    }
    
    [TestMethod]
    public void ConfigureCustomLifeCycle_Should_BeCallable()
    {
        // Act
        _testComponent.ConfigureCustomLifeCycleForTest();

        // Assert
        Assert.IsTrue(_testComponent.ConfigureCustomLifeCycleCalled);
    }

    [TestMethod]
    public async Task OnInitializedAsync_Should_InvokeOnInitialize()
    {
        // Arrange
        _testComponent.UseOnInitializeForTest<IOnInitialize>();

        // Act
        await _testComponent.OnInitializAsyncForTest();

        // Assert
        _mockOnInitialize.Verify(i => i.OnInitialize(_testComponent), Times.Once);
    }

    [TestMethod]
    public async Task OnAfterRenderAsync_Should_InvokeOnAfterRenderAsync()
    {
        // Arrange
        _testComponent.UseOnAfterRenderAsyncForTest<IOnAfterRenderAsync>();

        // Act
        await _testComponent.OnAfterRenderAsyncForTest(true);

        // Assert
        _mockOnAfterRenderAsync.Verify(ar => ar.OnAfterRenderAsync(_testComponent, true), Times.Once);
    }

    [TestMethod]
    public async Task BStateRender_Should_InvokeOnBStateRenderAndTriggerStateHasChanged()
    {
        // Arrange
        _testComponent.UseOnBStateRenderForTest<IOnBStateRender>();

        // Act
        await _testComponent.TestBStateRender();

        // Assert
        _mockOnBStateRender.Verify(br => br.OnBStateRender(_testComponent), Times.Once);
        Assert.IsTrue(_testComponent.StateHasChangedTriggered);
    }

    [TestMethod]
    public async Task DisposeAsync_Should_ClearComponentAndInvokeOnDispose()
    {
        // Arrange
        _testComponent.UseOnDisposeAsyncForTest<IOnDisposeAsync>();

        // Act
        await _testComponent.DisposeAsync();

        // Assert
        _mockComponentRegister.Verify(cr => cr.Clear(_testComponent), Times.Once);
        _mockOnDisposeAsync.Verify(d => d.OnDisposeAsync(_testComponent), Times.Once);
    }

    [TestMethod]
    public void OnParametersSet_Should_InvokeOnParametersSetHandlers()
    {
        // Arrange
        _testComponent.UseOnParametersSetForTest<IOnParametersSet>();

        // Act
        _testComponent.OnParametersSetForTest();

        // Assert
        _mockOnParametersSet.Verify(ps => ps.OnParametersSet(_testComponent), Times.Once);
    }

    private class TestBStateComponent : BStateComponent
    {
        public bool ConfigureCustomLifeCycleCalled { get; private set; }
        public bool StateHasChangedTriggered { get; private set; }

        internal void ConfigureCustomLifeCycleForTest() => ConfigureCustomLifeCycle();
        internal void UseOnInitializeForTest<T>() where T : IOnInitialize => UseOnInitialize<T>();
        internal void UseOnAfterRenderAsyncForTest<T>() where T : IOnAfterRenderAsync => UseOnAfterRenderAsync<T>();
        internal void UseOnBStateRenderForTest<T>() where T : IOnBStateRender => UseOnBStateRender<T>();
        internal void UseOnDisposeAsyncForTest<T>() where T : IOnDisposeAsync => UseOnDisposeAsync<T>();
        internal void UseOnParametersSetForTest<T>() where T : IOnParametersSet => UseOnParametersSet<T>();
        
        internal Task OnInitializAsyncForTest() => OnInitializedAsync();
        internal Task OnAfterRenderAsyncForTest(bool firstRender) => OnAfterRenderAsync(firstRender);
        internal Task BStateRenderForTest() => BStateRender();
        internal ValueTask DisposeAsyncForTest() => DisposeAsync();
        internal void OnParametersSetForTest() => OnParametersSet();
        
        protected override void ConfigureCustomLifeCycle()
        {
            ConfigureCustomLifeCycleCalled = true;
        }
        
        internal async Task TestBStateRender()
        {
            await InvokeOnBStateRender();
            StateHasChangedTriggered = true; // Simulate that StateHasChanged would be called
        }

        
        internal T PublicUseStateForTest<T>() where T : BState => base.UseState<T>();
      
    }

    private class TestState : BState
    {
        public TestState() : base(new Mock<IActionBus>().Object)
        {
        }

        protected override void Initialize()
        {
        }
    }
}