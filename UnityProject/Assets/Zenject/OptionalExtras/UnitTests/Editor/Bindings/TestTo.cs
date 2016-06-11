using System;
using System.Collections.Generic;
using Zenject;
using NUnit.Framework;
using System.Linq;
using ModestTree;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestTo : ZenjectUnitTestFixture
    {
        [Test]
        public void TestSelfSingle()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfSingleExplicit()
        {
            Container.Bind<Foo>().ToSelf().FromNew().AsSingle().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfTransient()
        {
            Container.Bind<Foo>().AsTransient().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestSelfCached()
        {
            Container.Bind<Foo>().AsCached().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestConcreteSingle()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsSingle().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteTransient()
        {
            Container.Bind<IFoo>().To<Foo>().AsTransient().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteTransient2()
        {
            Container.Bind<IFoo>().To<Foo>().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestConcreteCached()
        {
            Container.Bind<Foo>().AsCached().NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsCached().NonLazy();

            Container.Validate();

            Assert.IsNotNull(Container.Resolve<Foo>());
            Assert.IsNotNull(Container.Resolve<IFoo>());

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<Foo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<Foo>());
        }

        [Test]
        public void TestDuplicateBindingsFail()
        {
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Foo>().AsSingle().NonLazy();

            Container.Validate();

            Assert.Throws(
                delegate { Container.Resolve<Foo>(); });

            Assert.IsEqual(Container.ResolveAll<Foo>().Count, 2);
        }

        [Test]
        public void TestConcreteMultipleTransient()
        {
            Container.Bind<IFoo>().To(typeof(Foo), typeof(Bar)).AsTransient().NonLazy();

            Container.Validate();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsNotEqual(foos[0], foos2[0]);
            Assert.IsNotEqual(foos[1], foos2[1]);
        }

        [Test]
        public void TestConcreteMultipleSingle()
        {
            Container.Bind<IFoo>().To(typeof(Foo), typeof(Bar)).AsSingle().NonLazy();

            Container.Validate();

            var foos = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos.Count, 2);
            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            var foos2 = Container.ResolveAll<IFoo>();

            Assert.IsEqual(foos[0], foos2[0]);
            Assert.IsEqual(foos[1], foos2[1]);
        }

        [Test]
        [ExpectedException]
        public void TestMultipleBindingsSingleFail1()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).AsSingle();
            Container.Resolve<IFoo>();
        }

        [Test]
        [ExpectedException]
        public void TestMultipleBindingsSingleFail2()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Qux>().AsSingle();
            Container.Resolve<IFoo>();
        }

        [Test]
        public void TestMultipleBindingsSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsSingle().NonLazy();

            Container.Validate();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
            Assert.That(Container.Resolve<IFoo>() is Foo);
        }

        [Test]
        public void TestMultipleBindingsTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsTransient().NonLazy();

            Container.Validate();

            Assert.That(Container.Resolve<IFoo>() is Foo);
            Assert.That(Container.Resolve<IBar>() is Foo);

            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsNotEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To<Foo>().AsCached().NonLazy();

            Container.Validate();

            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<IFoo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleSingle()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsSingle().NonLazy();
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Bar>().AsSingle().NonLazy();

            Container.Validate();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);

            Assert.IsEqual(foos[0], Container.Resolve<Foo>());
            Assert.IsEqual(foos[1], Container.Resolve<Bar>());
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleTransient()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsTransient().NonLazy();

            Container.Validate();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsNotEqual(foos[0], bars[0]);
            Assert.IsNotEqual(foos[1], bars[1]);
        }

        [Test]
        public void TestMultipleBindingsConcreteMultipleCached()
        {
            Container.Bind(typeof(IFoo), typeof(IBar)).To(new List<Type>() {typeof(Foo), typeof(Bar)}).AsCached().NonLazy();
            Container.Bind<Foo>().AsSingle().NonLazy();
            Container.Bind<Bar>().AsSingle().NonLazy();

            Container.Validate();

            var foos = Container.ResolveAll<IFoo>();
            var bars = Container.ResolveAll<IBar>();

            Assert.IsEqual(foos.Count, 2);
            Assert.IsEqual(bars.Count, 2);

            Assert.That(foos[0] is Foo);
            Assert.That(foos[1] is Bar);

            Assert.IsEqual(foos[0], bars[0]);
            Assert.IsEqual(foos[1], bars[1]);

            Assert.IsNotEqual(foos[0], Container.Resolve<Foo>());
            Assert.IsNotEqual(foos[1], Container.Resolve<Bar>());
        }

        [Test]
        public void TestSingletonIdsSameIdsSameInstance()
        {
            Container.Bind<IBar>().To<Foo>().AsSingle("foo").NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo").NonLazy();
            Container.Bind<Foo>().AsSingle("foo").NonLazy();

            Container.Validate();

            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
            Assert.IsEqual(Container.Resolve<Foo>(), Container.Resolve<IBar>());
        }

        [Test]
        public void TestSingletonIdsDifferentIdsDifferentInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo").NonLazy();
            Container.Bind<Foo>().AsSingle("bar").NonLazy();

            Container.Validate();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingletonIdsNoIdDifferentInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle().NonLazy();
            Container.Bind<Foo>().AsSingle("bar").NonLazy();

            Container.Validate();

            Assert.IsNotEqual(Container.Resolve<Foo>(), Container.Resolve<IFoo>());
        }

        [Test]
        public void TestSingletonIdsManyInstances()
        {
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo1").NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo2").NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo3").NonLazy();
            Container.Bind<IFoo>().To<Foo>().AsSingle("foo4").NonLazy();

            Container.Validate();

            Assert.IsEqual(Container.ResolveAll<IFoo>().Count, 4);
        }

        interface IBar
        {
        }

        interface IFoo
        {
        }

        class Foo : IFoo, IBar
        {
        }

        class Bar : IFoo, IBar
        {
        }

        public class Qux
        {
        }
    }
}