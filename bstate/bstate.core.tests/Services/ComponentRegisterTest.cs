using bstate.core.Components;
using bstate.core.Services;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace bstate.core.tests.Services;

[TestClass]
[TestSubject(typeof(ComponentRegister))]
public class ComponentRegisterTest
{
    [TestMethod]
    public void Add_ShouldAddComponentToCorrectStateType()
    {
        // Arrange
        var register = new ComponentRegister();
        var component = new TestComponent();

        // Act
        register.Add<BState>(component);

        // Assert
        var components = register.GetComponents<BState>();
        Assert.AreEqual(1, components.Length);
        Assert.AreEqual(component, components[0]);
    }

    [TestMethod]
    public void Remove_ShouldRemoveComponentFromCorrectStateType()
    {
        // Arrange
        var register = new ComponentRegister();
        var component = new TestComponent();
        register.Add<BState>(component);

        // Act
        register.Remove<BState>(component);

        // Assert
        var components = register.GetComponents<BState>();
        Assert.AreEqual(0, components.Length);
    }

    [TestMethod]
    public void Clear_ShouldRemoveComponentFromAllStateTypes()
    {
        // Arrange
        var register = new ComponentRegister();
        var component = new TestComponent();
        register.Add<BState>(component);
        register.Add<AnotherBState>(component);

        // Act
        register.Clear(component);

        // Assert
        Assert.AreEqual(0, register.GetComponents<BState>().Length);
        Assert.AreEqual(0, register.GetComponents<AnotherBState>().Length);
    }

    [TestMethod]
    public void GetComponentsByType_ShouldReturnComponentsForSpecificStateType()
    {
        // Arrange
        var register = new ComponentRegister();
        var component1 = new TestComponent();
        var component2 = new TestComponent();
        register.Add<BState>(component1);
        register.Add<BState>(component2);

        // Act
        var components = register.GetComponents<BState>();

        // Assert
        Assert.AreEqual(2, components.Length);
        CollectionAssert.AreEquivalent(new[] { component1, component2 }, components);
    }

    [TestMethod]
    public void GetComponents_ShouldReturnAllComponents()
    {
        // Arrange
        var register = new ComponentRegister();
        var component1 = new TestComponent();
        var component2 = new TestComponent();
        register.Add<BState>(component1);
        register.Add<AnotherBState>(component2);

        // Act
        var components = register.GetComponents();

        // Assert
        Assert.AreEqual(2, components.Length);
        CollectionAssert.AreEquivalent(new[] { component1, component2 }, components);
    }

    [TestMethod]
    public void GetComponentsByType_WhenTypeHasNoComponents_ShouldReturnEmptyArray()
    {
        // Arrange
        var register = new ComponentRegister();

        // Act
        var components = register.GetComponents<BState>();

        // Assert
        Assert.AreEqual(0, components.Length);
    }

    [TestMethod]
    public void Dispose_ShouldReleaseResources()
    {
        // Arrange
        var register = new ComponentRegister();

        // Act
        register.Dispose();

        // Assert
        try
        {
            register.Dispose();
        }
        catch
        {
            Assert.Fail("Dispose should be idempotent, but threw an exception.");
        }
    }

    [TestMethod]
    public void Add_MultipleComponentsForSameType_ShouldAddAllComponents()
    {
        // Arrange
        var register = new ComponentRegister();
        var component1 = new TestComponent();
        var component2 = new TestComponent();

        // Act
        register.Add<BState>(component1);
        register.Add<BState>(component2);

        // Assert
        var components = register.GetComponents<BState>();
        Assert.AreEqual(2, components.Length);
        CollectionAssert.AreEquivalent(new[] { component1, component2 }, components);
    }

    [TestMethod]
    public void Remove_ComponentNotExistingInType_ShouldNotThrow()
    {
        // Arrange
        var register = new ComponentRegister();
        var component = new TestComponent();

        // Act & Assert
        register.Remove<BState>(component);
    }

    [TestMethod]
    public void Clear_EmptyState_ShouldNotThrow()
    {
        // Arrange
        var register = new ComponentRegister();
        var component = new TestComponent();

        // Act
        register.Clear(component);

        // Assert
        Assert.AreEqual(0, register.GetComponents().Length);
    }

    private class TestComponent : BStateComponent
    {
    }

    private class AnotherBState : BState
    {
        public AnotherBState() : base(null!)
        {
        }
    }

 
}